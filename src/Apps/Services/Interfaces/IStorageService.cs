using NuGet.Packaging;
using System.Collections.Generic;
using System.IO;
using Yesolm.DevOps.Models;

namespace Yesolm.DevOps.Services
{
    public interface IStorageService
    {
        Result EnsureNugetOutDirExsistAndHaveWriteAccess(string nugetOutDir, bool createIfNotFound, bool clearNugetOutputDir);
        Result LocateLibrariesAndFramworks(Args args, string dllFileName, IEnumerable<string> frameworksToPack,
            AppStrings strings, SearchOption searchOption = SearchOption.AllDirectories);
        Result GetProjectFileDirectories(string path, AppStrings appStrings, string extension = ".csproj");
        Result GeneratePackage(IEnumerable<PackageBuilderDetail> packageBuilders, Args args);
    }
}