using FileTransfer.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.WebAPI.Services
{
    public abstract class FileTransfersControllerService
    {
        public abstract Task<Guid> CreateFileTransfer(CreateFileTransferDto aRequest);
        public abstract Task<FileTransferDto> GetFileTransferStatus(Guid aID);
        public abstract Task<bool> DeleteTransfer(Guid aID, out bool aForbidden);
    }
}
