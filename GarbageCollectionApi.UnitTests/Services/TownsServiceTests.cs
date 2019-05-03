namespace GarbageCollectionApi.UnitTests.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using GarbageCollectionApi.Services;
    using Microsoft.EntityFrameworkCore;
    using Mongo2Go;
    using MongoDB.Driver;
    using NUnit.Framework;

    [TestFixture]
    public class TownsServiceTests
    {
        private static MongoDbRunner runner;
        private static IMongoDatabase database;

        [SetUp]
        public void Setup()
        {
             runner = MongoDbRunner.Start();

             var client = new MongoClient(runner.ConnectionString);
             database = client.GetDatabase("UnitTests");
        }

        [TearDown]
        public void TearDown()
        {
             runner.Dispose();
        }

        [Test]
        public async Task GetAllItemsAsync()
        {
            var towns = database.GetCollection<Town>("Towns");
            towns.DeleteMany(_ => true);
            towns.InsertOne(new Town { Id = "1", Name = "Goslar" });
            towns.InsertOne(new Town { Id = "2", Name = "Oker" });

            var service = new TownsService(towns);
            var result = await service.GetAllItemsAsync().ConfigureAwait(false);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Goslar"));
            Assert.That(result.Skip(1).First().Name, Is.EqualTo("Oker"));
        }
    }
}