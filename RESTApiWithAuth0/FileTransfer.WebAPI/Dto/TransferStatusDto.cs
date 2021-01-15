using FileTransfer.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.WebAPI.Dto
{
    /*
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
    */
    public class TransferStatusDto
    {
        public Guid TransferRequestID { get; set; }

        public TransferResultStatus Status { get; set; }

        public string Description { get; set; }
    }
}
