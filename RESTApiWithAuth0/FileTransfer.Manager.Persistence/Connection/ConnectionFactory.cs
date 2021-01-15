
namespace FileTransfer.Manager.Persistence.Connection
{
    public class ConnectionFactory :
        IConnectionFactory
    {
        public IConnection CreateConnection()
        {
            return new ConnectionDummy();
        }
     
    }
}
