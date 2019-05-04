namespace GarbageCollectionApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class StatusService : MongoService, IStatusService
    {
        private readonly IMongoCollection<DataRefreshStatus> statusCollection;

        public StatusService(IOptions<MongoConnectionSettings> settings)
            : base(settings)
        {
            this.statusCollection = this.Database.GetCollection<DataRefreshStatus>(MongoConnectionSettings.DataRefreshStatusCollectionName);
        }

        public async Task<DataRefreshStatus> GetStatusAsync()
        {
            await this.EnsureCollectionExistsAsync(MongoConnectionSettings.DataRefreshStatusCollectionName).ConfigureAwait(false);

            DataRefreshStatus refreshStatus;
            using (var session = await this.Client.StartSessionAsync().ConfigureAwait(false))
            {
                session.StartTransaction();

                refreshStatus = await this.statusCollection.Find(session, _ => true).FirstOrDefaultAsync().ConfigureAwait(false);

                if (refreshStatus == null)
                {
                    refreshStatus = new DataRefreshStatus
                    {
                        LatestStamp = DateTime.MinValue,
                        LatestRefresh = DateTime.MinValue,
                    };
                    await this.statusCollection.InsertOneAsync(session, refreshStatus).ConfigureAwait(false);
                }

                await session.CommitTransactionAsync().ConfigureAwait(false);
            }

            return refreshStatus;
        }
    }
}