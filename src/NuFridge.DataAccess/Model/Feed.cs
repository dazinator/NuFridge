using MongoDB.Bson.Serialization.Attributes;
using System;
using NuFridge.DataAccess.Entity;

namespace NuFridge.DataAccess.Model
{
    public class Feed : IEntityBase
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }

        public string APIKey { get; set; }

        [BsonIgnore]
        public string FeedURL { get; set; }
    }
}