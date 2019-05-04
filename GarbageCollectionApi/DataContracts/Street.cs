namespace GarbageCollectionApi.DataContracts
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Street in town
    /// </summary>
    /// <remark>
    /// This might also be a small area containing several streets.
    /// </remark>
    public class Street
    {
        /// <summary>
        /// Unique identifier for street in town
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Street name
        /// </summary>
        public string Name { get; set; }
    }
}