using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Persistence.Connection
{
    public class ConnectionFactory :
        IConnectionFactory
    {
        public IConnection CreateConnection()
        {
            //return new DatabaseConnection();
            return new ConnectionDummy();
        }
     
    }
}
