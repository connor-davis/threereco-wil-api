using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Authorize(Policy = "Products")]
    public class ProductsController(DatabaseContext context) : ControllerBase
    {
        // GET: api/Products?includeBusiness=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(bool includeBusiness = false)
        {
            var query = context.Products.AsQueryable(); // Start with the base query for products

            if (includeBusiness)
            {
                // Include the related Category data if includeCategory is true
                query = query.Include(p => p.Business); // Assuming the Product has a Category navigation property
            }

            // Step 1: Extract the email from the user's claims
            var Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (Email == null) return Unauthorized(); // If email is not found, return Unauthorized.

            // Step 2: Find the user by email in the database
            var UserFound = await context.Users
                .Where(u => u.Email == Email)
                .FirstOrDefaultAsync();

            if (UserFound == null) return Unauthorized(); // If the user is not found, return Unauthorized.

            // Step 3: Check if the user is a Business (by roles or any other criteria)
            // Assuming that UserFound has a Roles property, and "Business" is one of the roles.
            var isBusiness = UserFound.Roles.Contains(Roles.Business);

            if (isBusiness)
            {
                // Step 4: Find the user's business profile
                var businessProfile = await context.Businesses
                    .Where(b => b.UserId == UserFound.Id)
                    .FirstOrDefaultAsync();

                if (businessProfile == null) return Unauthorized(); // If the business profile does not exist, return Unauthorized.

                // Only return products for the user's business
                query = query.Where(p => p.BusinessId == businessProfile.Id); // Assuming Product has a BusinessId linking it to a business
            }

            // Step 5: Get the products associated with the user's business
            var products = await query.ToListAsync();

            return Ok(products); // Return the filtered products.
        }

        // GET: api/Products/5?includeBusiness=true
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id, bool includeBusiness = false)
        {
            var query = context.Products.AsQueryable();

            if (includeBusiness)
            {
                // Include the related Category data if includeCategory is true
                query = query.Include(p => p.Business); // Assuming the Product has a Category navigation property
            }

            var product = await query
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(Guid id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            context.Entry(product).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(Guid id)
        {
            return context.Products.Any(e => e.Id == id);
        }
    }
}
