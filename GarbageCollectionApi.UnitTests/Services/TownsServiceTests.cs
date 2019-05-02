using NUnit.Framework;
using GarbageCollectionApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Mongo2Go;

namespace GarbageCollectionApi.UnitTests.Services
{
    [TestFixture]
    public class TownsServiceTests
    {
        internal static MongoDbRunner _runner;
        internal static IMongoDatabase _db;

        [SetUp]
        public void Setup()
        {
            _runner = MongoDbRunner.Start();

            var client = new MongoClient(_runner.ConnectionString);
            _db = client.GetDatabase("UnitTests");
        }

        [TearDown]
        public void TearDown()
        {
            _runner.Dispose();
        }

        [Test]
        public async Task GetAllItemsAsync()
        {
            var towns = _db.GetCollection<Town>("Towns");
            towns.DeleteMany(_ => true);
            towns.InsertOne(new Town { Id = "1", Name = "Goslar" });
            towns.InsertOne(new Town { Id = "2", Name = "Oker" });

            var service = new TownsService(towns);
            var result = await service.GetAllItemsAsync();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Goslar"));
            Assert.That(result.Skip(1).First().Name, Is.EqualTo("Oker"));
        }
    }
}