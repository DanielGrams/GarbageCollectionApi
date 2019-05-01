using System.Collections.Generic;
using GarbageCollectionApi.Models;
using System.Threading.Tasks;

public interface IUpdateService
{
    Task UpdateAsync(List<Town> towns, List<Event> events);
}