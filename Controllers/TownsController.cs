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

        [HttpGet("{id}")]
        public async Task<ActionResult<Town>> GetTown(long id)
        {
            var Town = await _context.Towns.FindAsync(id);

            if (Town == null)
            {
                return NotFound();
            }

            return Town;
        }

        /// <summary>
        /// Creates a town.
        /// </summary>
        /// <param name="town">Town</param>
        /// <returns>A newly created town</returns>
        [HttpPost]
        [Authorize("api.write")]
        public async Task<ActionResult<Town>> PostTown(Town town)
        {
            _context.Towns.Add(town);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTown), new { id = town.Id }, town);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTown(long id, Town town)
        {
            if (id != town.Id)
            {
                return BadRequest();
            }

            _context.Entry(town).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific Town.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTown(long id)
        {
            var Town = await _context.Towns.FindAsync(id);

            if (Town == null)
            {
                return NotFound();
            }

            _context.Towns.Remove(Town);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}