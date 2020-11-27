using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace Yesolm.DevOps.Utils
{
    public static class FileSystemUtil
    {    

        /// <summary>
        /// Checks if <paramref name="path"/> is valid.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsValidPath(this string path)
        {
            var dir = new DirectoryInfo(path);
            return dir.Exists || new FileInfo(path).Exists;
        }
        /// <summary>
        /// Checks if the user has write access to <paramref name="directoryInfo"/>
        /// </summary>
        /// <param name="directoryInfo">Directory to check access to </param>
        /// <returns></returns>
        public static bool HasWritePermissionOnDir(this DirectoryInfo directoryInfo)
        {
            var writeAllow = false;
            var writeDeny = false;
            var accessControlList = directoryInfo.GetAccessControl();
            if (accessControlList == null)
                return false;
            var accessRules = accessControlList.GetAccessRules(true, true,
                                        typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
                return false;

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                    continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                    writeAllow = true;
                else if (rule.AccessControlType == AccessControlType.Deny)
                    writeDeny = true;
            }

            return writeAllow && !writeDeny;
        }

    }
}
