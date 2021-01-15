using FileTransfer.Definitions;
using FileTransfer.Definitions.BaseImplementation;
using FileTransfer.Definitions.Dto;
using FileTransfer.WebAPI.Dto;
using Shared.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Sources.FileTransferWebAPI
{
    public class TransferSource :
        ITransferSource
    {
        private readonly Logger<TransferSource> _logger;
        private TransferSourceDto _source;
        private FileTransferWebAPIAccess _fileTransferWebAPIAccess;
        private int _statusRequestInterval;
        public TransferSourceType SourceType =>  TransferSourceType.FileTransferWebAPI;

        public bool IsConnected { get; }

        public int MaxParallelNumberOfTransferRequests => 2;

        public TransferSource(int aStatusRequestInterval)
        {
            this.IsConnected = true;
            _logger = new Logger<TransferSource>();
            _statusRequestInterval = aStatusRequestInterval;
        }

        public void Finit()
        {
            if (_source != null)
            {
                _logger.LogAlways($"FileTransferWebAPI {_source.ID} - {_source.Name} closed.");
                _source = null;
            }
        }

        public void Init(TransferSourceDto aSource)
        {
            _source = aSource;
            _fileTransferWebAPIAccess = new FileTransferWebAPIAccess(aSource);
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

                string url = string.Empty;
                string transferJobGuid = string.Empty;

                try
                {
                    aTransferProgress.Report(aRequest.ID, TransferResultStatus.Started, DateTime.Now);
                    //--- PREPARING FOR TRANSFER REQUEST
                    cancellationToken.ThrowIfCancellationRequested();
                    aTransferProgress.Report(aRequest.ID, TransferResultStatus.InProgress, DateTime.Now, "Sending the transfer request...");

                    //--- ADD API KEY
                    //client.DefaultRequestHeaders.Add(ApiKeyAuthenticationHandler.ApiKeyHeaderName, ApiKey.KEY);

                    //--- SENDING POST TO CREATE TRANSFER REQUEST
                    transferJobGuid = await _fileTransferWebAPIAccess.SendTransferRequest(client, aRequest);

                    //--- SENDING GET TO UPDATE TRANSFER STATUS UNTIL THE STATUS WILL BE SET AS FINISHED
                    TransferStatusDto finalResult = null;

                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        TransferStatusDto statusResponse = await _fileTransferWebAPIAccess.SendTransferStatusRequest(client, transferJobGuid);

                        if (statusResponse.Status == TransferResultStatus.Success || statusResponse.Status == TransferResultStatus.Cancelled || statusResponse.Status == TransferResultStatus.Error)
                        {
                            finalResult = statusResponse;
                            break;
                        }

                        var statusDescription = string.IsNullOrEmpty(statusResponse.Description) ? "Transfer in progress..." : statusResponse.Description;
                        aTransferProgress.Report(aRequest.ID, statusResponse.Status, DateTime.Now, statusDescription);

                        _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} updated the transfer {statusResponse.TransferRequestID} progress: {statusResponse.Status}, {statusDescription}.");

                        await Task.Delay(_statusRequestInterval, cancellationToken);

                    } while (true);

                    aTransferProgress.Report(aRequest.ID, finalResult.Status, DateTime.Now, finalResult.Description);

                    _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} finished successfully the transfer {aRequest.ID}.");

                    result = new TransferResult(aRequest.ID, finalResult.Status, DateTime.Now, finalResult.Description);

                    try
                    {
                        await _fileTransferWebAPIAccess.SendTransferDeleteRequest(client, transferJobGuid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex);
                    }
                }
                catch (OperationCanceledException)
                {
                    if (!string.IsNullOrEmpty(transferJobGuid))
                    {
                        await _fileTransferWebAPIAccess.SendTransferCancelRequest(client, transferJobGuid);
                    }

                    aTransferProgress.Report(aRequest.ID, TransferResultStatus.Cancelled, DateTime.Now, aCancellationToken.CancellationReason);

                    _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} cancelled the transfer {aRequest.ID}.{aCancellationToken.CancellationReason}");

                    result = new TransferResult(aRequest.ID, TransferResultStatus.Cancelled, DateTime.Now, aCancellationToken.CancellationReason);
                }
                catch (Exception ex)
                {
                    if (aCancellationToken.IsCancellationRequested)
                    {
                        if (!string.IsNullOrEmpty(transferJobGuid))
                        {
                            await client.PutAsync($"{url}/api/transfers/{aRequest.ID}/cancel", null);
                        }

                        aTransferProgress.Report(aRequest.ID, TransferResultStatus.Cancelled, DateTime.Now, aCancellationToken.CancellationReason);

                        _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} cancelled the transfer {aRequest.ID}. {aCancellationToken.CancellationReason}");

                        result = new TransferResult(aRequest.ID, TransferResultStatus.Cancelled, DateTime.Now, aCancellationToken.CancellationReason);
                    }
                    else
                    {
                        aTransferProgress.Report(aRequest.ID, TransferResultStatus.Error, DateTime.Now, ex.Message);

                        _logger.LogException(ex, $"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} catched an exception: {ex.Message} during the transfer {aRequest.ID}.");

                        result = new TransferResult(aRequest.ID, TransferResultStatus.Error, DateTime.Now, string.Empty, ex);
                    }
                }
                finally
                {
                    client.Dispose();
                }

                _logger.LogDebug($"FileTransferWebAPI {_fileTransferWebAPIAccess.ID} finished the transfer {aRequest.ID}.");

                return result;
            }, cancellationToken);
        }
    }
}
