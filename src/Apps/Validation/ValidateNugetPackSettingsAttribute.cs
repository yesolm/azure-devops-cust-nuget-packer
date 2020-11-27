using System;
using System.ComponentModel.DataAnnotations;
using Yesolm.DevOps.Utils;
using System.Linq;
using Yesolm.DevOps.Services;
using Serilog;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using Yesolm.DevOps.Models;

namespace Yesolm.DevOps.Validation
{
    [AttributeUsage(AttributeTargets.Class)]
    sealed public class ValidateNugetPackSettingsAttribute : Attribute
    {
        List<ValidationResult> results = new List<ValidationResult>();
        public bool Validate(NugetPackSettings settings)
        {
            if (settings is null)
                AddError("Nuget pack settings is null.");
            else
            {
                var context = new ValidationContext(settings, null, null);
                ValidateSettings(settings);

                if (!results.SelectMany(x => x.MemberNames).Where(x => x.Equals("ProjectConfigs",
                    StringComparison.OrdinalIgnoreCase)).Any()) // continue only if the list contains items.
                    settings.ProjectConfigs.ForEach(x => ValidateSettings(x));
            }
            LogValidationResult();
            return results.IsEmpty();
        }
        private void ValidateSettings(object obj)
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                foreach (Attribute attribute in Attribute.GetCustomAttributes(property))
                {
                    if (attribute is RuleValidationAttribute)
                    {
                        RuleValidationAttribute a = (RuleValidationAttribute)attribute;

                        switch (a.Rule)
                        {
                            case Rule.NotEmpty:
                            case Rule.NotEmptyValidateChildren:
                                if (obj.IsEmpty(property))
                                    AddError($"{property.Name} is empty.", property.Name);
                                break;
                            case Rule.MustBeADefinedEnum:
                                {
                                    object value = null;

                                    if (obj.IsEmpty(property))
                                        AddError($"{property.Name} is empty.", property.Name);
                                    else if (!Enum.TryParse(a.Type, property.GetValue(obj).ToString(), out value))
                                        AddError(string.Format("'{0}' is not valid value for {1}. Possible values: " +
                                        " [{2}].", property.GetValue(obj).ToString(), property.Name, a.Type.EnumToCsv()),
                                        memberNames: property.Name);
                                }
                                break;
                        }
                    }
                }
            }
        }
        private void AddError(string errorMessage, params string[] memberNames)
        {
            if (results is null)
                results = new List<ValidationResult>();

            results.Add(new ValidationResult(errorMessage, memberNames));
        }

        public void LogValidationResult()
        {
            if (results.IsEmpty())
                "Nuget pack settings are valid.".Debug();

            results.ForEach(x => Log.Error("{0} | Members => [{1}]",x.ErrorMessage, string.Join(',',x.MemberNames)));
        }
    }
}
