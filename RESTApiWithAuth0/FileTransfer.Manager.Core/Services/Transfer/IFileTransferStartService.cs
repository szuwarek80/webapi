
namespace FileTransfer.Manager.Core.Services.Transfer
{
    public interface IFileTransferStartService
    {
        bool Start(out string aError);
        void Stop();
    }
}
