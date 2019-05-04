namespace GarbageCollectionApi.Examples
{
    using System.Collections.Generic;
    using GarbageCollectionApi.DataContracts;
    using Swashbuckle.AspNetCore.Filters;

    public class CategoriesExamples : IExamplesProvider<IEnumerable<Category>>
    {
        public IEnumerable<Category> GetExamples()
        {
            return new List<Category>
            {
                new Category { Id = "1.5", Name = "Baum- und Strauchschnitt" },
                new Category { Id = "1.4", Name = "Biotonne" },
            };
        }
    }
}