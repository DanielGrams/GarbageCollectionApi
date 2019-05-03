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

    public class CategoriesService : MongoService, ICategoriesService
    {
        private readonly IMongoCollection<Models.Town> towns;

        public CategoriesService(IOptions<MongoConnectionSettings> settings)
            : base(settings)
        {
            this.towns = this.Database.GetCollection<Models.Town>(MongoConnectionSettings.TownsCollectionName);
        }

        /// <summary>
        /// Gets categories for street in town.
        /// </summary>
        /// <param name="townId">Id of town</param>
        /// <param name="streetId">Id of street</param>
        /// <returns>List of matching categories</returns>
        public async Task<List<DataContracts.Category>> GetByTownAndStreetAsync(string townId, string streetId)
        {
            return await this.towns
                .Find(town => town.Id == townId)
                .Project(town => town.Streets.First(street => street.Id == streetId).Categories.Select(category => new DataContracts.Category { Id = category.Id, Name = category.Name }).ToList())
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }
    }
}