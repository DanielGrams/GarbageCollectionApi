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
    public class TownsTests : ApiTests
    {
        [Test]
        public async Task TownsReturnsOk()
        {
            var response = await this.Client.GetAsync("/api/towns").ConfigureAwait(false);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}