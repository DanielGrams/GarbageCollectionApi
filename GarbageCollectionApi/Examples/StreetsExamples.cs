namespace GarbageCollectionApi.Examples
{
    using System.Collections.Generic;
    using GarbageCollectionApi.DataContracts;
    using Swashbuckle.AspNetCore.Filters;

    public class StreetsExamples : IExamplesProvider<IEnumerable<Street>>
    {
        public IEnumerable<Street> GetExamples()
        {
            return new List<Street>
            {
                new Street { Id = "2523.907.1", Name = "Schreiberstraße" },
                new Street { Id = "2523.921.1", Name = "Zwingerwall" },
            };
        }
    }
}