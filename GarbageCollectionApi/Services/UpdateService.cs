using System.Collections.Generic;
using GarbageCollectionApi.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class UpdateService : IUpdateService
{
    private readonly IMongoCollection<Town> _towns;
    private readonly IMongoCollection<Event> _events;

    public UpdateService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("Database"));
        var database = client.GetDatabase("GarbageCollectionDb");

        _towns = database.GetCollection<Town>("Towns");
        _events = database.GetCollection<Event>("Events");
    }

    public async Task UpdateAsync(List<Town> towns, List<Event> events)
    {
        // TODO BulkWrite?
        await _towns.DeleteManyAsync(_ => true);
        await _towns.InsertManyAsync(towns);

        await _events.DeleteManyAsync(_ => true);
        await _events.InsertManyAsync(events);
    }
}