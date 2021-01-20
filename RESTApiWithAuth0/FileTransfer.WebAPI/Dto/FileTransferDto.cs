using FileTransfer.Definitions;
using System;

namespace FileTransfer.WebAPI.Definitions.Dto
{

    public class FileTransferDto
    {
        public Guid TransferRequestID { get; set; }

        public TransferResultStatus Status { get; set; }

        public string Description { get; set; }

        public string Result { get; set; }
    }
}
