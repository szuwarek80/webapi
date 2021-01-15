﻿using FileTransfer.Manager.Persistence.Entities;
using FileTransfer.Manager.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FileTransfer.Manager.Persistence.Connection
{
     internal class ConnectionDummy :
        IConnection
    {

        static List<Source> _sources = new List<Source>()
        {
            new Source()
            {
               SourceID = Guid.Parse("ED44319A-0E96-4EAD-BEA7-78739058D797"),
               Name = "MAM_Dummy",
               Type = 0, //=>TransferSourceType.FileTransferWebAPI
               ConnectionDescription = "http://localhost:3000",
            },
        };

        static List<TransferRequest> _transferRequests = new List<TransferRequest>();
        //{
        //    new TransferRequest()
        //    {
        //        TransferRequestID= Guid.NewGuid(),
        //        FileID ="xxx",
        //        FileHash = "xxx",
        //        FileName = "xxx",
        //        SourceID = _sources[0].SourceID,
        //    }
        //};

        public ConnectionDummy()
        {
            this.TransferRequestRepository = new TransferRequestRepositoryDummy(_transferRequests);
            this.SourceRepository = new SourceRespositoryDummy(_sources);
            this.ConnectionState = ConnectionState.Closed;
        }

        public ConnectionState ConnectionState { get; protected set; }
        public ITransferRequestRepository<TransferRequest> TransferRequestRepository { get;  }
        public ISourceRepository<Source> SourceRepository { get; }

        public void Close()
        {
            this.ConnectionState = ConnectionState.Closed;
        }

        public void Dispose()
        {
            this.Close();
        }

        public bool Open(IConnectionSettings aSettings, out string aError)
        {
            aError = string.Empty;

            this.ConnectionState = ConnectionState.Open;

            return true;
        }



    }


    public class SourceRespositoryDummy : Repository<Source>, ISourceRepository<Source>
    {
        List<Source> _sources;
        public SourceRespositoryDummy(List<Source> aSources)
        {
            _sources = aSources;
        }


        protected override string TableName => "Source";

        protected override string TableIDColumnName => "SourceID";

        public override Guid Insert(Source aEntity)
        {
            //ToDo
            return Guid.NewGuid();
        }

        public override void Update(Source aEntity)
        {

        }



        public override IEnumerable<Source> GetAll()
        {
            return _sources;
        }
    }


    public class TransferRequestRepositoryDummy : Repository<TransferRequest>, ITransferRequestRepository<TransferRequest>
    {
        List<TransferRequest> _transfers;
        public TransferRequestRepositoryDummy(List<TransferRequest> aTransfers)
        {
            _transfers = aTransfers;
        }


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
            var transfers = new List<TransferRequest>();

            foreach (var tr in _transfers)
                if (tr.Status == 0)
                    transfers.Add(tr);


            return transfers;
        }


        public void UpdateRequestOnPrepared(Guid aTransferRequestID, int aStatus)
        {
            foreach (var tr in _transfers)
                if (tr.TransferRequestID == aTransferRequestID)
                {
                    tr.Status = aStatus;
                    break;
                }
        }

        public void UpdateRequestOnStart(Guid aTransferRequestID, DateTime aStarted, int aStatus)
        {
            foreach (var tr in _transfers)
                if (tr.TransferRequestID == aTransferRequestID)
                {
                    tr.Status = aStatus;
                    break;
                }

        }

        public void UpdateRequestOnProgressChange(Guid aTransferRequestID, DateTime aUpdated, int aStatus, string aDescription)
        {
            foreach (var tr in _transfers)
                if (tr.TransferRequestID == aTransferRequestID)
                {
                    tr.Status = aStatus;
                    tr.Description = aDescription;
                    break;
                }

        }

        public void UpdateRequestOnFinish(Guid aTransferRequestID, DateTime aFinished, int aStatus, string aResult, string aDescription)
        {
            foreach (var tr in _transfers)
                if (tr.TransferRequestID == aTransferRequestID)
                {
                    tr.Status = aStatus;
                    tr.Description = aDescription;
                    break;
                }

        }


        public TransferRequest RequestStart(RequestStartDto aData)
        {
            foreach (var tr in _transfers)
                if (tr.FileID == aData.FileID && tr.SourceID == aData.SourceID)
                return tr;

            TransferRequest newTr =
            new TransferRequest()
            {
                TransferRequestID = Guid.NewGuid(),
                FileID = aData.FileID,
                SourceID = aData.SourceID
            };

            _transfers.Add(newTr);
            return newTr;
        }

        public override TransferRequest GetById(Guid aID)
        {
            foreach (var tr in _transfers)
                if (tr.TransferRequestID == aID)
                    return tr;
            return null;
        }


    }
}
