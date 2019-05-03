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
        public string Id { get; set; }

        public string Name { get; set; }
    }
}