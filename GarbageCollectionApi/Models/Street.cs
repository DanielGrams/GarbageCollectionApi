using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GarbageCollectionApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Street
    {
        public int Id { get; set; }

        [Required]
        public int TownId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}