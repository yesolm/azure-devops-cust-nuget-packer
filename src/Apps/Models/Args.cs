using System;
using System.Collections.Generic;
using System.Text;

namespace Yesolm.DevOps.Models
{
    public class Args
    {
        public string BuildDir { get; set; }
        public string SourceDir { get; set; }
        public string NugetOutDir { get; set; }
    }
}
