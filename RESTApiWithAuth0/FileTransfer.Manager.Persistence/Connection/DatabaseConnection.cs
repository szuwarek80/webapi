using FileTransfer.Manager.Persistence.Entities;
using FileTransfer.Manager.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FileTransfer.Manager.Persistence.Connection
{
    public class DatabaseConnection :
        IConnection
    {
     
        public DatabaseConnection()
        {
            this.TransferRequestRepository = new TransferRequestRepository();
            this.SourceRepository = new SourceRespository();
        }

        public ConnectionState ConnectionState { get; }
        public ITransferRequestRepository<TransferRequest> TransferRequestRepository { get; }
        public ISourceRepository<Source> SourceRepository { get; }

        public void Close()
        {
            //dummy
            //ToDo
        }

        public void Dispose()
        {
            //ToDo
        }

        public bool Open(IConnectionSettings aSettings, out string aError)
        {
            aError = string.Empty;

            //dummy
            //ToDo
            return true;
        }



    }
}
