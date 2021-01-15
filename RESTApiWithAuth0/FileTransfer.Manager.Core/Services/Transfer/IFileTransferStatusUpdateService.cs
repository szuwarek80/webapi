using FileTransfer.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Core.Services.Transfer
{
    public interface IFileTransferStatusUpdateService
    {
        void Start();
        void Stop();
        void UpdateStatus(Guid aTransferID, TransferResultStatus aCurrentStatus,
            DateTime aDate, string aDescription);
    }
}
