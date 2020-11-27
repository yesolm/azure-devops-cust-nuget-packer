using NuGet.Packaging;
using System.Collections.Generic;
using Yesolm.DevOps.Utils;

namespace Yesolm.DevOps
{
    public class NugetPackFiles 
    {        public string DirectoryPathRelativeToProject { get; set; }
        public string Exclude { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(DirectoryPathRelativeToProject)
                   && !string.IsNullOrWhiteSpace(Source)
                   && !string.IsNullOrWhiteSpace(Target);
        }        
      
    }
}