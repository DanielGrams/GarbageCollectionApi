namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;

    public interface IEventsService
    {
        Task<List<CollectionEvent>> GetByTownAndStreetAsync(string townId, string streetId, List<string> categoryIds = null);
    }
}