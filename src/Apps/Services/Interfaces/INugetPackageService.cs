using System.Collections.Generic;
using System.IO;
using Yesolm.DevOps.Apps;
using Yesolm.DevOps.Models;

namespace Yesolm.DevOps.Services
{
    public interface INugetPackageService
    {
        Result Build(Args args, ProjectConfig config, IEnumerable<FileInfo> libsToPack, DirectoryInfo projectDir);
    }
}