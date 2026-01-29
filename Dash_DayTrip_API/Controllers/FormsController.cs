using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dash_DayTrip_API.Models;
using Dash_DayTrip_API.Data;

namespace Dash_DayTrip_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormsController : ControllerBase
    {
        private readonly ApiContext _context;

        public FormsController(ApiContext context)
        {
            _context = context;
        }
        
        // GET: api/Forms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Form>>> GetForms([FromQuery] string? status = null)
        {
            var query = _context.Forms
                .Include(f => f.FormSettings)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(f => f.Status == status);
            }
            
            return await query.OrderByDescending(f => f.CreatedAt).ToListAsync();
        }
        
        // GET: api/Forms/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Form>> GetForm(string id)
        {
            var form = await _context.Forms
                .Include(f => f.FormSettings)
                .Include(f => f.Packages)
                .FirstOrDefaultAsync(f => f.FormId == id);
            
            if (form == null)
            {
                return NotFound();
            }
            
            return form;
        }
        
        // POST: api/Forms
        [HttpPost]
        public async Task<ActionResult<Form>> CreateForm(Form form)
        {
            if (string.IsNullOrEmpty(form.FormId))
            {
                form.FormId = Guid.NewGuid().ToString();
            }
            
            form.CreatedAt = DateTime.UtcNow;
            form.UpdatedAt = DateTime.UtcNow;
            
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetForm), new { id = form.FormId }, form);
        }
        
        // PUT: api/Forms/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(string id, Form form)
        {
            if (id != form.FormId)
            {
                return BadRequest();
            }
            
            form.UpdatedAt = DateTime.UtcNow;
            _context.Entry(form).State = EntityState.Modified;
            
            // Don't update these fields
            _context.Entry(form).Property(f => f.CreatedAt).IsModified = false;
            _context.Entry(form).Property(f => f.SubmissionCount).IsModified = false;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            
            return NoContent();
        }
        
        // PATCH: api/Forms/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateFormStatus(string id, [FromBody] string status)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound();
            }
            
            form.Status = status;
            form.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return Ok(new { FormId = id, NewStatus = status });
        }
        
        // DELETE: api/Forms/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(string id)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound();
            }
            
            _context.Forms.Remove(form);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        
        private bool FormExists(string id)
        {
            return _context.Forms.Any(f => f.FormId == id);
        }
    }
}