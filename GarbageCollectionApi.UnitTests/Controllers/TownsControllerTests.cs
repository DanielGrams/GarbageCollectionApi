using System.Linq;
using System;
using NUnit.Framework;
using GarbageCollectionApi.DataContracts;
using NSubstitute;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using GarbageCollectionApi.Controllers;

namespace GarbageCollectionApi.UnitTests.Controllers
{
    [TestFixture]
    public class TownsControllerTests
    {
        [Test]
        public void Constructor_NullParameters_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new TownsController(null, null, null, null));
        }

        [Test]
        public void Constructor_AllParameters_CreatesInstance()
        {
            var townsService = NSubstitute.Substitute.For<ITownsService>();
            var streetsService = NSubstitute.Substitute.For<IStreetsService>();
            var categoriesService = NSubstitute.Substitute.For<ICategoriesService>();
            var eventsService = NSubstitute.Substitute.For<IEventsService>();

            Assert.NotNull(new TownsController(townsService, streetsService, categoriesService, eventsService));
        }

        [Test]
        public async Task GetTowns_WhenCalled_ReturnsOkResult()
        {
            var townsService = NSubstitute.Substitute.For<ITownsService>();
            var streetsService = NSubstitute.Substitute.For<IStreetsService>();
            var categoriesService = NSubstitute.Substitute.For<ICategoriesService>();
            var eventsService = NSubstitute.Substitute.For<IEventsService>();
            var controller = new TownsController(townsService, streetsService, categoriesService, eventsService);

            var actionResult = await controller.GetTownsAsync();

            Assert.That(actionResult.Result, Is.TypeOf(typeof(OkObjectResult)));
        }

        [Test]
        public async Task GetTowns_WhenCalled_ReturnsTownsFromService()
        {
            var serviceTowns = new List<Town> {
                new Town { Name = "Goslar" },
                new Town { Name = "Oker" }
            };

            var townsService = NSubstitute.Substitute.For<ITownsService>();
            townsService.GetAllItemsAsync().Returns(Task.Run(() => serviceTowns));

            var streetsService = NSubstitute.Substitute.For<IStreetsService>();
            var categoriesService = NSubstitute.Substitute.For<ICategoriesService>();
            var eventsService = NSubstitute.Substitute.For<IEventsService>();
            var controller = new TownsController(townsService, streetsService, categoriesService, eventsService);

            var actionResult = await controller.GetTownsAsync();
            var value = (actionResult.Result as OkObjectResult).Value;
            Assert.That(value, Is.AssignableTo(typeof(IEnumerable<Town>)));
            
            var resultTowns = value as IEnumerable<Town>;
            Assert.That(resultTowns.Count, Is.EqualTo(2));
            Assert.That(resultTowns.First().Name, Is.EqualTo("Goslar"));
            Assert.That(resultTowns.Skip(1).First().Name, Is.EqualTo("Oker"));
        }
    }
}