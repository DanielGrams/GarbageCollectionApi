using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GarbageCollectionApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Town
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}