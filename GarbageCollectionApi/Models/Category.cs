using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GarbageCollectionApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Category
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}