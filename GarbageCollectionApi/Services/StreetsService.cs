namespace GarbageCollectionApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;

    public class StreetsService : IStreetsService
    {
        private readonly IMongoCollection<GarbageCollectionApi.Models.Town> towns;

        public StreetsService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("Database"));
            var database = client.GetDatabase("GarbageCollectionDb");
            this.towns = database.GetCollection<GarbageCollectionApi.Models.Town>("Towns");
        }

        /// <summary>
        /// Gets streets in town
        /// </summary>
        /// <param name="townId">Id of town</param>
        /// <returns>List of matching streets</returns>
        public async Task<List<Street>> GetByTownAsync(string townId)
        {
            return await this.towns
                .Find(town => town.Id == townId)
                .Project(town => town.Streets.Select(street => new Street { Id = street.Id, Name = street.Name }).ToList())
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }
    }
}