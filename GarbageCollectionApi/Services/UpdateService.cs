namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;

    public class UpdateService : MongoService, IUpdateService
    {
        private readonly IMongoCollection<Town> towns;
        private readonly IMongoCollection<CollectionEvent> events;

        public UpdateService(IOptions<MongoConnectionSettings> settings)
            : base(settings)
        {
            this.towns = this.Database.GetCollection<Town>(MongoConnectionSettings.TownsCollectionName);
            this.events = this.Database.GetCollection<CollectionEvent>(MongoConnectionSettings.EventsCollectionName);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(List<Town> towns, List<CollectionEvent> events)
        {
            using (var session = await this.Client.StartSessionAsync().ConfigureAwait(false))
            {
                session.StartTransaction();

                await this.towns.DeleteManyAsync(session, _ => true).ConfigureAwait(false);
                await this.towns.InsertManyAsync(session, towns).ConfigureAwait(false);

                await this.events.DeleteManyAsync(session, _ => true).ConfigureAwait(false);
                await this.events.InsertManyAsync(session, events).ConfigureAwait(false);

                await session.CommitTransactionAsync().ConfigureAwait(false);
            }
        }
    }
}