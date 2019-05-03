namespace GarbageCollectionApi.DataContracts
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Garbage category
    /// </summary>
    public class Category
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}