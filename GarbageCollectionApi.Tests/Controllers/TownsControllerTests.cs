using System;
using NUnit.Framework;
using GarbageCollectionApi.Models;
using NSubstitute;

namespace GarbageCollectionApi.Controllers
{
    [TestFixture]
    public class TownsControllerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor_AllParameters_CreatesInstance()
        {
            var service = NSubstitute.Substitute.For<ITownsService>();
            Assert.NotNull(new TownsController(service));
        }

        [Test]
        public void Constructor_NullParameters_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new TownsController(null));
        }
    }
}