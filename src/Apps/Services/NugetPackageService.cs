using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Options;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Utils;

namespace Yesolm.DevOps.Services
{
    public class NugetPackageService : ServiceBase, INugetPackageService
    {
        private readonly IList<PackageBuilderDetail> _builders;
        public NugetPackageService(IServiceLocator serviceLocator, IOptionsMonitor<NugetPackSettings> settings, IOptionsMonitor<AppStrings> appStrings) :
            base(serviceLocator, settings, appStrings)
        {
            _builders = new List<PackageBuilderDetail>();
        }
        public Result Build(Args args, ProjectConfig config, IEnumerable<FileInfo> libsToPack, DirectoryInfo projectDir)
        {
            if (libsToPack == null)
                throw new ArgumentNullException(nameof(libsToPack));

            libsToPack.ForEach(x =>
            {  
                PackageBuilder builder = new PackageBuilder();              

                AddLibrary(builder,x);
                ResolvePackageDependencyAndCreateManifest(builder,config,x);
                AddManifestFiles(builder,config, args, projectDir);
                _builders.Add(new PackageBuilderDetail(x.Directory.Name, builder));
            });
            return _builders.IsEmpty() ?
                Result.Fail("No nuget packages built"):
                Result.Ok(_builders);
        }
        private void AddLibrary(PackageBuilder builder, FileInfo lib) =>
            builder.PopulateFiles(lib.DirectoryName, new ManifestFile[] { new ManifestFile { Source = lib.FullName, Target = Constants.NugetLibraryDir } });
        private void ResolvePackageDependencyAndCreateManifest(PackageBuilder builder, ProjectConfig config, FileInfo lib)
        {
            var manifestMetadata = new ManifestMetadata()
            {
                Id = config.Id,
                Authors = config.Authors,
                Version = new NuGetVersion(config.Version),
                Description = config.Description,

            };
            var depenncyGroup = FetchPackageDependencyGroups(lib);
            if (!depenncyGroup.IsEmpty()) manifestMetadata.DependencyGroups = depenncyGroup; 
            Services.LogService.LogObject(manifestMetadata);
            builder.Populate(manifestMetadata);
        }       
        private void AddManifestFiles(PackageBuilder builder, ProjectConfig config, Args args,
            DirectoryInfo projectDir)
        {
            if (!config.ManifestFilesGroup.IsEmpty() && projectDir.Exists)
            {
                foreach (var entry in config.ManifestFilesGroup)
                {
                    if (entry.IsValid())
                    {                       
                        builder.PopulateFiles(
                            Path.Combine(projectDir.FullName, entry.DirectoryPathRelativeToProject),
                            new ManifestFile[] 
                            {   new ManifestFile 
                                {
                                    Source = entry.Source,
                                    Target = entry.Target,
                                    Exclude = entry.Exclude
                                }
                            });

                    }
                }
            }
        }
        private IEnumerable<PackageDependencyGroup> FetchPackageDependencyGroups(FileInfo file)
        {
            if (file.Exists && file.Extension.Equals(Constants.LibExtension))
            {
                PackageDependencyGroup packageDependencyGroup = null;
                try
                {
                    var assembly = Assembly.LoadFrom(file.FullName);
                    var lib = DependencyContext.Load(assembly)?.CompileLibraries.
                    Where(x => x.Name == assembly.GetName().Name).FirstOrDefault();

                    if (lib != null)
                    {
                        packageDependencyGroup = new PackageDependencyGroup(NuGetFramework.
                        Parse(assembly.GetTargetFrameworkName()), lib.Dependencies.
                        Select(x => new PackageDependency(x.Name, VersionRange.
                        Parse(x.Version))));
                    }
                }
                catch (Exception ex) { ex.Log(ex.Message); }

                if (packageDependencyGroup != null)
                    yield return packageDependencyGroup;
            }
            yield break;
        }
    }
}
