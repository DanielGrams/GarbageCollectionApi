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
        public async Task<ActionResult<IEnumerable<Town>>> GetTownsAsync()
        {
            var towns = await this.townsService.GetAllItemsAsync().ConfigureAwait(false);
            return this.Ok(towns);
        }

        [HttpGet("{id}/streets")]
        public async Task<ActionResult<IEnumerable<Street>>> GetStreetsByTownAsync(string id)
        {
            return await this.streetsService.GetByTownAsync(id).ConfigureAwait(false);
        }

        [HttpGet("{id}/streets/{streetId}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesByTownAndStreetAsync(string id, string streetId)
        {
            return await this.categoriesService.GetByTownAndStreetAsync(id, streetId).ConfigureAwait(false);
        }

        [HttpGet("{id}/streets/{streetId}/events")]
        public async Task<ActionResult<IEnumerable<CollectionEvent>>> GetEventsByTownAndStreetAsync(string id, string streetId)
        {
            return await this.eventsService.GetByTownAndStreetAsync(id, streetId).ConfigureAwait(false);
        }
    }
}