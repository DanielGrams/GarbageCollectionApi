namespace GarbageCollectionApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using GarbageCollectionApi.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;

    public class StreetsService : MongoService, IStreetsService
    {
        private readonly IMongoCollection<Models.Town> towns;

        public StreetsService(IOptions<MongoConnectionSettings> settings)
            : base(settings)
        {
            this.towns = this.Database.GetCollection<Models.Town>(MongoConnectionSettings.TownsCollectionName);
        }

        /// <summary>
        /// Gets streets in town
        /// </summary>
        /// <param name="townId">Id of town</param>
        /// <returns>List of matching streets</returns>
        public async Task<List<DataContracts.Street>> GetByTownAsync(string townId)
        {
            return await this.towns
                .Find(town => town.Id == townId)
                .Project(town => town.Streets.Select(street => new DataContracts.Street { Id = street.Id, Name = street.Name }).ToList())
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }
    }
}