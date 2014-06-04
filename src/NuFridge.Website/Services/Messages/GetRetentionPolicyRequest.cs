using System.Runtime.Serialization;

namespace NuFridge.Website.Services.Messages
{
    [DataContract]
    public class GetRetentionPolicyRequest
    {
        [DataMember]
        public string FeedName { get; set; }
    }
}