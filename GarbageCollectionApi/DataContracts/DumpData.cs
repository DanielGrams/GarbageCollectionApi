namespace GarbageCollectionApi.DataContracts
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Dump data
    /// </summary>
    public class DumpData
    {
        public DumpData()
        {
            this.Towns = new List<TownWithStreets>();
        }

        /// <summary>
        /// Status of data refreshment
        /// </summary>
        public DataRefreshStatus RefreshStatus { get; set; }

        /// <summary>
        /// Towns
        /// </summary>
        public List<TownWithStreets> Towns { get; }
    }
}