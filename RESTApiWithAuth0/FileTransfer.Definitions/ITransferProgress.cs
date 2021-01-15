using System;

namespace FileTransfer.Definitions
{
    public interface ITransferProgress
    {
        void Report(Guid aTransferID, 
            TransferResultStatus aCurrentStatus, 
            DateTime aDate, 
            string aDescription = null);
    }
}
