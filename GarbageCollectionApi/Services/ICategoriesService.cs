namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;

    public interface ICategoriesService
    {
        Task<List<Category>> GetByTownAndStreetAsync(string townId, string streetId);
    }
}