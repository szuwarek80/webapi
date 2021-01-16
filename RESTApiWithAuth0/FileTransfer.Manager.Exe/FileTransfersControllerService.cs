using FileTransfer.Definitions;
using FileTransfer.Manager.Persistence.Connection;
using FileTransfer.Manager.Persistence.Entities;
using FileTransfer.WebAPI.Dto;
using FileTransfer.WebAPI.Services;
using Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Exe
{
    public class FileTransfersControllerServiceImpl :
        FileTransfersControllerService
    {
        Logger<FileTransfersControllerServiceImpl> _logger;
        IConnection _connection;
        SemaphoreSlim _connectionSingleThreadAccesss;

        public FileTransfersControllerServiceImpl(ConnectionFactory aConnectionFactory)
        {
            _connectionSingleThreadAccesss = new SemaphoreSlim(1, 1);
            _connection = aConnectionFactory.CreateConnection();
            _logger = new Logger<FileTransfersControllerServiceImpl>();
        }

        public override async Task<Guid> CreateFileTransfer(CreateFileTransferDto aRequest)
        {            
            TransferRequest tr = null;

            try
            {
                await _connectionSingleThreadAccesss.WaitAsync().ConfigureAwait(false);

                string[] ids = aRequest.FileID.Split(":");
                tr = _connection.TransferRequestRepository.RequestStart(
                      new Persistence.Repositories.RequestStartDto()
                      {
                          SourceID = Guid.Parse(ids[0]),
                          FileID = ids[1]
                      });

            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                _connectionSingleThreadAccesss.Release();
            }

            if (tr != null)
                return tr.TransferRequestID;

            return Guid.Empty;
        }

        public override async Task<FileTransferDto> GetFileTransferStatus(Guid aID)
        {
            TransferRequest tr = null;

            try
            {
                await _connectionSingleThreadAccesss.WaitAsync().ConfigureAwait(false);

                tr = _connection.TransferRequestRepository.GetById(aID);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                _connectionSingleThreadAccesss.Release();
            }

            if (tr != null)
                return new FileTransferDto()
                {
                    TransferRequestID = tr.TransferRequestID,
                    Status = (TransferResultStatus)tr.Status,
                    Description = tr.Description,
                    Result = tr.Result
                };

            return null;
        }

        public override Task<bool> DeleteTransfer(Guid aID, out bool aForbidden)
        {
            aForbidden = true;
            return Task.FromResult(false);
        }

    }
}
