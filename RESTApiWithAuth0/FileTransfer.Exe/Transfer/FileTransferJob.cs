using FileTransfer.Definitions;
using FileTransfer.WebAPI.Definitions.Dto;
using System;

namespace FileTransfer.Exe.Transfer
{
    public class FileTransferJob 
    {
        protected readonly IFileTransferJobExecutorFactory _executorFactory;
        protected IFileTransferJobExecutor _executor;

        public Guid ID { get;  }
        
        public FileTransferJob(string aFileID, IFileTransferJobExecutorFactory aFileTransferJobExecutorFactory)
        {
            this.ID = Guid.NewGuid();
            _executorFactory = aFileTransferJobExecutorFactory;
            _executor = null;
        }

        public void Start()
        {
            if (_executor == null)
            {
                _executor = _executorFactory.CreateExecutor();
                _executor.Start();
            }
        }

        public FileTransferDto GetStatus()
        {
            var result = _executor?.GetStatus();

            if (result == null)
                return new FileTransferDto()
                {
                    TransferRequestID = this.ID,
                    Status = TransferResultStatus.Scheduled
                };

            result.TransferRequestID = this.ID;
            return result;
        }

    }
}
