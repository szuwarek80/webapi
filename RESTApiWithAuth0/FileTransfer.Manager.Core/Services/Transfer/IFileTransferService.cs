
namespace FileTransfer.Manager.Core.Services
{
    public interface IFileTransferService
    {
        bool Start(out string aError);
        void Stop();
    }
}
