using FileTransfer.Manager.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Persistence.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        T GetById(Guid aID);
        IEnumerable<T> GetAll();
        Guid Insert(T aEntity);
        void Update(T aEntity);
        void Delete(Guid aID);
    }
}
