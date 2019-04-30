using System.Net.NetworkInformation;
using System.Net;
using GarbageCollectionApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCollectionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreetsController : ControllerBase
    {
        private readonly GarbageCollectionContext _context;

        public StreetsController(GarbageCollectionContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesForStreet(int id)
        {
            var street = await _context.Streets.FindAsync(id);
            
            if (street == null)
            {
                return NotFound();
            }
            
            return street.Categories.ToList();
        }
    }
}