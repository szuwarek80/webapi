using FileTransfer.Definitions;
using FileTransfer.WebAPI.Definitions.Dto;
using System.Threading.Tasks;

namespace FileTransfer.Exe.Transfer
{
    public class FileTransferJobExecutorFactory :
        IFileTransferJobExecutorFactory
    {
        public IFileTransferJobExecutor CreateExecutor()
        {
            return new DummyFileTransferJobExecutor();
        }
    }


    public class DummyFileTransferJobExecutor :
      IFileTransferJobExecutor
    {
        FileTransferDto _status;

        public DummyFileTransferJobExecutor()
        {
            _status = new FileTransferDto() { Status = TransferResultStatus.Prepared };
        }

        public void Start()
        {
            _status.Status = TransferResultStatus.Started;
            Task.Run(async () =>
            {
                _status.Status = TransferResultStatus.InProgress;
                for (int i = 1; i < 10; i++)
                {
                    _status.Description = $"{i}0 %";
                    await Task.Delay(5000);
                }

                _status.Result = "file relative path against the ROOT of the fstp/sftp";
                _status.Status = TransferResultStatus.Success;
                _status.Description = "Done";
            });
        }

        public FileTransferDto GetStatus()
        {
            return _status;
        }
    }
}
