namespace GarbageCollectionApi.DataContracts
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Status of data refreshment
    /// </summary>
    public class DataRefreshStatus
    {
        /// <summary>
        /// The property indicates the maximum date/time that the event was created
        /// </summary>
        public DateTime LatestStamp { get; set; }

        /// <summary>
        /// The property indicates the date/time that the data was fetched
        /// </summary>
        public DateTime LatestRefresh { get; set; }
    }
}