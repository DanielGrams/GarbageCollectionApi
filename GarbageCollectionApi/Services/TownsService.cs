using System.Collections.Generic;
using GarbageCollectionApi.DataContracts;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class TownsService : ITownsService
{
    private readonly IMongoCollection<GarbageCollectionApi.Models.Town> _towns;

    public TownsService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("Database"));
        var database = client.GetDatabase("GarbageCollectionDb");
        _towns = database.GetCollection<GarbageCollectionApi.Models.Town>("Towns");
    }

    public async Task<List<Town>> GetAllItemsAsync()
    {
        return await _towns
            .Find(town => true)
            .Project(town => new Town { Id = town.Id, Name = town.Name })
            .ToListAsync();
    }
}