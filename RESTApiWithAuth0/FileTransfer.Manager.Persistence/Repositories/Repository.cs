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
            var result = new List<T>();

            //ToDo

            return result;
        }

        public virtual T GetById(Guid aID)
        {
            var entity = new T();

            //ToDo

            return entity;
        }

        public void Delete(Guid aID)
        {
            //ToDo
        }

        public abstract Guid Insert(T aEntity);
        public abstract void Update(T aEntity);
    }
}
