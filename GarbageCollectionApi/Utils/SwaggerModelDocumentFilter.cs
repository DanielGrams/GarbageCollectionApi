namespace GarbageCollectionApi.Utils
{
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class SwaggerModelDocumentFilter<T> : IDocumentFilter
        where T : class
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            context.SchemaRegistry.GetOrRegister(typeof(T));
        }
    }
}