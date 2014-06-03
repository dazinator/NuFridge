using System.Runtime.Serialization;

namespace NuFridge.Website.Services
{
    [DataContract]
    public class GetRetentionPolicyRequest
    {
        [DataMember]
        public string FeedName { get; set; }
    }
}