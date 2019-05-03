namespace GarbageCollectionApi.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Garbage category
    /// </summary>
    public class Category
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}