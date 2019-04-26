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
    public class IndexTests : ApiTests
    {
        [Test]
        public async Task RootRedirectsToIndex()
        {
            var response = await this.Client.GetAsync("/");
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Moved));
            Assert.That(response.Headers.Location.OriginalString, Is.EqualTo("index.html"));
        }
    }
}