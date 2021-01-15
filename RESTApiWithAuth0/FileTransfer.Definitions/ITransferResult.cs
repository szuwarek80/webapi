using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Definitions
{
    public enum TransferResultStatus
    {
        Scheduled = 0,
        Prepared = 1,
        Started = 2,
        InProgress = 3,
        Error = 4,
        Success = 5,
        Cancelled = 6,
    }

    public interface ITransferResult
    {
        Guid TransferID { get; }
        TransferResultStatus Status { get; }
        string Result { get; }
        DateTime Finished { get; }
        Exception Error { get; }
    }
}
