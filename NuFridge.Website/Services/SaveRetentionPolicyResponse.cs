using System.Runtime.Serialization;

namespace NuFridge.Website.Services
{
    public class SaveRetentionPolicyResponse
    {
        [DataMember]
        public bool Success { get; set; }

        public SaveRetentionPolicyResponse(bool success)
        {
            Success = success;
        }

        public SaveRetentionPolicyResponse()
        {

        }
    }
}