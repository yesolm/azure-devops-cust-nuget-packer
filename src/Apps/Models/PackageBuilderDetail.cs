using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yesolm.DevOps.Models
{
    public class PackageBuilderDetail 
    {
        public PackageBuilderDetail(string framework, PackageBuilder packageBuilder)
        {
            Framework = framework;
            PackageBuilder = packageBuilder;
        }
        public string Framework { get; }
        public PackageBuilder PackageBuilder { get; }
    }
}
