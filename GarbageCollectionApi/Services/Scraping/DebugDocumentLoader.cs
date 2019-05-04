namespace GarbageCollectionApi.Services.Scraping
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

    public class DebugDocumentLoader : DocumentLoader
    {
        private readonly string documentBasePath;
        private readonly Encoding encoding;

        public DebugDocumentLoader()
            : base()
        {
            this.encoding = Encoding.GetEncoding("iso-8859-1");
            this.documentBasePath = Path.Combine(AppContext.BaseDirectory, "Documents");
            Directory.CreateDirectory(this.documentBasePath);
        }

        public override async Task<IDocument> LoadTownsDocumentAsync(IBrowsingContext context, CancellationToken cancellationToken)
        {
            var document = await base.LoadTownsDocumentAsync(context, cancellationToken).ConfigureAwait(false);
            await this.StoreDocument(document, "towns.html", cancellationToken).ConfigureAwait(false);
            return document;
        }

        public override async Task<IDocument> LoadStreetsDocumentAsync(string townId, IBrowsingContext context, CancellationToken cancellationToken)
        {
            var document = await base.LoadStreetsDocumentAsync(townId, context, cancellationToken).ConfigureAwait(false);
            await this.StoreDocument(document, $"streets-{townId}.html", cancellationToken).ConfigureAwait(false);
            return document;
        }

        public override async Task<IDocument> LoadCategoriesDocumentAsync(string townId, string streetId, IBrowsingContext context, CancellationToken cancellationToken)
        {
            var document = await base.LoadCategoriesDocumentAsync(townId, streetId, context, cancellationToken).ConfigureAwait(false);
            await this.StoreDocument(document, $"categories-{townId}-{streetId}.html", cancellationToken).ConfigureAwait(false);
            return document;
        }

        public override async Task<string> LoadEventsIcalTextAsync(string townId, string streetId, string year, CancellationToken cancellationToken)
        {
            var icalText = await base.LoadEventsIcalTextAsync(townId, streetId, year, cancellationToken).ConfigureAwait(false);
            await this.StoreContent(icalText, $"events-{townId}-{streetId}-{year}.ics", Encoding.UTF8, cancellationToken).ConfigureAwait(false);
            return icalText;
        }

        public async Task StoreDocument(IDocument document, string filename, CancellationToken cancellationToken)
        {
            await this.StoreContent(document.Source.Text, filename, this.encoding, cancellationToken).ConfigureAwait(false);
        }

        public async Task StoreContent(string content, string filename, Encoding encoding, CancellationToken cancellationToken)
        {
            var path = Path.Combine(this.documentBasePath, filename);
            await File.WriteAllTextAsync(path, content, encoding, cancellationToken).ConfigureAwait(false);
        }
    }
}