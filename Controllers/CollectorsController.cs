using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using three_api.Lib.Database;
using three_api.Lib.Models;

namespace three_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "Collectors")]
    public class CollectorsController(DatabaseContext context) : ControllerBase
    {
        // GET: api/Collectors?includeUser=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Collector>>> GetCollectors(bool includeUser = false)
        {
            var query = context.Collectors.AsQueryable(); // Start with the base query for collectors

            if (includeUser)
            {
                // Include the related User data if includeUser is true
                query = query.Include(c => c.User); // Assuming the Collector has a User navigation property
            }

            var collectors = await query.ToListAsync();

            return Ok(collectors);
        }

        // GET: api/Collectors/5?includeUser=true
        [HttpGet("{id}")]
        public async Task<ActionResult<Collector>> GetCollector(Guid id, bool includeUser = false)
        {
            var query = context.Collectors.AsQueryable();

            if (includeUser)
            {
                // Include the related User data if includeUser is true
                query = query.Include(c => c.User); // Assuming the Collector has a User navigation property
            }

            var collector = await query
                .FirstOrDefaultAsync(c => c.Id == id);

            if (collector == null)
            {
                return NotFound();
            }

            return Ok(collector);
        }

        // PUT: api/Collectors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollector(Guid id, Collector collector)
        {
            if (id != collector.Id)
            {
                return BadRequest();
            }

            context.Entry(collector).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Collectors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Collector>> PostCollector(Collector collector)
        {
            context.Collectors.Add(collector);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetCollector", new { id = collector.Id }, collector);
        }

        // DELETE: api/Collectors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollector(Guid id)
        {
            var collector = await context.Collectors.FindAsync(id);
            if (collector == null)
            {
                return NotFound();
            }

            context.Collectors.Remove(collector);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool CollectorExists(Guid id)
        {
            return context.Collectors.Any(e => e.Id == id);
        }
    }
}
