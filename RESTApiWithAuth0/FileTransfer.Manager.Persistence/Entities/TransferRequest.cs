using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace FileTransfer.Manager.Persistence.Entities
{
    public class TransferRequest : BaseEntity
    {
        public Guid TransferRequestID { get; internal set; }
        public string FileID { get; set; }
        public string FileName { get; set; }
        public string FileHash { get; set; }
        public Guid SourceID { get; set; }
        public string Result { get; set; }
        public SqlDateTime Started { get; internal set; }
        public SqlDateTime Finished { get; internal set; }
        public SqlDateTime StatusUpdated { get; set; }
        public int Status { get; set; } = 0;
        public string Description { get; set; }


        //public Source Source { get; internal set; }
    }
}
