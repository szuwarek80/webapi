using FileTransfer.Definitions.Dto;
using FileTransfer.WebAPI.Definitions.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Sources.FileTransferWebAPI
{
    public interface IFileTransferWebAPIAccess :
        IDisposable
    {
        string ID { get; }
        bool IsConnected { get; }

        Task<Guid> TransferCreate(FileTransferCreateDto<string> aReqest);
        Task<FileTransferDto> TransferGet(Guid aID);
        Task<bool> TransferRemove(Guid aID);
    }
}
