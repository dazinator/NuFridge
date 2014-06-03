using System.Runtime.Serialization;

namespace NuFridge.Website.Services
{
    [DataContract]
    public class CreateFeedRequest
    {
        [DataMember]
        public string FeedName { get; set; }
    }
}