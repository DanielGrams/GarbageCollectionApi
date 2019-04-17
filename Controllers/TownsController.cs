using System.Net.NetworkInformation;
using System.Net;
using GarbageCollectionApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCollectionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TownsController : ControllerBase
    {
        private readonly GarbageCollectionContext _context;

        public TownsController(GarbageCollectionContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Town>>> GetTowns()
        {
            return await _context.Towns.ToListAsync();
        }

        [HttpGet("{id}/streets")]
        public async Task<ActionResult<IEnumerable<Street>>> GetStreetsForTown(int id)
        {
            return await _context.Streets.Where(s => s.TownId == id).ToListAsync();
        }
    }
}