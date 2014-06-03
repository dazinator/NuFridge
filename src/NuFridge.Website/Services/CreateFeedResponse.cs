using System.Runtime.Serialization;

namespace NuFridge.Website.Services
{
    [DataContract]
    public class CreateFeedResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }
        public CreateFeedResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public CreateFeedResponse()
        {

        }
    }
}