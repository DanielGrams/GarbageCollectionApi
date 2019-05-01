using System.Collections.Generic;
using GarbageCollectionApi.DataContracts;
using System.Threading.Tasks;

public interface ICategoriesService
{
    Task<List<Category>> GetByTownAndStreetAsync(string townId, string streetId);
}