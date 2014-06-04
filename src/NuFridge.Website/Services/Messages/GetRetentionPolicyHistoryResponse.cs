using System.Collections.Generic;
using System.Runtime.Serialization;
using NuFridge.DataAccess.Entity;

namespace NuFridge.Website.Services.Messages
{
    [DataContract]
    public class GetRetentionPolicyHistoryResponse
    {
        [DataMember]
        public List<RetentionPolicyHistoryEntity> Entries { get; set; }

        public GetRetentionPolicyHistoryResponse(List<RetentionPolicyHistoryEntity> historyEntities)
        {
            Entries = historyEntities;
        }

        public GetRetentionPolicyHistoryResponse()
        {

        }
    }
}