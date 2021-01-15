using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Persistence.Connection
{
    public interface IConnectionFactory
    {
        IConnection CreateConnection();
    }
}
