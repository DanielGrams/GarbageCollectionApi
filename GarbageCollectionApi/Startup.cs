using GarbageCollectionApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GarbageCollectionApi.Services;
using MongoDB.Driver;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace GarbageCollectionApi
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Creates a new Startup instance
        /// </summary>
        /// <param name="configuration">Config</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration specified in constructor
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<DataRefreshService>();
            services.AddScoped<ITownsService, TownsService>();
            services.AddScoped<IStreetsService, StreetsService>();
            services.AddScoped<IUpdateService, UpdateService>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<IEventsService, EventsService>();

            var client = new MongoClient(this.Configuration.GetConnectionString("Database"));
            var database = client.GetDatabase("GarbageCollectionDb");
            var towns = database.GetCollection<Town>("Towns");
            services.AddScoped<IMongoCollection<Town>>(_ => towns);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // TODO
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {
                    Title = "Goslar GarbageCollection API",
                    Version = "v1",
                    Description = "API for garbage collection in the district of Goslar",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Daniel Grams",
                        Email = string.Empty,
                        Url = "https://github.com/DanielGrams/GarbageCollectionApi"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">App</param>
        /// <param name="env">Environment</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GarbageCollection API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
