using FileTransfer.Manager.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Manager.Persistence.Repositories
{
    public interface ISourceRepository<T> : IRepository<T> where T : BaseEntity
    {
    }
}
