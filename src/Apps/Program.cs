using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Yesolm.DevOps.Apps;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Services;
using Yesolm.DevOps.Utils;

namespace Yesolm.DevOps
{
    class Program
    {
        private static async Task Main(string[] args)
        {
          
            /* 
             * Using Serilog for logging.
             * You can add more log sinks by using 'WriteTo'
             * Logs sinks include : File, Email, EventLog, MsBuild...
             */

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Fatal)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code) 
                .CreateLogger();

            string contentRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            await Host.CreateDefaultBuilder(args)
               .UseContentRoot(contentRoot)
               .ConfigureAppConfiguration(x => x.AddStrings(contentRoot))
               .UseSerilog(Log.Logger)               
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddHostedService<ApplicationHost>();
                   services.Configure<AppSettings>(hostContext.Configuration);
                   services.Configure<AppStrings>(hostContext.Configuration);
                   services.Configure<NugetPackSettings>(hostContext.Configuration);
                   services.AddSingleton<ILogService, LogService>()
                   .AddSingleton<INugetPackageService, NugetPackageService>()
                   .AddSingleton<IStorageService, StorageService>()
                   .AddSingleton<IServiceLocator, ServiceLocator>();

                   services.AddSingleton<IServiceLocator>(x => new ServiceLocator(services));

               })
              .RunConsoleAsync();


        }
    }
}
    