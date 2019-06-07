namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;

    /// <summary>
    /// Interface to update database
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// Updates database with given items
        /// </summary>
        /// <param name="towns">List of all towns</param>
        /// <param name="events">List of all events</param>
        /// <param name="refreshStatus">Refresh status</param>
        /// <returns>Task</returns>
        Task UpdateAsync(List<Town> towns, List<CollectionEvent> events, DataRefreshStatus refreshStatus);

        /// <summary>
        /// Updates database with given status
        /// </summary>
        /// <param name="refreshStatus">Refresh status</param>
        /// <returns>Task</returns>
        Task UpdateStatusAsync(DataRefreshStatus refreshStatus);
    }
}