using System.Runtime.Serialization;
using NuFridge.DataAccess.Entity;

namespace NuFridge.Website.Services
{
    public class GetRetentionPolicyResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public RetentionPolicyEntity RetentionPolicy { get; set; }

        public GetRetentionPolicyResponse(bool success, RetentionPolicyEntity policy)
        {
            Success = success;
            RetentionPolicy = policy;
        }

        public GetRetentionPolicyResponse()
        {

        }
    }
}