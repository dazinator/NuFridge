using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuFridge.Common.Jobs.CCNet.Octo
{
    public class OctoProject : ConfigurationElement
    {
        public string Name
        { get; set; }

        public string BuildServerDirectoryPath
        { get; set; }

        public bool EnforcePackageVersionChange
        { get; set; }

        public string TFSURL
        { get; set; }

        public string TFSPath
        { get; set; }

        public string NuGetPackageBuildNumberRegex
        { get; set; }

        public string NuGetPackageId
        { get; set; }
    }
}