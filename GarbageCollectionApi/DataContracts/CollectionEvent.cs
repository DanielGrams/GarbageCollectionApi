namespace GarbageCollectionApi.DataContracts
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Event of a collection
    /// </summary>
    public class CollectionEvent
    {
        public string Id { get; set; }

        public Category Category { get; set; }

        public DateTime Date { get; set; }

        public DateTime Stamp { get; set; }
    }
}