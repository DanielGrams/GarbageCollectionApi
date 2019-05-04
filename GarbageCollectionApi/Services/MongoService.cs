namespace GarbageCollectionApi.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public abstract class MongoService
    {
        protected MongoService(IOptions<MongoConnectionSettings> settings)
        {
            this.Client = new MongoClient(settings.Value.ConnectionString);
            this.Database = this.Client.GetDatabase(settings.Value.Database);
        }

        protected MongoClient Client { get;  }

        protected IMongoDatabase Database { get; }
    }
}