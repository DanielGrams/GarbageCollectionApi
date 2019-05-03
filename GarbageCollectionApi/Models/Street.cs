namespace GarbageCollectionApi.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Street
    /// </summary>
    public class Street
    {
        public Street()
        {
            this.Categories = new List<Category>();
        }

        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public List<Category> Categories { get; }
    }
}