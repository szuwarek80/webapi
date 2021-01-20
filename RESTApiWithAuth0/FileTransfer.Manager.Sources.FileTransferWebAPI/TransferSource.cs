using FileTransfer.Definitions;
using FileTransfer.Definitions.BaseImplementation;
using FileTransfer.Definitions.Dto;
using FileTransfer.WebAPI.Definitions.Dto;
using Shared.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Sources.FileTransferWebAPI
{
    public class TransferSource :
        ITransferSource
    {
        protected readonly IFileTransferWebAPIAccessFactory _fileTransferWebAPIAccessFactory;
        protected  readonly Logger<TransferSource> _logger;
        protected readonly int _statusRequestInterval;

        private TransferSourceDto _source;
        private IFileTransferWebAPIAccess _fileTransferWebAPIAccess;


        public TransferSourceType SourceType =>  TransferSourceType.FileTransferWebAPI;

        public bool IsConnected { get { return (_fileTransferWebAPIAccess == null? false: _fileTransferWebAPIAccess.IsConnected);} }

        public int MaxParallelNumberOfTransferRequests => 2;

        public TransferSource(int aStatusRequestInterval, IFileTransferWebAPIAccessFactory aIFileTransferWebAPIAccessFactory)
        {
            _logger = new Logger<TransferSource>();
            _statusRequestInterval = aStatusRequestInterval;
            _fileTransferWebAPIAccessFactory = aIFileTransferWebAPIAccessFactory;
        }

        public void Finit()
        {
            if (_source != null)
            {
                _logger.LogAlways($"FileTransferWebAPI {_source.ID} - {_source.Name} closed.");
                _source = null;
            }
            
            _fileTransferWebAPIAccess?.Dispose();
            _fileTransferWebAPIAccess = null;
        }

        public void Init(TransferSourceDto aSource)
        {
            _source = aSource;

            _fileTransferWebAPIAccess?.Dispose();

            _fileTransferWebAPIAccess = _fileTransferWebAPIAccessFactory.Create(aSource);

            _logger.LogAlways($"FileTransferWebAPI {aSource.ID} - {aSource.Name} initialized.");
        }

        public Task<ITransferResult> StartTransferRequest(TransferRequestDto aRequest, 
            ITransferProgress aTransferProgress, CancellationTokenWithReason aCancellationToken)
        {
            var cancellationToken = aCancellationToken.Token;

            return Task.Run(async () =>
            {
                _logger.LogDebug($"FileTransferWebAPI {aRequest.Source.ID} - {aRequest.Source.Name} started the transfer {aRequest.ID}.");

                ITransferResult result = null;
                var client = new HttpClient();
                Guid transferJobGuid = Guid.Empty;

                try
                {
                    aTransferProgress.Report(aRequest.ID, TransferResultStatus.Started, DateTime.Now);
                    //--- PREPARING FOR TRANSFER REQUEST
                    cancellationToken.ThrowIfCancellationRequested();

                    //--- SENDING CREATE TRANSFER REQUEST
                    aTransferProgress.Report(aRequest.ID, TransferResultStatus.InProgress, DateTime.Now, "Sending the transfer request...");

                    transferJobGuid = await _fileTransferWebAPIAccess.TransferCreate(new FileTransferCreateDto<string>()
                            { 
                                FileID = aRequest.FileID
                            });

                    FileTransferDto fileTransfer = null;                   
                    //--- SENDING GET TRANSFER REQUEST TO UPDATE THE TRANSFER SATUS 
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        fileTransfer = await _fileTransferWebAPIAccess.TransferGet(transferJobGuid);

                        if (fileTransfer.Status == TransferResultStatus.Success)
                        {
                            aTransferProgress.Report(aRequest.ID, fileTransfer.Status, DateTime.Now, fileTransfer.Result);
                            break;
                        }
                        else if (fileTransfer.Status == TransferResultStatus.Error)
                        {
                            aTransferProgress.Report(aRequest.ID, fileTransfer.Status, DateTime.Now, fileTransfer.Description);
                            break;
                        }

                        var statusDescription = string.IsNullOrEmpty(fileTransfer.Description) ? "Transfer in progress..." : fileTransfer.Description;
                        aTransferProgress.Report(aRequest.ID, fileTransfer.Status, DateTime.Now, statusDescription);

                        _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} updated the transfer {fileTransfer.TransferRequestID} progress: {fileTransfer.Status}, {statusDescription}.");

                        await Task.Delay(_statusRequestInterval, cancellationToken);

                    } while (true);

                    _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} finished successfully the transfer {aRequest.ID}.");

                    result = new TransferResult(aRequest.ID, fileTransfer.Status, DateTime.Now, fileTransfer.Result);

                    try
                    {
                        await _fileTransferWebAPIAccess.TransferRemove( transferJobGuid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex);
                    }
                }
                catch (OperationCanceledException)
                {
                    if (transferJobGuid != Guid.Empty)
                    {
                        await _fileTransferWebAPIAccess.TransferRemove(transferJobGuid);
                    }

                    aTransferProgress.Report(aRequest.ID, TransferResultStatus.Error, DateTime.Now, aCancellationToken.CancellationReason);

                    _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} cancelled the transfer {aRequest.ID}.{aCancellationToken.CancellationReason}");

                    result = new TransferResult(aRequest.ID, TransferResultStatus.Error, DateTime.Now, aCancellationToken.CancellationReason);
                }
                catch (Exception ex)
                {
                    if (aCancellationToken.IsCancellationRequested)
                    {
                        if (transferJobGuid != Guid.Empty)
                        {
                            await _fileTransferWebAPIAccess.TransferRemove(transferJobGuid);
                        }

                        aTransferProgress.Report(aRequest.ID, TransferResultStatus.Error, DateTime.Now, aCancellationToken.CancellationReason);

                        _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} cancelled the transfer {aRequest.ID}. {aCancellationToken.CancellationReason}");

                        result = new TransferResult(aRequest.ID, TransferResultStatus.Error, DateTime.Now, aCancellationToken.CancellationReason);
                    }
                    else
                    {
                        aTransferProgress.Report(aRequest.ID, TransferResultStatus.Error, DateTime.Now, ex.Message);

                        _logger.LogException(ex, $"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} catched an exception: {ex.Message} during the transfer {aRequest.ID}.");

                        result = new TransferResult(aRequest.ID, TransferResultStatus.Error, DateTime.Now, string.Empty, ex);
                    }
                }               

                _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} finished the transfer {aRequest.ID}.");

                return result;
            }, cancellationToken);
        }
    }
}
