using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dash_DayTrip_API.Models;
using Dash_DayTrip_API.Models.DTOs;
using Dash_DayTrip_API.Data;

namespace Dash_DayTrip_API.Controllers
{
    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

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
        public async Task<ActionResult<IEnumerable<FormDto>>> GetForms([FromQuery] string? status = null)
        {
            var query = _context.Forms
                .Include(f => f.FormSettings)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(f => f.Status == status);
            }
            
            var forms = await query.OrderByDescending(f => f.CreatedAt).ToListAsync();
            
            return Ok(forms.Select(ToDto));
        }
        
        // GET: api/Forms/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FormDto>> GetForm(string id)
        {
            var form = await _context.Forms
                .Include(f => f.FormSettings)
                .Include(f => f.Packages)
                .FirstOrDefaultAsync(f => f.FormId == id);
            
            if (form == null)
            {
                return NotFound();
            }
            
            return Ok(ToDto(form));
        }
        
        // POST: api/Forms
        [HttpPost]
        public async Task<ActionResult<FormDto>> CreateForm(Form form)
        {
            if (string.IsNullOrEmpty(form.FormId))
            {
                form.FormId = Guid.NewGuid().ToString();
            }
            
            form.CreatedAt = DateTime.UtcNow;
            form.UpdatedAt = DateTime.UtcNow;
            
            _context.Forms.Add(form);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetForm), new { id = form.FormId }, ToDto(form));
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
        public async Task<IActionResult> UpdateFormStatus(string id, [FromBody] UpdateStatusRequest request)
        {
            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound();
            }
            
            form.Status = request.Status;
            form.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            return Ok(new { FormId = id, NewStatus = request.Status });
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

        // Mapping helper method
        private static FormDto ToDto(Form form) => new()
        {
            FormId = form.FormId,
            Title = form.Title,
            Status = form.Status,
            IsDefault = form.IsDefault,
            SubmissionCount = form.SubmissionCount,
            LogoUrl = form.LogoUrl,
            LogoName = form.LogoName,
            BrandingSubtitle = form.BrandingSubtitle,
            BrandingDescription = form.BrandingDescription,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt,
            FormSettings = form.FormSettings == null ? null : new FormSettingsDto
            {
                SettingId = form.FormSettings.SettingId,
                FormId = form.FormSettings.FormId,
                SalesExecutives = form.FormSettings.SalesExecutives,
                TaxIdNumber = form.FormSettings.TaxIdNumber,
                Currency = form.FormSettings.Currency,
                NextDayCutoffTime = form.FormSettings.NextDayCutoffTime,
                MaxGuestPerDay = form.FormSettings.MaxGuestPerDay,
                DepositMode = form.FormSettings.DepositMode,
                DepositAmount = form.FormSettings.DepositAmount,
                SSTEnabled = form.FormSettings.SSTEnabled,
                SSTPercentage = form.FormSettings.SSTPercentage,
                CreatedAt = form.FormSettings.CreatedAt,
                UpdatedAt = form.FormSettings.UpdatedAt
            }
        };
    }
}