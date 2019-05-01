using System.Linq;
using System;
using System.Collections.Generic;
using GarbageCollectionApi.DataContracts;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class StreetsService : IStreetsService
{
    private readonly IMongoCollection<GarbageCollectionApi.Models.Town> _towns;

    public StreetsService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("Database"));
        var database = client.GetDatabase("GarbageCollectionDb");
        _towns = database.GetCollection<GarbageCollectionApi.Models.Town>("Towns");
    }

    public async Task<List<Street>> GetByTownAsync(string townId)
    {
        return await _towns
            .Find(town => town.Id == townId)
            .Project(town => town.Streets.Select(street => new Street { Id = street.Id, Name = street.Name }).ToList())
            .SingleOrDefaultAsync();
    }
}