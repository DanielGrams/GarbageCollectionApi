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
            this.AvailableYears = new List<string>();
        }

        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

#pragma warning disable CA2227
        public List<Category> Categories { get; set; }
#pragma warning restore CA2227

        [BsonIgnore]
        public List<string> AvailableYears { get; }
    }
}