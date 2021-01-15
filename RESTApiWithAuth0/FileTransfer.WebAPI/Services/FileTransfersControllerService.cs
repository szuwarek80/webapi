using FileTransfer.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.WebAPI.Services
{
    public abstract class FileTransfersControllerService
    {
        public abstract Guid CreateFileTransfer(CreateTransferDto aRequest);
        public abstract TransferStatusDto GetFileTransferStatus(Guid aID);
        public abstract bool DeleteTransfer(Guid aID, out bool aForbidden);
    }
}
