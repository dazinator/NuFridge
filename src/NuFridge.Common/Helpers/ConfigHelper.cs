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
        public static string FeedWebsitePortNumberKey = "IIS.FeedWebsite.PortNumber";
        public static string FeedWebsitePathKey = "IIS.FeedWebsite.RootDirectory";
        public static string FeedWebsiteNameKey = "IIS.FeedWebsite.Name";
        public static string MongoDBDatabaseNameKey = "MongoDB.DatabaseName";
        public static string MongoDBConnectionStringKey = "MongoDB.ConnectionString";
        public static string MongoDBServerNameKey = "MongoDB.ServerName";

        public static Configuration OpenConfigFile(string configPath, string websiteName)
        {
            var configFile = new FileInfo(configPath);
            var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/", websiteName);
        }

        public static int GetFeedWebsitePortNumber()
        {
            var nuFridgePort = ConfigurationManager.AppSettings[FeedWebsitePortNumberKey];

            int nuFridgePortNumber;
            if (!int.TryParse(nuFridgePort, out nuFridgePortNumber))
            {
                nuFridgePortNumber = WebsiteManager.DefaultWebsitePortNumber;
            }
            return nuFridgePortNumber;
        }

        public static string GetFeedWebsitePhysicalPath()
        {
            return ConfigurationManager.AppSettings[FeedWebsitePathKey];
        }

        public static bool GetFeedWebsiteName(out string message, out string nuFridgeWebsiteName)
        {
            message = null;
            nuFridgeWebsiteName = ConfigurationManager.AppSettings[FeedWebsiteNameKey];


            if (string.IsNullOrWhiteSpace(nuFridgeWebsiteName))
            {
                message = "The IIS.FeedWebsite.Name app setting has not been configured.";
                return false;
            }
            return true;
        }

        public static string GetMongoDBDatabaseName()
        {
            return ConfigurationManager.AppSettings[MongoDBDatabaseNameKey];
        }

        public static string GetMongoDBServerName()
        {
            return ConfigurationManager.AppSettings[MongoDBServerNameKey];
        }
    }
}
