using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuFridge.DataAccess.Entity;

namespace NuFridge.DataAccess.CCNetEntity
{
    public class CCNetProject : IEntityBase 
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Name { get; set; }

        public string OctopusProjectName { get; set; }
        public string OctopusNuGetPacakgeId { get; set; }

        public string LastSuccessfulReleaseLabel { get; set; }
        public string LastSuccessfulBuildLabel { get; set; }

        public string ChangeSetNumberRegex { get; set; }
        public string TFSUrl { get; set; }
        public string TFSPath { get; set; }
    }
}
