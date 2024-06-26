using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.EventLog;

namespace FileTransfer.Exe
{
    public class Program
    {
        private static int _port;

        public static void Main(string[] args)
        {
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
                  // WindowsService logs
                  services.Configure<EventLogSettings>(config =>
                  {
                      config.LogName = config.SourceName = "FileTransfer Service";
                  });
              })
             .UseWindowsService() // Windows Service
             .UseSystemd(); // Linux Demon
    }
}
