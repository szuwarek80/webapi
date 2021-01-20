using FileTransfer.Definitions.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Sources.FileTransferWebAPI
{
    class FileTransferWebAPIAccessFactory :
        IFileTransferWebAPIAccessFactory
    {
        public IFileTransferWebAPIAccess Create(TransferSourceDto aSource)
        {
            return new FileTransferWebAPIAccess(aSource);
        }
    }
}
