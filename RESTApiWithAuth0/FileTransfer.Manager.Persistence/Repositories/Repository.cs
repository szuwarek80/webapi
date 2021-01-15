using FileTransfer.Manager.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Persistence.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : BaseEntity, new()
    {

        protected abstract string TableName { get; }
        protected abstract string TableIDColumnName { get; }

      

        public virtual IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public virtual T GetById(Guid aID)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid aID)
        {
            throw new NotImplementedException();
        }

        public abstract Guid Insert(T aEntity);
        public abstract void Update(T aEntity);
    }
}
