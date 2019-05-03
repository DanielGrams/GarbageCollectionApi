namespace GarbageCollectionApi.IntegrationTest.Utils
{
    using System;
    using System.Runtime.CompilerServices;
    using GarbageCollectionApi.Models;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Mongo2Go;
    using MongoDB.Driver;

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private MongoDbRunner runner;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                this.runner = MongoDbRunner.Start();

                var client = new MongoClient(this.runner.ConnectionString);
                var db = client.GetDatabase("IntegrationTests");

                var towns = db.GetCollection<Town>("Towns");
                services.AddScoped<IMongoCollection<Town>>(_ => towns);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.runner?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}