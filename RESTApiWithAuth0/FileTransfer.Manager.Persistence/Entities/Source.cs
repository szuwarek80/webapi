using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace FileTransfer.Manager.Persistence.Entities
{
    public class Source : BaseEntity
    {
        public Guid SourceID { get; internal set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string ConnectionDescription { get; set; }
        public SqlDateTime Created { get; internal set; }
        public SqlDateTime Modified { get; internal set; }

    }
}
