using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GarbageCollectionApi.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    public class Category
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}