using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.WebAPI.Definitions.Dto
{
    public class FileTransferCreateDto<TFileID>
    {
        public TFileID FileID { get; set; }
    }
}
