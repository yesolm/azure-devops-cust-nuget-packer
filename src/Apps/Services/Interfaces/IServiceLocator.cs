using Yesolm.DevOps.Models;

namespace Yesolm.DevOps.Services
{
    public interface IServiceLocator
    {
        ILogService LogService { get; }
        IStorageService StorageService { get; }
        INugetPackageService NugetService { get; }
    }
}