using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using NuFridge.Common.IIS;
using NuFridge.Common.Manager;

namespace NuFridge.Common.Managers.IIS
{
    public class ApplicationManager
    {
        private WebsiteManager WebsiteManager { get; set; }

        public ApplicationManager(WebsiteManager websiteManager)
        {
            WebsiteManager = websiteManager;
        }

        public ApplicationManager(string websiteName)
            : this(new WebsiteManager(websiteName))
        {
            
        }

        public bool ApplicationExists(string path)
        {
                var website = WebsiteManager.GetWebsite();

                if (website == null)
                {
                    return false;
                }

                return website.Applications.Any(st => st.Path.ToLower() == path.ToLower());
        }

        public ApplicationInfo GetApplication(string appPath)
        {
            var website = WebsiteManager.GetWebsite();
                if (website == null)
                {
                    throw new InvalidOperationException("A website with the name: " + WebsiteManager.WebsiteName + " doesn't exist.");
                }

                var application = website.Applications.FirstOrDefault(app => app.Path == appPath);
                if (application == null)
                {
                    throw new InvalidOperationException("An application with the path: " + appPath + " doesn't exist.");
                }

                var appInfo = new ApplicationInfo { Path = appPath };
                foreach (var v in application.VirtualDirectories)
                {
                    var vinfo = new VirtualDirectoryInfo { Path = v.Path, PhysicalPath = v.PhysicalPath };
                    appInfo.VirtualDirectories.Add(vinfo);
                }

                return appInfo;
        }

        public void CreateApplication(string path, string physicalPath)
        {
            var website = WebsiteManager.ServerManager.Sites.FirstOrDefault(w => w.Name == WebsiteManager.WebsiteName);
            if (website == null)
            {
                throw new InvalidOperationException("A website with the name: " + WebsiteManager.WebsiteName + " doesn't exist.");
            }

            //Add the website to IIS
            website.Applications.Add(path, physicalPath);

            //Commit the changes to IIS
            WebsiteManager.ServerManager.CommitChanges();
        }

        public void DeleteApplication(string appPath)
        {
            //We cant use the websitemanager here as we need to make changes to the website and commit the changes
            var website = WebsiteManager.ServerManager.Sites.FirstOrDefault(w => w.Name == WebsiteManager.WebsiteName);
            if (website == null)
            {
                throw new InvalidOperationException("A website with the name: " + WebsiteManager.WebsiteName + " doesn't exist.");
            }

            var application = website.Applications.FirstOrDefault(app => app.Path == appPath);
            if (application == null)
            {
                throw new InvalidOperationException("An application with the path: " + appPath + " doesn't exist.");
            }

            //Remove the website from IIS
            website.Applications.Remove(application);

            //Commit the changes to IIS
            WebsiteManager.ServerManager.CommitChanges();
        }

        public void UpdateApplication(ApplicationInfo appInfo)
        {
            //We cant use the websitemanager here as we need to make changes to the website and commit the changes
            var website = WebsiteManager.ServerManager.Sites.FirstOrDefault(w => w.Name == WebsiteManager.WebsiteName);
            if (website == null)
            {
                throw new InvalidOperationException("A website with the name: " + WebsiteManager.WebsiteName + " doesn't exist.");
            }

            var application = website.Applications.FirstOrDefault(app => app.Path == appInfo.PreviousPath);
            if (application == null)
            {
                throw new InvalidOperationException("An application with the path: " + appInfo.PreviousPath +
                                                    " doesn't exist.");
            }

            //Update the application path
            application.Path = appInfo.Path;

            //Update the application physical path
            application.VirtualDirectories[0].PhysicalPath = appInfo.VirtualDirectories[0].PhysicalPath;

            //Save the changes in IIS
            WebsiteManager.ServerManager.CommitChanges();

            //Set the previous path back to null
            appInfo.PreviousPath = null;
        }
    }
}