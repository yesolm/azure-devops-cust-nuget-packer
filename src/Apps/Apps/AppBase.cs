using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Services;
using Yesolm.DevOps.Utils;

namespace Yesolm.DevOps.Apps
{
    /// <summary>
    /// Common app items to add more Apps
    /// </summary>
    public class AppBase
    {
        protected IServiceLocator Services { get; }
        protected ILogger Log { get; }
        protected ILogService LogService { get; }
        protected NugetPackSettings NugetPackSettings { get; }
        protected AppStrings @String { get; }
        protected AppSettings AppSettings { get; }
        protected RootCommand RootCommand { get; }
        public AppBase(IServiceLocator serviceLocator,
                       IOptionsMonitor<AppSettings> appSettings,
                       IOptionsMonitor<AppStrings> strings,
                       bool treatUnmatchedTokensAsErrors = false)
        {
            Services = serviceLocator;
            Log = serviceLocator.LogService.Log;
            LogService = serviceLocator.LogService;

            AppSettings = appSettings.CurrentValue;
            NugetPackSettings = AppSettings?.NugetPackSettings;
            @String = strings.CurrentValue;

            RootCommand = new RootCommand(@String?.AppDescription ?? AppDomain.CurrentDomain.FriendlyName);
            RootCommand.TreatUnmatchedTokensAsErrors = treatUnmatchedTokensAsErrors;

        }

        protected async Task<int> InvokeRootCommand(string[] args, Action<Args> action)
        {
            RootCommand.Handler = CommandHandler.Create<Args>(action);
            return await RootCommand.InvokeAsync(args);
        }

        protected void LogAppStartingValues()
        {
            var message = new StringBuilder();
            var args = Environment.GetCommandLineArgs();
            if (!args.IsEmpty())
            { 
                message.Append(string.Format(@String.AppStarting, args.First()));
                for (int i = 1; i < args.Length; i++)
                {
                    string arg = args[i];
                    if (arg.StartsWith("-"))
                    {
                        message.AppendLine();
                        message.Append(string.Format("\tArg [{0}] => ", arg));
                    }  
                    else
                        message.Append(arg);
                }
                Log.Debug(message.ToString().PrepemdHorizontalLine('-'));
            }
        }

        protected void LogAppExit(int exitCode)
        {
            Log.Debug(@String.AppExiting, exitCode);
        }
    }
}