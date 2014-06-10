using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NuFridge.DataAccess.Entity;

namespace NuFridge.Website.MVC.Models
{
    public class Feed : IEntityBase 
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}