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
using NuFridge.Common.Feeds.Configuration;
using NuFridge.DataAccess.Model;
using NuFridge.Common.Validators;
using FluentValidation;

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

        private static bool HasFeedNameChanged(Feed feed, out string oldFeedName)
        {
            var repo = new MongoDbRepository<Feed>();

            oldFeedName = null;

            var existingFeed = repo.GetById(feed.Id);
            if (existingFeed.Name != feed.Name)
            {
                oldFeedName = existingFeed.Name;
                return true;
            }

            return false;
        }

        public static bool UpdateFeed(Feed feed, out string message)
        {
            ValidatorBuilder validatorBuilder = new ValidatorBuilder();
            validatorBuilder.Add<FeedValidator>(feed);

            var results = validatorBuilder.Validate();
            foreach (var result in results)
            {
                if (!result.IsValid)
                {
                    message = string.Join(Environment.NewLine, result.Errors.Select(err => err.ErrorMessage));
                    return false;
                }
            }

            string feedWebsiteName;
            if (!GetFeedWebsiteName(out message, out feedWebsiteName)) 
                return false;

            //Create the website and application managers which interact with IIS
            var feedWebsiteManager = new WebsiteManager(feedWebsiteName);
            var feedApplicationManager = new ApplicationManager(feedWebsiteManager);

            //Check the feed website has been created before we try to add to it
            if (!DoesFeedWebsiteExist(ref message, feedWebsiteManager))
                return false;

            ApplicationInfo application = null;
            string applicationPath = null;

            string oldFeedName;
            if (HasFeedNameChanged(feed, out oldFeedName))
            {

                //Check the old app path to see if it already exists, if it doesn't return false as we can't continue
                string oldAppPath;
                var isOldPathValid = GetApplicationPathForFeed(feedApplicationManager, oldFeedName, true, out oldAppPath);
                if (!isOldPathValid)
                {
                    message = "Could not find an application with a path of " + oldAppPath;
                    return false;
                }

                //Check the new app path to see if it already exists, if it does return false as we can't continue
                var isNewPathValid = GetApplicationPathForFeed(feedApplicationManager, feed.Name, false, out applicationPath);
                if (!isNewPathValid)
                {
                    message = "An existing application was already found at " + applicationPath;
                    return false;
                }

                //Get the existing IIS application for the feed to change
                application = feedApplicationManager.GetApplication(oldAppPath);

                //Get the IIS application physical paths for the old feed name and the new feed name
                string oldPath;
                string newPath;
                GetApplicationDirectoryPathsForRename(feed.Name, application, out oldPath, out newPath);

                //Update the app path and physical path with the new values
                application.Path = applicationPath;
                application.VirtualDirectories[0].PhysicalPath = newPath;

                //Save the application in IIS
                feedApplicationManager.UpdateApplication(application);


                //Move the directory from the old directory folder to the new one
                Directory.Move(oldPath, newPath);
            }

            if (applicationPath == null)
                GetApplicationPathForFeed(feedApplicationManager, feed.Name, true, out applicationPath);

            if (application == null)
                application = feedApplicationManager.GetApplication(applicationPath);

            IFeedConfig feedConfig = new KlondikeFeedConfig(Path.Combine(application.VirtualDirectories[0].PhysicalPath, @"Web.config"));
            feedConfig.UpdateAPIKey(feed.APIKey);

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

        private static bool GetFeedWebsiteName(out string message, out string nuFridgeWebsiteName)
        {
            message = null;
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


        public static bool CreateFeed(NuFridge.DataAccess.Model.Feed feed, out string message)
        {
            ValidatorBuilder validatorBuilder = new ValidatorBuilder();
            validatorBuilder.Add<FeedValidator>(feed);

            var results = validatorBuilder.Validate();
            foreach (var result in results)
            {
                if (!result.IsValid)
                {
                    message = string.Join(Environment.NewLine, result.Errors.Select(err => err.ErrorMessage));
                    return false;
                }
            }

            

            //Get the feed website name
            string feedWebsiteName;
            if (!GetFeedWebsiteName(out message, out feedWebsiteName))
                return false;

            //Get port number and directory path for the feed website
            var nuFridgePortNumber = GetFeedWebsitePortNumber();
            var nuFridgeFeedDirectory = ConfigurationManager.AppSettings["IIS.FeedWebsite.RootDirectory"];

            if (!Directory.Exists(nuFridgeFeedDirectory))
            {
                try
                {
                    Directory.CreateDirectory(nuFridgeFeedDirectory);
                }
                catch (Exception e)
                {
                    message = "Unable to create feed website directory: " + nuFridgeFeedDirectory + " - " +
                              e.Message;
                    return false;
                }

            }

            //Create the managers which interact with IIS
            var feedWebsiteManager = new WebsiteManager(feedWebsiteName);
            var feedApplicationManager = new ApplicationManager(feedWebsiteManager);

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
            bool pathResult = GetApplicationPathForFeed(feedApplicationManager, feed.Name, false, out appPath);
            if (!pathResult)
            {
                message = "An existing application was already found at " + appPath;
                return false;
            }

            var feedRootFolder = website.Applications[0].VirtualDirectories[0].PhysicalPath;
            var feedDirectory = Path.Combine(feedRootFolder, feed.Name);

            if (Directory.Exists(feedDirectory))
            {
                throw new Exception("A directory already exists for the " + feed.Name + " feed.");
            }



            //Create the NuGet feed
            CreateFilesInFeed(feedDirectory, feed.APIKey);

            //Create the application in IIS
            feedApplicationManager.CreateApplication(appPath, feedDirectory);

            message = "Successfully created a feed called " + feed.Name;



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

        private static void CreateFilesInFeed(string feedDirectory, string APIKey)
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
                    try
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
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

            IFeedConfig feedConfig = new KlondikeFeedConfig(Path.Combine(feedDirectory, @"Web.config"));
            feedConfig.UpdateAPIKey(APIKey);

        }


        public static bool DeleteFeed(Feed feed, out string message)
        {
            ValidatorBuilder validatorBuilder = new ValidatorBuilder();
            validatorBuilder.Add<FeedValidator>(feed);

            var results = validatorBuilder.Validate();
            foreach (var result in results)
            {
                if (!result.IsValid)
                {
                    message = string.Join(Environment.NewLine, result.Errors.Select(err => err.ErrorMessage));
                    return false;
                }
            }

            //Get the feed website name
            string feedWebsiteName;
            if (!GetFeedWebsiteName(out message, out feedWebsiteName))
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
            var pathResult = GetApplicationPathForFeed(feedApplicationManager, feed.Name, true, out appPath);
            if (!pathResult)
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

            //Delete the application from IIS
            feedApplicationManager.DeleteApplication(appPath);

            //Delete the retention policy if one is set up
            RetentionPolicyManager.DeleteRetentionPolicy(feed.Name);

            //Delete files with checks on whether files are locked & with retries
            FileHelper.DeleteFilesInFolder(feedDirectory);

            //Delete the feed directory
            Directory.Delete(feedDirectory, true);

            message = "Successfully removed the feed and deleted all packages for " + feed.Name;
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