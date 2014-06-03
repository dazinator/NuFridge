using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NuFridge.Website.Services
{
    [Serializable]
    [DataContract]
    public class GetFeedsResponse
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public List<string> Feeds { get; set; }
        [DataMember]
        public string Message { get; set; }

        public GetFeedsResponse(bool success, List<string> feeds)
        {
            Success = success;
            Feeds = feeds;
        }

        public GetFeedsResponse(bool success, List<string> feeds, string message)
        {
            Success = success;
            Feeds = feeds;
            Message = message;
        }

        public GetFeedsResponse()
        {

        }
    }
}