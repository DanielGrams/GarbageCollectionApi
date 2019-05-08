namespace GarbageCollectionApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using GarbageCollectionApi.Storage;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class DumpService : IDumpService
    {
        private readonly IDumpStorage storage;

        public DumpService(IDumpStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task DumpAsync(List<Town> towns, List<CollectionEvent> events, DataRefreshStatus refreshStatus)
        {
            var dumpData = CreateDumpData(towns, events, refreshStatus);
            await this.SaveToStorageAsync(dumpData).ConfigureAwait(false);
        }

        private static DataContracts.DumpData CreateDumpData(List<Town> towns, List<CollectionEvent> events, DataRefreshStatus refreshStatus)
        {
            var dumpStatus = new DataContracts.DataRefreshStatus { LatestRefresh = refreshStatus.LatestRefresh, LatestStamp = refreshStatus.LatestStamp };
            var dumpData = new DataContracts.DumpData { RefreshStatus = dumpStatus };

            foreach (var town in towns)
            {
                var dumpTown = new DataContracts.TownWithStreets { Id = town.Id, Name = town.Name };
                dumpData.Towns.Add(dumpTown);

                foreach (var street in town.Streets)
                {
                    var dumpStreet = new DataContracts.StreetWithEvents { Id = street.Id, Name = street.Name };
                    dumpTown.Streets.Add(dumpStreet);

                    foreach (var category in street.Categories)
                    {
                        var dumpCategory = new DataContracts.Category { Id = category.Id, Name = category.Name };
                        dumpStreet.Categories.Add(dumpCategory);
                    }

                    var eventsInStreet = events.Where(e => e.TownId == town.Id && e.StreetId == street.Id);

                    foreach (var eventInStreet in eventsInStreet)
                    {
                        var dumpEvent = new DataContracts.CollectionEvent
                        {
                            Id = eventInStreet.Id,
                            Category = new DataContracts.Category { Id = eventInStreet.Category.Id, Name = eventInStreet.Category.Name },
                            Date = eventInStreet.Start,
                            Stamp = eventInStreet.Stamp,
                        };
                        dumpStreet.Events.Add(dumpEvent);
                    }
                }
            }

            return dumpData;
        }

        private async Task SaveToStorageAsync(DataContracts.DumpData dumpData)
        {
            var storageStream = await this.storage.OpenWriteAsync().ConfigureAwait(false);

            using (ZipArchive archive = new ZipArchive(storageStream, ZipArchiveMode.Create))
            {
                var entry = archive.CreateEntry("dump.json", CompressionLevel.Optimal);
                var zipStream = entry.Open();

                using (var streamWriter = new StreamWriter(zipStream))
                {
                    using (var jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        var jsonSerializer = new JsonSerializer { Formatting = Newtonsoft.Json.Formatting.None };
                        jsonSerializer.Serialize(jsonWriter, dumpData);
                    }
                }
            }
        }
    }
}