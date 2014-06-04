using System;
using System.Runtime.Serialization;

namespace NuFridge.Website.Services.Messages
{
    [Serializable]
    [DataContract]
    public class DeleteFeedResponse
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Message { get; set; }

        public DeleteFeedResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public DeleteFeedResponse()
        {

        }
    }
}