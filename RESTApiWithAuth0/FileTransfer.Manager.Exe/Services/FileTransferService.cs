using FileTransfer.Definitions;
using FileTransfer.Manager.Exe.Models;
using FileTransfer.Manager.Persistence.Connection;
using FileTransfer.Manager.Persistence.Entities;
using FileTransfer.WebAPI.Definitions.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Exe.Services
{
    public interface IFileTransferService
    {
        Task<Guid> TransferCreate(FileTransferCreateDto<SystemFileID> aReqest);
        Task<FileTransferDto> TransferGet(Guid aID);
        Task<List<FileTransferDto>> TransferGet();
    }

    public class FileTransferService :
        IFileTransferService
    {
        IConnection _connection;
        SemaphoreSlim _connectionSingleThreadAccesss;
        ILogger<FileTransferService> _logger;

        public FileTransferService(ILogger<FileTransferService> aLogger, IConnectionFactory aConnectionFactory)
        {
            _logger = aLogger;
            _connectionSingleThreadAccesss = new SemaphoreSlim(1, 1);
            _connection = aConnectionFactory.CreateConnection();          
        }

        public async Task<Guid> TransferCreate(FileTransferCreateDto<SystemFileID> aReqest)
        {
            TransferRequest tr = null;

            try
            {
                await _connectionSingleThreadAccesss.WaitAsync().ConfigureAwait(false);

                tr = _connection.TransferRequestRepository.RequestStart(
                      new Persistence.Repositories.RequestStartDto()
                      {
                          SourceID = aReqest.FileID.SystemID,
                          FileID = aReqest.FileID.FileID
                      });

            }
            catch (Exception ex)
            {
                _logger?.LogError("{0}", ex);
            }
            finally
            {
                _connectionSingleThreadAccesss.Release();
            }

            if (tr != null)
                return tr.TransferRequestID;

            return Guid.Empty;
        }
      
        public async Task<FileTransferDto> TransferGet(Guid aID)
        {
            TransferRequest tr = null;

            try
            {
                await _connectionSingleThreadAccesss.WaitAsync().ConfigureAwait(false);

                tr = _connection.TransferRequestRepository.GetById(aID);
            }
            catch (Exception ex)
            {
                _logger?.LogError("{0}", ex);
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

        public Task<List<FileTransferDto>> TransferGet()
        {
            return Task.FromResult(new List<FileTransferDto>());
        }    
    }
}
