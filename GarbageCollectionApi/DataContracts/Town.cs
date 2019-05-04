namespace GarbageCollectionApi.DataContracts
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Town
    /// </summary>
    public class Town
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Town name
        /// </summary>
        public string Name { get; set; }
    }
}