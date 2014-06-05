using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Xml.Serialization;
using Microsoft.Web.Administration;
using System.Configuration;
using NuFridge.DataAccess.Entity;
using NuFridge.DataAccess.Repositories;

namespace NuFridge.Common.Manager
{
    public static class RetentionPolicyManager
    {
        public static void DeleteRetentionPolicy(string feedName)
        {
            var repository = new MongoDbRepository<RetentionPolicyEntity>();
            var policy = repository.Get(rt => rt.FeedName == feedName).FirstOrDefault();
            if (policy != null)
            {
                repository.Delete(policy);
            }
        }

        public static bool ApplyRetentionPolicy(RetentionPolicyEntity policy, out string message, out long fileLengthDeleted, out int packagesDeleted)
        {
            message = "";
            fileLengthDeleted = 0;
            packagesDeleted = 0;

            if (!policy.Enabled)
            {
                message = "The retention policy is not enabled";
                return true;
            }

            if (policy.DaysToKeepPackages <= 0)
            {
                message = "The retention policy is not enabled (days to keep packages is not set)";
                return true;
            }

            var nuFridgeWebsiteName = ConfigurationManager.AppSettings["NuFridge.Website.Name"];

            var mgr = new ServerManager();

            var site = mgr.Sites.FirstOrDefault(st => st.Name == nuFridgeWebsiteName);
            if (site == null)
            {
                message = "IIS Website not found for " + nuFridgeWebsiteName;
                return false;
            }

            var binding = site.Bindings.FirstOrDefault();
            if (binding == null)
            {
                message = "No IIS bindings found for " + nuFridgeWebsiteName;
                return false;
            }

            var application = site.Applications.FirstOrDefault(st => st.Path.ToLower() == string.Format("/feeds/{0}", policy.FeedName.ToLower()));
            if (application == null)
            {
                message = "No IIS application found for " + policy.FeedName;
                return false;
            }

            var feedDirectory = Path.Combine(application.VirtualDirectories[0].PhysicalPath, "App_Data\\Packages");
            if (!Directory.Exists(feedDirectory))
            {
                message = "The feed package folder does not exist at " + feedDirectory;
                return false;
            }

            var listOfExcludedPackages = new List<ExcludedPackageEntity>();

            message += "Feed name: " + policy.FeedName + Environment.NewLine;
            message += "Days to keep: " + policy.DaysToKeepPackages + Environment.NewLine;
            message += "Versions to keep: " + policy.VersionsToKeep + Environment.NewLine;

            if (policy.ExcludedPackages != null)
            {
                listOfExcludedPackages = policy.ExcludedPackages;

                if (listOfExcludedPackages.Any())
                {
                    message += "List of package ids provided to skip:" + Environment.NewLine;

                    foreach (var excludedPackage in listOfExcludedPackages)
                    {
                        message += "Package Id: '" + excludedPackage.PackageId + "', Partial Match: '" +
                                   excludedPackage.PartialMatch + "'" + Environment.NewLine;
                    }
                }

                message += Environment.NewLine;
            }

            message += "Applying retention policy..." + Environment.NewLine;

           // message += "Checking files in the root feed directory. Max versions to keep is not applied to the root folder." + Environment.NewLine;
            //Remove all older files in root folder
            foreach (string newPath in Directory.GetFiles(feedDirectory, "*.nupkg", SearchOption.TopDirectoryOnly))
            {
                var fi = new FileInfo(newPath);
                fi.Attributes &= ~FileAttributes.ReadOnly;

                bool isExcluded = IsFileExcluded(fi, listOfExcludedPackages);
                bool isGreaterThanDaysToKeep = IsFileOlderThanXDays(policy, fi);
               
                if (!isExcluded && isGreaterThanDaysToKeep)
                {
                    DeleteFile(ref message, ref fileLengthDeleted, ref packagesDeleted, fi, ReasonForDelete.OlderThanXDays);
                }
            }


            var subDirectories = Directory.GetDirectories(feedDirectory);

            foreach (var subDirectory in subDirectories)
            {
                if (Directory.Exists(subDirectory))
                {
                    var subFolderFileNames = Directory.GetFiles(subDirectory, "*.nupkg", SearchOption.TopDirectoryOnly);
                    if (!subFolderFileNames.Any())
                        break;

                   // message += "Checking files in the " + subDirectory + " folder" + Environment.NewLine;

                    var subFolderFiles = new List<FileInfo>();
                    subFolderFiles.AddRange(subFolderFileNames.Where(File.Exists).Select(fn => new FileInfo(fn)).OrderByDescending(fi => fi.LastWriteTime));

                    if (policy.VersionsToKeep > 0)
                    {
                        if (subFolderFiles.Count() > policy.VersionsToKeep)
                        {
                            var filesToDelete = subFolderFiles.Skip(policy.VersionsToKeep);

                            if (policy.DaysToKeepPackages > 0)
                            {
                                //message +=
                                //    string.Format(
                                //        "{0} package versions for '{1}' may be removed as it breaks the max versions to keep rule (where files are more than {2} day{3} old)." +
                                //        Environment.NewLine, subFolderFiles.Count() - policy.VersionsToKeep,
                                //        subDirectory, policy.DaysToKeepPackages,
                                //        policy.DaysToKeepPackages == 1 ? "" : "s");

                                ApplyPolicyForVersionsToKeepWithDaysToKeep(policy, ref message, ref fileLengthDeleted,
                                                                           ref packagesDeleted, filesToDelete,
                                                                           listOfExcludedPackages);
                            }
                            else
                            {
                                //message +=
                                //    string.Format(
                                //        "{0} package versions for '{1}' will be removed as it breaks the max versions to keep rule." +
                                //        Environment.NewLine, subFolderFiles.Count() - policy.VersionsToKeep,
                                //        subDirectory);

                                ApplyPolicyForVersionsToKeepWithoutDaysToKeep(ref message, ref fileLengthDeleted,
                                                                              ref packagesDeleted, filesToDelete,
                                                                              listOfExcludedPackages);
                            }
                        }
                    }
                    else if (policy.DaysToKeepPackages > 0)
                    {
                        foreach (var subFolderFile in subFolderFiles)
                        {
                            bool isExcluded = IsFileExcluded(subFolderFile, listOfExcludedPackages);
                            bool isGreaterThanDaysToKeep = IsFileOlderThanXDays(policy, subFolderFile);

                            if (!isExcluded && isGreaterThanDaysToKeep)
                            {
                                DeleteFile(ref message, ref fileLengthDeleted, ref packagesDeleted, subFolderFile, ReasonForDelete.OlderThanXDays);
                            }
                        }
                    }
                }
            }

            message += "The retention policy completed successfully";

            return true;
        }

        public enum ReasonForDelete
        {
            OlderThanXDays,
            KeepLimitedVersionsAndOlderThanXDays,
            KeepLimitedVersions
        }

        private static void ApplyPolicyForVersionsToKeepWithoutDaysToKeep(ref string message, ref long fileLengthDeleted,
                                                                          ref int packagesDeleted, IEnumerable<FileInfo> filesToDelete,
                                                                          List<ExcludedPackageEntity> listOfExcludedPackages)
        {
            foreach (var fileInfo in filesToDelete)
            {
                bool isExcluded = IsFileExcluded(fileInfo, listOfExcludedPackages);
                if (!isExcluded)
                {
                    DeleteFile(ref message, ref fileLengthDeleted, ref packagesDeleted, fileInfo, ReasonForDelete.KeepLimitedVersions);
                }
                else
                {
                    message += "File " + fileInfo.FullName + " is excluded from being deleted";
                }
            }
        }

        private static void ApplyPolicyForVersionsToKeepWithDaysToKeep(RetentionPolicyEntity policy, ref string message,
                                                                       ref long fileLengthDeleted, ref int packagesDeleted,
                                                                       IEnumerable<FileInfo> filesToDelete, List<ExcludedPackageEntity> listOfExcludedPackages)
        {
            foreach (var fileInfo in filesToDelete)
            {
                bool isExcluded = IsFileExcluded(fileInfo, listOfExcludedPackages);
                bool isGreaterThanDaysToKeep = IsFileOlderThanXDays(policy, fileInfo);

                if (!isExcluded)
                {
                    if (isGreaterThanDaysToKeep)
                    {
                        DeleteFile(ref message, ref fileLengthDeleted, ref packagesDeleted, fileInfo, ReasonForDelete.KeepLimitedVersionsAndOlderThanXDays);
                    }
                }
                else
                {
                    message += "File " + fileInfo.FullName + " is excluded from being deleted";
                }
            }
        }

        private static bool IsFileOlderThanXDays(RetentionPolicyEntity policy, FileInfo fi)
        {
            if (policy.DaysToKeepPackages > 0)
            {
                if (fi.LastWriteTime < DateTime.Now.AddDays(-policy.DaysToKeepPackages) &&
                    fi.LastAccessTime < DateTime.Now.AddDays(-policy.DaysToKeepPackages))
                {
                    return true;
                }
            }
            return false;
        }

        private static void DeleteFile(ref string message, ref long fileLengthDeleted, ref int packagesDeleted, FileInfo fi, ReasonForDelete reason)
        {          
            try
            {
                packagesDeleted++;
                fileLengthDeleted += fi.Length;
                fi.Delete();
                message += "Successfully deleted package: " + fi.FullName + ". Reason: " + reason + Environment.NewLine;
            }
            catch (Exception ex)
            {
                message += "Failed to delete package: " + fi.FullName + " (" + ex.Message + ")" + Environment.NewLine;
            }
        }

        private static bool IsFileExcluded(FileInfo fi, IEnumerable<ExcludedPackageEntity> listOfExcludedPackages)
        {
            foreach (var excludedPackage in listOfExcludedPackages)
            {
                if (excludedPackage.PartialMatch)
                {
                    if (fi.Name.ToLower().Contains(excludedPackage.PackageId.ToLower()))
                    {
                        return true;
                    }
                }
                else
                {
                    if (fi.Name.ToLower().Equals(excludedPackage.PackageId.ToLower()))
                    {
                        return true;
                    }
                }
            }


            return false;
        }


        public static bool UpdateRetentionPolicy(RetentionPolicyEntity retentionPolicy)
        {
            var repository = new MongoDbRepository<RetentionPolicyEntity>();
            repository.Update(retentionPolicy);

            return true;
        }

        public static string GetRetentionPolicyLog(Guid guid)
        {
            var repository = new MongoDbRepository<RetentionPolicyHistoryEntity>();
            var retentionPolicyHistory = repository.GetById(guid);

            if (retentionPolicyHistory == null)
            {
                return "Failed to find the retention policy log";
            }

            return retentionPolicyHistory.Log ?? "The log file is empty";
        }

        public static List<RetentionPolicyHistoryEntity> GetRetentionPolicyHistory()
        {

            var repository = new MongoDbRepository<RetentionPolicyHistoryEntity>();
            var history = repository.GetAll().OrderByDescending(hist => hist.Date);

            if (history.Count() > 50)
            {
                var removedResults = history.Take(50).ToList();
                removedResults.ForEach(r => r.Log = null);
                return removedResults;
            }

            var results = history.ToList();
            results.ForEach(r => r.Log = null);
            return results;
        }

        public static RetentionPolicyEntity FindRetentionPolicy(string feedName)
        {
            var repository = new MongoDbRepository<RetentionPolicyEntity>();
            return repository.Get(rt => rt.FeedName == feedName).FirstOrDefault();
        }

        public static List<RetentionPolicyEntity> GetRetentionPolicys()
        {
            var repository = new MongoDbRepository<RetentionPolicyEntity>();
            return repository.GetAll().ToList();
        }

        public static void InsertRetentionPolicy(RetentionPolicyEntity retentionPolicy)
        {
            var repository = new MongoDbRepository<RetentionPolicyEntity>();
            repository.Insert(retentionPolicy);
        }
    }
}