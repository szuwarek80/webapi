using FileTransfer.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

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
            return new TransferSource(_statusRequestInterval);
        }
    }
}
