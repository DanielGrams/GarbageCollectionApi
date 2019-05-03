namespace GarbageCollectionApi.Services.Scraping
{
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

    public interface IDocumentLoader
    {
        Task<IDocument> LoadTownsDocument(IBrowsingContext context, CancellationToken cancellationToken);
    }
}