using FileTransfer.Manager.Persistence.Entities;
using System;
using System.Collections.Generic;

namespace FileTransfer.Manager.Persistence.Repositories
{
    public class RequestStartDto
    {
        public Guid SourceID { get; set; }
        public string FileID { get; set; }
    }

    public interface ITransferRequestRepository<T> : IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetNewRequestsBySourceIDOrderedByPriorityAndScheduledTime(Guid sourceID, int maxItems = 1);

        void UpdateRequestOnPrepared(Guid aTransferRequestID, int aStatus);
        void UpdateRequestOnStart(Guid aTransferRequestID, DateTime aStarted, int aStatus);
        void UpdateRequestOnProgressChange(Guid aTransferRequestID, DateTime aUpdated, int aStatus, string aDescription);
        void UpdateRequestOnFinish(Guid aTransferRequestID, DateTime aFinished, int aStatus, string aResult, string aDescription);


        TransferRequest RequestStart(RequestStartDto aData);
    }
}
