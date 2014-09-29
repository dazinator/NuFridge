using MongoDB.Bson.Serialization.Attributes;
using NuFridge.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuFridge.DataAccess.Model
{
    public class FeedGroup : IEntityBase
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }
    }
}
