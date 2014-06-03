using System;
using System.Runtime.Serialization;

namespace NuFridge.Website.Services
{
    [Serializable]
    [DataContract]
    public class DeleteFeedRequest
    {
        [DataMember]
        public string FeedName { get; set; }
    }
}