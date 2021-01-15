using FileTransfer.Manager.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Persistence.Repositories
{
    public class SourceRespository : Repository<Source>, ISourceRepository<Source>
    {
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
    }
}
