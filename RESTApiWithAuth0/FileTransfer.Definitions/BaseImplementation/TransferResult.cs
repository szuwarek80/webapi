using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Definitions.BaseImplementation
{
    public class TransferResult : ITransferResult
    {
        public TransferResult(Guid aTransferID, TransferResultStatus aStatus, DateTime aFinished, string aResult = null, Exception aError = null)
        {
            this.TransferID = aTransferID;
            this.Status = aStatus;
            this.Finished = aFinished;
            this.Result = aResult;
            this.Error = aError;
        }

        public Guid TransferID { get; }

        public TransferResultStatus Status { get; }

        public string Result { get; }

        public DateTime Finished { get; }

        public Exception Error { get; }
    }
}
