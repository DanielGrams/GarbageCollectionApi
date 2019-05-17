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

    public class EventsService : MongoService, IEventsService
    {
        private readonly IMongoCollection<Models.CollectionEvent> events;

        public EventsService(IOptions<MongoConnectionSettings> settings)
            : base(settings)
        {
            this.events = this.Database.GetCollection<Models.CollectionEvent>(MongoConnectionSettings.EventsCollectionName);
        }

        public async Task<List<DataContracts.CollectionEvent>> GetByTownAndStreetAsync(string townId, string streetId, List<string> categoryIds = null)
        {
            var baseFilter = Builders<Models.CollectionEvent>.Filter.Where(e => e.TownId == townId && e.StreetId == streetId);

            FilterDefinition<Models.CollectionEvent> filter;

            if (categoryIds == null || !categoryIds.Any())
            {
                filter = baseFilter;
            }
            else
            {
                filter = Builders<Models.CollectionEvent>.Filter.And(
                    baseFilter,
                    Builders<Models.CollectionEvent>.Filter.In(e => e.Category.Id, categoryIds));
            }

            return await this.events.Find(filter)
                .Project(e => new DataContracts.CollectionEvent
                {
                    Id = e.Id,
                    Category = new DataContracts.Category { Id = e.Category.Id, Name = e.Category.Name },
                    Date = e.Start,
                    Stamp = e.Stamp,
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}