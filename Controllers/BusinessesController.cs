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
    [Authorize(Policy = "Businesses")]
    public class BusinessesController(DatabaseContext context) : ControllerBase
    {
        // GET: api/Businesses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Business>>> GetBusinesses(bool includeUser = false)
        {
            var query = context.Businesses.AsQueryable(); // Start with the base query for businesses

            if (includeUser)
            {
                // Include the related User data if includeUser is true
                query = query.Include(b => b.User);
            }

            var businesses = await query.ToListAsync();

            return Ok(businesses);
        }

        // GET: api/Businesses/5?includeUser=true
        [HttpGet("{id}")]
        public async Task<ActionResult<Business>> GetBusiness(Guid id, bool includeUser = false)
        {
            // Start with the base query for the business
            var query = context.Businesses.AsQueryable();

            if (includeUser)
            {
                // Include the related User data if includeUser is true
                query = query.Include(b => b.User);
            }

            // Fetch the business by its ID
            var business = await query
                .FirstOrDefaultAsync(b => b.Id == id);

            if (business == null)
            {
                return NotFound();
            }

            return Ok(business);
        }

        // PUT: api/Businesses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBusiness(Guid id, Business business)
        {
            if (id != business.Id)
            {
                return BadRequest();
            }

            context.Entry(business).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BusinessExists(id))
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

        // POST: api/Businesses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Business>> PostBusiness(Business business)
        {
            context.Businesses.Add(business);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetBusiness", new { id = business.Id }, business);
        }

        // DELETE: api/Businesses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusiness(Guid id)
        {
            var business = await context.Businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound();
            }

            context.Businesses.Remove(business);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool BusinessExists(Guid id)
        {
            return context.Businesses.Any(e => e.Id == id);
        }
    }
}
