namespace GarbageCollectionApi.UnitTests.Services.Scraping
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;
    using GarbageCollectionApi.Models;
    using GarbageCollectionApi.Services.Scraping;
    using GarbageCollectionApi.UnitTests.Utils;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DataRefreshServiceTests
    {
        [Test]
        public void Constructor_NullParameters_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataRefreshService(null, null, null));
        }

        [Test]
        public void Constructor_AllParameters_CreatesInstance()
        {
            var serviceProvider = NSubstitute.Substitute.For<IServiceProvider>();
            var logger = NSubstitute.Substitute.For<ILogger<DataRefreshService>>();
            var documentLoader = NSubstitute.Substitute.For<IDocumentLoader>();

            Assert.NotNull(new DataRefreshService(serviceProvider, logger, documentLoader));
        }

        [Test]
        public async Task LoadTownsAsync()
        {
            var serviceProvider = NSubstitute.Substitute.For<IServiceProvider>();
            var logger = NSubstitute.Substitute.For<ILogger<DataRefreshService>>();
            var documentLoader = NSubstitute.Substitute.For<IDocumentLoader>();

            documentLoader.LoadTownsDocumentAsync(Arg.Any<IBrowsingContext>(), Arg.Any<CancellationToken>()).Returns(async ci =>
            {
                return await this.LoadTestDataAsync(ci.Arg<IBrowsingContext>(), "towns.html").ConfigureAwait(false);
            });

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader);
            var cancellationToken = new CancellationTokenSource().Token;
            var towns = await dataRefreshService.LoadTownsAsync(cancellationToken).ConfigureAwait(false);

            Assert.That(towns.Count, Is.EqualTo(10));
            Assert.That(towns[0].Id, Is.EqualTo("62.53"));
            Assert.That(towns[0].Name, Is.EqualTo("Bad Harzburg"));
            Assert.That(towns[3].Id, Is.EqualTo("62.1"));
            Assert.That(towns[3].Name, Is.EqualTo("Goslar"));
            Assert.That(towns[9].Id, Is.EqualTo("62.5"));
            Assert.That(towns[9].Name, Is.EqualTo("Vienenburg"));
        }

        [Test]
        public async Task LoadStreetsAsync()
        {
            var serviceProvider = NSubstitute.Substitute.For<IServiceProvider>();
            var logger = NSubstitute.Substitute.For<ILogger<DataRefreshService>>();
            var documentLoader = NSubstitute.Substitute.For<IDocumentLoader>();

            documentLoader.LoadStreetsDocumentAsync("62.1", Arg.Any<IBrowsingContext>(), Arg.Any<CancellationToken>()).Returns(async ci =>
            {
                return await this.LoadTestDataAsync(ci.Arg<IBrowsingContext>(), "streets-goslar.html").ConfigureAwait(false);
            });

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader);
            var cancellationToken = new CancellationTokenSource().Token;
            var goslar = new Town { Id = "62.1", Name = "Goslar" };
            var towns = new List<Town>() { goslar };
            await dataRefreshService.LoadStreetsAsync(towns, cancellationToken).ConfigureAwait(false);

            var streets = goslar.Streets;
            Assert.That(streets.Count, Is.EqualTo(154));
            Assert.That(streets[0].Id, Is.EqualTo("2523.361.1"));
            Assert.That(streets[0].Name, Is.EqualTo(" Ortsteil - Hahndorf"));
            Assert.That(streets[122].Id, Is.EqualTo("2523.907.1"));
            Assert.That(streets[122].Name, Is.EqualTo("Schreiberstraße"));
            Assert.That(streets[153].Id, Is.EqualTo("523.921.1"));
            Assert.That(streets[153].Name, Is.EqualTo("Zwingerwall"));
        }

        [Test]
        public async Task LoadCategoriesAsync()
        {
            var serviceProvider = NSubstitute.Substitute.For<IServiceProvider>();
            var logger = NSubstitute.Substitute.For<ILogger<DataRefreshService>>();
            var documentLoader = NSubstitute.Substitute.For<IDocumentLoader>();

            documentLoader.LoadCategoriesDocumentAsync("62.1", "2523.907.1", Arg.Any<IBrowsingContext>(), Arg.Any<CancellationToken>()).Returns(async ci =>
            {
                return await this.LoadTestDataAsync(ci.Arg<IBrowsingContext>(), "categories-schreiberstrasse.html").ConfigureAwait(false);
            });

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader);
            var cancellationToken = new CancellationTokenSource().Token;
            var schreiberstrasse = new Street { Id = "2523.907.1", Name = "Schreiberstraße" };

            var goslar = new Town { Id = "62.1", Name = "Goslar" };
            goslar.Streets.Add(schreiberstrasse);

            var towns = new List<Town>() { goslar };
            await dataRefreshService.LoadCategoriesAsync(towns, cancellationToken).ConfigureAwait(false);

            var categories = schreiberstrasse.Categories;
            Assert.That(categories.Count, Is.EqualTo(7));
            Assert.That(categories[0].Id, Is.EqualTo("1.5"));
            Assert.That(categories[0].Name, Is.EqualTo("Baum- und Strauchschnitt"));
            Assert.That(categories[6].Id, Is.EqualTo("2523.1"));
            Assert.That(categories[6].Name, Is.EqualTo("Wertstofftonne danach"));
        }

        [Test]
        public async Task LoadEventsAsync()
        {
            var serviceProvider = NSubstitute.Substitute.For<IServiceProvider>();
            var logger = NSubstitute.Substitute.For<ILogger<DataRefreshService>>();
            var documentLoader = NSubstitute.Substitute.For<IDocumentLoader>();

            documentLoader.LoadEventsIcalTextAsync("62.1", "2523.907.1", "2019", Arg.Any<CancellationToken>()).Returns(ci =>
            {
                return TestDataLoader.LoadTestData("events-schreiberstrasse.ics");
            });

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader);
            var cancellationToken = new CancellationTokenSource().Token;

            var schreiberstrasse = new Street { Id = "2523.907.1", Name = "Schreiberstraße" };
            schreiberstrasse.Categories.Add(new Category { Id = "1.5", Name = "Baum- und Strauchschnitt" });
            schreiberstrasse.Categories.Add(new Category { Id = "1.4", Name = "Biotonne" });
            schreiberstrasse.Categories.Add(new Category { Id = "1.3", Name = "Blaue Tonne" });
            schreiberstrasse.Categories.Add(new Category { Id = "1.2", Name = "Gelber Sack" });
            schreiberstrasse.Categories.Add(new Category { Id = "1.1", Name = "Restmülltonne" });
            schreiberstrasse.Categories.Add(new Category { Id = "1.6", Name = "Weihnachtsbäume" });
            schreiberstrasse.Categories.Add(new Category { Id = "2523.1", Name = "Wertstofftonne danach" });

            var goslar = new Town { Id = "62.1", Name = "Goslar" };
            goslar.Streets.Add(schreiberstrasse);

            var towns = new List<Town>() { goslar };
            var events = await dataRefreshService.LoadEventsAsync(towns, cancellationToken).ConfigureAwait(false);

            Assert.That(events.Count, Is.EqualTo(85));
            Assert.That(events[0].Id, Is.EqualTo("fdc0b08929027ca3edef21a3107e766a"));
            Assert.That(events[0].TownId, Is.EqualTo("62.1"));
            Assert.That(events[0].StreetId, Is.EqualTo("2523.907.1"));
            Assert.That(events[0].Start, Is.EqualTo(BerlinToUtc(new DateTime(2019, 2, 21))));
            Assert.That(events[0].Stamp, Is.EqualTo(new DateTime(2018, 11, 28)));
            Assert.That(events[0].Category.Name, Is.EqualTo("Baum- und Strauchschnitt"));
            Assert.That(events[84].Id, Is.EqualTo("454f9e3ff522d7fb127819ba24dccdf9"));
            Assert.That(events[84].TownId, Is.EqualTo("62.1"));
            Assert.That(events[84].StreetId, Is.EqualTo("2523.907.1"));
            Assert.That(events[84].Start, Is.EqualTo(BerlinToUtc(new DateTime(2019, 1, 29))));
            Assert.That(events[84].Stamp, Is.EqualTo(new DateTime(2018, 11, 28)));
            Assert.That(events[84].Category.Name, Is.EqualTo("Weihnachtsbäume"));
        }

        private static DateTime BerlinToUtc(DateTime berlinDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(berlinDateTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin"));
        }

        private async Task<IDocument> LoadTestDataAsync(IBrowsingContext context, string filename)
        {
            var content = TestDataLoader.LoadTestData(filename);
            return await context.OpenAsync(req => req.Content(content)).ConfigureAwait(false);
        }
    }
}