namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;

    public interface IDumpService
    {
        Task DumpAsync(List<Town> towns, List<CollectionEvent> events, DataRefreshStatus refreshStatus);
    }
}