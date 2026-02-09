using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dash_DayTrip_API.Models;
using Dash_DayTrip_API.Models.Responses;
using Dash_DayTrip_API.Data;

namespace Dash_DayTrip_API.Controllers
{
    // DTO for status updates
    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ApiContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderPackages)
                .ToListAsync();
        }

        // GET: api/Orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderPackages)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            order.OrderId = Guid.NewGuid().ToString();
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        // PUT: api/Orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, [FromBody] Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest("ID mismatch");
            }

            // 1. Fetch the EXISTING order from database (with packages)
            var existingOrder = await _context.Orders
                .Include(o => o.OrderPackages)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            // 2. Update parent order properties
            _context.Entry(existingOrder).CurrentValues.SetValues(order);
            existingOrder.UpdatedAt = DateTime.UtcNow;

            // 3. Handle child packages ONLY if provided
            if (order.OrderPackages != null)
            {
                existingOrder.OrderPackages ??= new List<OrderPackage>();

                var newPackageIds = order.OrderPackages.Select(p => p.OrderPackageId).ToList();

                // A. DELETE packages that were removed
                var packagesToDelete = existingOrder.OrderPackages
                    .Where(p => !newPackageIds.Contains(p.OrderPackageId) && p.OrderPackageId != 0)
                    .ToList();

                if (packagesToDelete.Any())
                {
                    _context.OrderPackages.RemoveRange(packagesToDelete);
                }

                // B. ADD or UPDATE packages
                foreach (var package in order.OrderPackages)
                {
                    var existingPackage = existingOrder.OrderPackages
                        .FirstOrDefault(p => p.OrderPackageId == package.OrderPackageId && p.OrderPackageId != 0);

                    if (existingPackage != null)
                    {
                        // UPDATE existing package
                        _context.Entry(existingPackage).CurrentValues.SetValues(package);
                    }
                    else
                    {
                        // INSERT new package
                        package.OrderPackageId = 0; // Let DB generate new ID
                        existingOrder.OrderPackages.Add(package);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Orders/statistics
        [HttpGet("statistics")]
        public async Task<ActionResult<OrderStatistics>> GetStatistics(
            [FromQuery] string? formId = null,
            [FromQuery] string? merchantId = null)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(formId))
            {
                query = query.Where(o => o.FormId == formId);
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                query = query.Where(o => o.MerchantId == merchantId);
            }

            var today = DateTime.Today;

            var stats = new OrderStatistics
            {
                TotalOrders = await query.CountAsync(),
                TotalRevenue = await query.SumAsync(o => o.GrandTotal),
                TotalDeposits = await query.SumAsync(o => o.DepositPaid),
                OutstandingBalance = await query.SumAsync(o => o.BalanceDue),
                TodayOrders = await query.CountAsync(o => o.CreatedAt.Date == today),
                TodayRevenue = await query.Where(o => o.CreatedAt.Date == today).SumAsync(o => o.GrandTotal),
                PendingCount = await query.CountAsync(o => o.Status == "pending"),
                ConfirmedCount = await query.CountAsync(o => o.Status == "confirmed"),
                CompletedCount = await query.CountAsync(o => o.Status == "completed"),
                CancelledCount = await query.CountAsync(o => o.Status == "cancelled")
            };

            return stats;
        }

        // GET: api/Orders/form/{formId}
        [HttpGet("form/{formId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByForm(string formId)
        {
            return await _context.Orders
                .Where(o => o.FormId == formId)
                .Include(o => o.OrderPackages)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // PATCH: api/Orders/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = request.Status;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { OrderId = id, NewStatus = request.Status });
        }

        private bool OrderExists(string id)
        {
            return _context.Orders.Any(o => o.OrderId == id);
        }
    }
}
