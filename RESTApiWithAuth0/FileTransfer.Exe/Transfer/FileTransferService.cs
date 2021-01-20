using FileTransfer.Exe.Transfer;
using FileTransfer.WebAPI.Definitions.Dto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FileTransfer.Exe.Transfer
{
    public interface IFileTransferService
    {
        Guid TransferCreate(FileTransferCreateDto<string> aReqest);
        FileTransferDto TransferGet(Guid aID);
        List<FileTransferDto> TransferGet();
        bool TransferRemove(Guid aID);
    }

    public class FileTransferService :
        IFileTransferService
    {
        ConcurrentDictionary<Guid, FileTransferJob> _jobs;
        IFileTransferJobExecutorFactory _executorFactory;

        public FileTransferService(IFileTransferJobExecutorFactory aFileTransferJobExecutorFactory)
        {
            _jobs = new ConcurrentDictionary<Guid, FileTransferJob>();
            _executorFactory = aFileTransferJobExecutorFactory;
        }

        public Guid TransferCreate(FileTransferCreateDto<string> aReqest)
        {
            var job = new FileTransferJob(aReqest.FileID, _executorFactory);

            if (_jobs.TryAdd(job.ID, job))
            {
                job.Start();
            }
            
            return job.ID;
        }

        public FileTransferDto TransferGet(Guid aID)
        {
            if (_jobs.TryGetValue(aID, out FileTransferJob job))
            {
                return job.GetStatus();
            };

            return null;
        }

        public List<FileTransferDto> TransferGet()
        {
            var result = new List<FileTransferDto>();

            foreach (var job in _jobs.Values)
                result.Add(job.GetStatus());

            return result;
        }

        public bool TransferRemove(Guid aID)
        {
            return _jobs.TryGetValue(aID, out FileTransferJob job);
        }
    }
}
