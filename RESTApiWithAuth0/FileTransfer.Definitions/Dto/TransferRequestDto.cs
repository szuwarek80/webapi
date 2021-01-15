using System;

namespace FileTransfer.Definitions.Dto
{
    public class TransferRequestDto
    {
        public TransferRequestDto(Guid aID, string aFileID, string aFileHash, string aFileName,
            TransferSourceDto aSource)
        {
            ID = aID;
            FileID = aFileID;
            FileName = aFileName;
            FileHash = aFileHash;
            Source = aSource;
        }

        public Guid ID { get; }
        public string FileID { get; }
        public string FileHash { get; }
        public string FileName { get; }
        public TransferSourceDto Source { get; }
    }
}
