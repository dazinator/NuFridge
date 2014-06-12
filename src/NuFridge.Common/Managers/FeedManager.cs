using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
            var websiteManager = new WebsiteManager();

            try
            {
                if (!IsFeedNameValid(feedName, out message)) return false;

                var nuFridgeWebsiteName = ConfigurationManager.AppSettings["IIS.FeedWebsite.Name"];
                var nuFridgePort = ConfigurationManager.AppSettings["IIS.FeedWebsite.PortNumber"];
                var nuFridgeFeedDirectory = ConfigurationManager.AppSettings["IIS.FeedWebsite.RootDirectory"];

                int nuFridgePortNumber;
                if (!int.TryParse(nuFridgePort, out nuFridgePortNumber))
                {
                    nuFridgePortNumber = WebsiteManager.DefaultWebsitePortNumber;
                }

                WebsiteInfo website;
                bool exists = websiteManager.WebsiteExists(nuFridgeWebsiteName);
                if (!exists)
                {
                    var websiteInfo = new CreateWebsiteArgs(nuFridgeWebsiteName, nuFridgeFeedDirectory)
                        {
                            HostName = "*",
                            PortNumber = nuFridgePortNumber
                        };
                    website = websiteManager.CreateWebsite(websiteInfo);
                }
                else
                {
                    website = websiteManager.GetWebsite(nuFridgeWebsiteName);
                }

                var appPath = string.Format("/{0}", feedName);

                var applicationExists = websiteManager.ApplicationExists(nuFridgeWebsiteName, appPath);
                if (applicationExists)
                {
                    throw new Exception("Feed already exists at: " + appPath);
                }

                var binding = website.Bindings.FirstOrDefault();
                if (binding == null)
                {
                    throw new Exception("No IIS bindings found for " + nuFridgeWebsiteName);
                }

                var feedRootFolder = GetRootFeedFolder(website);
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

                CreateFilesInFeed(feedDirectory);

                websiteManager.CreateApplication(nuFridgeWebsiteName, appPath, feedDirectory);

                message = "Successfully created a feed called " + feedName;

                return true;

            }
            catch (Exception e)
            {
                message = e.Message + Environment.NewLine + (e.StackTrace ?? "");
                return false;
            }
        }

        private static string GetRootFeedFolder(WebsiteInfo website)
        {
            return website.Applications[0].VirtualDirectories[0].PhysicalPath;

            //var feedRootFolder = ConfigurationManager.AppSettings["NuFridge.Feeds.Folder"];
            //if (!Path.IsPathRooted(feedRootFolder))
            //{
            //    feedRootFolder = Path.Combine(website.Applications[0].VirtualDirectories[0].PhysicalPath, feedRootFolder);
            //}
            //return feedRootFolder;
        }

        private static bool IsFeedNameValid(string feedName, out string message)
        {
            message = null;

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

            return true;
        }

        private static void CreateFilesInFeed(string feedDirectory)
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


        public static bool DeleteFeed(string feedName, out string message)
        {
            if (!IsFeedNameValid(feedName, out message)) return false;

            var websiteManager = new WebsiteManager();

            var nuFridgeWebsiteName = ConfigurationManager.AppSettings["IIS.FeedWebsite.Name"];

            bool exists = websiteManager.WebsiteExists(nuFridgeWebsiteName);
            if (!exists)
            {
                message = "Could not find a feed website in IIS called '" + nuFridgeWebsiteName + "'";
                return false;
            }

            var website = websiteManager.GetWebsite(nuFridgeWebsiteName);
            var appPath = string.Format("/{0}", feedName);

            var applicationExists = websiteManager.ApplicationExists(nuFridgeWebsiteName, appPath);
            if (!applicationExists)
            {
                message = "No IIS application found for " + feedName;
                return false;
            }

            var application = website.Applications.FirstOrDefault(app => app.Path == appPath);
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

            var hasDeleteAccess = DirectoryHelper.HasDeleteRights(feedDirectory, identityName);
            if (!hasDeleteAccess)
            {
                throw new SecurityException(string.Format("The '{0}' user does not have delete rights for the '{1}' directory.", identityName, feedDirectory));
            }

            websiteManager.DeleteApplication(website.Name, appPath);

            RetentionPolicyManager.DeleteRetentionPolicy(feedName);

            //Delete files with checks on whether files are locked & with retries
            FileHelper.DeleteFilesInFolder(feedDirectory);

            Directory.Delete(feedDirectory, true);

            message = "Successfully removed the feed and deleted all packages for " + feedName;
            return true;
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