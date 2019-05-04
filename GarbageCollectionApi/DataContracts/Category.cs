namespace GarbageCollectionApi.DataContracts
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Garbage category, e.g. "Biotonne" or "Gelber Sack"
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Unique identifier for category in street
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Category name, e.g. "Biotonne" or "Gelber Sack"
        /// </summary>
        public string Name { get; set; }
    }
}