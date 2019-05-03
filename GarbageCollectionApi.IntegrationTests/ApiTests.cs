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
    using NSubstitute;
    using NUnit.Framework;

    public class ApiTests : IDisposable
    {
        private CustomWebApplicationFactory<Startup> factory;
        private HttpClient client;
        private bool disposedValue = false;

        ~ApiTests()
        {
            this.Dispose(false);
        }

        public HttpClient Client => this.client;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SetUp]
        public void Setup()
        {
             this.factory = new CustomWebApplicationFactory<Startup>();
             this.client = this.factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.client.Dispose();
                    this.factory.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}