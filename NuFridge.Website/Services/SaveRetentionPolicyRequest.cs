using System.Collections.Generic;
using System.Runtime.Serialization;
using NuFridge.DataAccess.Entity;

namespace NuFridge.Website.Services
{
    [DataContract]
    public class SaveRetentionPolicyRequest
    {
        [DataMember]
        public string FeedName { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public int DaysToKeepPackages { get; set; }

        [DataMember]
        public int VersionsToKeep { get; set; }

        [DataMember]
        public List<ExcludedPackageEntity> ExcludedPackages { get; set; }
    }
}