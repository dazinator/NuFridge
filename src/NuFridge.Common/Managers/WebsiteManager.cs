using System;
using System.Linq;
using Microsoft.Web.Administration;
using NuFridge.Common.IIS;

namespace NuFridge.Common.Manager
{
    public class WebsiteManager
    {

        public const int DefaultWebsitePortNumber = 8420;

        public WebsiteManager()
        {

        }

        public bool WebsiteExists(string sitename)
        {
            using (var manager = new ServerManager())
            {
                return manager.Sites.Any(w => w.Name == sitename);
            }
        }

        public WebsiteInfo CreateWebsite(CreateWebsiteArgs createWebsiteArgs)
        {
            using (var manager = new ServerManager())
            {
                if (WebsiteExists(createWebsiteArgs.Name))
                {
                    throw new InvalidOperationException("A website with the name: " + createWebsiteArgs.Name + " already exists.");
                }
                var bindingInfoString = createWebsiteArgs.GetBindingInformationString();
                var site = manager.Sites.Add(createWebsiteArgs.Name, createWebsiteArgs.Protocol.ToString().ToLower(), bindingInfoString, createWebsiteArgs.PhysicalPath);
                manager.CommitChanges();
                return LoadWebsiteInfo(site);
            }
        }

        public WebsiteInfo GetWebsite(string websiteName)
        {
            using (var manager = new ServerManager())
            {
                var website = manager.Sites.FirstOrDefault(w => w.Name == websiteName);
                if (website == null)
                {
                    return null;
                }
                return LoadWebsiteInfo(website);
            }
        }

        public WebsiteInfo LoadWebsiteInfo(Site website)
        {
            var info = new WebsiteInfo();
            info.Name = website.Name;
            foreach (var b in website.Bindings)
            {
                var binfo = new BindingInfo();
                binfo.BindingInformation = b.BindingInformation;
                binfo.Host = b.Host;
                binfo.IsIpPortHostBinding = b.IsIPPortHostBinding;
                binfo.Protocol = b.Protocol;
                binfo.EndPoint = b.EndPoint;
                info.Bindings.Add(binfo);
            }
            foreach (var a in website.Applications)
            {
                var ainfo = new ApplicationInfo();
                ainfo.Path = a.Path;
                foreach (var v in a.VirtualDirectories)
                {
                    var vinfo = new VirtualDirectoryInfo();
                    vinfo.Path = v.Path;
                    vinfo.PhysicalPath = v.PhysicalPath;
                    ainfo.VirtualDirectories.Add(vinfo);
                }
                info.Applications.Add(ainfo);
            }
            return info;
        }

        public bool ApplicationExists(string websiteName, string path)
        {

            using (var manager = new ServerManager())
            {
                var website = manager.Sites.FirstOrDefault(w => w.Name == websiteName);
                if (website == null)
                {
                    return false;
                }

                return website.Applications.Any(st => st.Path.ToLower() == path.ToLower());

            }

        }

        public ApplicationInfo GetApplication(string websiteName, string appPath)
        {
            using (var manager = new ServerManager())
            {
                var website = manager.Sites.FirstOrDefault(w => w.Name == websiteName);
                if (website == null)
                {
                    throw new InvalidOperationException("A website with the name: " + websiteName + " doesn't exist.");
                }

                var application = website.Applications.FirstOrDefault(app => app.Path == appPath);
                if (application == null)
                {
                    throw new InvalidOperationException("An application with the path: " + appPath + " doesn't exist.");
                }

               var appInfo = new ApplicationInfo {Path = appPath};
                foreach (var v in application.VirtualDirectories)
               {
                   var vinfo = new VirtualDirectoryInfo {Path = v.Path, PhysicalPath = v.PhysicalPath};
                   appInfo.VirtualDirectories.Add(vinfo);
               }

                return appInfo;
            }
        }

        public void DeleteApplication(string websiteName, string appPath)
        {
            using (var manager = new ServerManager())
            {
                var website = manager.Sites.FirstOrDefault(w => w.Name == websiteName);
                if (website == null)
                {
                    throw new InvalidOperationException("A website with the name: " + websiteName + " doesn't exist.");
                }

                var application = website.Applications.FirstOrDefault(app => app.Path == appPath);
                if (application == null)
                {
                    throw new InvalidOperationException("An application with the path: " + appPath + " doesn't exist.");
                }

                website.Applications.Remove(application);
                manager.CommitChanges();
            }
        }
        
        public void CreateApplication(string websiteName, string path, string physicalPath)
        {
            using (var manager = new ServerManager())
            {
                var website = manager.Sites.FirstOrDefault(w => w.Name == websiteName);
                if (website == null)
                {
                    throw new InvalidOperationException("A website with the name: " + websiteName + " doesn't exist.");
                }
                website.Applications.Add(path, physicalPath);
                //   website.Applications.Add("/Feeds/" + feedName, feedDirectory);
                manager.CommitChanges();
            }
        }

        public void UpdateApplication(string websiteName, ApplicationInfo appInfo)
        {
            using (var manager = new ServerManager())
            {
                var website = manager.Sites.FirstOrDefault(w => w.Name == websiteName);
                if (website == null)
                {
                    throw new InvalidOperationException("A website with the name: " + websiteName + " doesn't exist.");
                }

                var application = website.Applications.FirstOrDefault(app => app.Path == appInfo.PreviousPath);
                if (application == null)
                {
                    throw new InvalidOperationException("An application with the path: " + appInfo.PreviousPath +
                                                        " doesn't exist.");
                }

                application.Path = appInfo.Path;
        
                application.VirtualDirectories[0].PhysicalPath = appInfo.VirtualDirectories[0].PhysicalPath;
                manager.CommitChanges();

                //Set the previous path back to null
                appInfo.PreviousPath = null;
            }
        }
    }
}