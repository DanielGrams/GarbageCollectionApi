namespace GarbageCollectionApi.Services.Scraping
{
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

    public class DocumentLoader : IDocumentLoader
    {
        public Task<IDocument> LoadTownsDocument(IBrowsingContext context, CancellationToken cancellationToken)
        {
            return context.OpenAsync("https://www.kwb-goslar.de/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender", cancellationToken);
        }
    }
}