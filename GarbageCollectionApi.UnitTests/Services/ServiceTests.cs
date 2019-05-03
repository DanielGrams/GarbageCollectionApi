namespace GarbageCollectionApi.UnitTests.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using GarbageCollectionApi.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Mongo2Go;
    using MongoDB.Driver;
    using NSubstitute;
    using NUnit.Framework;

    public class ServiceTests
    {
        protected MongoDbRunner Runner { get; private set; }

        protected MongoClient MongoClient { get; private set; }

        protected IMongoDatabase Database { get; private set; }

        protected MongoConnectionSettings Settings { get; private set; }

        protected IOptions<MongoConnectionSettings> Options { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            this.Runner = MongoDbRunner.Start();

            this.Settings = new MongoConnectionSettings
            {
                ConnectionString = this.Runner.ConnectionString,
                Database = "UnitTests",
            };

            this.MongoClient = new MongoClient(this.Settings.ConnectionString);
            this.Database = this.MongoClient.GetDatabase(this.Settings.Database);

            var optionsSubstitute = NSubstitute.Substitute.For<IOptions<MongoConnectionSettings>>();
            optionsSubstitute.Value.Returns(this.Settings);
            this.Options = optionsSubstitute;
        }

        [TearDown]
        public void TearDown()
        {
            this.MongoClient = null;
            this.Runner.Dispose();
        }
    }
}