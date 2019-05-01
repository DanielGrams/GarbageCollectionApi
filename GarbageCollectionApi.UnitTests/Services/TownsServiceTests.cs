using NUnit.Framework;
using GarbageCollectionApi.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCollectionApi.UnitTests.Services
{
    [TestFixture]
    public class TownsServiceTests
    {
        private SqliteConnection _connection;
        private DbContextOptions<GarbageCollectionContext> _options;

        [SetUp]
        public void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<GarbageCollectionContext>()
                    .UseSqlite(_connection)
                    .Options;
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        [Test]
        public async Task GetAllItems()
        {
            // Create the schema in the database
            using (var context = new GarbageCollectionContext(_options))
            {
                context.Database.EnsureCreated();
            }

            // Insert seed data into the database using one instance of the context
            using (var context = new GarbageCollectionContext(_options))
            {
                context.Towns.Add(new Town { Name = "Goslar" });
                context.Towns.Add(new Town { Name = "Oker" });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new GarbageCollectionContext(_options))
            {
                var service = new TownsService(context);
                var result = await service.GetAllItemsAsync();

                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result.First().Name, Is.EqualTo("Goslar"));
                Assert.That(result.Skip(1).First().Name, Is.EqualTo("Oker"));
            }
        }
    }
}