namespace GarbageCollectionApi.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Event of a collection
    /// </summary>
    public class CollectionEvent
    {
        [BsonId]
        public string Id { get; set; }

        public string TownId { get; set; }

        public string StreetId { get; set; }

        public Category Category { get; set; }

        public DateTime Start { get; set; }

        public DateTime Stamp { get; set; }
    }
}