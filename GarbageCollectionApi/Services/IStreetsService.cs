namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;

    public interface IStreetsService
    {
        Task<List<Street>> GetByTownAsync(string townId);
    }
}