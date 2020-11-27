using Microsoft.Extensions.Options;
using Serilog;
using Yesolm.DevOps.Models;

namespace Yesolm.DevOps.Services
{
    abstract public class ServiceBase
    {
        protected IServiceLocator Services { get; }
        protected ILogger Log { get; }
        public NugetPackSettings Settings { get; }
        public AppStrings @String { get; }

        public ServiceBase(IServiceLocator serviceLocator, IOptionsMonitor<NugetPackSettings> settings, IOptionsMonitor<AppStrings> appStrings)
        {
            Services = serviceLocator;
            Log = serviceLocator.LogService.Log;
            Settings = settings.CurrentValue;
            @String = appStrings.CurrentValue;            
        }

    }
}