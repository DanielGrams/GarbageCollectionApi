namespace GarbageCollectionApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using GarbageCollectionApi.Examples;
    using GarbageCollectionApi.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Swashbuckle.AspNetCore.Filters;

    /// <summary>
    /// Controller for towns and all its contained types
    /// </summary>
    [Route("api/towns")]
    [ApiController]
    public class TownsController : ControllerBase
    {
        private readonly ITownsService townsService;
        private readonly IStreetsService streetsService;
        private readonly ICategoriesService categoriesService;
        private readonly IEventsService eventsService;

        public TownsController(ITownsService townsService, IStreetsService streetsService, ICategoriesService categoriesService, IEventsService eventService)
        {
             this.townsService = townsService ?? throw new ArgumentNullException(nameof(townsService));
             this.streetsService = streetsService ?? throw new ArgumentNullException(nameof(streetsService));
             this.categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
             this.eventsService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        /// <summary>
        /// Lists all towns
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Town>>> GetTownsAsync()
        {
            var towns = await this.townsService.GetAllItemsAsync().ConfigureAwait(false);
            return this.Ok(towns);
        }

        /// <summary>
        /// Lists all streets in town with given <paramref name="id" />
        /// </summary>
        /// <param name="id">Town id</param>
        /// <response code="404">If town with given <paramref name="id" /> does not exist</response>
        [HttpGet("{id}/streets")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Street>>> GetStreetsByTownAsync(string id)
        {
            var streets = await this.streetsService.GetByTownAsync(id).ConfigureAwait(false);

            if (streets == null)
            {
                return this.NotFound();
            }

            return this.Ok(streets);
        }

        /// <summary>
        /// Lists all categories for street with given <paramref name="streetId" /> in town with given <paramref name="id" />
        /// </summary>
        /// <param name="id">Town id</param>
        /// <param name="streetId">Street id</param>
        /// <response code="404">If town with given <paramref name="id" /> or street with given <paramref name="streetId" /> does not exist</response>
        [HttpGet("{id}/streets/{streetId}/categories")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesByTownAndStreetAsync(string id, string streetId)
        {
            var categories = await this.categoriesService.GetByTownAndStreetAsync(id, streetId).ConfigureAwait(false);

            if (categories == null)
            {
                return this.NotFound();
            }

            return this.Ok(categories);
        }

        /// <summary>
        /// Lists all events for street with given <paramref name="streetId" /> in town with given <paramref name="id" />
        /// </summary>
        /// <param name="id">Town id</param>
        /// <param name="streetId">Street id</param>
        /// <param name="categoryIds">Optional: If supplied, events are filtered by given category ids</param>
        /// <response code="404">If town with given <paramref name="id" /> or street with given <paramref name="streetId" /> does not exist</response>
        [HttpGet("{id}/streets/{streetId}/events")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<CollectionEvent>>> GetEventsByTownAndStreetAsync(string id, string streetId, [FromQuery] List<string> categoryIds = null)
        {
            if (categoryIds != null)
            {
                Console.WriteLine(categoryIds);
            }

            var events = await this.eventsService.GetByTownAndStreetAsync(id, streetId, categoryIds).ConfigureAwait(false);

            if (events == null)
            {
                return this.NotFound();
            }

            return this.Ok(events);
        }
    }
}