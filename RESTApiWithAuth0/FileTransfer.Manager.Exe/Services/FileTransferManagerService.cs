using FileTransfer.Manager.Core.Services.Settings;
using FileTransfer.Manager.Core.Services.Transfer;
using FileTransfer.Manager.Persistence.Connection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Exe.Services
{
    public class FileTransferManagerService : BackgroundService
    {
        private readonly ILogger<FileTransferManagerService> _logger;
        private IFileTransferStartService _fileTransferStartService;

        public FileTransferManagerService(ILogger<FileTransferManagerService> aLogger,
            ISettingsService aSettingService,
            IConnectionFactory aConnectionFactory,
            ITransferSourceFactoryService aTransferSourceFactoryService)
        {
            _logger = aLogger;


            _fileTransferStartService = new FileTransferStartService(aSettingService,
                aConnectionFactory,
                aTransferSourceFactoryService,
                new FileTransferStatusUpdateService(aConnectionFactory, aSettingService.AppSettings));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var started = _fileTransferStartService.Start(out string startErrorMessage);

            if (started)
            {
               
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                }

                _fileTransferStartService.Stop();
            }
            else
            {
                _logger.LogInformation($"File transfer not started: {startErrorMessage}.");

                // Stop windows service
                throw new Exception(startErrorMessage);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _fileTransferStartService.Stop();

            return Task.CompletedTask;
        }
    }
}
