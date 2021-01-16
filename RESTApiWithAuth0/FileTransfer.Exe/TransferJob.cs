using FileTransfer.Definitions;
using FileTransfer.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileTransfer.Exe
{
    public interface ITransferJobExecutor
    {
        void Start();
        TransferResultStatus GetStatus(out string aResult, out string aDescription);
    }

    public class DummyTransferJobExecutor :
        ITransferJobExecutor
    {
        string _description;
        string _result;
        TransferResultStatus _status;

        public DummyTransferJobExecutor()
        {
            _status = TransferResultStatus.Prepared;
        }

        public void Start()
        {
            _status = TransferResultStatus.Started;
            Task.Run(async () =>
            {
                _status = TransferResultStatus.InProgress;
                for (int i = 1; i < 10; i++)
                {
                    _description = $"{i}0 %";
                    await Task.Delay(5000);
                }

                _result = "file relative path against the ROOT of the fstp/sftp";
                _status = TransferResultStatus.Success;
                _description = "Done";
            });
        }

        public TransferResultStatus GetStatus(out string aResult, out string aDescription)
        {
            aResult = _result;
            aDescription = _description;
            return _status;
        }
    }

    public class TransferJob
    {
        public Guid ID { get;  }
        public ITransferJobExecutor Executor { get; }
        public TransferJob(string aFileID)
        {
            this.ID = Guid.NewGuid();
            this.Executor = new DummyTransferJobExecutor();
        }
    }
}
