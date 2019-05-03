namespace GarbageCollectionApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using GarbageCollectionApi.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;

    public class TownsService : MongoService, ITownsService
    {
        private readonly IMongoCollection<Models.Town> towns;

        public TownsService(IOptions<MongoConnectionSettings> settings)
            : base(settings)
        {
            this.towns = this.Database.GetCollection<Models.Town>(MongoConnectionSettings.TownsCollectionName);
        }

        public async Task<List<DataContracts.Town>> GetAllItemsAsync()
        {
            return await this.towns
                .Find(town => true)
                .Project(town => new DataContracts.Town { Id = town.Id, Name = town.Name })
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}