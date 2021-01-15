using System;
using System.Collections.Generic;
using System.Text;

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
