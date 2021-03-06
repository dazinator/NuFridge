﻿using System;
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
using NuGet;

namespace NuFridge.Common.Manager
{
    public class FeedManager
    {
        private IRepository<Feed> FeedRepository { get; set; }
        private IRepository<FeedGroup> FeedGroupRepository { get; set; }

        private WebsiteManager WebsiteManager { get; set; }
        private ApplicationManager ApplicationManager { get; set; }

        public FeedManager(IRepository<Feed> feedRepository, IRepository<FeedGroup> feedGroupRepository)
        {
            FeedRepository = feedRepository;
            FeedGroupRepository = feedGroupRepository;
            Init();
        }

        public IEnumerable<Feed> GetAll()
        {
            var feeds = FeedRepository.GetAll();

            var website = WebsiteManager.GetWebsite();

            List<FeedGroup> groups = new List<FeedGroup>();

            foreach (var feed in feeds)
            {
                if (groups.Any(grp => grp.Id == feed.GroupId))
                {
                    feed.GroupName = groups.Single(grp => grp.Id == feed.GroupId).Name;
                }
                else
                {
                    var group = FeedGroupRepository.GetById(feed.GroupId);
                    feed.GroupName = group.Name;
                    groups.Add(group);
                }

                feed.FeedURL = string.Format("{0}/{1}", website.Bindings[0].Url, feed.Name);
            }

            return feeds;
        }

        public Feed GetById(Guid id)
        {
            var feed = FeedRepository.GetById(id);
            if (feed != null)
            {
                var website = WebsiteManager.GetWebsite();

                var group = FeedGroupRepository.GetById(feed.GroupId);
                feed.FeedURL = string.Format("{0}/{1}", website.Bindings[0].Url, feed.Name);
                feed.GroupName = group.Name;
            }
            return feed;
        }


        private void Init()
        {
            string message;
            string feedWebsiteName;

            if (!ConfigHelper.GetFeedWebsiteName(out message, out feedWebsiteName))
                throw new Exception(message);

            WebsiteManager = new WebsiteManager(feedWebsiteName);
            ApplicationManager = new ApplicationManager(WebsiteManager);

            //Get port number and directory path for the feed website
            var nuFridgePortNumber = ConfigHelper.GetFeedWebsitePortNumber();
            var nuFridgeFeedDirectory = ConfigurationManager.AppSettings["IIS.FeedWebsite.RootDirectory"];

            if (!Directory.Exists(nuFridgeFeedDirectory))
            {
                Directory.CreateDirectory(nuFridgeFeedDirectory);
            }

            if (!WebsiteManager.WebsiteExists())
            {
                var websiteInfo = new CreateWebsiteArgs(nuFridgeFeedDirectory)
                {
                    HostName = "*",
                    PortNumber = nuFridgePortNumber
                };
                WebsiteManager.CreateWebsite(websiteInfo);
            }
        }

        public void ScheduleImportPackagesJob(string sourceFeedUrl, string importToFeedName, string apiKey, out string jobName)
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

        private bool HasFeedNameChanged(Feed feed, out string oldFeedName)
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

        public bool UpdateFeed(Feed feed, out string message)
        {
            message = null;

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

            ApplicationInfo application = null;
            string applicationPath = null;

            string oldFeedName;
            if (HasFeedNameChanged(feed, out oldFeedName))
            {

                //Check the old app path to see if it already exists, if it doesn't return false as we can't continue
                string oldAppPath;
                var isOldPathValid = GetApplicationPathForFeed(oldFeedName, true, out oldAppPath);
                if (!isOldPathValid)
                {
                    message = "Could not find an application with a path of " + oldAppPath;
                    return false;
                }

                //Check the new app path to see if it already exists, if it does return false as we can't continue
                var isNewPathValid = GetApplicationPathForFeed(feed.Name, false, out applicationPath);
                if (!isNewPathValid)
                {
                    message = "An existing application was already found at " + applicationPath;
                    return false;
                }

                //Get the existing IIS application for the feed to change
                application = ApplicationManager.GetApplication(oldAppPath);

                //Get the IIS application physical paths for the old feed name and the new feed name
                string oldPath;
                string newPath;
                GetApplicationDirectoryPathsForRename(feed.Name, application, out oldPath, out newPath);

                //Update the app path and physical path with the new values
                application.Path = applicationPath;
                application.VirtualDirectories[0].PhysicalPath = newPath;

                //Move the directory from the old directory folder to the new one
                Directory.Move(oldPath, newPath);

                //Save the application in IIS
                ApplicationManager.UpdateApplication(application);
            }

            if (applicationPath == null)
                GetApplicationPathForFeed(feed.Name, true, out applicationPath);

            if (application == null)
                application = ApplicationManager.GetApplication(applicationPath);

            IFeedConfig feedConfig = new KlondikeFeedConfig(Path.Combine(application.VirtualDirectories[0].PhysicalPath, @"Web.config"), WebsiteManager.WebsiteName);
            feedConfig.UpdateAPIKey(feed.APIKey);

            PutFeedInGroup(feed);

            FeedRepository.Update(feed);

        


            return true;
        }

        private void PutFeedInGroup(Feed newEntity)
        {
            var groupName = newEntity.GroupName;
            var group = FeedGroupRepository.GetAll().FirstOrDefault(grp => grp.Name.ToLower() == groupName.ToLower());
            if (group != null)
            {
                newEntity.GroupId = group.Id;
            }
            else
            {
                group = new FeedGroup() {  Name = groupName, Id = Guid.NewGuid() };
                FeedGroupRepository.Insert(group);
                newEntity.GroupId = group.Id;
            }
        }


        private void GetApplicationDirectoryPathsForRename(string newFeedName, ApplicationInfo app, out string oldPath,
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

        private bool GetApplicationPathForFeed(string feedName, bool shouldExistAsApplication, out string appPath)
        {
            appPath = string.Format("/{0}", feedName);

            var applicationExists = ApplicationManager.ApplicationExists(appPath);

            return shouldExistAsApplication ? applicationExists : !applicationExists;
        }


        public bool CreateFeed(NuFridge.DataAccess.Model.Feed feed, out string message)
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


            //Check website exists, get or create the feed website
            WebsiteInfo website = WebsiteManager.GetWebsite();

            //Get the IIS application path and check the feed doesn't already exist in IIS
            string appPath;
            bool pathResult = GetApplicationPathForFeed(feed.Name, false, out appPath);
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
            CreateFilesInFeed(feedDirectory);

            //Update the feed config
            IFeedConfig feedConfig = new KlondikeFeedConfig(Path.Combine(feedDirectory, @"Web.config"), website.Name);
            feedConfig.UpdateAPIKey(feed.APIKey);

            //Create the application in IIS
            ApplicationManager.CreateApplication(appPath, feedDirectory);

            PutFeedInGroup(feed);

            FeedRepository.Insert(feed);



            message = "Successfully created a feed called " + feed.Name;

            return true;
        }



        private void CreateFilesInFeed(string feedDirectory)
        {
            Directory.CreateDirectory(feedDirectory);

            var resource = FileResources.Klondike;
            var archive = new ZipArchive(new MemoryStream(resource));

            FileHelper.ExtractZipToFolder(feedDirectory, archive);
        }


        public bool DeleteFeed(Feed feed, out string message)
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

            var website = WebsiteManager.GetWebsite();

            string appPath;
            var pathResult = GetApplicationPathForFeed(feed.Name, true, out appPath);
            if (!pathResult)
            {
                message = "No IIS application could be found at " + appPath;
                return false;
            }

            var application = ApplicationManager.GetApplication(appPath);

            //Check to see if we can find the feed website folder
            var feedDirectory = application.VirtualDirectories[0].PhysicalPath;
            if (!Directory.Exists(feedDirectory))
            {
                message = "The feed package folder does not exist at " + feedDirectory;
                return false;
            }

            var identityName = WindowsIdentity.GetCurrent().Name;

            //Delete the application from IIS
            ApplicationManager.DeleteApplication(appPath);

            //Delete the retention policy if one is set up
            RetentionPolicyManager.DeleteRetentionPolicy(feed.Name);

            //Delete files with checks on whether files are locked & with retries
            FileHelper.DeleteFilesInFolder(feedDirectory);

            //Delete the feed directory
            Directory.Delete(feedDirectory, true);

            FeedRepository.Delete(feed);

            message = "Successfully removed the feed and deleted all packages for " + feed.Name;
            return true;
        }

        public bool UploadPackage(Feed item, int contentLength, Stream stream, out string message)
        {
            message = null;
            var testProgetUri = new Uri(string.Format("{0}/api/odata", item.FeedURL));
            var repos = new DataServicePackageRepository(testProgetUri);

            var packageServer = new PackageServer(string.Format("{0}/api/packages", item.FeedURL), "NuFridge");

            var packageToUpload = new ZipPackage(stream);
            if (repos.Exists(packageToUpload.Id, packageToUpload.Version))
            {
                message = string.Format("The package called '{0}' with the version '{1}' already exists in the feed.", packageToUpload.Id, packageToUpload.Version);
                return false;
            }

            
            packageServer.PushPackage(item.APIKey, packageToUpload, contentLength, 50000, false);

            message = "The package was successfully uploaded.";
            return true;
        }

        public Stream DownloadPackage(Guid feedId, string packageId, string version)
        {
            var item = GetById(feedId);

            var testProgetUri = new Uri(string.Format("{0}/api/odata", item.FeedURL));
            var repos = new DataServicePackageRepository(testProgetUri);

            var package = repos.FindPackage(packageId, SemanticVersion.Parse(version));
            if (package != null)
                return package.GetStream();

            return null;
        }
    }
}