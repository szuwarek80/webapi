using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileTransfer.Manager.Core.Services.Settings;
using FileTransfer.Manager.Exe.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace FileTransfer.Manager.Exe
{
    public class Program
    {
        private static int _port;
        //static SettingsService _settingsService;

        public static void Main(string[] args)
        {
            //_settingsService = new SettingsService(true);

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _port = configuration.GetValue<int>("Port");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<Startup>();
              }).ConfigureWebHost(config =>
              {
                  // IMPORTANT!!!!
                  // If API runs from VS, this url will ignore - the url from launchSettings.json will be taken! 
                  config.UseUrls($"http://*:{_port}"); // force server to listen on that ports in Release
              })
            .ConfigureServices(services =>
            {
                //services.AddHostedService(x => new FileTransferManagerBackgroundService(_settingsService));
                      // WindowsService logs
                services.Configure<EventLogSettings>(config =>
                {
                    config.LogName = config.SourceName = "TaskExecutor Service";
                });
            })
           .UseWindowsService() // Windows Service
           .UseSystemd(); // Linux Demon
    }
}
