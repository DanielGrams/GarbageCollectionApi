using System.Collections.Generic;
using GarbageCollectionApi.DataContracts;
using System.Threading.Tasks;

public interface IStreetsService
{
    Task<List<Street>> GetByTownAsync(string townId);
}