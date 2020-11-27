using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Services;
using Yesolm.DevOps.Utils;
using Yesolm.DevOps.Validation;

namespace Yesolm.DevOps.Apps
{
    sealed class ApplicationHost : AppBase, IHostedService
    {
        private int _exitCode;
        private readonly IHostApplicationLifetime _appLifetime;

        public string FrameworkFoundDetail { get; private set; }

        public ApplicationHost(IServiceLocator serviceLocator, IHostApplicationLifetime appLifetime,
            IOptionsMonitor<AppSettings> appSettings, IOptionsMonitor<AppStrings> strings) : base(serviceLocator, 
                appSettings,strings)
        {
            _appLifetime = appLifetime;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
            Task.Run(async () =>
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    LogAppStartingValues();
                    var validator = new ValidateNugetPackSettingsAttribute();
                    if (validator.Validate(NugetPackSettings))
                    {
                        #region Command Options Builder
                        /*
                         * RootCommandis initilized in base class
                         * If Aliases are empty, the option won't be build
                         */
                        var optionsBuilder = new OptionsBuilder(RootCommand, AppSettings.OptionsConfig);
                        optionsBuilder.AddOption<string>(CommandOption.SOURCE_DIR)
                           .AddOption<string>(CommandOption.BUILD_DIR)
                           .AddOption<string>(CommandOption.NUGET_OUT_DIR)
                           .Build();
                        #endregion
                        _exitCode = await InvokeRootCommand(Environment.GetCommandLineArgs(), (args) =>
                        {
                            var nugetDirResult = Services.StorageService.EnsureNugetOutDirExsistAndHaveWriteAccess(args.NugetOutDir,
                                NugetPackSettings.CreateNugetOutputDirIfNotFound, NugetPackSettings.ClearNugetOutputDir);

                            if (!nugetDirResult.Success)
                            {
                                Log.Error(nugetDirResult.Error);
                                _exitCode = (int)ExitCode.NugetDirectoryAccessException;
                                _appLifetime.StopApplication();
                            }

                            Dictionary<string, DirectoryInfo> projFiles = null;

                            if (NugetPackSettings.HasProjectsWithManifestFiles)
                            {
                                var projFilesResult = Services.StorageService.GetProjectFileDirectories(args.SourceDir, @String);
                                if (!projFilesResult.Success)
                                {
                                    Log.Error(@String.NoProjsFourd, args.SourceDir);
                                    _exitCode = (int)ExitCode.NoPojectFilesFound;
                                    _appLifetime.StopApplication();
                                }
                                else
                                    projFiles = ((Result<Dictionary<string, DirectoryInfo>>)projFilesResult).Value;
                            }

                            var generatedPackages = new List<FileInfo>();

                            foreach (var config in NugetPackSettings.ProjectConfigs)
                            {
                                Log.Debug(@String.Processing, config.Id);
                                var libsResult = Services.StorageService.LocateLibrariesAndFramworks(args, config.GetDllFileName(),
                                config.TargetFrameworksToPack, @String);

                                if (libsResult.Success)
                                {
                                    #region Logging
                                    Log.Information(FrameworkFoundDetail, string.Join(',', ((Result<IEnumerable<FileInfo>>)libsResult).Value));
                                    #endregion

                                    DirectoryInfo projFile = null;

                                    if (!config.ManifestFilesGroup.IsEmpty())
                                    {
                                        if (!projFiles.ContainsKey(config.UniqueProjectFileName))
                                        {
                                            Log.Error(@String.ConfigMissingProjectFiles, config.Id, args.SourceDir);
                                            continue;
                                        }

                                        projFile = projFiles[config.UniqueProjectFileName];
                                    }

                                    var nugetBuildResult = Services.NugetService.Build(args, config, ((Result<IEnumerable<FileInfo>>)libsResult).Value
                                        , projFile);

                                    if (nugetBuildResult.Success)
                                    {
                                        var builderResult = ((Result<IList<PackageBuilderDetail>>)nugetBuildResult).Value;
                                        if (!builderResult.IsEmpty())
                                        {
                                            var result = Services.StorageService.GeneratePackage(builderResult, args);
                                            if (!result.Success)
                                            {
                                                Log.Error(result.Error);
                                                continue;
                                            }
                                            generatedPackages.AddRange(((Result<List<FileInfo>>)result).Value);
                                        }
                                        else
                                            Log.Warning(@String.NoPkgsFound);
                                    }
                                }
                                else
                                    libsResult.Error.Log();
                            }


                            if (!generatedPackages.IsEmpty())
                            {
                                Log.Information(@String.NugetPackagingSummary.PrepemdHorizontalLine('-')
                                    .AppendHorizontalLine('-'), generatedPackages.Count(), string.Join(null, generatedPackages.
                                    Select(x => string.Format("\n\t{0}\t[OK]", x.Name))));
                            }
                        });
                    }
                    Log.Debug(@String.ProcessRunTimespan, (DateTime.Now - startTime).Seconds, "".AppendHorizontalLine('═'));
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Fatal(ex.StackTrace);
                    _exitCode = (int)ExitCode.UnhandledException;
                }
                finally
                {
                    _appLifetime.StopApplication();
                }
                });
            });

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            LogAppExit(_exitCode);
            Environment.ExitCode = _exitCode;
            return Task.CompletedTask;
        }
    }
}