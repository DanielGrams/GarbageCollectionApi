using System.Collections.Generic;
using GarbageCollectionApi.DataContracts;
using System.Threading.Tasks;

public interface ITownsService
{
    Task<List<Town>> GetAllItemsAsync();
}