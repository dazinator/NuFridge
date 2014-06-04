using System.Runtime.Serialization;

namespace NuFridge.Website.Services.Messages
{
    [DataContract]
    public class CreateFeedRequest
    {
        [DataMember]
        public string FeedName { get; set; }
    }
}