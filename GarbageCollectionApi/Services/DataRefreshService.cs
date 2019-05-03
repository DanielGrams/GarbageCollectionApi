namespace GarbageCollectionApi.Services
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
    using Ical.Net;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class DataRefreshService : HostedService
    {
        private readonly ILogger logger;
        private readonly System.IServiceProvider services;
        private readonly IBrowsingContext browsingContext;

        public DataRefreshService(System.IServiceProvider services, ILogger<DataRefreshService> logger)
        {
             this.services = services;
             this.logger = logger;
             this.browsingContext = BrowsingContext.New(AngleSharp.Configuration.Default.WithDefaultLoader());
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
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
                     this.logger.LogError(e, "Parsing");
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<List<Town>> LoadTownsAsync(CancellationToken cancellationToken)
        {
            var document = await this.browsingContext.OpenAsync("https://www.kwb-goslar.de/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender", cancellationToken).ConfigureAwait(false);

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

        private async Task LoadStreetsAsync(List<Town> towns, CancellationToken cancellationToken)
        {
            foreach (var town in towns)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var document = await this.browsingContext.OpenAsync($"https://www.kwb-goslar.de/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender/index.php?ort={town.Id}", cancellationToken).ConfigureAwait(false);
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

                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        private async Task LoadCategoriesAsync(List<Town> towns, CancellationToken cancellationToken)
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

                    var document = await this.browsingContext.OpenAsync($"https://www.kwb-goslar.de/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender/index.php?ort={town.Id}&strasse={street.Id}", cancellationToken).ConfigureAwait(false);
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

                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
        }

        private async Task<List<CollectionEvent>> LoadEventsAsync(List<Town> towns, CancellationToken cancellationToken)
        {
            var events = new List<CollectionEvent>();

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

                    // TODOdgr: 2019 dynamisch anpassen? Man kann das Datum aus dem Dropdown auslesen. (Wenn man auch die Categories einliest)
                    var url = new Uri($"https://www.kwb-goslar.de/output/abfall_export.php?csv_export=1&mode=vcal&ort={town.Id}&strasse={street.Id}&vtyp=4&vMo=1&vJ=2019&bMo=12");
                    var icalText = string.Empty;

                    using (var client = new HttpClient())
                    {
                        using (var result = await client.GetAsync(url, cancellationToken).ConfigureAwait(false))
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                icalText = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                            }
                        }
                    }

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
                            Start = calEvent.DtStart.AsUtc,
                            End = calEvent.DtEnd.AsUtc,
                            Stamp = calEvent.DtStamp.AsUtc,
                            Summary = calEvent.Summary,
                            Description = calEvent.Description,
                        };

                        events.Add(collectionEvent);
                    }

                    await Task.Delay(100).ConfigureAwait(false);
                }
            }

            return events;
        }
    }
}