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
            var document = await this.LoadDocumentAsync(context, "towns.html").ConfigureAwait(false);

            if (document != null)
            {
                return document;
            }

            document = await base.LoadTownsDocumentAsync(context, cancellationToken).ConfigureAwait(false);
            await this.StoreDocumentAsync(document, "towns.html", cancellationToken).ConfigureAwait(false);
            return document;
        }

        public override async Task<IDocument> LoadStreetsDocumentAsync(string townId, IBrowsingContext context, CancellationToken cancellationToken)
        {
            var document = await this.LoadDocumentAsync(context, $"streets-{townId}.html").ConfigureAwait(false);

            if (document != null)
            {
                return document;
            }

            document = await base.LoadStreetsDocumentAsync(townId, context, cancellationToken).ConfigureAwait(false);
            await this.StoreDocumentAsync(document, $"streets-{townId}.html", cancellationToken).ConfigureAwait(false);
            return document;
        }

        public override async Task<IDocument> LoadCategoriesDocumentAsync(string townId, string streetId, IBrowsingContext context, CancellationToken cancellationToken)
        {
            var document = await this.LoadDocumentAsync(context, $"categories-{townId}-{streetId}.html").ConfigureAwait(false);

            if (document != null)
            {
                return document;
            }

            document = await base.LoadCategoriesDocumentAsync(townId, streetId, context, cancellationToken).ConfigureAwait(false);
            await this.StoreDocumentAsync(document, $"categories-{townId}-{streetId}.html", cancellationToken).ConfigureAwait(false);
            return document;
        }

        public override async Task<string> LoadEventsIcalTextAsync(string townId, string streetId, string year, CancellationToken cancellationToken)
        {
            var icalText = this.LoadContent($"events-{townId}-{streetId}-{year}.ics", Encoding.UTF8);

            if (icalText != null)
            {
                return icalText;
            }

            icalText = await base.LoadEventsIcalTextAsync(townId, streetId, year, cancellationToken).ConfigureAwait(false);
            await this.StoreContentAsync(icalText, $"events-{townId}-{streetId}-{year}.ics", Encoding.UTF8, cancellationToken).ConfigureAwait(false);
            return icalText;
        }

        private string GetDocumentPath(string filename)
        {
            return Path.Combine(this.documentBasePath, filename);
        }

        private async Task<IDocument> LoadDocumentAsync(IBrowsingContext context, string filename)
        {
            var content = this.LoadContent(filename, this.encoding).Replace("; charset=iso-8859-1", string.Empty, StringComparison.InvariantCultureIgnoreCase);

            if (content == null)
            {
                return null;
            }

            return await context.OpenAsync(req => req.Content(content)).ConfigureAwait(false);
        }

        private string LoadContent(string filename, Encoding encoding)
        {
            var path = this.GetDocumentPath(filename);

            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllText(path, encoding);
        }

        private async Task StoreDocumentAsync(IDocument document, string filename, CancellationToken cancellationToken)
        {
            await this.StoreContentAsync(document.Source.Text, filename, this.encoding, cancellationToken).ConfigureAwait(false);
        }

        private async Task StoreContentAsync(string content, string filename, Encoding encoding, CancellationToken cancellationToken)
        {
            var path = this.GetDocumentPath(filename);
            await File.WriteAllTextAsync(path, content, encoding, cancellationToken).ConfigureAwait(false);
        }
    }
}