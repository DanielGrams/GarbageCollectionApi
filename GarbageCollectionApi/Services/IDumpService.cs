namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;

    public interface IDumpService
    {
        string ZipFilePath { get; }

        void Dump(List<Town> towns, List<CollectionEvent> events, DataRefreshStatus refreshStatus);
    }
}