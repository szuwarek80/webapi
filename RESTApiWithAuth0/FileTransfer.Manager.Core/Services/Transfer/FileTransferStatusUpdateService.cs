using FileTransfer.Definitions;
using FileTransfer.Manager.Persistence.Connection;
using Shared.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Core.Services.Transfer
{
    public class FileTransferStatusUpdateService : 
        IFileTransferStatusUpdateService
    {
        private readonly ConcurrentQueue<StatusDto> _statusQueue = new ConcurrentQueue<StatusDto>();

        private readonly SyncEvents _syncEvents = new SyncEvents();

        private readonly Logger<FileTransferStatusUpdateService> _logger = new Logger<FileTransferStatusUpdateService>();

        private readonly IConnection _databseConnection;

        private readonly IConnectionSettings _connectionSettings;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Task _serviceTask;

        public FileTransferStatusUpdateService(IConnectionFactory aConnectionFactory, IConnectionSettings aConnectionSettings)
        {
            _databseConnection = aConnectionFactory.CreateConnection();
            _connectionSettings = aConnectionSettings;
        }

        public void UpdateStatus(Guid aTransferID, TransferResultStatus aCurrentStatus,
            DateTime aDate, string aDescription)
        {
            _statusQueue.Enqueue(new StatusDto
            {
                TransferID = aTransferID,
                CurrentStatus = aCurrentStatus,
                Date = aDate,
                Description = aDescription
            });

            _syncEvents.NewItemEvent.Set();
        }

        public void Start()
        {
            _logger.LogInfo("Starting service...");

            _serviceTask = Task.Factory.StartNew(() => this.ServiceLoop(), 
                _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            _logger.LogInfo("Service started.");
        }

        public void Stop()
        {
            _logger.LogInfo("Stopping service...");

            _syncEvents.ExitThreadEvent.Set();
            _serviceTask?.Wait();
            _databseConnection.Close();

            _logger.LogAlways("Service stoped.");
        }

        private void ServiceLoop()
        {
            if (!_databseConnection.Open(_connectionSettings, out string error))
            {
                // if there was an error, the next try to connect will be done during update
                _logger.LogError($"Could not open database connection: [{error}].");
            }

            try
            {
                while (WaitHandle.WaitAny(_syncEvents.EventArray) != SyncEvents.EXIT)
                {
                    while (_statusQueue.Count > 0)
                    {
                        if (_statusQueue.TryDequeue(out StatusDto recived))
                        {
                            try
                            {
                                _logger.LogDebug(recived.ToString());
                                this.UpdateStatus(recived);
                            }
                            catch (Exception ex)
                            {
                                if (_databseConnection.ConnectionState != System.Data.ConnectionState.Open)
                                {
                                    // inProgress status can be ommited (to avoid progress decrease)
                                    if (recived.CurrentStatus != TransferResultStatus.InProgress)
                                    {
                                        _statusQueue.Enqueue(recived);
                                    }

                                    _logger.LogError("Connection lost. Trying to reconnect...");

                                    while (!_databseConnection.Open(_connectionSettings, out error))
                                    {
                                        _logger.LogError($"Still connection is closed - [{error}]. The next reconnection attempt will be made after {_connectionSettings.NewRequestsQueryInterval} ms.");
                                        Thread.Sleep(_connectionSettings.NewRequestsQueryInterval);

                                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    }

                                    _logger.LogInfo($"Connection restored!");
                                }
                                else
                                {
                                    _logger.LogException(ex, $"Error during update the item: {recived}");
                                }
                            }
                        }
                    }
                }

                // be nice and log all queued items. 
                while (_statusQueue.Count > 0)
                {
                    if (_statusQueue.TryDequeue(out StatusDto recived))
                    {
                        this.UpdateStatus(recived);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_statusQueue.Count > 0)
                {
                    _logger.LogError($"Not all queued items was sucesffully saved in the database.");
                    _logger.LogException(ex);
                }
            }
        }

        private void UpdateStatus(StatusDto statusDto)
        {
            switch (statusDto.CurrentStatus)
            {
                case TransferResultStatus.Started:
                    _databseConnection.TransferRequestRepository.UpdateRequestOnStart(statusDto.TransferID, 
                        statusDto.Date, (int)statusDto.CurrentStatus);
                    break;
                case TransferResultStatus.InProgress:
                    _databseConnection.TransferRequestRepository.UpdateRequestOnProgressChange(statusDto.TransferID, 
                        statusDto.Date, (int)statusDto.CurrentStatus, statusDto.Description);
                    break;
                case TransferResultStatus.Success:
                    _databseConnection.TransferRequestRepository.UpdateRequestOnFinish(statusDto.TransferID, statusDto.Date,
                        (int)statusDto.CurrentStatus, statusDto.Description, "");
                    break;
                case TransferResultStatus.Error:
                    _databseConnection.TransferRequestRepository.UpdateRequestOnFinish(statusDto.TransferID, statusDto.Date,
                        (int)statusDto.CurrentStatus,"", statusDto.Description);
                    break;

            }
        }
    }

    public class StatusDto
    {
        public Guid TransferID { get; set; }
        public TransferResultStatus CurrentStatus { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{TransferID} - {CurrentStatus} - {Date} - {Description}";
        }
    }

    class SyncEvents
    {
        public const int NEW_ITEM = 0;
        public const int EXIT = 1;

        public SyncEvents()
        {
            EventArray[0] = NewItemEvent;
            EventArray[1] = ExitThreadEvent;
        }

        public EventWaitHandle NewItemEvent { get; } = new AutoResetEvent(false);
        public EventWaitHandle ExitThreadEvent { get; } = new ManualResetEvent(false);
        public WaitHandle[] EventArray { get; } = new WaitHandle[2];
    }
}
