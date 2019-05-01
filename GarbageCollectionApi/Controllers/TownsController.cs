using System;
using System.Net.NetworkInformation;
using System.Net;
using GarbageCollectionApi.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCollectionApi.Controllers
{
    [Route("api/towns")]
    [ApiController]
    public class TownsController : ControllerBase
    {
        private readonly ITownsService _townsService;
        private readonly IStreetsService _streetsService;
        private readonly ICategoriesService _categoriesService;

        public TownsController(ITownsService townsService, IStreetsService streetsService, ICategoriesService categoriesService)
        {
            _townsService = townsService ?? throw new ArgumentNullException(nameof(townsService));
            _streetsService = streetsService ?? throw new ArgumentNullException(nameof(streetsService));
            _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Town>>> GetTownsAsync()
        {
            var towns = await _townsService.GetAllItemsAsync();
            return Ok(towns);
        }

        [HttpGet("{id}/streets")]
        public async Task<ActionResult<IEnumerable<Street>>> GetStreetsByTownAsync(string id)
        {
            return await _streetsService.GetByTownAsync(id);
        }

        [HttpGet("{id}/streets/{streetId}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoryByTownAndStreetAsync(string id, string streetId)
        {
            return await _categoriesService.GetByTownAndStreetAsync(id, streetId);
        }
    }
}