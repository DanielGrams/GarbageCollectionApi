using System.Globalization;
using Microsoft.VisualBasic.CompilerServices;
using System.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using GarbageCollectionApi.Models;
using System.Net.Http;
using Ical.Net;
using Microsoft.Extensions.DependencyInjection;

namespace GarbageCollectionApi.Services
{
    public class DataRefreshService : HostedService
    {
        private readonly ILogger _logger;
        private readonly System.IServiceProvider _services;
        private readonly IBrowsingContext _browsingContext;
        
        public DataRefreshService(System.IServiceProvider services, ILogger<DataRefreshService> logger)
        {
            _services = services;
            _logger = logger;
            _browsingContext = BrowsingContext.New(AngleSharp.Configuration.Default.WithDefaultLoader());
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var towns = await this.LoadTownsAsync(cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.LoadStreetsAsync(towns, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.LoadCategoriesAsync(towns, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    var events = await this.LoadEventsAsync(towns, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    using (var scope = _services.CreateScope())
                    {
                        var updateService = scope.ServiceProvider.GetRequiredService<IUpdateService>();
                        await updateService.UpdateAsync(towns, events);
                    }

                    return;
                    
/*
                    // Events
                    foreach (var street in streets)
                    {
                        var url = $"https://www.kwb-goslar.de/output/abfall_export.php?csv_export=1&mode=vcal&ort={street.TownId}&strasse={street.Id}&vtyp=4&vMo=1&vJ=2019&bMo=12";
                        var icalText = string.Empty;

                        using (var client = new HttpClient())
                        {
                            using (var result = await client.GetAsync(url))
                            {
                                if (result.IsSuccessStatusCode)
                                {
                                    icalText = await result.Content.ReadAsStringAsync();
                                }
                            }
                        }

                        var calendar = Ical.Net.Calendar.Load(icalText);
                        foreach (var calEvent in calendar.Events)
                        {
                            _logger.LogDebug($"{calEvent.Start} {calEvent.Summary}");
                        }

                        await Task.Delay(100);

                        // TODO
                        break;
                    } */
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Parsing");
                }

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
            }
        }

        private async Task<List<Town>> LoadTownsAsync(CancellationToken cancellationToken)
        {
            var document = await _browsingContext.OpenAsync("https://www.kwb-goslar.de/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender", cancellationToken);

            // Towns
            var select = document.QuerySelectorAll("select").Where(m => m.HasAttribute("name") && m.GetAttribute("name").Equals("ort")).First();
            var options = select.QuerySelectorAll("option");

            var towns = new List<Town>();

            foreach(var option in options)
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

                var document = await _browsingContext.OpenAsync($"https://www.kwb-goslar.de/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender/index.php?ort={town.Id}", cancellationToken);
                var select = document.QuerySelectorAll("select").Where(m => m.HasAttribute("name") && m.GetAttribute("name").Equals("strasse")).First();
                var options = select.QuerySelectorAll("option");

                var streets = new List<Street>();
                foreach(var option in options)
                {
                    var value = option.GetAttribute("value");
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }

                    var street = new Street { Id = value, Name = option.InnerHtml };
                    streets.Add(street);
                }
                town.Streets = streets;

                await Task.Delay(100);

                // TODO
                break;
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

                    var document = await _browsingContext.OpenAsync($"https://www.kwb-goslar.de/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender/index.php?ort={town.Id}&strasse={street.Id}", cancellationToken);
                    var checkboxes = document.QuerySelectorAll("input").Where(m => 
                    {
                        return m.HasAttribute("type") && m.GetAttribute("type").Equals("checkbox") && 
                            m.HasAttribute("name") && m.GetAttribute("name").Equals("abfart[]");
                    });

                    var categories = new List<Category>();
                    foreach(var checkbox in checkboxes)
                    {
                        var elementId = checkbox.GetAttribute("id");
                        var value = checkbox.GetAttribute("value");
                        
                        var label = document.QuerySelectorAll("label").Where(m => m.HasAttribute("for") && m.GetAttribute("for").Equals(elementId)).First();
                        var category = new Category { Id = value, Name = label.InnerHtml };
                        categories.Add(category);
                    };
                    street.Categories = categories;

                    await Task.Delay(100);

                    // TODO
                    break;
                }
            }
        }

        private async Task<List<Event>> LoadEventsAsync(List<Town> towns, CancellationToken cancellationToken)
        {
            var events = new List<Event>();

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

                    // TODO: 2019 dynamisch anpassen? oder immer aktuell und das nächste? das nächste ist dann aber optional.
                    // Nein! Man kann das Datum aus dem Dropdown auslesen. (Wenn man auch die Categories einliest)

                    var url = $"https://www.kwb-goslar.de/output/abfall_export.php?csv_export=1&mode=vcal&ort={town.Id}&strasse={street.Id}&vtyp=4&vMo=1&vJ=2019&bMo=12";
                    var icalText = string.Empty;

                    using (var client = new HttpClient())
                    {
                        using (var result = await client.GetAsync(url, cancellationToken))
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                icalText = await result.Content.ReadAsStringAsync();
                            }
                        }
                    }

                    var calendar = Ical.Net.Calendar.Load(icalText);

                    foreach (var calEvent in calendar.Events)
                    {
                        var category = street.Categories.First(c => calEvent.Summary.Contains(c.Name));

                        var collectionEvent = new Event
                        {
                            Id = calEvent.Uid,
                            TownId = town.Id,
                            StreetId = street.Id,
                            Category = category,
                            Start = calEvent.DtStart.AsUtc,
                            End = calEvent.DtEnd.AsUtc,
                            Stamp = calEvent.DtStamp.AsUtc,
                            Summary = calEvent.Summary,
                            Description = calEvent.Description
                        };
                        
                        events.Add(collectionEvent);
                    }

                    await Task.Delay(100);

                    // TODO
                    break;
                }
            }

            return events;
        }
    }
}