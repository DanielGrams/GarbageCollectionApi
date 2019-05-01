using System.Collections.Generic;
using GarbageCollectionApi.DataContracts;
using System.Threading.Tasks;

public interface IEventsService
{
    Task<List<Event>> GetByTownAndStreetAsync(string townId, string streetId);
}