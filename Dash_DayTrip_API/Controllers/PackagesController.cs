using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dash_DayTrip_API.Models;
using Dash_DayTrip_API.Data;

namespace Dash_DayTrip_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly ApiContext _context;

        public PackagesController(ApiContext context)
        {
            _context = context;
        }

        // GET: api/Packages or api/Packages?formId=xxx
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Package>>> GetPackages([FromQuery] string? formId)
        {
            var query = _context.Packages.AsQueryable();

            // Filter by formId if provided
            if (!string.IsNullOrEmpty(formId))
            {
                query = query.Where(p => p.FormId == formId);
            }

            return await query.OrderBy(p => p.CreatedAt).ToListAsync();
        }

        // GET: api/Packages/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Package>> GetPackage(string id)
        {
            var package = await _context.Packages.FindAsync(id);
            
            if (package == null)
            {
                return NotFound();
            }
            
            return package;
        }
        
        // GET: api/Packages/form/{formId}
        [HttpGet("form/{formId}")]
        public async Task<ActionResult<IEnumerable<Package>>> GetPackagesByForm(string formId)
        {
            return await _context.Packages
                .Where(p => p.FormId == formId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }
        
        // POST: api/Packages
        [HttpPost]
        public async Task<ActionResult<Package>> CreatePackage(Package package)
        {
            if (string.IsNullOrEmpty(package.PackageId))
            {
                package.PackageId = Guid.NewGuid().ToString();
            }
            
            package.CreatedAt = DateTime.UtcNow;
            package.UpdatedAt = DateTime.UtcNow;
            
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetPackage), new { id = package.PackageId }, package);
        }
        
        // PUT: api/Packages/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackage(string id, Package package)
        {
            if (id != package.PackageId)
            {
                return BadRequest();
            }
            
            package.UpdatedAt = DateTime.UtcNow;
            _context.Entry(package).State = EntityState.Modified;
            _context.Entry(package).Property(p => p.CreatedAt).IsModified = false;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackageExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            
            return NoContent();
        }
        
        // DELETE: api/Packages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(string id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            
            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        private bool PackageExists(string id)
        {
            return _context.Packages.Any(p => p.PackageId == id);
        }
    }
}