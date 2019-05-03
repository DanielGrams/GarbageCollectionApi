namespace GarbageCollectionApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;

    public class EventsService : IEventsService
    {
        private readonly IMongoCollection<GarbageCollectionApi.Models.CollectionEvent> events;

        public EventsService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("Database"));
            var database = client.GetDatabase("GarbageCollectionDb");
            this.events = database.GetCollection<GarbageCollectionApi.Models.CollectionEvent>("Events");
        }

        public async Task<List<CollectionEvent>> GetByTownAndStreetAsync(string townId, string streetId)
        {
            return await this.events
                .Find(e => e.TownId == townId)
                .Project(e => new CollectionEvent
                {
                    Id = e.Id,
                    Category = new Category { Id = e.Category.Id, Name = e.Category.Name },
                    Date = e.Start,
                    Stamp = e.Stamp,
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}