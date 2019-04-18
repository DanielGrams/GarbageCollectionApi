using System;
using NUnit.Framework;
using GarbageCollectionApi.Models;
using NSubstitute;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GarbageCollectionApi.Controllers
{
    [TestFixture]
    public class TownsControllerTests
    {
        [Test]
        public void Constructor_NullParameters_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new TownsController(null));
        }

        [Test]
        public void Constructor_AllParameters_CreatesInstance()
        {
            var service = NSubstitute.Substitute.For<ITownsService>();
            Assert.NotNull(new TownsController(service));
        }

        [Test]
        public async Task GetTowns_WhenCalled_ReturnsOkResult()
        {
            var service = NSubstitute.Substitute.For<ITownsService>();
            var controller = new TownsController(service);

            var actionResult = await controller.GetTowns();

            Assert.That(actionResult.Result, Is.TypeOf(typeof(OkObjectResult)));
        }
    }
}