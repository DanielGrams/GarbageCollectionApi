namespace GarbageCollectionApi.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using GarbageCollectionApi.IntegrationTest.Utils;
    using GarbageCollectionApi.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Mongo2Go;
    using MongoDB.Driver;
    using NSubstitute;
    using NUnit.Framework;

    public class ApiTests : IDisposable
    {
        private CustomWebApplicationFactory<Startup> factory;
        private bool disposedValue = false;

        ~ApiTests()
        {
            this.Dispose(false);
        }

        public HttpClient Client { get; private set; }

        protected MongoDbRunner Runner { get; private set; }

        protected MongoClient MongoClient { get; private set; }

        protected IMongoDatabase Database { get; private set; }

        protected MongoConnectionSettings Settings { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SetUp]
        public virtual void Setup()
        {
            this.Runner = MongoDbRunner.Start();

            this.Settings = new MongoConnectionSettings
            {
                ConnectionString = this.Runner.ConnectionString,
                Database = "IntegrationTests",
            };

            this.MongoClient = new MongoClient(this.Settings.ConnectionString);
            this.Database = this.MongoClient.GetDatabase(this.Settings.Database);

            this.factory = new CustomWebApplicationFactory<Startup>(this.Settings);
            this.Client = this.factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
        }

        [TearDown]
        public void TearDown()
        {
            this.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.MongoClient = null;
                    this.Runner.Dispose();
                    this.Client.Dispose();
                    this.factory.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}