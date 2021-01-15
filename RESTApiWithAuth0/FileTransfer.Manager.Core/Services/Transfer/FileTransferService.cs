using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileTransfer.Manager.Core.Services.Settings;
using FileTransfer.Definitions;
using FileTransfer.Definitions.BaseImplementation;
using FileTransfer.Definitions.Dto;
using FileTransfer.Manager.Persistence.Connection;
using FileTransfer.Manager.Persistence.Entities;
using Shared.Logging;

namespace FileTransfer.Manager.Core.Services.Transfer
{
    public class FileTransferService : IFileTransferService
    {
        private IConnection _connection;
        private Dictionary<TransferSourceType, ITransferSourceFactory> _transferSourceFactory;
        private Dictionary<Guid, TransferSourceItemProxy> _transferSources;

        private Task startLoopTask;
        private CancellationTokenSource cancelSource;

        private readonly Logger<FileTransferService> _logger;
        private readonly IFileTransferStatusUpdateService _fileTransferStatusUpdateService;
        private readonly ISettingsService _settingsService;

        public FileTransferService(ISettingsService aSettingsService, 
            IConnectionFactory aConnectionFactory, 
            Dictionary<TransferSourceType, ITransferSourceFactory> aTransferSourceFactory,
            IFileTransferStatusUpdateService aFileTransferStatusUpdateService)
        {
            _settingsService = aSettingsService;

            _connection = aConnectionFactory.CreateConnection();

            _transferSourceFactory = aTransferSourceFactory;
            _transferSources = new Dictionary<Guid, TransferSourceItemProxy>();

            _fileTransferStatusUpdateService = aFileTransferStatusUpdateService;

            _logger = new Logger<FileTransferService>();
        }

        public bool Start(out string aError)
        {
            aError = string.Empty;

            if (_connection.Open(_settingsService.AppSettings, out aError))
            {
                try
                {
                    cancelSource = new CancellationTokenSource();

                    _fileTransferStatusUpdateService.Start();

                    startLoopTask = Task.Factory.StartNew(() => this.StartLoop(), cancelSource.Token,
                        TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
                catch (Exception e)
                {
                    aError = e.Message;
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Stop()
        {
            cancelSource.Cancel();
            startLoopTask.Wait();

            _fileTransferStatusUpdateService.Stop();
            _connection.Close();
        }

        private void StartLoop()
        {
            _logger.LogAlways("Started processing loop.");

            while (!cancelSource.IsCancellationRequested)
            {
                List<Guid> keys = _transferSources.Keys.ToList<Guid>();

                try
                {
                    foreach (Source source in _connection.SourceRepository.GetAll())
                    {
                        keys.Remove(source.SourceID);

                        var proxy = _transferSources.ContainsKey(source.SourceID) ? _transferSources[source.SourceID] : null;

                        if (proxy == null)
                        {
                            var transferSource = _transferSourceFactory.ContainsKey((TransferSourceType)source.Type) ?
                                _transferSourceFactory[(TransferSourceType)source.Type].CreateTransferSource() : null;

                            if (transferSource != null)
                            {
                                proxy = new TransferSourceItemProxy(transferSource);
                                _transferSources.Add(source.SourceID, proxy);

                                proxy.Init(source);
                            }
                        }

                        if (proxy != null)
                        {
                            if (proxy.IsSourceChanged(source))
                            {
                                _logger.LogInfo($"The source '{source.SourceID} [{source.Name}]' has changed. Reinitialization.");
                                proxy.ReInit(source);
                            }                           

                            var numberOfNewRequests = proxy.GetNumberOfTransferRequestsPossibleToStart();

                            if (numberOfNewRequests == 0)
                            {
                                _logger.LogDebug($"The source '{source.SourceID} [{source.Name}]' has no free tasks to do the transfer.");
                                continue;
                            }

                            _logger.LogDebug($"The source '{source.SourceID} [{source.Name}]' can start {numberOfNewRequests} requests.");

                            var requests = _connection.TransferRequestRepository.GetNewRequestsBySourceIDOrderedByPriorityAndScheduledTime(source.SourceID, numberOfNewRequests);

                            if (requests == null || requests.Count() == 0)
                            {
                                _logger.LogDebug($"No new requests for the transfer : {source.SourceID}.");

                            }
                            else
                            {
                                if (!proxy.IsConnected)
                                {
                                    _logger.LogError("Transfer source not connected.");
                                    foreach (var request in requests)
                                    {
                                        _connection.TransferRequestRepository.UpdateRequestOnProgressChange(request.TransferRequestID,
                                            DateTime.Now, request.Status, "Transfer source not connected.");
                                    }
                                }
                                else
                                {
                                    _logger.LogDebug($"{numberOfNewRequests} requests to start for '{source.SourceID} [{source.Name}]'.");

                                    foreach (var request in requests)
                                    {
                                        var sourceDto = new TransferSourceDto(source.Type, source.SourceID, 
                                            source.Name,  source.ConnectionDescription);
                                        var destDto = new TransferDestinationDto();
                                        var requestDto = new TransferRequestDto(request.TransferRequestID,
                                            request.FileID, request.FileHash, 
                                            request.FileName, sourceDto, destDto);

                                        // force to change status from Scheduled to Prepared -- if true transfer can be started
                                        if (MarkTransferRequestAsPrepared(request.TransferRequestID))
                                        {
                                            proxy.StartTransferRequest(requestDto, new TransferProgress(_fileTransferStatusUpdateService.UpdateStatus));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            _logger.LogError("Cannot start transfer");
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (_connection.ConnectionState != System.Data.ConnectionState.Open)
                    {
                        _logger.LogError("Connection lost. Trying to reconnect...");

                        while (!_connection.Open(_settingsService.AppSettings, out string error) && !cancelSource.IsCancellationRequested)
                        {
                            _logger.LogError($"Still connection is closed - [{error}]. The next reconnection attempt will be made after {_settingsService.AppSettings.NewRequestsQueryInterval} ms.");
                            Thread.Sleep(_settingsService.AppSettings.NewRequestsQueryInterval);
                        }

                        _logger.LogInfo($"Connection restored!");
                        continue;
                    }
                    else
                    {
                        _logger.LogException(ex);
                    }
                }

                foreach (Guid key in keys)
                {
                    _transferSources[key].Finit();
                    _transferSources.Remove(key);
                }

                Thread.Sleep(_settingsService.AppSettings.NewRequestsQueryInterval);
            }

            _logger.LogAlways("Stopped processing loop.");
        }

        private bool MarkTransferRequestAsPrepared(Guid aTransferID)
        {
            _logger.LogDebug($"Marking Transfer {aTransferID} as prepared... ");

            try
            {
                _connection.TransferRequestRepository.UpdateRequestOnPrepared(aTransferID,
                    (int)TransferResultStatus.Prepared);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return false;
            }

            _logger.LogDebug($"Transfer {aTransferID} prepared.");

            return true;
        }



        private class TransferSourceItemProxy
        {
            private readonly Dictionary<Guid, TransferJob> _transfers = new Dictionary<Guid, TransferJob>();
            private readonly ITransferSource _transferSource;
            private Source _source;

            public TransferSourceItemProxy(ITransferSource aTransferSource)
            {
                _transferSource = aTransferSource;
            }
          
            public void Init(Source aSource)
            {
                _source = aSource;

                _transferSource.Init(new TransferSourceDto(aSource.Type, aSource.SourceID, 
                    aSource.Name, aSource.ConnectionDescription));
            }

            public void ReInit(Source source)
            {
                _source = source;

                // first cancel all previous transfers
                foreach (var transfer in _transfers)
                {
                    transfer.Value.Cancel("Transfer task was canceled by FileTransfer.Manager, because the connection settings changed during transfering.");
                }

                // end old connection
                _transferSource.Finit();

                // init new 
                _transferSource.Init(new TransferSourceDto(source.Type, source.SourceID, source.Name,  source.ConnectionDescription));
            }

            public bool IsSourceChanged(Source source)
            {
                return false;
                /*
                if (_source.Modified == _source.Modified)
                {
                    return false;
                }

                return true;
                */
            }

            public void Finit()
            {
                _transferSource.Finit();
            }

            public bool IsConnected
            {
                get { return _transferSource.IsConnected; }
            }

            public void StartTransferRequest(TransferRequestDto aRequest, ITransferProgress aTransferProgress)
            {
                var tokenSource = new CancellationTokenWithReason();
                var task = _transferSource.StartTransferRequest(aRequest, aTransferProgress, tokenSource);

                _transfers.Add(aRequest.ID, new TransferJob(task, tokenSource));
            }



            public int GetNumberOfTransferRequestsPossibleToStart()
            {
                // keep only 'inProgress' tasks
                foreach (var transfer in _transfers.ToList())
                {
                    if (transfer.Value.Job.IsCompleted)
                    {
                        _transfers.Remove(transfer.Key);
                    }
                }

                return _transferSource.MaxParallelNumberOfTransferRequests - _transfers.Count;
            }
       
            private class TransferJob
            {
                private readonly CancellationTokenWithReason cancellationTokenSource;

                public Task<ITransferResult> Job { get; }

                public TransferJob(Task<ITransferResult> job, CancellationTokenWithReason cancellationTokenSource)
                {
                    this.cancellationTokenSource = cancellationTokenSource;
                    this.Job = job;
                }

                public void Cancel(string reason)
                {
                    this.cancellationTokenSource.Cancel(reason);
                }
            }
        }

    }
}
