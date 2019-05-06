namespace GarbageCollectionApi.UnitTests.Services.Scraping
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;
    using GarbageCollectionApi.Models;
    using GarbageCollectionApi.Services;
    using GarbageCollectionApi.Services.Scraping;
    using GarbageCollectionApi.UnitTests.Utils;
    using GarbageCollectionApi.Utils;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DataRefreshServiceTests
    {
        [Test]
        public void Constructor_NullParameters_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataRefreshService(null, null, null, null));
        }

        [Test]
        public void Constructor_AllParameters_CreatesInstance()
        {
            var serviceProvider = NSubstitute.Substitute.For<IServiceProvider>();
            var logger = NSubstitute.Substitute.For<ILogger<DataRefreshService>>();
            var documentLoader = NSubstitute.Substitute.For<IDocumentLoader>();
            var options = NSubstitute.Substitute.For<IOptions<DataRefreshSettings>>();
            options.Value.Returns(new DataRefreshSettings());

            Assert.NotNull(new DataRefreshService(serviceProvider, logger, documentLoader, options));
        }

        [Test]
        public async Task RefreshAsync()
        {
            var serviceCollection = new ServiceCollection();

            var updateService = NSubstitute.Substitute.For<IUpdateService>();
            serviceCollection.AddSingleton<IUpdateService>(updateService);

            var dumpService = NSubstitute.Substitute.For<IDumpService>();
            serviceCollection.AddSingleton<IDumpService>(dumpService);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = NSubstitute.Substitute.For<ILogger<DataRefreshService>>();
            var documentLoader = new TestDocumentLoader();
            var options = NSubstitute.Substitute.For<IOptions<DataRefreshSettings>>();
            options.Value.Returns(new DataRefreshSettings());

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader, options);
            var cancellationToken = new CancellationTokenSource().Token;

            var dataRefreshStatus = new DataRefreshStatus();

            await dataRefreshService.RefreshAsync(dataRefreshStatus, cancellationToken).ConfigureAwait(false);

            await updateService.Received().UpdateAsync(
                Arg.Is<List<Town>>(towns => this.CheckTowns(towns)),
                Arg.Is<List<CollectionEvent>>(events => this.CheckEvents(events)),
                Arg.Is<DataRefreshStatus>(status => this.CheckStatus(status))).ConfigureAwait(false);
        }

        [Test]
        public async Task LoadTownsAsync()
        {
            var serviceProvider = NSubstitute.Substitute.For<IServiceProvider>();
            var logger = NSubstitute.Substitute.For<ILogger<DataRefreshService>>();
            var documentLoader = NSubstitute.Substitute.For<IDocumentLoader>();
            var options = NSubstitute.Substitute.For<IOptions<DataRefreshSettings>>();
            options.Value.Returns(new DataRefreshSettings());

            documentLoader.LoadTownsDocumentAsync(Arg.Any<IBrowsingContext>(), Arg.Any<CancellationToken>()).Returns(async ci =>
            {
                return await this.LoadTestDataAsync(ci.Arg<IBrowsingContext>(), "towns.html").ConfigureAwait(false);
            });

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader, options);
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
            var options = NSubstitute.Substitute.For<IOptions<DataRefreshSettings>>();
            options.Value.Returns(new DataRefreshSettings());

            documentLoader.LoadStreetsDocumentAsync("62.1", Arg.Any<IBrowsingContext>(), Arg.Any<CancellationToken>()).Returns(async ci =>
            {
                return await this.LoadTestDataAsync(ci.Arg<IBrowsingContext>(), "streets-goslar.html").ConfigureAwait(false);
            });

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader, options);
            var cancellationToken = new CancellationTokenSource().Token;
            var goslar = new Town { Id = "62.1", Name = "Goslar" };
            var towns = new List<Town>() { goslar };
            await dataRefreshService.LoadStreetsAsync(towns, cancellationToken).ConfigureAwait(false);

            var streets = goslar.Streets;
            Assert.That(streets.Count, Is.EqualTo(154));
            Assert.That(streets[0].Id, Is.EqualTo("2523.361.1"));
            Assert.That(streets[0].Name, Is.EqualTo(" Ortsteil - Hahndorf"));
            Assert.That(streets[123].Id, Is.EqualTo("2523.455.1"));
            Assert.That(streets[123].Name, Is.EqualTo("Schuhhof"));
            Assert.That(streets[153].Id, Is.EqualTo("2523.921.1"));
            Assert.That(streets[153].Name, Is.EqualTo("Zwingerwall"));
        }

        [Test]
        public async Task LoadCategoriesAsync()
        {
            var serviceProvider = NSubstitute.Substitute.For<IServiceProvider>();
            var logger = NSubstitute.Substitute.For<ILogger<DataRefreshService>>();
            var documentLoader = NSubstitute.Substitute.For<IDocumentLoader>();
            var options = NSubstitute.Substitute.For<IOptions<DataRefreshSettings>>();
            options.Value.Returns(new DataRefreshSettings());

            documentLoader.LoadCategoriesDocumentAsync("62.1", "2523.907.1", Arg.Any<IBrowsingContext>(), Arg.Any<CancellationToken>()).Returns(async ci =>
            {
                return await this.LoadTestDataAsync(ci.Arg<IBrowsingContext>(), "categories-schreiberstrasse.html").ConfigureAwait(false);
            });

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader, options);
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
            var options = NSubstitute.Substitute.For<IOptions<DataRefreshSettings>>();
            options.Value.Returns(new DataRefreshSettings());

            documentLoader.LoadEventsIcalTextAsync("62.1", "2523.907.1", "2019", Arg.Any<CancellationToken>()).Returns(ci =>
            {
                return TestDataLoader.LoadTestData("events-schreiberstrasse.ics");
            });

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader, options);
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

            var firstEvent = events[0];
            Assert.That(firstEvent.Id, Is.EqualTo("fdc0b08929027ca3edef21a3107e766a"));
            Assert.That(firstEvent.TownId, Is.EqualTo("62.1"));
            Assert.That(firstEvent.StreetId, Is.EqualTo("2523.907.1"));
            Assert.That(firstEvent.Start, Is.EqualTo(DateTimeUtils.Utc(2019, 2, 20, 23)));
            Assert.That(firstEvent.Stamp, Is.EqualTo(DateTimeUtils.Utc(2018, 11, 28)));
            Assert.That(firstEvent.Category.Name, Is.EqualTo("Baum- und Strauchschnitt"));

            var lastEvent = events[84];
            Assert.That(lastEvent.Id, Is.EqualTo("454f9e3ff522d7fb127819ba24dccdf9"));
            Assert.That(lastEvent.TownId, Is.EqualTo("62.1"));
            Assert.That(lastEvent.StreetId, Is.EqualTo("2523.907.1"));
            Assert.That(lastEvent.Start, Is.EqualTo(DateTimeUtils.Utc(2019, 1, 28, 23)));
            Assert.That(lastEvent.Stamp, Is.EqualTo(DateTimeUtils.Utc(2018, 11, 28)));
            Assert.That(lastEvent.Category.Name, Is.EqualTo("Weihnachtsbäume"));
        }

        private async Task<IDocument> LoadTestDataAsync(IBrowsingContext context, string filename)
        {
            var content = TestDataLoader.LoadTestData(filename);
            return await context.OpenAsync(req => req.Content(content)).ConfigureAwait(false);
        }

        private bool CheckTowns(List<Town> towns)
        {
            Assert.That(towns.Count, Is.EqualTo(10));

            var goslar = towns.First(t => t.Name == "Goslar" && t.Id == "62.1");

            var streets = goslar.Streets;
            Assert.That(streets.Count, Is.EqualTo(154));
            Assert.That(streets[0].Id, Is.EqualTo("2523.361.1"));
            Assert.That(streets[0].Name, Is.EqualTo(" Ortsteil - Hahndorf"));
            Assert.That(streets[122].Id, Is.EqualTo("2523.907.1"));
            Assert.That(streets[122].Name, Is.EqualTo("Schreiberstraße"));
            Assert.That(streets[153].Id, Is.EqualTo("2523.921.1"));
            Assert.That(streets[153].Name, Is.EqualTo("Zwingerwall"));

            foreach (var town in towns)
            {
                foreach (var street in streets)
                {
                    var categories = street.Categories;
                    Assert.That(categories.Count, Is.EqualTo(7));
                    Assert.That(categories[0].Id, Is.EqualTo("1.5"));
                    Assert.That(categories[0].Name, Is.EqualTo("Baum- und Strauchschnitt"));
                    Assert.That(categories[6].Id, Is.EqualTo("2523.1"));
                    Assert.That(categories[6].Name, Is.EqualTo("Wertstofftonne danach"));
                }
            }

            return true;
        }

        private bool CheckEvents(List<CollectionEvent> events)
        {
            var knownKeys = new HashSet<string>();
            var containsDuplicateIds = events.Any(item => !knownKeys.Add(item.Id));
            Assert.That(containsDuplicateIds, Is.False);

            var eventsInStreet = events.Where(e => e.TownId == "62.1" && e.StreetId == "2523.907.1").ToList();
            Assert.That(eventsInStreet.Count, Is.EqualTo(85));

            var firstEvent = eventsInStreet[0];
            Assert.That(firstEvent.Id, Is.EqualTo("fdc0b08929027ca3edef21a3107e766a"));
            Assert.That(firstEvent.TownId, Is.EqualTo("62.1"));
            Assert.That(firstEvent.StreetId, Is.EqualTo("2523.907.1"));
            Assert.That(firstEvent.Start, Is.EqualTo(DateTimeUtils.Utc(2019, 2, 20, 23)));
            Assert.That(firstEvent.Stamp, Is.EqualTo(DateTimeUtils.Utc(2018, 11, 28)));
            Assert.That(firstEvent.Category.Name, Is.EqualTo("Baum- und Strauchschnitt"));

            var lastEvent = eventsInStreet[84];
            Assert.That(lastEvent.Id, Is.EqualTo("454f9e3ff522d7fb127819ba24dccdf9"));
            Assert.That(lastEvent.TownId, Is.EqualTo("62.1"));
            Assert.That(lastEvent.StreetId, Is.EqualTo("2523.907.1"));
            Assert.That(lastEvent.Start, Is.EqualTo(DateTimeUtils.Utc(2019, 1, 28, 23)));
            Assert.That(lastEvent.Stamp, Is.EqualTo(DateTimeUtils.Utc(2018, 11, 28)));
            Assert.That(lastEvent.Category.Name, Is.EqualTo("Weihnachtsbäume"));

            return true;
        }

        private bool CheckStatus(DataRefreshStatus status)
        {
            return status.LatestStamp.Equals(DateTimeUtils.Utc(2019, 1, 8));
        }
    }
}