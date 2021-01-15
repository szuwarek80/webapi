using FileTransfer.Definitions;
using System;

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
