using System.Collections.Generic;
using NuFridge.Common.IIS;

namespace NuFridge.Common.Manager
{
    public class WebsiteInfo
    {
        public WebsiteInfo()
        {
            Bindings = new List<BindingInfo>();
            Applications = new List<ApplicationInfo>();
        }

        public string Name { get; set; }

        public List<BindingInfo> Bindings { get; set; }

        public List<ApplicationInfo> Applications { get; set; }
    }
}