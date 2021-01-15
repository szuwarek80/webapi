using FileTransfer.Manager.Persistence.Entities;
using FileTransfer.Manager.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FileTransfer.Manager.Persistence.Connection
{
    public interface IConnection : IDisposable
    {
        ConnectionState ConnectionState { get; }
        bool Open(IConnectionSettings aSettings, out string aError);
        void Close();

        ITransferRequestRepository<TransferRequest> TransferRequestRepository { get; }
        ISourceRepository<Source> SourceRepository { get; }
    }
}
