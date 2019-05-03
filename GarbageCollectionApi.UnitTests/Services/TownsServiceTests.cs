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
    public class TownsServiceTests : ServiceTests
    {
        private IMongoCollection<Models.Town> towns;

        public override void Setup()
        {
            base.Setup();
            this.towns = this.Database.GetCollection<Models.Town>(MongoConnectionSettings.TownsCollectionName);
        }

        [Test]
        public async Task GetAllItemsAsync()
        {
            this.towns.DeleteMany(_ => true);
            this.towns.InsertOne(new Town { Id = "1", Name = "Goslar" });
            this.towns.InsertOne(new Town { Id = "2", Name = "Oker" });

            var service = new TownsService(this.Options);
            var result = await service.GetAllItemsAsync().ConfigureAwait(false);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Goslar"));
            Assert.That(result.Skip(1).First().Name, Is.EqualTo("Oker"));
        }
    }
}