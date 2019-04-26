using System;
using GarbageCollectionApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class CustomWebApplicationFactory<TStartup> 
    : WebApplicationFactory<TStartup> where TStartup: class
{
    private SqliteConnection _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        builder.ConfigureServices(services =>
        {
            // Create a new service provider.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();

            // Add a database context using an sqlite
            // database for testing.
            services.AddDbContext<GarbageCollectionContext>(options => 
            {
                options.UseSqlite(_connection);
                options.UseInternalServiceProvider(serviceProvider);
            });

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database
            // context.
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<GarbageCollectionContext>();
                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                try
                {
                    // Seed the database with test data.
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred seeding the " +
                        "database with test messages. Error: {ex.Message}");
                }
            }
        });
    }

    public void TearDown()
    {
        _connection.Close();
    }
}