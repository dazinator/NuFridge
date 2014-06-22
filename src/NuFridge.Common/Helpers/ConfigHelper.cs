using NuFridge.Common.Managers.IIS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace NuFridge.Common.Helpers
{
    public static class ConfigHelper
    {
        public static Configuration OpenConfigFile(string configPath)
        {
            var configFile = new FileInfo(configPath);
            var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
        }

        public static int GetFeedWebsitePortNumber()
        {
            var nuFridgePort = ConfigurationManager.AppSettings["IIS.FeedWebsite.PortNumber"];

            int nuFridgePortNumber;
            if (!int.TryParse(nuFridgePort, out nuFridgePortNumber))
            {
                nuFridgePortNumber = WebsiteManager.DefaultWebsitePortNumber;
            }
            return nuFridgePortNumber;
        }

        public static string GetFeedWebsitePhysicalPath()
        {
           return ConfigurationManager.AppSettings["IIS.FeedWebsite.RootDirectory"];
        }

        public static bool GetFeedWebsiteName(out string message, out string nuFridgeWebsiteName)
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

        public static string GetMongoDBDatabaseName()
        {
            return ConfigurationManager.AppSettings["MongoDB.DatabaseName"];
        }

        public static string GetMongoDBServerName()
        {
            return ConfigurationManager.AppSettings["MongoDB.ServerName"];
        }
    }
}
