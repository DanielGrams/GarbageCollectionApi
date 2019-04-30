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

namespace GarbageCollectionApi.Services
{
    public class DataRefreshService : HostedService
    {
        private readonly ILogger _logger;
        
        public DataRefreshService(ILogger<DataRefreshService> logger)
        {
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var browsingContext = BrowsingContext.New(AngleSharp.Configuration.Default.WithDefaultLoader());
                    var document = await browsingContext.OpenAsync("https://www.kwb-goslar.de/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender", cancellationToken);

                    // Towns
                    var select = document.QuerySelectorAll("select").Where(m => m.HasAttribute("name") && m.GetAttribute("name").Equals("ort")).First();
                    var options = select.QuerySelectorAll("option");
                    var towns = new List<KwbTown>();

                    foreach(var option in options)
                    {
                        var value = option.GetAttribute("value");
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            continue;
                        }

                        var town = new KwbTown { Id = value, Name = option.InnerHtml };
                        towns.Add(town);
                    }

                    await Task.Delay(100);

                    // Streets
                    var streets = new List<KwbStreet>();
                    foreach (var town in towns)
                    {
                        document = await browsingContext.OpenAsync($"https://www.kwb-goslar.de/Abfallwirtschaft/Abfuhr/Online-Abfuhrkalender/index.php?ort={town.Id}");
                        select = document.QuerySelectorAll("select").Where(m => m.HasAttribute("name") && m.GetAttribute("name").Equals("strasse")).First();
                        options = select.QuerySelectorAll("option");

                        foreach(var option in options)
                        {
                            var value = option.GetAttribute("value");
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                continue;
                            }

                            var street = new KwbStreet { Id = value, Name = option.InnerHtml, TownId = town.Id };
                            streets.Add(street);
                        }

                        await Task.Delay(100);

                        // TODO
                        break;
                    }

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
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Parsing");
                }

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
            }
        }

        class KwbTown
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        class KwbStreet
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string TownId { get; set; }
        }
    }
}