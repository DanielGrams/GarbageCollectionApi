using System.Collections.Generic;
using GarbageCollectionApi.Models;
using System.Threading.Tasks;

public interface ITownsService
{
    Task<IEnumerable<Town>> GetAllItems();
}