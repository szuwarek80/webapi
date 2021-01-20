using FileTransfer.Definitions;
using FileTransfer.Manager.Core.Services.Settings;
using FileTransfer.Manager.Core.Services.Transfer;
using FileTransfer.Manager.Persistence.Connection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransfer.Manager.Exe.Services
{
    public class FileTransferManagerBackgroundService : BackgroundService
    {
        private readonly ILogger<FileTransferManagerBackgroundService> _logger;
        private readonly ISettingsService _settingsService;
        private IFileTransferStartService _fileTransferService;

        public FileTransferManagerBackgroundService(ILogger<FileTransferManagerBackgroundService> aLogger, ISettingsService aSettings)
        {
            _logger = aLogger;
            _settingsService = aSettings;

            var sourceFactory = new Dictionary<TransferSourceType, ITransferSourceFactory>();

            sourceFactory.Add(TransferSourceType.FileTransferWebAPI,
                new FileTransfer.Manager.Sources.FileTransferWebAPI.TransferSourceFactory(_settingsService.AppSettings.StatusRequestInterval));

            var connectionFactory = new ConnectionFactory();

            _fileTransferService = new FileTransferStartService(_settingsService,
                connectionFactory,
                sourceFactory,
                new FileTransferStatusUpdateService(connectionFactory, _settingsService.AppSettings));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var started = _fileTransferService.Start(out string startErrorMessage);

            if (started)
            {
               
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                }

                _fileTransferService.Stop();
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
            _fileTransferService.Stop();

            return Task.CompletedTask;
        }
    }
}
