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
            var refreshStatus = await this.statusCollection.Find(_ => true).FirstOrDefaultAsync().ConfigureAwait(false);

            if (refreshStatus == null)
            {
                refreshStatus = new DataRefreshStatus
                {
                    LatestStamp = DateTime.MinValue,
                    LatestRefresh = DateTime.MinValue,
                };
                await this.statusCollection.InsertOneAsync(refreshStatus).ConfigureAwait(false);
            }

            return refreshStatus;
        }
    }
}