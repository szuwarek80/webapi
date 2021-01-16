using FileTransfer.Definitions;
using FileTransfer.WebAPI.Dto;
using FileTransfer.WebAPI.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileTransfer.Exe
{
    public class FileTransfersControllerServiceImpl :
        FileTransfersControllerService
    {
        ConcurrentDictionary<Guid, TransferJob> _jobs;

        public FileTransfersControllerServiceImpl()
        {
            _jobs = new ConcurrentDictionary<Guid, TransferJob>();
        }

        public override Task<Guid> CreateFileTransfer(CreateFileTransferDto aRequest)
        {
            var job = new TransferJob(aRequest.FileID);

            if (_jobs.TryAdd(job.ID, job))
            {
                job.Executor.Start();
            }

            var status = job.Executor.GetStatus(out string result, out string desc);
            
            return Task.FromResult<Guid>(job.ID);
        }

        public override Task<FileTransferDto> GetFileTransferStatus(Guid aID)
        {
            if (_jobs.TryGetValue(aID, out TransferJob job))
            {
                var status = job.Executor.GetStatus(out string result, out string desc);

                return Task.FromResult<FileTransferDto>(new FileTransferDto
                {
                    Status = status,
                    Description = desc,
                    Result = result,
                    TransferRequestID = job.ID,
                });
            };

            return Task.FromResult<FileTransferDto>(null);            
        }


        public override Task<bool> DeleteTransfer(Guid aID, out bool aForbidden)
        {
            aForbidden = false;
            if (_jobs.TryGetValue(aID, out TransferJob job))
            {
                switch (job.Executor.GetStatus(out string result, out string desc))
                {
                    case TransferResultStatus.Error:
                    case TransferResultStatus.Success:
                        if (_jobs.TryRemove(aID, out TransferJob jobRemoved))
                            return Task.FromResult<bool>(true);
                        return Task.FromResult<bool>(false);

                    default:
                        aForbidden = true;
                        return Task.FromResult<bool>(false);
                }
            };

            return Task.FromResult<bool>(job != null);
        }

    }
}
