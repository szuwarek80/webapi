using FileTransfer.Definitions;

namespace FileTransfer.Manager.Sources.FileTransferWebAPI
{
    public class TransferSourceFactory :
        ITransferSourceFactory
    {
        private int _statusRequestInterval;

        public TransferSourceFactory(int aStatusRequestInterval)
        {
            _statusRequestInterval = aStatusRequestInterval;
        }

        public ITransferSource CreateTransferSource()
        {
            return new TransferSource(_statusRequestInterval, new FileTransferWebAPIAccessFactory());
        }
    }
}
