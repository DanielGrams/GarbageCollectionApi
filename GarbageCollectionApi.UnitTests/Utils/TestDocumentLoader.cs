namespace GarbageCollectionApi.UnitTests.Utils
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;
    using AngleSharp.Html.Parser;
    using AngleSharp.Io;
    using GarbageCollectionApi.Services.Scraping;
    using NUnit.Framework;

    public class TestDocumentLoader : GarbageCollectionApi.Services.Scraping.IDocumentLoader
    {
        private readonly string documentBasePath;
        private readonly Encoding encoding;

        public TestDocumentLoader()
        {
            this.documentBasePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Set");
            this.encoding = Encoding.GetEncoding("iso-8859-1");
        }

        public async Task<IDocument> LoadTownsDocumentAsync(IBrowsingContext context, CancellationToken cancellationToken)
        {
            return await this.LoadDocumentAsync(context, "towns.html").ConfigureAwait(false);
        }

        public async Task<IDocument> LoadStreetsDocumentAsync(string townId, IBrowsingContext context, CancellationToken cancellationToken)
        {
            return await this.LoadDocumentAsync(context, $"streets-{townId}.html").ConfigureAwait(false);
        }

        public async Task<IDocument> LoadCategoriesDocumentAsync(string townId, string streetId, IBrowsingContext context, CancellationToken cancellationToken)
        {
            return await this.LoadDocumentAsync(context, $"categories-{townId}-{streetId}.html").ConfigureAwait(false);
        }

        public Task<string> LoadEventsIcalTextAsync(string townId, string streetId, string year, CancellationToken cancellationToken)
        {
            return Task.Run(() => this.LoadTestData($"events-{townId}-{streetId}-{year}.ics", Encoding.UTF8));
        }

        private async Task<IDocument> LoadDocumentAsync(IBrowsingContext context, string filename)
        {
            var content = this.LoadTestData(filename, this.encoding).Replace("; charset=iso-8859-1", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            return await context.OpenAsync(req => req.Content(content)).ConfigureAwait(false);
        }

        private string GetTestDataPath(string filename)
        {
            return Path.Combine(this.documentBasePath, filename);
        }

        private string LoadTestData(string filename, Encoding encoding)
        {
            var path = this.GetTestDataPath(filename);
            return File.ReadAllText(path, encoding);
        }
    }
}