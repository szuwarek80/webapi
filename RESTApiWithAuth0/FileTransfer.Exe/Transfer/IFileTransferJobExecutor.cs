using FileTransfer.WebAPI.Definitions.Dto;

namespace FileTransfer.Exe.Transfer
{
    public interface IFileTransferJobExecutor
    {
        void Start();
        FileTransferDto GetStatus();
    }
}
