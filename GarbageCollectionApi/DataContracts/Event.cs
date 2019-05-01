using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GarbageCollectionApi.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    public class Event
    {
        public string Id { get; set; }

        public Category Category { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public DateTime Stamp { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }
    }
}