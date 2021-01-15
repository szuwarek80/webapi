using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer.Definitions.Dto
{
    public class TransferSourceDto
    {
        public TransferSourceDto(int aType, Guid aID, string aName, string aConnectionDescription)
        {
            Type = aType;
            ID = aID;
            Name = aName;
            ConnectionDescription = aConnectionDescription;
        }

        public int Type { get; }
        public Guid ID { get; }
        public string Name { get; }
        public string ConnectionDescription { get; }
    }
}
