using FileTransfer.Definitions;
using FileTransfer.Manager.Core.Services.Settings;
using FileTransfer.Manager.Core.Services.Transfer;
using System.Collections.Generic;

namespace FileTransfer.Manager.Exe.Services
{
    public class FileTransferSourceFactoryService :
        ITransferSourceFactoryService
    {
        Dictionary<TransferSourceType, ITransferSourceFactory> _sourceFactory = new Dictionary<TransferSourceType, ITransferSourceFactory>();

        public FileTransferSourceFactoryService(ISettingsService aSettingService)
        {
            _sourceFactory.Add(TransferSourceType.FileTransferWebAPI,
                    new FileTransfer.Manager.Sources.FileTransferWebAPI.TransferSourceFactory(aSettingService.AppSettings.StatusRequestInterval));
        }

        public ITransferSource CreateTransferSource(TransferSourceType aType)
        {
            return _sourceFactory.ContainsKey(aType) ?
                _sourceFactory[aType].CreateTransferSource() 
                :
                null;
        }
    }
}
