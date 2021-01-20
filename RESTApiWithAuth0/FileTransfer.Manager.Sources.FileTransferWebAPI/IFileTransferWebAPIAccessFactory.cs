using FileTransfer.Definitions.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Sources.FileTransferWebAPI
{
    public interface IFileTransferWebAPIAccessFactory
    {
        IFileTransferWebAPIAccess Create(TransferSourceDto aSource);
    }
}
