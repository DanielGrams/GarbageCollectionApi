using System.Linq;
using System;
using System.Collections.Generic;
using GarbageCollectionApi.DataContracts;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class CategoriesService : ICategoriesService
{
    private readonly IMongoCollection<GarbageCollectionApi.Models.Town> _towns;

    public CategoriesService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("Database"));
        var database = client.GetDatabase("GarbageCollectionDb");
        _towns = database.GetCollection<GarbageCollectionApi.Models.Town>("Towns");
    }

    public async Task<List<Category>> GetByTownAndStreetAsync(string townId, string streetId)
    {
        return await _towns
            .Find(town => town.Id == townId)
            .Project(town => town.Streets.First(street => street.Id == streetId).Categories.Select(category => new Category { Id = category.Id, Name = category.Name }).ToList())
            .SingleOrDefaultAsync();
    }
}