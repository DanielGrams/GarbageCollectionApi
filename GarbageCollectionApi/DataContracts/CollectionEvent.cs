namespace GarbageCollectionApi.DataContracts
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Event of a collection. Everyone is happy when the garbage truck comes up. Tuut tuut!
    /// </summary>
    public class CollectionEvent
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Describes category of garbage that is collected
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// Date of collection event
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The property indicates the date/time that the event was created
        /// </summary>
        public DateTime Stamp { get; set; }
    }
}