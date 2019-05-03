namespace GarbageCollectionApi.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Testing;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TownsTests : ApiTests
    {
        private IMongoCollection<Models.Town> towns;

        public override void Setup()
        {
            base.Setup();
            this.towns = this.Database.GetCollection<Models.Town>(MongoConnectionSettings.TownsCollectionName);
        }

        [Test]
        public async Task TownsReturnsList()
        {
            this.towns.DeleteMany(_ => true);
            this.towns.InsertOne(new Town { Id = "1", Name = "Goslar" });
            this.towns.InsertOne(new Town { Id = "2", Name = "Oker" });

            var responseMessage = await this.Client.GetAsync("/api/towns").ConfigureAwait(false);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var stringResponse = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var list = JsonConvert.DeserializeObject<IEnumerable<DataContracts.Town>>(stringResponse);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list.First().Name, Is.EqualTo("Goslar"));
            Assert.That(list.Skip(1).First().Name, Is.EqualTo("Oker"));
        }
    }
}