namespace GarbageCollectionApi.Examples
{
    using System.Collections.Generic;
    using GarbageCollectionApi.DataContracts;
    using Swashbuckle.AspNetCore.Filters;

    public class TownsExamples : IExamplesProvider<IEnumerable<Town>>
    {
        public IEnumerable<Town> GetExamples()
        {
            return new List<Town>
            {
                new Town { Id = "62.1", Name = "Goslar" },
                new Town { Id = "62.4", Name = "Oker" },
            };
        }
    }
}