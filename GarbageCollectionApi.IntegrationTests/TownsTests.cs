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
    public class TownsTests
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
        }

        [Test]
        public async Task RootRedirectsToIndex()
        {
            var response = await _client.GetAsync("/");
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Moved));
            Assert.That(response.Headers.Location.OriginalString, Is.EqualTo("index.html"));
        }
    }
}