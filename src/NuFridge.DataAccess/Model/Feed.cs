using MongoDB.Bson.Serialization.Attributes;
using System;
using NuFridge.DataAccess.Entity;

namespace NuFridge.DataAccess.Model
{
    public class Feed : IEntityBase 
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string PublishPackagesUrl { get; set; }
        public string APIKey { get; set; }
    }
}