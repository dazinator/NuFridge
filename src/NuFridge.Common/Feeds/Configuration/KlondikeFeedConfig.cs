using NuFridge.Common.Helpers;
using NuFridge.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace NuFridge.Common.Feeds.Configuration
{
    public class KlondikeFeedConfig : IFeedConfig
    {
        private string ConfigPath { get; set; }
        private const string APIKeySettingKey = "NuGet.Lucene.Web:localAdministratorApiKey";

        public KlondikeFeedConfig(string configPath)
        {
            ConfigPath = configPath;
        }

        public string GetAPIKey()
        {
            var config = ConfigHelper.OpenConfigFile(ConfigPath);
            if (!config.AppSettings.Settings.AllKeys.Contains(APIKeySettingKey))
            {
                return null;
            }

            return config.AppSettings.Settings[APIKeySettingKey].Value;
        }

        public void UpdateAPIKey(string APIKey)
        {
            var config = ConfigHelper.OpenConfigFile(ConfigPath);
            if (!config.AppSettings.Settings.AllKeys.Contains(APIKeySettingKey))
            {
                config.AppSettings.Settings.Add(APIKeySettingKey, APIKey);
                config.Save();
            }
            else if (config.AppSettings.Settings[APIKeySettingKey].Value != APIKey)
            {
                config.AppSettings.Settings[APIKeySettingKey].Value = APIKey;
                config.Save();
            }
        }
    }
}