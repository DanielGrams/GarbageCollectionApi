namespace GarbageCollectionApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using GarbageCollectionApi.Models;
    using GarbageCollectionApi.Services;
    using GarbageCollectionApi.Services.Scraping;
    using GarbageCollectionApi.Storage;
    using GarbageCollectionApi.Utils;
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using Swashbuckle.AspNetCore.Filters;
    using Swashbuckle.AspNetCore.Swagger;

    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Config</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Gets configuration specified in constructor
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoConnectionSettings>(this.Configuration.GetSection("MongoConnection"));
            services.Configure<DataRefreshSettings>(this.Configuration.GetSection("DataRefresh"));
            services.Configure<StorageSettings>(this.Configuration.GetSection("Storage"));
            services.Configure<AuthorizationSettings>(this.Configuration.GetSection("Authorization"));

            services.AddHostedService<DataRefreshService>();
#if DEBUG
            services.AddSingleton<IDocumentLoader, DebugDocumentLoader>();
#else
            services.AddSingleton<IDocumentLoader, DocumentLoader>();
#endif
            services.AddScoped<ITownsService, TownsService>();
            services.AddScoped<IStreetsService, StreetsService>();
            services.AddScoped<IUpdateService, UpdateService>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<IEventsService, EventsService>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IDumpService, DumpService>();

            services.AddApplicationInsightsTelemetry();
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true;
            });
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(AuthorizationKeyFilterAttribute));
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.ProducesResponseTypeAttribute(401));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddProblemDetails();

            // Register the Swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Goslar GarbageCollection API",
                    Version = "v1",
                    Description = "API for garbage collection in the district of Goslar",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Daniel Grams",
                        Url = "https://github.com/DanielGrams/GarbageCollectionApi",
                    },
                });

                c.AddSecurityDefinition("ApiKeyAuth", new ApiKeyScheme()
                {
                    In = "header",
                    Name = AuthorizationKeyFilterAttribute.ApiKeyHeaderName,
                    Type = "apiKey",
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "ApiKeyAuth", Enumerable.Empty<string>() },
                });

                c.OperationFilter<SwaggerFileOperationFilter>();
                c.ExampleFilters();
                c.OperationFilter<AddResponseHeadersFilter>();
                c.DocumentFilter<SwaggerModelDocumentFilter<DumpData>>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerExamplesFromAssemblyOf<Startup>();

            var storageSettings = services.BuildServiceProvider().GetService<IOptionsMonitor<StorageSettings>>();
            var connectionString = storageSettings?.CurrentValue?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                services.AddScoped<IDumpStorage, LocalDumpStorage>();
            }
            else
            {
                services.AddScoped<IDumpStorage, AzureDumpStorage>();
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">App</param>
        /// <param name="env">Environment</param>
#pragma warning disable CA1822
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#pragma warning restore CA1822
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                TelemetryConfiguration.Active.DisableTelemetry = true;
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GarbageCollection API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseProblemDetails();
            app.UseMvc();
        }
    }
}
