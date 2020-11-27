using Microsoft.Extensions.Options;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Utils;

namespace Yesolm.DevOps.Services
{
    public class StorageService : ServiceBase, IStorageService
    {
        IEnumerable<FileInfo> dlls = null;
        public StorageService(IServiceLocator serviceLocator, IOptionsMonitor<NugetPackSettings> settings, IOptionsMonitor<AppStrings> appStrings) : 
            base(serviceLocator,settings, appStrings) 
        {
            dlls = new List<FileInfo>();
        }
        public Result EnsureNugetOutDirExsistAndHaveWriteAccess(string nugetOutDir, bool createIfNotFound, bool clearNugetOutputDir)
        {
            if (nugetOutDir is null)
                throw new ArgumentNullException(nameof(nugetOutDir));

            var dir = new DirectoryInfo(nugetOutDir);
            if (!dir.Exists)
            {
                if (!createIfNotFound)
                    return Result.Fail("@String.NugetDirCreateNotAllowed");
                        
                if (!dir.Parent.Exists)
                    return Result.Fail(@String.DirAndParentDirNotFound, nugetOutDir);

                if (!dir.Parent.HasWritePermissionOnDir())
                    return Result.Fail(@String.NoDirCreatePermission,dir.Parent.FullName);

                Log.Information(@String.CreatingDir,dir);

                return Directory.CreateDirectory(nugetOutDir).Exists ? Result.Ok(string.Empty) :
                       Result.Fail(@String.CannotCreateDir);
            }

            if (dir.HasWritePermissionOnDir())
            {
                if (clearNugetOutputDir)
                {
                    var outDir = new DirectoryInfo(nugetOutDir);
                    outDir.EnumerateFiles().ForEach(x =>
                    {
                        Log.Information(@String.DeletingFile, x.Name);
                        x.Delete();
                    });
                    outDir.EnumerateDirectories().ForEach(x =>
                    {
                        Log.Information(@String.DeletingDir, x.FullName);
                        x.Delete(true);
                    });
                }
                return Result.Ok();
            }
            return Result.Fail(@String.DirFoundButWritePermissionDenied,dir.FullName);
        }

        /// <summary>
        /// Locates dll files and frameworks
        /// </summary>
        /// <param name="args">Commandline arguments</param>
        /// <param name="dllFileName">The dll file name. aka Id + .dll</param>
        /// <param name="frameworksToPack">The framwork outputs to be packed.</param>
        /// <param name="String">App string values from appsettings.json</param>
        /// <param name="searchOption"><see cref="SearchOption.TopDirectoryOnly"/> or 
        /// <see cref="SearchOption.AllDirectories"/></param>
        /// <returns>Libraries to pack.</returns>
        public Result LocateLibrariesAndFramworks(Args args, string dllFileName, IEnumerable<string> frameworksToPack,
           AppStrings @String, SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (Directory.Exists(args.BuildDir))
            {
                dlls = new DirectoryInfo(args.BuildDir).GetFiles(dllFileName, searchOption);
                if (!dlls.IsEmpty())
                {
                    IEnumerable<FileInfo> result;
                    #region Logging                
                    dlls.ForEach((x) => Log.Information(@String.FoundLibDetail, x.FullName));
                    #endregion
                    if (Settings.EnableMultiFramworkPackagiing)
                    {
                        if (frameworksToPack is null)
                            return Result.Fail(@String.NoMatchedFrameworks);
                        Log.Information(@String.FrameworksToBePacked, string.Join(',', frameworksToPack));
                        result = dlls.Join(frameworksToPack, x => x.Directory.Name, y => y, (x, y) => x);
                    }
                    else
                    {
                        result = dlls;
                    }
                    return Result.Ok(result);
                }
            }
            return Result.Fail<IEnumerable<FileInfo>>(string.Format(@String.BuildDirNotFound, args.BuildDir));
        }
        /// <summary>
        /// Tries to locate project files so that <see cref="NugetPackageService"/> can pack files like images...
        /// </summary>
        /// <param name="sourceDirPath">Root path to source directory</param>
        /// <param name="extension">Extension for the projects.</param>
        /// <returns><see cref="FileInfo.Directory"/></returns>
        public Result GetProjectFileDirectories(string sourceDirPath, AppStrings @String, string extension =
            Constants.DefaultProjectExtension)
        {
            if (!sourceDirPath.IsValidPath())
                return Result.Fail(string.Format(@String.SourceDirNotFound, sourceDirPath));

            var files = Directory.GetFiles(sourceDirPath, $"*{extension}", SearchOption.AllDirectories);

            return files.IsEmpty() ?
                Result.Fail(@String.SourceDirNotFound,sourceDirPath) :      
                Result.Ok(files.Select(x => new FileInfo(x)).ToDictionary(x => x.Name, y => y.Directory));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageBuilders"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Result GeneratePackage(IEnumerable<PackageBuilderDetail> packageBuilders, Args args)
        {
            if (packageBuilders is null)
                throw new ArgumentNullException(nameof(packageBuilders));
            if (args is null)
                throw new ArgumentNullException(nameof(args));
            if (string.IsNullOrWhiteSpace(args.NugetOutDir))
                throw new ArgumentNullException(nameof(args.NugetOutDir));

            var outputDir = new DirectoryInfo(args.NugetOutDir);

            if (!outputDir.Exists)
            {
                Log.Debug(@String.CreatingDir, args.NugetOutDir);
                Directory.CreateDirectory(args.NugetOutDir);
            }
            
              
            var result = new List<FileInfo>();

            packageBuilders.ForEach(x => 
            {
                PackageBuilder builder = x.PackageBuilder;
                string package =  $"{builder.Id}.{builder.Version.ToNormalizedString()}.nupkg";
                Log.Debug(@String.Processing,package);

            var frameworkDir = new DirectoryInfo(Path.Combine(args.NugetOutDir, x.Framework));

            if (!frameworkDir.Exists)
                frameworkDir.Create();           

            using FileStream outputStream = new FileStream(Path.Combine(frameworkDir.FullName, package),
                   FileMode.OpenOrCreate);

            builder.Save(outputStream);
                Log.Information(@String.PkgSaveOk, outputStream.Name);
                result.Add(new FileInfo(outputStream.Name));                
            });

            return result.IsEmpty() ?
                Result.Fail(@String.ProducedNada) :
                Result.Ok(result);
        }       
    }
}