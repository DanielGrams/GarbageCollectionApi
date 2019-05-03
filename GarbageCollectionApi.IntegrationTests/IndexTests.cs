namespace GarbageCollectionApi.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using GarbageCollectionApi.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class IndexTests : ApiTests
    {
        [Test]
        public async Task RootRedirectsToIndex()
        {
            var response = await this.Client.GetAsync("/").ConfigureAwait(false);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Moved));
            Assert.That(response.Headers.Location.OriginalString, Is.EqualTo("index.html"));
        }
    }
}