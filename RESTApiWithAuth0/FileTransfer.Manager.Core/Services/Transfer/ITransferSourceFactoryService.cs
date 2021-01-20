using FileTransfer.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Core.Services.Transfer
{
    public interface ITransferSourceFactoryService
    {
        ITransferSource CreateTransferSource(TransferSourceType aType);
    }
}
