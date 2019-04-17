using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GarbageCollectionApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}