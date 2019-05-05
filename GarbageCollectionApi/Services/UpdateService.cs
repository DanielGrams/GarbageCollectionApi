namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class UpdateService : MongoService, IUpdateService
    {
        private readonly IMongoCollection<Town> towns;
        private readonly IMongoCollection<CollectionEvent> events;
        private readonly IMongoCollection<DataRefreshStatus> statusCollection;

        public UpdateService(IOptions<MongoConnectionSettings> settings)
            : base(settings)
        {
            this.towns = this.Database.GetCollection<Town>(MongoConnectionSettings.TownsCollectionName);
            this.events = this.Database.GetCollection<CollectionEvent>(MongoConnectionSettings.EventsCollectionName);
            this.statusCollection = this.Database.GetCollection<DataRefreshStatus>(MongoConnectionSettings.DataRefreshStatusCollectionName);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(List<Town> towns, List<CollectionEvent> events, DataRefreshStatus refreshStatus)
        {
            await this.towns.DeleteManyAsync(_ => true).ConfigureAwait(false);
            await this.towns.InsertManyAsync(towns).ConfigureAwait(false);

            await this.events.DeleteManyAsync(_ => true).ConfigureAwait(false);
            await this.CreateEventIndex().ConfigureAwait(false);
            await this.events.InsertManyAsync(events).ConfigureAwait(false);

            await this.statusCollection.ReplaceOneAsync(status => status.Id == refreshStatus.Id, refreshStatus).ConfigureAwait(false);
        }

        private async Task CreateEventIndex()
        {
            var indexOptions = new CreateIndexOptions { Name = "TownStreet", Sparse = true, Unique = false, Background = true };
            var indexKeys = Builders<CollectionEvent>.IndexKeys.Ascending(e => e.TownId).Ascending(e => e.StreetId);
            var indexModel = new CreateIndexModel<CollectionEvent>(indexKeys, indexOptions);
            await this.events.Indexes.CreateOneAsync(indexModel).ConfigureAwait(false);
        }
    }
}