namespace GarbageCollectionApi.DataContracts
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Street with events and categories
    /// </summary>
    public class StreetWithEvents : Street
    {
        public StreetWithEvents()
            : base()
        {
            this.Categories = new List<Category>();
            this.Events = new List<CollectionEvent>();
        }

        /// <summary>
        /// Categories for street
        /// </summary>
        public List<Category> Categories { get; }

        /// <summary>
        /// Events for street
        /// </summary>
        public List<CollectionEvent> Events { get; }
    }
}