using FileTransfer.Manager.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Persistence.Repositories
{
    public class TransferRequestRepository : Repository<TransferRequest>, ITransferRequestRepository<TransferRequest>
    {
        protected override string TableName => "TransferRequest";

        protected override string TableIDColumnName => "TransferRequestID";

        public override Guid Insert(TransferRequest entity)
        {
            //ToDo
            return Guid.NewGuid();
            throw new NotImplementedException();
        }

        public override void Update(TransferRequest entity)
        {
            //ToDo
        }

        public IEnumerable<TransferRequest> GetNewRequestsBySourceIDOrderedByPriorityAndScheduledTime(Guid sourceID, int maxItems = 1)
        {
            var result = new List<TransferRequest>();

            //ToDo

            return result;
        }


        public void UpdateRequestOnPrepared(Guid aTransferRequestID, int aStatus)
        {

        }

        public void UpdateRequestOnStart(Guid aTransferRequestID, DateTime aStarted, int aStatus)
        { 
            //ToDo
        }
        
        public void UpdateRequestOnProgressChange(Guid aTransferRequestID, DateTime aUpdated, int aStatus, string aDescription)
        {
            //ToDo
        }
        
        public void UpdateRequestOnFinish(Guid aTransferRequestID, DateTime aFinished, int aStatus, string aResult, string aDescription)
        {
            //ToDo
        }


        public TransferRequest RequestStart(RequestStartDto aData)
        {
            return new TransferRequest();
        }

        public TransferRequest RequestStartForce(RequestStartDto aData)
        {
            return new TransferRequest();
        }

        public void RequestCancel(Guid aTransferRequestID, string aScheduledBy)
        { 
        }

    }
}
