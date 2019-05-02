using System;
using GarbageCollectionApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Driver;

public class CustomWebApplicationFactory<TStartup> 
    : WebApplicationFactory<TStartup> where TStartup: class
{
    internal static MongoDbRunner _runner;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            _runner = MongoDbRunner.Start();

            var client = new MongoClient(_runner.ConnectionString);
            var db = client.GetDatabase("IntegrationTests");

            var towns = db.GetCollection<Town>("Towns");
            services.AddScoped<IMongoCollection<Town>>(_ => towns);
        });
    }

    public void TearDown()
    {
        _runner.Dispose();
    }
}