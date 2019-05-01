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

        public DateTime Date { get; set; }

        public DateTime Stamp { get; set; }
    }
}