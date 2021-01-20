using System;

namespace FileTransfer.Definitions.BaseImplementation
{
    public class TransferProgress : ITransferProgress
    {
        private Action<Guid, TransferResultStatus, DateTime, string> _informAction;

        public TransferProgress(Action<Guid, TransferResultStatus, DateTime, string> aInformAction)
        {
            _informAction = aInformAction;
        }

        public void Report(Guid aTransferID, TransferResultStatus aCurrentStatus, DateTime aDate, string aDescription = null)
        {
            _informAction?.Invoke(aTransferID, aCurrentStatus, aDate, aDescription);
        }
    }
}
