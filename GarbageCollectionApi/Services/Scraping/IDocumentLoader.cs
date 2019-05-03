namespace GarbageCollectionApi.Services.Scraping
{
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

    public interface IDocumentLoader
    {
        Task<IDocument> LoadTownsDocumentAsync(IBrowsingContext context, CancellationToken cancellationToken);

        Task<IDocument> LoadStreetsDocumentAsync(string townId, IBrowsingContext context, CancellationToken cancellationToken);

        Task<IDocument> LoadCategoriesDocumentAsync(string townId, string streetId, IBrowsingContext context, CancellationToken cancellationToken);

        Task<string> LoadEventsIcalTextAsync(string townId, string streetId, string year, CancellationToken cancellationToken);
    }
}