using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
    [Authorize(Policy = "Collections")]
    public class CollectionsController(DatabaseContext context) : ControllerBase
    {
        // GET: api/Collections?includeBusiness=true&includeCollector=true&includeProduct=true
        [HttpGet("Export")]
        [Authorize(Policy = "Collections Export")]
        public async Task<ActionResult> ExportCollections(bool includeBusiness = false, bool includeCollector = false, bool includeProduct = false)
        {
            // Step 1: Start with the base query for collections
            var query = context.Collections.AsQueryable();

            // Step 2: Conditionally include related entities based on query parameters
            if (includeBusiness)
            {
                query = query.Include(c => c.Business); // Assuming Collection has a Business navigation property
            }

            if (includeCollector)
            {
                query = query.Include(c => c.Collector); // Assuming Collection has a Collector navigation property
            }

            if (includeProduct)
            {
                query = query.Include(c => c.Product); // Assuming Collection has a Product navigation property
            }

            // Step 3: Get all collections (no role checks or user filtering)
            var collections = await query.ToListAsync();

            // Step 4: Convert the collection data to CSV format
            var csv = GenerateCsv(collections);

            // Step 5: Return the CSV string as a response
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "collections.csv");
        }

        private static string GenerateCsv(IEnumerable<Collection> collections)
        {
            var csv = new StringBuilder();

            // Add headers to the CSV (adjust fields based on the related entities)
            csv.AppendLine("CollectionId,CollectionWeight,BusinessId,BusinessUserId,BusinessName,BusinessDescription,BusinessPhoneNumber,BusinessAddress,BusinessCity,BusinessProvince,BusinessZipCode,BusinessType,CollectorId,CollectorUserId,CollectorFirstName,CollectorLastName,CollectorIdNumber,CollectorPhoneNumber,CollectorAddress,CollectorCity,CollectorProvince,CollectorZipCode,CollectorBankName,CollectorBankAccountHolder,CollectorBankAccountNumber,ProductId,ProductBusinessId,ProductName,ProductPrice,ProductGwCode,ProductCarbonFactor");

            // Add each collection as a row in the CSV
            foreach (var collection in collections)
            {
                // Ensure null-safe handling for related entities
                var business = collection.Business;
                var collector = collection.Collector;
                var product = collection.Product;

                // Handle Business properties (null-safe)
                var businessId = business?.Id.ToString() ?? "N/A";
                var businessUserId = business?.UserId.ToString() ?? "N/A";
                var businessName = business?.Name ?? "N/A";
                var businessDescription = business?.Description ?? "N/A";
                var businessPhoneNumber = business?.PhoneNumber ?? "N/A";
                var businessAddress = business?.Address ?? "N/A";
                var businessCity = business?.City ?? "N/A";
                var businessProvince = business?.Province ?? "N/A";
                var businessZipCode = business?.ZipCode ?? "N/A";
                var businessType = business?.BusinessType.ToString() ?? "N/A"; // Enum to string

                // Handle Collector properties (null-safe)
                var collectorId = collector?.Id.ToString() ?? "N/A";
                var collectorUserId = collector?.UserId.ToString() ?? "N/A";
                var collectorFirstName = collector?.FirstName ?? "N/A";
                var collectorLastName = collector?.LastName ?? "N/A";
                var collectorIdNumber = collector?.IdNumber ?? "N/A";
                var collectorPhoneNumber = collector?.PhoneNumber ?? "N/A";
                var collectorAddress = collector?.Address ?? "N/A";
                var collectorCity = collector?.City ?? "N/A";
                var collectorProvince = collector?.Province ?? "N/A";
                var collectorZipCode = collector?.ZipCode ?? "N/A";
                var collectorBankName = collector?.BankName ?? "N/A";
                var collectorBankAccountHolder = collector?.BankAccountHolder ?? "N/A";
                var collectorBankAccountNumber = collector?.BankAccountNumber ?? "N/A";

                // Handle Product properties (null-safe)
                var productId = product?.Id.ToString() ?? "N/A";
                var productBusinessId = product?.BusinessId.ToString() ?? "N/A";
                var productName = product?.Name ?? "N/A";
                var productPrice = product?.Price ?? "N/A";
                var productGwCode = product?.GwCode ?? "N/A";
                var productCarbonFactor = product?.CarbonFactor ?? "N/A";

                // Generate the CSV row for this collection
                csv.AppendLine($"{collection.Id},{collection.Weight},{businessId},{businessUserId},{businessName},{businessDescription},{businessPhoneNumber},{businessAddress},{businessCity},{businessProvince},{businessZipCode},{businessType},{collectorId},{collectorUserId},{collectorFirstName},{collectorLastName},{collectorIdNumber},{collectorPhoneNumber},{collectorAddress},{collectorCity},{collectorProvince},{collectorZipCode},{collectorBankName},{collectorBankAccountHolder},{collectorBankAccountNumber},{productId},{productBusinessId},{productName},{productPrice},{productGwCode},{productCarbonFactor}");
            }

            return csv.ToString();
        }

        // GET: api/Collections?includeBusiness=true&includeCollector=true&includeProduct=true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Collection>>> GetCollections(bool includeBusiness = false, bool includeCollector = false, bool includeProduct = false)
        {
            var query = context.Collections.AsQueryable(); // Start with the base query for collections

            if (includeBusiness)
            {
                // Include related User data if includeUser is true
                query = query.Include(c => c.Business); // Assuming Collection has a User navigation property
            }

            if (includeCollector)
            {
                // Include related Items data if includeItems is true
                query = query.Include(c => c.Collector); // Assuming Collection has a collection of Items
            }

            if (includeProduct)
            {
                // Include related Items data if includeItems is true
                query = query.Include(c => c.Product); // Assuming Collection has a collection of Items
            }

            // Step 1: Extract the email from the user's claims
            var Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (Email == null) return Unauthorized(); // If email is not found, return Unauthorized.

            // Step 2: Find the user by email in the database
            var UserFound = await context.Users
                .Where(u => u.Email == Email)
                .FirstOrDefaultAsync();

            if (UserFound == null) return Unauthorized(); // If the user is not found, return Unauthorized.

            // Step 3: Check if the user is a Business or Collector (by roles or any other criteria)
            // Assuming that UserFound has a Roles property, and "Business" or "Collector" is one of the roles.
            var isBusiness = UserFound.Roles.Contains(Roles.Business);
            var isCollector = UserFound.Roles.Contains(Roles.Collector);

            if (isBusiness)
            {
                // Step 4: Find the user's business profile
                var businessProfile = await context.Businesses
                    .Where(b => b.UserId == UserFound.Id)
                    .FirstOrDefaultAsync();

                if (businessProfile == null) return Unauthorized(); // If the business profile does not exist, return Unauthorized.

                // Only return collection for the user's business
                query = query.Where(c => c.BusinessId == businessProfile.Id); // Assuming Collection has a BusinessId linking it to a business
            }

            if (isCollector)
            {
                // Step 4: Find the user's collector profile
                var collectorProfile = await context.Collectors
                    .Where(b => b.UserId == UserFound.Id)
                    .FirstOrDefaultAsync();

                if (collectorProfile == null) return Unauthorized(); // If the collector profile does not exist, return Unauthorized.

                // Only return collection for the user's collector
                query = query.Where(c => c.CollectorId == collectorProfile.Id); // Assuming Collection has a CollectorId linking it to a business
            }

            // Step 5: Get the collections associated with the user's business
            var collections = await query.ToListAsync();

            return Ok(collections);
        }

        // GET: api/Collections/5?includeBusiness=true&includeCollector=true&includeProduct=true
        [HttpGet("{id}")]
        public async Task<ActionResult<Collection>> GetCollection(Guid id, bool includeBusiness = false, bool includeCollector = false, bool includeProduct = false)
        {
            var query = context.Collections.AsQueryable();

            if (includeBusiness)
            {
                // Include related User data if includeUser is true
                query = query.Include(c => c.Business); // Assuming Collection has a User navigation property
            }

            if (includeCollector)
            {
                // Include related Items data if includeItems is true
                query = query.Include(c => c.Collector); // Assuming Collection has a collection of Items
            }

            if (includeProduct)
            {
                // Include related Items data if includeItems is true
                query = query.Include(c => c.Product); // Assuming Collection has a collection of Items
            }

            var collection = await query
                .FirstOrDefaultAsync(c => c.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            return Ok(collection);
        }

        // PUT: api/Collections/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCollection(Guid id, Collection collection)
        {
            if (id != collection.Id)
            {
                return BadRequest();
            }

            context.Entry(collection).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CollectionExists(id))
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

        // POST: api/Collections
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Collection>> PostCollection(Collection collection)
        {
            context.Collections.Add(collection);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetCollection", new { id = collection.Id }, collection);
        }

        // DELETE: api/Collections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollection(Guid id)
        {
            var collection = await context.Collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            context.Collections.Remove(collection);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool CollectionExists(Guid id)
        {
            return context.Collections.Any(e => e.Id == id);
        }
    }
}
