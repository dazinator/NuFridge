﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Microsoft.Web.Administration;
using System.Configuration;
using NuFridge.DataAccess.Entity;
using NuFridge.DataAccess.Repositories;
using NuFridge.Common.Jobs;
using Quartz;
using Quartz.Impl;

namespace NuFridge.Common.Manager
{
    public class FeedManager
    {
      
        readonly IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

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

            jobName = string.Format("Import Packages to {0} {1}", importToFeedName,
                                           DateTime.Now.ToString("dd-MM-yyyy hhmmssfff tt"));


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

            var klondikeWebsiteName = ConfigurationManager.AppSettings["KlondikeWebsiteName"];




            ServerManager mgr = new ServerManager();

            var site = mgr.Sites.FirstOrDefault(st => st.Name == klondikeWebsiteName);
            if (site == null)
            {
                message = "IIS Website not found for " + klondikeWebsiteName;
                return false;
            }

            var application = site.Applications.FirstOrDefault(st => st.Path.ToLower() == string.Format("/feeds/{0}", feedName.ToLower()));
            if (application != null)
            {
                message = "A feed already exists for " + feedName;
                return false;
            }

            var klondikeTemplatePath = Path.Combine(site.Applications.First().VirtualDirectories[0].PhysicalPath, "FeedTemplate");

            if (!Directory.Exists(klondikeTemplatePath))
            {
                message = "Could not find NuGet feed template directory at path " + klondikeTemplatePath;
                return false;
            }

            var files = Directory.GetFiles(klondikeTemplatePath);
            if (files == null || !files.Any())
            {
                message = "No NuGet feed template has been setup at path " + klondikeTemplatePath;
                return false;
            }

            var binding = site.Bindings.FirstOrDefault();
            if (binding == null)
            {
                throw new Exception("No IIS bindings found for " + klondikeWebsiteName);
            }


            var rootWebsiteUrl = string.Format("{0}://{1}:{2}", binding.Protocol, Environment.MachineName, binding.EndPoint.Port);


            MongoDbRepository<FeedEntity> repository = new MongoDbRepository<FeedEntity>();
            repository.Insert(new FeedEntity(feedName, string.Format("{0}/Feeds/{1}", rootWebsiteUrl, feedName)));

            var feedDirectory = Path.Combine(site.Applications.First().VirtualDirectories[0].PhysicalPath, "Feeds\\", feedName);
            //if (!Directory.Exists(feedDirectory))
            //{
            //    message = "The feed package folder does not exist at " + feedDirectory;
            //    return false;
            //}

            try
            {
                Directory.CreateDirectory(feedDirectory);

                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(klondikeTemplatePath, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(klondikeTemplatePath, feedDirectory));

                //Copy all the files & Replaces any files with the same name
                foreach (string newPath in Directory.GetFiles(klondikeTemplatePath, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(klondikeTemplatePath, feedDirectory), true);
            }
            catch (Exception ex)
            {
                message = "An exception was thrown when creating the feed directory: " + ex.Message;
                return false;
            }

            try
            {
                site.Applications.Add("/Feeds/" + feedName, feedDirectory);
                mgr.CommitChanges();
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

        public static bool DeleteFeed(string feedName, out string message)
        {

            if (string.IsNullOrWhiteSpace(feedName))
            {
                message = "Feed name is mandatory";
                return false;
            }
            var klondikeWebsiteName = ConfigurationManager.AppSettings["KlondikeWebsiteName"];



            ServerManager mgr = new ServerManager();

            var site = mgr.Sites.FirstOrDefault(st => st.Name == klondikeWebsiteName);
            if (site == null)
            {
                message = "IIS Website not found for " + klondikeWebsiteName;
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