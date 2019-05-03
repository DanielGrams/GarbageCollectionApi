namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;

    public class UpdateService : IUpdateService
    {
        private readonly IMongoCollection<Town> towns;
        private readonly IMongoCollection<CollectionEvent> events;

        public UpdateService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("Database"));
            var database = client.GetDatabase("GarbageCollectionDb");

            this.towns = database.GetCollection<Town>("Towns");
            this.events = database.GetCollection<CollectionEvent>("Events");
        }

        /// <inheritdoc />
        public async Task UpdateAsync(List<Town> towns, List<CollectionEvent> events)
        {
            // TODOdgr BulkWrite?
            await this.towns.DeleteManyAsync(_ => true).ConfigureAwait(false);
            await this.towns.InsertManyAsync(towns).ConfigureAwait(false);

            await this.events.DeleteManyAsync(_ => true).ConfigureAwait(false);
            await this.events.InsertManyAsync(events).ConfigureAwait(false);
        }
    }
}