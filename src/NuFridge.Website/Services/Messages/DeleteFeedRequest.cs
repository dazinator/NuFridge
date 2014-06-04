using System;
using System.Runtime.Serialization;

namespace NuFridge.Website.Services.Messages
{
    [Serializable]
    [DataContract]
    public class DeleteFeedRequest
    {
        [DataMember]
        public string FeedName { get; set; }
    }
}