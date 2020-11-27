using System;
using System.Collections.Generic;
using System.Text;
using Yesolm.DevOps.Models;

namespace Yesolm.DevOps
{
    public sealed class AppSettings
    {
        public NugetPackSettings NugetPackSettings { get; set; }
        public IList<OptionsConfig> OptionsConfig { get; set; }
    }
}