using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace NuFridge.Common.Helpers
{
    public static class DirectoryHelper
    {
        public static bool HasWriteAccess(string path, string ntAccountName)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            DirectorySecurity acl = di.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(NTAccount));

            //Go through the rules returned from the DirectorySecurity
            foreach (AuthorizationRule rule in rules)
            {
                //If we find one that matches the identity we are looking for
                if (rule.IdentityReference.Value.Equals(ntAccountName, StringComparison.CurrentCultureIgnoreCase))
                {
                    //Cast to a FileSystemAccessRule to check for access rights
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.WriteData) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HasDeleteRights(string path, string ntAccountName)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            DirectorySecurity acl = di.GetAccessControl(AccessControlSections.All);
            AuthorizationRuleCollection rules = acl.GetAccessRules(true, true, typeof(NTAccount));

            //Go through the rules returned from the DirectorySecurity
            foreach (AuthorizationRule rule in rules)
            {
                //If we find one that matches the identity we are looking for
                if (rule.IdentityReference.Value.Equals(ntAccountName, StringComparison.CurrentCultureIgnoreCase))
                {
                    //Cast to a FileSystemAccessRule to check for access rights
                    if ((((FileSystemAccessRule)rule).FileSystemRights & FileSystemRights.DeleteSubdirectoriesAndFiles) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}