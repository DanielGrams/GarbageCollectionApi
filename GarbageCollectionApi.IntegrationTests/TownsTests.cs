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
    using GarbageCollectionApi.Utils;
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

            var firstTown = list.First();
            Assert.That(firstTown.Id, Is.EqualTo("1"));
            Assert.That(firstTown.Name, Is.EqualTo("Goslar"));

            var secondTown = list.Skip(1).First();
            Assert.That(secondTown.Id, Is.EqualTo("2"));
            Assert.That(secondTown.Name, Is.EqualTo("Oker"));
        }

        [Test]
        public async Task StreetsReturnsList()
        {
            var goslar = new Town { Id = "62.1", Name = "Goslar" };
            goslar.Streets.Add(new Street { Id = "2523.907.1", Name = "Schreiberstraße" });
            goslar.Streets.Add(new Street { Id = "2523.921.1", Name = "Zwingerwall" });

            this.towns.DeleteMany(_ => true);
            this.towns.InsertOne(goslar);

            var responseMessage = await this.Client.GetAsync("/api/towns/62.1/streets").ConfigureAwait(false);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var stringResponse = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var list = JsonConvert.DeserializeObject<IEnumerable<DataContracts.Street>>(stringResponse);
            Assert.That(list.Count, Is.EqualTo(2));

            var firstStreet = list.First();
            Assert.That(firstStreet.Id, Is.EqualTo("2523.907.1"));
            Assert.That(firstStreet.Name, Is.EqualTo("Schreiberstraße"));

            var secondStreet = list.Skip(1).First();
            Assert.That(secondStreet.Id, Is.EqualTo("2523.921.1"));
            Assert.That(secondStreet.Name, Is.EqualTo("Zwingerwall"));
        }

        [Test]
        public async Task CategoriesReturnsList()
        {
            var schreiberstrasse = new Street { Id = "2523.907.1", Name = "Schreiberstraße" };
            schreiberstrasse.Categories.Add(new Category { Id = "1.5", Name = "Baum- und Strauchschnitt" });
            schreiberstrasse.Categories.Add(new Category { Id = "1.4", Name = "Biotonne" });

            var goslar = new Town { Id = "62.1", Name = "Goslar" };
            goslar.Streets.Add(schreiberstrasse);

            this.towns.DeleteMany(_ => true);
            this.towns.InsertOne(goslar);

            var responseMessage = await this.Client.GetAsync("/api/towns/62.1/streets/2523.907.1/categories").ConfigureAwait(false);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var stringResponse = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var list = JsonConvert.DeserializeObject<IEnumerable<DataContracts.Category>>(stringResponse);
            Assert.That(list.Count, Is.EqualTo(2));

            var firstCategory = list.First();
            Assert.That(firstCategory.Id, Is.EqualTo("1.5"));
            Assert.That(firstCategory.Name, Is.EqualTo("Baum- und Strauchschnitt"));

            var secondCategory = list.Skip(1).First();
            Assert.That(secondCategory.Id, Is.EqualTo("1.4"));
            Assert.That(secondCategory.Name, Is.EqualTo("Biotonne"));
        }

        [Test]
        public async Task EventsReturnsList()
        {
            var schreiberstrasse = new Street { Id = "2523.907.1", Name = "Schreiberstraße" };
            schreiberstrasse.Categories.Add(new Category { Id = "1.5", Name = "Baum- und Strauchschnitt" });
            schreiberstrasse.Categories.Add(new Category { Id = "1.4", Name = "Biotonne" });

            var goslar = new Town { Id = "62.1", Name = "Goslar" };
            goslar.Streets.Add(schreiberstrasse);

            this.towns.DeleteMany(_ => true);
            this.towns.InsertOne(goslar);

            var events = this.Database.GetCollection<Models.CollectionEvent>(MongoConnectionSettings.EventsCollectionName);
            events.DeleteMany(_ => true);
            events.InsertOne(new CollectionEvent
                {
                    Id = "fdc0b08929027ca3edef21a3107e766a",
                    TownId = "62.1",
                    StreetId = "2523.907.1",
                    Start = DateTimeUtils.Utc(2019, 2, 21),
                    Stamp = DateTimeUtils.Utc(2018, 11, 28),
                    Category = new Category { Id = "1.5", Name = "Baum- und Strauchschnitt" },
                });
            events.InsertOne(new CollectionEvent
                {
                    Id = "454f9e3ff522d7fb127819ba24dccdf9",
                    TownId = "62.1",
                    StreetId = "2523.907.1",
                    Start = DateTimeUtils.Utc(2019, 1, 29),
                    Stamp = DateTimeUtils.Utc(2018, 11, 28),
                    Category = new Category { Id = "1.4", Name = "Weihnachtsbäume" },
                });

            var responseMessage = await this.Client.GetAsync("/api/towns/62.1/streets/2523.907.1/events").ConfigureAwait(false);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var stringResponse = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var list = JsonConvert.DeserializeObject<IEnumerable<DataContracts.CollectionEvent>>(stringResponse);
            Assert.That(list.Count, Is.EqualTo(2));

            var firstEvent = list.First();
            Assert.That(firstEvent.Id, Is.EqualTo("fdc0b08929027ca3edef21a3107e766a"));
            Assert.That(firstEvent.Date, Is.EqualTo(DateTimeUtils.Utc(2019, 2, 21)));
            Assert.That(firstEvent.Stamp, Is.EqualTo(DateTimeUtils.Utc(2018, 11, 28)));
            Assert.That(firstEvent.Category.Name, Is.EqualTo("Baum- und Strauchschnitt"));

            var secondEvent = list.Skip(1).First();
            Assert.That(secondEvent.Id, Is.EqualTo("454f9e3ff522d7fb127819ba24dccdf9"));
            Assert.That(secondEvent.Date, Is.EqualTo(DateTimeUtils.Utc(2019, 1, 29)));
            Assert.That(secondEvent.Stamp, Is.EqualTo(DateTimeUtils.Utc(2018, 11, 28)));
            Assert.That(secondEvent.Category.Name, Is.EqualTo("Weihnachtsbäume"));
        }
    }
}