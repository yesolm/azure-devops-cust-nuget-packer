using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Yesolm.DevOps.Models;
using Yesolm.DevOps.Utils;
using Yesolm.DevOps.Validation;

namespace Yesolm.DevOps
{
    [ValidateNugetPackSettings]
    public class NugetPackSettings
    {
        [RuleValidation(Rule.MustBeADefinedEnum, typeof(SolutionConfigType))]
        public string SolutionConfig { get; set; }
        public bool EnableMultiFramworkPackagiing {get;set;}
        public bool EnableObjectLogging { get; set; }
        public bool CreateNugetOutputDirIfNotFound { get; set; }
        public bool ClearNugetOutputDir { get; set; }
        public bool HasProjectsWithManifestFiles => !ProjectConfigs.IsEmpty() && ProjectConfigs.Any(x => !x.ManifestFilesGroup.IsEmpty());

        [RuleValidation(Rule.NotEmptyValidateChildren)]
        public IList<ProjectConfig> ProjectConfigs { get; set; }
    }
}