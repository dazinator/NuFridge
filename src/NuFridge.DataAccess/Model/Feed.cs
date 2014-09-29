using MongoDB.Bson.Serialization.Attributes;
using System;
using NuFridge.DataAccess.Entity;

namespace NuFridge.DataAccess.Model
{
    //When adding new properties please remember to add them to the javascript view model so values are not lost on database updates
    public class Feed : IEntityBase
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }

        public string APIKey { get; set; }

        [BsonIgnore]
        public string FeedURL { get; set; }

        [BsonIgnore]
        public string GroupName { get; set; }

        [BsonRequired]
        public Guid GroupId { get; set; }
    }
}