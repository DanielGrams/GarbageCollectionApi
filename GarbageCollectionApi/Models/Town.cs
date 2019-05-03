namespace GarbageCollectionApi.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Town
    /// </summary>
    public class Town
    {
        public Town()
        {
            this.Streets = new List<Street>();
        }

        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

#pragma warning disable CA2227
        public List<Street> Streets { get; set; }
#pragma warning restore CA2227
    }
}