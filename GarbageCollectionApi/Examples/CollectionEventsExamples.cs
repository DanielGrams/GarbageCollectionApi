namespace GarbageCollectionApi.Examples
{
    using System.Collections.Generic;
    using GarbageCollectionApi.DataContracts;
    using GarbageCollectionApi.Utils;
    using Swashbuckle.AspNetCore.Filters;

    public class CollectionEventsExamples : IExamplesProvider<IEnumerable<CollectionEvent>>
    {
        public IEnumerable<CollectionEvent> GetExamples()
        {
            return new List<CollectionEvent>
            {
                new CollectionEvent
                {
                    Id = "fdc0b08929027ca3edef21a3107e766a",
                    Date = DateTimeUtils.Utc(2019, 2, 20, 23),
                    Stamp = DateTimeUtils.Utc(2018, 11, 28),
                    Category = new Category { Id = "1.5", Name = "Baum- und Strauchschnitt" },
                },
                new CollectionEvent
                {
                    Id = "454f9e3ff522d7fb127819ba24dccdf9",
                    Date = DateTimeUtils.Utc(2019, 1, 28, 23),
                    Stamp = DateTimeUtils.Utc(2018, 11, 28),
                    Category = new Category { Id = "1.4", Name = "Weihnachtsbäume" },
                },
            };
        }
    }
}