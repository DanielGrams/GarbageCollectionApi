using System.Net;
using System.Runtime.CompilerServices;
using System.Linq;
using System;
using NUnit.Framework;
using GarbageCollectionApi.Models;
using NSubstitute;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GarbageCollectionApi.IntegrationTest
{
    [TestFixture]
    public class ApiTests
    {
        private CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
        }

        [TearDown]
        public void TearDown()
        {
            _factory.TearDown();
        }

        public HttpClient Client => this._client;
    }
}