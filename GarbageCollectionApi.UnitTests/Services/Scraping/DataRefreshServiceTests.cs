namespace GarbageCollectionApi.UnitTests.Services.Scraping
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;
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

            documentLoader.LoadTownsDocument(Arg.Any<IBrowsingContext>(), Arg.Any<CancellationToken>()).Returns(ci =>
            {
                return this.LoadTestDataAsync(ci.Arg<IBrowsingContext>(), "towns.html");
            });

            var dataRefreshService = new DataRefreshService(serviceProvider, logger, documentLoader);
            var cancellationToken = new CancellationTokenSource().Token;
            var towns = await dataRefreshService.LoadTownsAsync(cancellationToken).ConfigureAwait(false);

            Assert.That(towns.Count, Is.EqualTo(10));
        }

        private async Task<IDocument> LoadTestDataAsync(IBrowsingContext context, string filename)
        {
            var content = TestDataLoader.LoadTestData(filename);
            return await context.OpenAsync(req => req.Content(content)).ConfigureAwait(false);
        }
    }
}