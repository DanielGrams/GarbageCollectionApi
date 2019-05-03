namespace GarbageCollectionApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;

    public class TownsService : ITownsService
    {
        private readonly IMongoCollection<GarbageCollectionApi.Models.Town> towns;

        public TownsService(IMongoCollection<GarbageCollectionApi.Models.Town> towns)
        {
            this.towns = towns ?? throw new ArgumentNullException(nameof(towns));
        }

        public async Task<List<Town>> GetAllItemsAsync()
        {
            return await this.towns
                .Find(town => true)
                .Project(town => new Town { Id = town.Id, Name = town.Name })
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}