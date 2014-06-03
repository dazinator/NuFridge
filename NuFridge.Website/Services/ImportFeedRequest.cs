using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NuFridge.Website.Services
{
    [Serializable]
    [DataContract]
    public class ImportFeedRequest
    {
        [DataMember]
        public string FeedName { get; set; }

        [DataMember]
        public string SourceFeedUrl { get; set; }

        [DataMember]
        public string ApiKey { get; set; }

    }


    [Serializable]
    [DataContract]
    public class ImportFeedResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string JobName { get; set; }
    }
}
