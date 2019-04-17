using GarbageCollectionApi.Models;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using Swashbuckle.AspNetCore.Swagger;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            // Identity BEGIN
            services.AddIdentityServer()
                .AddInMemoryClients(new List<Client>
                {
                    new Client {
                        ClientId = "PetersGoslarApp",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = new List<Secret> {
                            new Secret("superPeterPassword".Sha256())},                         
                        AllowedScopes = new List<string> {"api.read"}
                    },
                    new Client {
                        ClientId = "lk-gs-scaper",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = new List<Secret> {
                            new Secret("hallohallohallo".Sha256())},                         
                        AllowedScopes = new List<string> {"api.read", "api.write"}
                    }
                })
                .AddInMemoryIdentityResources(new List<IdentityResource> {
                    new IdentityResources.OpenId(),
                    new IdentityResource {
                        Name = "scopes",
                        UserClaims = new List<string> {"scopes"}
                    }
                })
                .AddInMemoryApiResources(new List<ApiResource>
                {
                    new ApiResource {
                        Name = "API",
                        DisplayName = "API Access",
                        UserClaims = new List<string> {"scopes"},
                        Scopes = new List<Scope> {
                            new Scope("api.read"),
                            new Scope("api.write")
                        }
                    }
                })
                .AddDeveloperSigningCredential(); // TODO

            // Identity END


            // Authorization BEGIN
            services.AddAuthorization(options =>
            {
                options.AddPolicy("api.read", policy => policy.Requirements.Add(new HasScopeRequirement("api.read")));
                options.AddPolicy("api.write", policy => policy.Requirements.Add(new HasScopeRequirement("api.write")));
            });
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://localhost:5000"; // TODO: self
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };
                });
            // Authorization END

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            services.AddDbContext<GarbageCollectionContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("Database")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // TODO
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {
                    Title = "GarbageCollection API",
                    Version = "v1",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Shayne Boyer",
                        Email = string.Empty,
                        Url = "https://twitter.com/spboyer"
                    },
                    License = new License
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Identity BEGIN
            app.UseIdentityServer();
            // Identity END

            // Authorization BEGIN
            app.UseAuthentication();
            // Authorization END

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
