namespace GarbageCollectionApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;
    using GarbageCollectionApi.DataContracts;
    using GarbageCollectionApi.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Town>>> GetTownsAsync()
        {
            var towns = await this.townsService.GetAllItemsAsync().ConfigureAwait(false);
            return this.Ok(towns);
        }

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

        [HttpGet("{id}/streets/{streetId}/events")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<CollectionEvent>>> GetEventsByTownAndStreetAsync(string id, string streetId)
        {
            var events = await this.eventsService.GetByTownAndStreetAsync(id, streetId).ConfigureAwait(false);

            if (events == null)
            {
                return this.NotFound();
            }

            return this.Ok(events);
        }
    }
}