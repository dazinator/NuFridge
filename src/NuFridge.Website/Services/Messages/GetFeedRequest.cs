using System.Runtime.Serialization;

namespace NuFridge.Website.Services.Messages
{
    [DataContract]
    public class GetFeedRequest
    {
        [DataMember]
        public string FeedName { get; set; }
    }
}