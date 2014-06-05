using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Microsoft.Web.Administration;
using System.Configuration;
using NuFridge.Common.Helpers;
using NuFridge.DataAccess.Entity;
using NuFridge.DataAccess.Entity.Feeds;
using NuFridge.DataAccess.Repositories;
using NuFridge.Common.Jobs;
using Quartz;
using Quartz.Impl;
using System.IO.Compression;

namespace NuFridge.Common.Manager
{
    public class FeedManager
    {



        //readonly IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public FeedManager()
        {

        }

        public static void ScheduleImportPackagesJob(string sourceFeedUrl, string importToFeedName, string apiKey, out string jobName)
        {
            if (string.IsNullOrWhiteSpace(sourceFeedUrl))
            {
                throw new ArgumentNullException("sourceFeedUrl");
            }

            if (!Regex.IsMatch(importToFeedName, "^[a-zA-Z0-9.-]+$"))
            {
                throw new ArgumentNullException("Only alphanumeric characters are allowed in the feed name");
            }

            var runOnceImmediatelyTrigger = ScheduleManager.RunOnceImmediatelyTrigger<ImportPackagesJob>().Build();
            jobName = string.Format("Import Packages to {0} {1}", importToFeedName, DateTime.Now.ToString("dd-MM-yyyy hhmmssfff tt"));

            IJobDetail job = JobBuilder.Create<ImportPackagesJob>()
              .WithIdentity(jobName, "PackageImport")
            .UsingJobData(ImportPackagesJob.SourceFeedUrlKey, sourceFeedUrl)
            .UsingJobData(ImportPackagesJob.FeedNameKey, importToFeedName)
            .UsingJobData(ImportPackagesJob.ApiKey, apiKey)
            .Build();

            ScheduleManager.Schedule<ImportPackagesJob>(job, runOnceImmediatelyTrigger);
        }

        public static bool CreateFeed(string feedName, out string message)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(feedName))
                {
                    message = "Feed name is mandatory";
                    return false;
                }

                if (!Regex.IsMatch(feedName, "^[a-zA-Z0-9.-]+$"))
                {
                    message = "Only alphanumeric characters are allowed in the feed name";
                    return false;
                }

                var nuFridgeWebsiteName = ConfigurationManager.AppSettings["NuFridge.Website.Name"];
                var nuFridgePort = ConfigurationManager.AppSettings["NuFridge.Website.PortNumber"];
                int nuFridgePortNumber;
                if (!int.TryParse(nuFridgePort, out nuFridgePortNumber))
                {
                    nuFridgePortNumber = WebsiteManager.DefaultWebsitePortNumber;
                }

                var websiteManager = new WebsiteManager();
                bool exists = websiteManager.WebsiteExists(nuFridgeWebsiteName);
                if (!exists)
                {
                    var websitePath = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                    var websiteInfo = new CreateWebsiteArgs(nuFridgeWebsiteName, websitePath);
                    websiteInfo.HostName = "*";
                    websiteInfo.PortNumber = nuFridgePortNumber;
                    websiteManager.CreateWebsite(websiteInfo);
                }

                var appPath = string.Format("/feeds/{0}", feedName);
                var applicationExists = websiteManager.ApplicationExists(nuFridgeWebsiteName, appPath);
                if (applicationExists)
                {
                    throw new Exception("Feed already exists at: " + appPath);
                }

                var website = websiteManager.GetWebsite(nuFridgeWebsiteName);
                var binding = website.Bindings.FirstOrDefault();
                if (binding == null)
                {
                    throw new Exception("No IIS bindings found for " + nuFridgeWebsiteName);
                }
               
                var rootWebsiteUrl = string.Format("{0}://{1}:{2}", binding.Protocol, binding.GetFriendlyHostName(), binding.EndPoint.Port);
                
                var repository = new MongoDbRepository<FeedEntity>();
                repository.Insert(new FeedEntity(feedName, string.Format("{0}/Feeds/{1}", rootWebsiteUrl, feedName)));

                var feedRootFolder = ConfigurationManager.AppSettings["NuFridge.Feeds.Folder"];
                if (!Path.IsPathRooted(feedRootFolder))
                {
                    feedRootFolder = Path.Combine(website.Applications[0].VirtualDirectories[0].PhysicalPath, feedRootFolder);
                }

                var feedDirectory = Path.Combine(feedRootFolder, feedName);

                if (Directory.Exists(feedDirectory))
                {
                    throw new Exception("A directory already exists for the " + feedName + " feed.");
                }

                var identityName = WindowsIdentity.GetCurrent().Name;

                var hasWriteAccess = DirectoryHelper.HasWriteAccess(feedRootFolder, identityName);
                if (!hasWriteAccess)
                {
                    throw new SecurityException(string.Format("The '{0}' user does not have write access to the '{1}' directory.", identityName, feedRootFolder));
                }

                try
                {
                    Directory.CreateDirectory(feedDirectory);

                    var resource = FileResources.Klondike;

                    var stream = new MemoryStream(resource);
                    var archive = new ZipArchive(stream);

                    foreach (var entry in archive.Entries)
                    {
                        var directoryPath = Path.Combine(feedDirectory, Path.GetDirectoryName(entry.FullName));

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        var fileName = Path.Combine(directoryPath, entry.Name);

                        using (var entryStream = entry.Open())
                        {
                            using (var outputStream = File.Create(fileName))
                            {
                                entryStream.CopyTo(outputStream);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = "An exception was thrown when creating the feed directory: " + ex.Message;
                    return false;
                }

                try
                {
                    string path = "/Feeds/" + feedName;
                    websiteManager.CreateApplication(nuFridgeWebsiteName, path, feedDirectory);
                }
                catch (Exception ex)
                {
                    message = "An exception was thrown when adding the IIS application: " + ex.Message;
                    return false;
                }

                //RetentionPolicyManager.Instance.RefreshRetentionPolicies();
                message = "Successfully created a feed called " + feedName;
                return true;

            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }
        }


        public static bool DeleteFeed(string feedName, out string message)
        {

            if (string.IsNullOrWhiteSpace(feedName))
            {
                message = "Feed name is mandatory";
                return false;
            }
            var nuFridgeWebsiteName = ConfigurationManager.AppSettings["NuFridge.Website.Name"];



            ServerManager mgr = new ServerManager();

            var site = mgr.Sites.FirstOrDefault(st => st.Name == nuFridgeWebsiteName);
            if (site == null)
            {
                message = "IIS Website not found for " + nuFridgeWebsiteName;
                return false;
            }

            var application = site.Applications.FirstOrDefault(st => st.Path.ToLower() == string.Format("/feeds/{0}", feedName.ToLower()));
            if (application == null)
            {
                message = "No IIS application found for " + feedName;
                return false;
            }

            var feedDirectory = application.VirtualDirectories[0].PhysicalPath;
            if (!Directory.Exists(feedDirectory))
            {
                message = "The feed package folder does not exist at " + feedDirectory;
                return false;
            }

            var identityName = WindowsIdentity.GetCurrent().Name;

            var hasWriteAccess = DirectoryHelper.HasDeleteRights(feedDirectory, identityName);
            if (!hasWriteAccess)
            {
                throw new SecurityException(string.Format("The '{0}' user does not have write access to the '{1}' directory.", identityName, feedDirectory));
            }

            MongoDbRepository<FeedEntity> repository = new MongoDbRepository<FeedEntity>();
            var feed = repository.Get(fd => fd.Name == feedName).FirstOrDefault();
            repository.Delete(feed);

            try
            {
                site.Applications.Remove(application);
                mgr.CommitChanges();
            }
            catch (Exception ex)
            {
                message = "An exception was thrown when removing the IIS application (no packages have been deleted): " + ex.Message;
                return false;
            }

            try
            {
                RetentionPolicyManager.DeleteRetentionPolicy(feedName);
            }
            catch (Exception ex)
            {
                message = "An exception was thrown when deleting the retention policy for the + " + feedName + " feed: " + ex.Message;
                return false;
            }


            DirectoryInfo di = new DirectoryInfo(feedDirectory);
            //Append the opposite of the read only attribute to the directory
            di.Attributes &= ~FileAttributes.ReadOnly;

            foreach (string dirPath in Directory.GetDirectories(feedDirectory, "*", SearchOption.AllDirectories))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
                directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(feedDirectory, "*.*", SearchOption.AllDirectories))
            {
                FileInfo fi = new FileInfo(newPath);
                fi.Attributes &= ~FileAttributes.ReadOnly;
            }



            var parentFolders = Directory.GetDirectories(feedDirectory, "*", SearchOption.TopDirectoryOnly);

            DeleteFilesInFolder(parentFolders);
            Thread.Sleep(2500);

            try
            {
                Directory.Delete(feedDirectory, true);
            }
            catch (IOException ex)
            {
                message = "An exception was thrown when deleting the feed directory: " + ex.Message;
                return false;
            }



            //RefreshFeeds();
            // RetentionPolicyManager.Instance.RefreshRetentionPolicies();

            message = "Successfully removed the feed and deleted all packages for " + feedName;
            return true;
        }

        private static void DeleteFilesInFolder(string[] parentFolders)
        {
            foreach (string dirPath in parentFolders)
            {
                if (!dirPath.Contains("\\Lucene\\Users"))
                {
                    DeleteFilesInDirectory(dirPath);
                }

                var childFolders = Directory.GetDirectories(dirPath, "*", SearchOption.TopDirectoryOnly);
                DeleteFilesInFolder(childFolders);
            }
        }

        private static void DeleteFilesInDirectory(string dirPath)
        {
            foreach (string newPath in Directory.GetFiles(dirPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                if (!dirPath.Contains("\\Lucene\\Users"))
                {
                    FileInfo fi = new FileInfo(newPath);
                    try
                    {
                        fi.Delete();
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        public static FeedEntity FindFeed(string feedName)
        {
            MongoDbRepository<FeedEntity> repository = new MongoDbRepository<FeedEntity>();
            return repository.Get(fd => fd.Name == feedName).FirstOrDefault();
        }

        public static FeedEntity FindFeed(Guid id)
        {
            MongoDbRepository<FeedEntity> repository = new MongoDbRepository<FeedEntity>();
            return repository.Get(fd => fd.Id == id).FirstOrDefault();
        }
    }
}