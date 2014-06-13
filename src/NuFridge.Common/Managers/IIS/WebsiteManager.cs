using System;
using System.Linq;
using Microsoft.Web.Administration;
using NuFridge.Common.IIS;
using NuFridge.Common.Manager;

namespace NuFridge.Common.Managers.IIS
{
    public class WebsiteManager
    {
        internal ServerManager ServerManager { get; set; }
        public static string WebsiteName { get; protected set; }

        public const int DefaultWebsitePortNumber = 8420;

        public WebsiteManager(string websiteName, ServerManager serverManager)
        {
            ServerManager = serverManager;
            WebsiteName = websiteName;
        }

        public WebsiteManager(string websiteName)
            : this(websiteName, new ServerManager())
        {

        }

        public bool WebsiteExists()
        {
            return ServerManager.Sites.Any(w => w.Name == WebsiteName);
        }

        public WebsiteInfo CreateWebsite(CreateWebsiteArgs createWebsiteArgs)
        {
            if (WebsiteExists())
            {
                throw new InvalidOperationException(string.Format("A website with the name: {0} already exists.", WebsiteName));
            }
            var bindingInfoString = createWebsiteArgs.GetBindingInformationString();
            var site = ServerManager.Sites.Add(WebsiteName, createWebsiteArgs.Protocol.ToString().ToLower(),
                                               bindingInfoString, createWebsiteArgs.PhysicalPath);
            ServerManager.CommitChanges();

            return LoadWebsiteInfo(site);
        }

        public WebsiteInfo GetWebsite()
        {
            var website = ServerManager.Sites.FirstOrDefault(w => w.Name == WebsiteName);
            if (website == null)
            {
                return null;
            }
            return LoadWebsiteInfo(website);
        }

        public WebsiteInfo LoadWebsiteInfo(Site website)
        {
            var info = new WebsiteInfo {Name = website.Name};
            foreach (var b in website.Bindings)
            {
                var binfo = new BindingInfo
                    {
                        BindingInformation = b.BindingInformation,
                        Host = b.Host,
                        IsIpPortHostBinding = b.IsIPPortHostBinding,
                        Protocol = b.Protocol,
                        EndPoint = b.EndPoint
                    };
                info.Bindings.Add(binfo);
            }
            foreach (var a in website.Applications)
            {
                var ainfo = new ApplicationInfo {Path = a.Path};
                foreach (var v in a.VirtualDirectories)
                {
                    var vinfo = new VirtualDirectoryInfo {Path = v.Path, PhysicalPath = v.PhysicalPath};
                    ainfo.VirtualDirectories.Add(vinfo);
                }
                info.Applications.Add(ainfo);
            }
            return info;
        }
    }
}