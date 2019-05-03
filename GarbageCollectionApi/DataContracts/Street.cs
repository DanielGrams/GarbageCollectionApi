namespace GarbageCollectionApi.DataContracts
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Street
    /// </summary>
    public class Street
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}