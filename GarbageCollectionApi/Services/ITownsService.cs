namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;

    public interface ITownsService
    {
        Task<List<Town>> GetAllItemsAsync();
    }
}