using System.Collections.Generic;
using NuFridge.Common.Manager;

namespace NuFridge.Common.IIS
{
    public class ApplicationInfo
    {
        public ApplicationInfo()
        {
            this.VirtualDirectories = new List<VirtualDirectoryInfo>();
        }

        internal string PreviousPath { get; set; }
        private string _Path { get; set; }

        public string Path
        {
            get { return _Path; }
            set
            {
                //Check if its null so you can update the path multiple times without losing the original path - there is no id on the iis application
                if (PreviousPath == null)
                {
                    PreviousPath = _Path;
                }
                _Path = value;
            }
        }

        public List<VirtualDirectoryInfo> VirtualDirectories { get; set; }
    }
}