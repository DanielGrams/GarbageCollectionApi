using System;
using System.Collections.Generic;
using GarbageCollectionApi.DataContracts;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class TownsService : ITownsService
{
    private readonly IMongoCollection<GarbageCollectionApi.Models.Town> _towns;

    public TownsService(IMongoCollection<GarbageCollectionApi.Models.Town> towns)
    {
        _towns = towns ?? throw new ArgumentNullException(nameof(towns));
    }

    public async Task<List<Town>> GetAllItemsAsync()
    {
        return await _towns
            .Find(town => true)
            .Project(town => new Town { Id = town.Id, Name = town.Name })
            .ToListAsync();
    }
}