namespace GarbageCollectionApi.DataContracts
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Town with streets
    /// </summary>
    public class TownWithStreets : Town
    {
        public TownWithStreets()
            : base()
        {
            this.Streets = new List<StreetWithEvents>();
        }

        /// <summary>
        /// Streets in town
        /// </summary>
        public List<StreetWithEvents> Streets { get; }
    }
}