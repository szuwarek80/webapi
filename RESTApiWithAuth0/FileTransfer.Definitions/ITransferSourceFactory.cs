using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Definitions
{
    public interface ITransferSourceFactory
    {
        ITransferSource CreateTransferSource();
    }
}
