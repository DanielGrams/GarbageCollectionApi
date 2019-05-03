namespace GarbageCollectionApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;

    public class CategoriesService : ICategoriesService
    {
        private readonly IMongoCollection<GarbageCollectionApi.Models.Town> towns;

        public CategoriesService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("Database"));
            var database = client.GetDatabase("GarbageCollectionDb");
            this.towns = database.GetCollection<GarbageCollectionApi.Models.Town>("Towns");
        }

        /// <summary>
        /// Gets categories for street in town.
        /// </summary>
        /// <param name="townId">Id of town</param>
        /// <param name="streetId">Id of street</param>
        /// <returns>List of matching categories</returns>
        public async Task<List<Category>> GetByTownAndStreetAsync(string townId, string streetId)
        {
            return await this.towns
                .Find(town => town.Id == townId)
                .Project(town => town.Streets.First(street => street.Id == streetId).Categories.Select(category => new Category { Id = category.Id, Name = category.Name }).ToList())
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }
    }
}