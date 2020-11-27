using Microsoft.Extensions.DependencyInjection;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Utils;

namespace Yesolm.DevOps.Services
{
    public class ServiceLocator : IServiceLocator
    {
        private readonly IServiceCollection _services;
        public INugetPackageService  NugetService => _services.GetRequiredService<INugetPackageService>();
        public ILogService LogService => _services.GetRequiredService<ILogService>();
        public IStorageService StorageService => _services.GetRequiredService<IStorageService>();     
        public ServiceLocator(IServiceCollection services) => _services = services;
    }
}
