using System.Collections.Generic;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Utils;
using Yesolm.DevOps.Validation;

namespace Yesolm.DevOps
{
    public class ProjectConfig
    {
        [RuleValidation(Rule.NotEmpty)]
        public string Id { get; set; }
        [RuleValidation(Rule.NotEmpty)]
        public string UniqueProjectFileName { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public List<string> Authors { get; set; }
        public List<NugetPackFiles> ManifestFilesGroup { get; set; }
        [RuleValidation(Rule.NotEmpty)]
        public List<string> TargetFrameworksToPack { get; set; }

        #region Methods
        public string GetDllFileName()
        {
            return $"{UniqueProjectFileName.Substring(0, UniqueProjectFileName.LastIndexOf('.'))}.dll";
        }    
        #endregion
    }
}