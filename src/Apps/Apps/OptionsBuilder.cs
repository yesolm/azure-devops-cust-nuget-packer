using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Utils;

namespace Yesolm.DevOps.Apps
{
    /// <summary>
    /// Builds named command line argument options
    /// </summary>
    public class OptionsBuilder
    {
        private IList<Option> _options;
        RootCommand _rootCmd;
        private readonly IEnumerable<OptionsConfig> optionsConfig;    
        public OptionsBuilder(RootCommand rootCommand,IEnumerable<OptionsConfig> optionsConfig)
        {
            if (rootCommand is null)
                throw new ArgumentNullException(nameof(rootCommand));

            _rootCmd = rootCommand;
            this.optionsConfig = optionsConfig ?? GetFallover();
            _options = new List<Option>();
        }
        public void Build() => _options.ForEach(x => _rootCmd.AddOption(x));
        /// <summary>
        /// Adds command arg options
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the option.</typeparam>
        /// <param name="option"><see cref="CommandOption"/> choice.</param>
        /// <returns></returns>
        public OptionsBuilder AddOption<T>(CommandOption option)
        {
            var value = optionsConfig.Where(x => x.Key == option.ToString());
            if (!value.IsEmpty())
            {
                var v = value.First();
                var o = new Option<T>(v.Aliases, v.Description);
                o.IsRequired = v.IsRequired;
                _options.Add(o);
            }
            return this;
        }

        private IEnumerable<OptionsConfig> GetFallover()
        {
            return new OptionsConfig[]
            {
                new OptionsConfig {
              Key = "NUGET_OUT_DIR",
              Description = "Nuget output directory. Write permission required.",
              Aliases = new string[]{ "--nuget-outdir", "-n" },
              IsRequired = true
            },
                new OptionsConfig {
              Key = "BUILD_DIR",
              Description = "Root path to the build directory.",
              Aliases = new string[]{ "--build-dir", "-b" },
              IsRequired = true
            },
                new OptionsConfig {
              Key = "SOURCE_DIR",
              Description = "Root path to the source directory, if packing files.",
              Aliases = new string[]{ "--source-dir", "-s" }
            },
                new OptionsConfig {
              Key = "BUILD_NUMBER",
              Description = "Build number ",
              Aliases = new string[]{ "--nuget-outdir", "-o" },
              IsRequired = true
            }
            };
        }
    }
}