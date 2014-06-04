using System;
using System.Runtime.Serialization;

namespace NuFridge.Website.Services.Messages
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
