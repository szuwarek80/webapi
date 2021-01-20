namespace FileTransfer.Exe.Transfer
{
    public interface IFileTransferJobExecutorFactory
    {
        IFileTransferJobExecutor CreateExecutor();
    }
}
