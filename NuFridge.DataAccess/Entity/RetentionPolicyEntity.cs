using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NuFridge.DataAccess.Entity;

namespace NuFridge.DataAccess.Entity
{
    public class RetentionPolicyEntity : IEntityBase
    {
        public Guid Id { get; set; }
        public string FeedName { get; set; }
        public int DaysToKeepPackages { get; set; }
        public int VersionsToKeep { get; set; }
        public bool Enabled { get; set; }
        //public string FilePath { get; set; }
        //public string FeedDirectory { get; set; }

        public List<ExcludedPackageEntity> ExcludedPackages { get; set; }

        public RetentionPolicyEntity() : this(null)
        {

        }

        public RetentionPolicyEntity(string feedName)
        {
            FeedName = feedName;
            Enabled = false;
            ExcludedPackages = new List<ExcludedPackageEntity>();
        }


    }
}