using System.Runtime.Serialization;
using NuFridge.DataAccess.Entity;

namespace NuFridge.Website.Services
{
    public class GetFeedResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public FeedEntity Feed { get; set; }

        public GetFeedResponse(bool success, FeedEntity feed)
        {
            Success = success;
            Feed = feed;
        }

        public GetFeedResponse()
        {
            
        }
    }
}