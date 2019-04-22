using System.Linq;
using System;
using NUnit.Framework;
using GarbageCollectionApi.Models;
using NSubstitute;
using System.Threading.Tasks;
using System.Collections.Generic;
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

        [Test]
        public async Task GetTowns_WhenCalled_ReturnsTownsFromService()
        {
            IEnumerable<Town> serviceTowns = new List<Town> {
                new Town { Name = "Goslar" },
                new Town { Name = "Oker" }
            };

            var service = NSubstitute.Substitute.For<ITownsService>();
            service.GetAllItems().Returns(Task.Run(() => serviceTowns));

            var controller = new TownsController(service);

            var actionResult = await controller.GetTowns();
            var value = (actionResult.Result as OkObjectResult).Value;
            Assert.That(value, Is.AssignableTo(typeof(IEnumerable<Town>)));
            
            var resultTowns = value as IEnumerable<Town>;
            Assert.That(resultTowns.Count, Is.EqualTo(2));
            Assert.That(resultTowns.First().Name, Is.EqualTo("Goslar"));
            Assert.That(resultTowns.Skip(1).First().Name, Is.EqualTo("Oker"));
        }
    }
}