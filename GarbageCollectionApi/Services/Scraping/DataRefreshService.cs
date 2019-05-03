namespace GarbageCollectionApi.Services.Scraping
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using GarbageCollectionApi.Models;
    using GarbageCollectionApi.Utils;
    using Ical.Net;
    using Ical.Net.DataTypes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class DataRefreshService : HostedService
    {
        private readonly ILogger logger;
        private readonly IDocumentLoader documentLoader;
        private readonly System.IServiceProvider services;
        private readonly IBrowsingContext browsingContext;

        public DataRefreshService(
            System.IServiceProvider services,
            ILogger<DataRefreshService> logger,
            IDocumentLoader documentLoader)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.documentLoader = documentLoader ?? throw new ArgumentNullException(nameof(documentLoader));
            this.browsingContext = BrowsingContext.New(AngleSharp.Configuration.Default.WithDefaultLoader());
        }

        /// <inheritdoc />
        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var towns = await this.LoadTownsAsync(cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.LoadStreetsAsync(towns, cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.LoadCategoriesAsync(towns, cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    var events = await this.LoadEventsAsync(towns, cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var scope = this.services.CreateScope())
                    {
                        var updateService = scope.ServiceProvider.GetRequiredService<IUpdateService>();
                        await updateService.UpdateAsync(towns, events).ConfigureAwait(false);
                    }
                }
                catch (TaskCanceledException)
                {
                    // Cancellation is fine
                }
#pragma warning disable CA1031 // Catch generic exception to continue log running task
                catch (Exception e)
#pragma warning restore CA1031
                {
                     this.logger.LogError(e, "Error while scraping");
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<List<Town>> LoadTownsAsync(CancellationToken cancellationToken)
        {
            var document = await this.documentLoader.LoadTownsDocumentAsync(this.browsingContext, cancellationToken).ConfigureAwait(false);

            // Towns
            var select = document.QuerySelectorAll("select").First(m => m.HasAttribute("name") && m.GetAttribute("name") == "ort");
            var options = select.QuerySelectorAll("option");

            var towns = new List<Town>();

            foreach (var option in options)
            {
                var value = option.GetAttribute("value");
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                towns.Add(new Town { Id = value, Name = option.InnerHtml });
            }

            return towns;
        }

        public async Task LoadStreetsAsync(List<Town> towns, CancellationToken cancellationToken)
        {
            foreach (var town in towns)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var document = await this.documentLoader.LoadStreetsDocumentAsync(town.Id, this.browsingContext, cancellationToken).ConfigureAwait(false);
                var select = document.QuerySelectorAll("select").First(m => m.HasAttribute("name") && m.GetAttribute("name") == "strasse");
                var options = select.QuerySelectorAll("option");

                var streets = new List<Street>();
                foreach (var option in options)
                {
                    var value = option.GetAttribute("value");
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }

                    var street = new Street { Id = value, Name = option.InnerHtml };
                    streets.Add(street);
                }

                town.Streets.AddRange(streets);

                if (town.Id == "62.53")
                {
                    return;
                }

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        public async Task LoadCategoriesAsync(List<Town> towns, CancellationToken cancellationToken)
        {
            foreach (var town in towns)
            {
                if (town.Streets == null)
                {
                    continue;
                }

                foreach (var street in town.Streets)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var document = await this.documentLoader.LoadCategoriesDocumentAsync(town.Id, street.Id, this.browsingContext, cancellationToken).ConfigureAwait(false);
                    var checkboxes = document.QuerySelectorAll("input").Where(m =>
                    {
                        return m.HasAttribute("type") && m.GetAttribute("type") == "checkbox" &&
                            m.HasAttribute("name") && m.GetAttribute("name") == "abfart[]";
                    });

                    var categories = new List<Category>();
                    foreach (var checkbox in checkboxes)
                    {
                        var elementId = checkbox.GetAttribute("id");
                        var value = checkbox.GetAttribute("value");

                        var label = document.QuerySelectorAll("label").First(m => m.HasAttribute("for") && m.GetAttribute("for") == elementId);
                        var category = new Category { Id = value, Name = label.InnerHtml };
                        categories.Add(category);
                    }

                    street.Categories.AddRange(categories);

                    if (town.Id == "62.53")
                    {
                        return;
                    }

                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
        }

        public async Task<List<CollectionEvent>> LoadEventsAsync(List<Town> towns, CancellationToken cancellationToken)
        {
            var events = new List<CollectionEvent>();
            var icalTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");

            foreach (var town in towns)
            {
                if (town.Streets == null)
                {
                    continue;
                }

                foreach (var street in town.Streets)
                {
                    if (street.Categories == null)
                    {
                        continue;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    var icalText = await this.documentLoader.LoadEventsIcalTextAsync(town.Id, street.Id, "2019", cancellationToken).ConfigureAwait(false);
                    var calendar = Ical.Net.Calendar.Load(icalText);

                    foreach (var calEvent in calendar.Events)
                    {
                        var category = street.Categories.First(c => calEvent.Summary.Contains(c.Name, StringComparison.InvariantCulture));

                        var collectionEvent = new CollectionEvent
                        {
                            Id = calEvent.Uid,
                            TownId = town.Id,
                            StreetId = street.Id,
                            Category = category,
                            Start = ConvertToUtc(calEvent.DtStart, icalTimeZone),
                            Stamp = calEvent.DtStamp.AsUtc,
                        };

                        events.Add(collectionEvent);
                    }

                    if (town.Id == "62.53")
                    {
                        return events;
                    }

                    await Task.Delay(100).ConfigureAwait(false);
                }
            }

            return events;
        }

        private static DateTime ConvertToUtc(IDateTime input, TimeZoneInfo icalTimeZone)
        {
            var d = TimeZoneInfo.ConvertTime(input.AsSystemLocal, TimeZoneInfo.Local, icalTimeZone);
            return TimeZoneInfo.ConvertTimeToUtc(d, icalTimeZone);
        }
    }
}