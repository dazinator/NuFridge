using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuFridge.Website.MVC.Models
{
    public class Feed
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public Feed()
        {
            //TODO Remove this line once connected to a release database.
            Id = Guid.NewGuid().ToString();
        }
    }
}