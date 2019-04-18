using System.Collections.Generic;
using GarbageCollectionApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class TownsService : ITownsService
{
    private readonly GarbageCollectionContext _context;

    public TownsService(GarbageCollectionContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Town>> GetAllItems()
    {
        var items = await _context.Towns.ToListAsync();
        return items;
    }
}