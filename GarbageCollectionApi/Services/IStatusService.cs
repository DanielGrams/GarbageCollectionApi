namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;

    public interface IStatusService
    {
        Task<DataRefreshStatus> GetStatusAsync();

        Task<DataContracts.DataRefreshStatus> GetDataContractsStatusAsync();
    }
}