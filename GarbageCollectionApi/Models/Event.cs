using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GarbageCollectionApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Event
    {
        [BsonId]
        public string Id { get; set; }

        public string TownId { get; set; }

        public string StreetId { get; set; }

        public Category Category { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public DateTime Stamp { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }
    }
}