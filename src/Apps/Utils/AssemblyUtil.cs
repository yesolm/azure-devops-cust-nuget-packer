using Microsoft.Extensions.DependencyModel;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Reflection.Metadata;
using Yesolm.DevOps.Models;

namespace Yesolm.DevOps.Utils
{
    /// <summary>
    /// Assembly and metadata extensions.
    /// </summary>
    public static class AssemblyUtil
    {
        /// <summary>
        /// Get value of <see cref="targ"/>name from custom attributes in the assembly.
        /// </summary>
        /// <param name="assembly">The source <see cref="Assembly"/></param>
        /// <returns></returns>
        public static string GetTargetFrameworkName(this Assembly assembly)
        {
            return ((TargetFrameworkAttribute)assembly.GetCustomAttribute(typeof(TargetFrameworkAttribute))).FrameworkName;
        }
 
        /// <summary>
        /// Gets <see cref="IEnumerable{PropertyInfo} "/> using that <see cref="CustomAttribute"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesWithCustomAttribute<T>(this object obj)
            where T: class
        {
           return obj.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(T), true).Any());
        }
      /*  /// <summary>
        ///  Creates a <see cref="ManifestFile"/> entry in  nuget packages.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="target"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        [Obsolete("Method is deprecated, please use the DirectoryInfo extension instead.")]
        public static ManifestFile ToManifestFile(this FileInfo sourceFile, string target, string exclude = "")
        {
            if (!sourceFile.Exists)
                throw new FileNotFoundException("File not found.", fileName: sourceFile.FullName);

            return sourceFile.Directory.ToManifestFile(target, exclude);
        }*/
    } 
}
