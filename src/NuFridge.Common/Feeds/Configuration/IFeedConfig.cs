using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuFridge.Common.Feeds.Configuration
{
    interface IFeedConfig
    {
        string GetAPIKey();
        void UpdateAPIKey(string APIKey);
    }
}
