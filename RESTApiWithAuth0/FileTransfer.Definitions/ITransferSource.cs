using FileTransfer.Definitions.Dto;
using System.Threading.Tasks;

namespace FileTransfer.Definitions
{
    public interface ITransferSource
    {
        TransferSourceType SourceType { get; }

        bool IsConnected { get; }

        void Init(TransferSourceDto aSource);
        void Finit();

        int MaxParallelNumberOfTransferRequests { get; }

        Task<ITransferResult> StartTransferRequest(TransferRequestDto aRequest, 
            ITransferProgress aTransferProgress, 
            CancellationTokenWithReason aCancellationToken);

    }
}
