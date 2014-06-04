using System.Collections.Generic;

namespace NuFridge.Common.Manager
{
    public class ApplicationInfo
    {
        public ApplicationInfo()
        {
            this.VirtualDirectories = new List<VirtualDirectoryInfo>();
        }

        public string Path { get; set; }
        public List<VirtualDirectoryInfo> VirtualDirectories { get; set; }
    }
}