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
using NuFridge.Common.IIS;
using NuFridge.Common.Managers;
using NuFridge.Common.Managers.IIS;
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

        public static bool UpdateFeed(string oldFeedName, string newFeedName, out string message)
        {
            if (!IsFeedNameValid(newFeedName, out message)) return false;

            string feedWebsiteName;
            if (!GetFeedWebsiteName(ref message, out feedWebsiteName)) 
                return false;

            //Create the website and application managers which interact with IIS
            var feedWebsiteManager = new WebsiteManager(feedWebsiteName);
            var feedApplicationManager = new ApplicationManager(feedWebsiteManager);

            //Check the feed website has been created before we try to add to it
            if (!DoesFeedWebsiteExist(ref message, feedWebsiteManager))
                return false;

            //Check the old app path to see if it already exists, if it doesn't return false as we can't continue
            string oldAppPath;
            var isOldPathValid = GetApplicationPathForFeed(feedApplicationManager, oldFeedName, true, out oldAppPath);
            if (!isOldPathValid)
            {
                message = "Could not find an application with a path of " + oldAppPath;
                return false;
            }

            //Check the new app path to see if it already exists, if it does return false as we can't continue
            string newAppPath;
            var isNewPathValid = GetApplicationPathForFeed(feedApplicationManager, newFeedName, false, out newAppPath);
            if (!isNewPathValid)
            {
                message = "An existing application was already found at " + newAppPath;
                return false;
            }

            //Get the existing IIS application for the feed to change
            var application = feedApplicationManager.GetApplication(oldAppPath);

            //Get the IIS application physical paths for the old feed name and the new feed name
            string oldPath;
            string newPath;
            GetApplicationDirectoryPathsForRename(newFeedName, application, out oldPath, out newPath);

            //Update the app path and physical path with the new values
            application.Path = newAppPath;
            application.VirtualDirectories[0].PhysicalPath = newPath;

            //Save the application in IIS
            feedApplicationManager.UpdateApplication(application);

            //Move the directory from the old directory folder to the new one
            Directory.Move(oldPath, newPath);

            return true;
        }

        private static bool DoesFeedWebsiteExist(ref string message, WebsiteManager feedWebsiteManager)
        {
            bool exists = feedWebsiteManager.WebsiteExists();
            if (!exists)
            {
                message = "Failed to find the feed website";
                return false;
            }
            return true;
        }

        private static bool GetFeedWebsiteName(ref string message, out string nuFridgeWebsiteName)
        {
            nuFridgeWebsiteName = ConfigurationManager.AppSettings["IIS.FeedWebsite.Name"];

            if (string.IsNullOrWhiteSpace(nuFridgeWebsiteName))
            {
                message = "The IIS.FeedWebsite.Name app setting has not been configured.";
                return false;
            }
            return true;
        }

        private static void GetApplicationDirectoryPathsForRename(string newFeedName, ApplicationInfo app, out string oldPath,
                                                                  out string newPath)
        {
            oldPath = app.VirtualDirectories[0].PhysicalPath;

            if (!Directory.Exists(oldPath))
            {
                throw new DirectoryNotFoundException("Could not find a directory at " + oldPath + " for the feed");
            }

            var parentPath = Directory.GetParent(app.VirtualDirectories[0].PhysicalPath);

            newPath = Path.Combine(parentPath.FullName, newFeedName);

            if (Directory.Exists(newPath))
            {
                throw new Exception("A folder already exists for the new feed name at " + newPath);
            }
        }

        private static bool GetApplicationPathForFeed(ApplicationManager applicationManager, string feedName, bool shouldExistAsApplication, out string appPath)
        {
            appPath = string.Format("/{0}", feedName);

            var applicationExists = applicationManager.ApplicationExists(appPath);

            return shouldExistAsApplication ? applicationExists : !applicationExists;
        }


        public static bool CreateFeed(string feedName, out string message)
        {
            //Check the feed name provided is valid
            if (!IsFeedNameValid(feedName, out message)) return false;

            //Get the feed website name
            string feedWebsiteName;
            if (!GetFeedWebsiteName(ref message, out feedWebsiteName))
                return false;

            //Get port number and directory path for the feed website
            var nuFridgePortNumber = GetFeedWebsitePortNumber();
            var nuFridgeFeedDirectory = ConfigurationManager.AppSettings["IIS.FeedWebsite.RootDirectory"];

            //Create the managers which interact with IIS
            var feedWebsiteManager = new WebsiteManager(feedWebsiteName);
            var feedApplicationManager = new ApplicationManager(feedWebsiteManager);

            //Check the feed website has been created before we try to add to it
            if (!DoesFeedWebsiteExist(ref message, feedWebsiteManager))
                return false;


            //Check website exists, get or create the feed website
            WebsiteInfo website;
            bool exists = feedWebsiteManager.WebsiteExists();
            if (!exists)
            {
                var websiteInfo = new CreateWebsiteArgs(nuFridgeFeedDirectory)
                    {
                        HostName = "*",
                        PortNumber = nuFridgePortNumber
                    };
                website = feedWebsiteManager.CreateWebsite(websiteInfo);
            }
            else
            {
                website = feedWebsiteManager.GetWebsite();
            }

            //Get the IIS application path and check the feed doesn't already exist in IIS
            string appPath;
            bool result = GetApplicationPathForFeed(feedApplicationManager, feedName, false, out appPath);
            if (!result)
            {
                message = "An existing application was already found at " + appPath;
                return false;
            }

            var feedRootFolder = website.Applications[0].VirtualDirectories[0].PhysicalPath;
            var feedDirectory = Path.Combine(feedRootFolder, feedName);

            if (Directory.Exists(feedDirectory))
            {
                throw new Exception("A directory already exists for the " + feedName + " feed.");
            }

            var identityName = WindowsIdentity.GetCurrent().Name;

            //Check the user has write permission to the feed folder
            var hasWriteAccess = DirectoryHelper.HasWriteAccess(feedRootFolder, identityName);
            if (!hasWriteAccess)
            {
                throw new SecurityException(
                    string.Format("The '{0}' user does not have write access to the '{1}' directory.", identityName,
                                  feedRootFolder));
            }

            //Create the NuGet feed
            CreateFilesInFeed(feedDirectory);

            //Create the application in IIS
            feedApplicationManager.CreateApplication(appPath, feedDirectory);

            message = "Successfully created a feed called " + feedName;

            return true;
        }

        private static int GetFeedWebsitePortNumber()
        {
            var nuFridgePort = ConfigurationManager.AppSettings["IIS.FeedWebsite.PortNumber"];

            int nuFridgePortNumber;
            if (!int.TryParse(nuFridgePort, out nuFridgePortNumber))
            {
                nuFridgePortNumber = WebsiteManager.DefaultWebsitePortNumber;
            }
            return nuFridgePortNumber;
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

            //For each file in the zip archive
            foreach (var entry in archive.Entries)
            {
                //Get the parent folder and check it exists
                var directoryPath = Path.Combine(feedDirectory, Path.GetDirectoryName(entry.FullName));

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                //Construct the file path including the directory to store the feed in
                var fileName = Path.Combine(directoryPath, entry.Name);

                //Open the zip archive entry stream and copy it to the file output stream
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
            //Check the feed name provided is valid
            if (!IsFeedNameValid(feedName, out message)) return false;

            //Get the feed website name
            string feedWebsiteName;
            if (!GetFeedWebsiteName(ref message, out feedWebsiteName))
                return false;

            var feedWebsiteManager = new WebsiteManager(feedWebsiteName);
            var feedApplicationManager = new ApplicationManager(feedWebsiteManager);

            bool exists = feedWebsiteManager.WebsiteExists();
            if (!exists)
            {
                message = "Could not find a feed website in IIS called '" + feedWebsiteName + "'";
                return false;
            }

            var website = feedWebsiteManager.GetWebsite();

            string appPath;
            var result = GetApplicationPathForFeed(feedApplicationManager, feedName, true, out appPath);
            if (!result)
            {
                message = "No IIS application could be found at " + appPath;
                return false;
            }

            var application = feedApplicationManager.GetApplication(appPath);

            //Check to see if we can find the feed website folder
            var feedDirectory = application.VirtualDirectories[0].PhysicalPath;
            if (!Directory.Exists(feedDirectory))
            {
                message = "The feed package folder does not exist at " + feedDirectory;
                return false;
            }

            var identityName = WindowsIdentity.GetCurrent().Name;

            //Check we have permission to delete from the feed directory
            var hasDeleteAccess = DirectoryHelper.HasDeleteRights(feedDirectory, identityName);
            if (!hasDeleteAccess)
            {
                throw new SecurityException(string.Format("The '{0}' user does not have delete rights for the '{1}' directory.", identityName, feedDirectory));
            }

            //Delete the application from IIS
            feedApplicationManager.DeleteApplication(appPath);

            //Delete the retention policy if one is set up
            RetentionPolicyManager.DeleteRetentionPolicy(feedName);

            //Delete files with checks on whether files are locked & with retries
            FileHelper.DeleteFilesInFolder(feedDirectory);

            //Delete the feed directory
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