using Microsoft.Extensions.Options;
using Serilog;
using System;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Utils;

namespace Yesolm.DevOps.Services
{
    public class LogService : ILogService
    {
        private readonly NugetPackSettings settings;
        private readonly AppStrings @String;
        public LogService(IOptionsMonitor<AppSettings> appSettings, IOptionsMonitor<AppStrings> strings)
        {
            settings = appSettings.CurrentValue.NugetPackSettings;
            @String = strings.CurrentValue;
        }
        public ILogger Log { get; } = Serilog.Log.Logger;
        public void LogObject(object obj)
        {
            if (!settings.EnableObjectLogging)
                return;
            Log.Information("{0}".AppendHorizontalLine().PrepemdHorizontalLine(), obj.GetType().Name);
            obj.SerializeToJsonAndGetLines().ForEach(x => Log.Information("   {0}", x));
        }
        public void CloseAndFlush() => Logging.Logger.CloseAndFlush();
        public void Create(Func<ILogger> func) => Serilog.Log.Logger = func();    
    }
}
namespace Yesolm.DevOps.Services.Logging
{
    public static class Logger
    {
        public static void CloseAndFlush() => Log.CloseAndFlush();
        public static void Error(this ILogger log, Exception exception)
            => log.Error(exception, exception.Message);
    }
}
