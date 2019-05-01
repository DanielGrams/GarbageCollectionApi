using System.Linq;
using System;
using System.Collections.Generic;
using GarbageCollectionApi.DataContracts;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class EventsService : IEventsService
{
    private readonly IMongoCollection<GarbageCollectionApi.Models.Event> _events;

    public EventsService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("Database"));
        var database = client.GetDatabase("GarbageCollectionDb");
        _events = database.GetCollection<GarbageCollectionApi.Models.Event>("Events");
    }

    public async Task<List<Event>> GetByTownAndStreetAsync(string townId, string streetId)
    {
        return await _events
            .Find(e => e.TownId == townId)
            .Project(e => new Event
            { 
                Id = e.Id,
                Category = new Category { Id = e.Category.Id, Name = e.Category.Name },
                Start = e.Start,
                End = e.End,
                Stamp = e.Stamp,
                Summary = e.Summary,
                Description = e.Description
            })
            .ToListAsync();
    }
}