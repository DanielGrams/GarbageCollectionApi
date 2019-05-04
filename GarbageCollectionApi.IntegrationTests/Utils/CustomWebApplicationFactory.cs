namespace GarbageCollectionApi.IntegrationTest.Utils
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using GarbageCollectionApi.Models;
    using GarbageCollectionApi.Services.Scraping;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Mongo2Go;
    using MongoDB.Driver;
    using NSubstitute;

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private readonly MongoConnectionSettings mongoSettings;

        public CustomWebApplicationFactory(MongoConnectionSettings mongoSettings)
        {
            this.mongoSettings = mongoSettings;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<MongoConnectionSettings>(options =>
                    {
                        options.ConnectionString = this.mongoSettings.ConnectionString;
                        options.Database = this.mongoSettings.Database;
                    });

                var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ImplementationType == typeof(DataRefreshService));
                services.Remove(serviceDescriptor);
            });
        }
    }
}