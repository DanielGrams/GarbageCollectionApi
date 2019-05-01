using System.Collections.Generic;
using GarbageCollectionApi.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class UpdateService : IUpdateService
{
    private readonly IMongoCollection<Town> _towns;

    public UpdateService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("Database"));
        var database = client.GetDatabase("GarbageCollectionDb");

        _towns = database.GetCollection<Town>("Towns");
    }

    public async Task UpdateAsync(List<Town> towns)
    {
        await _towns.DeleteManyAsync(_ => true);
        await _towns.InsertManyAsync(towns);
    }
}