using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Core.Services
{
    public interface IFileTransferService
    {
        bool Start(out string aError);
        void Stop();
    }
}
