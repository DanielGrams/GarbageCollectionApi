namespace GarbageCollectionApi.Services.Scraping
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

    public class DocumentLoader : IDocumentLoader
    {
        private readonly string baseUrl;
        private readonly string abfuhrBaseUrl;

        public DocumentLoader()
        {
            this.baseUrl = "https://www.kwb-goslar.de";
            this.abfuhrBaseUrl = $"{this.baseUrl}/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender";
        }

        public virtual async Task<IDocument> LoadTownsDocumentAsync(IBrowsingContext context, CancellationToken cancellationToken)
        {
            return await context.OpenAsync(this.abfuhrBaseUrl, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IDocument> LoadStreetsDocumentAsync(string townId, IBrowsingContext context, CancellationToken cancellationToken)
        {
            return await context.OpenAsync($"{this.abfuhrBaseUrl}/index.php?ort={townId}", cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<IDocument> LoadCategoriesDocumentAsync(string townId, string streetId, IBrowsingContext context, CancellationToken cancellationToken)
        {
            return await context.OpenAsync($"{this.abfuhrBaseUrl}/index.php?ort={townId}&strasse={streetId}", cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<string> LoadEventsIcalTextAsync(string townId, string streetId, string year, CancellationToken cancellationToken)
        {
            var url = new Uri($"{this.baseUrl}/output/abfall_export.php?csv_export=1&mode=vcal&ort={townId}&strasse={streetId}&vtyp=4&vMo=1&vJ={year}&bMo=12");
            string icalText;

            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(url, cancellationToken).ConfigureAwait(false))
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"{url} returned with status {result.StatusCode}");
                    }

                    icalText = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            return icalText;
        }
    }
}