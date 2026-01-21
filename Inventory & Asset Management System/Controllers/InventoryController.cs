using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryAssetManagementSystem.Api.Models;

namespace InventoryAssetManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryItems()
        {
            var items = await _context.InventoryItems
                .OrderBy(i => i.Name)
                .ToListAsync();
            return Ok(items);
        }

        // GET: api/Inventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST: api/Inventory
        [HttpPost]
        public async Task<ActionResult<InventoryItem>> CreateInventoryItem(InventoryItem item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();

            var createdItem = await _context.InventoryItems.FindAsync(item.Id);
            return CreatedAtAction(nameof(GetInventoryItem), new { id = createdItem.Id }, createdItem);
        }

        // PUT: api/Inventory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventoryItem(int id, InventoryItem item)
        {
            var existingItem = await _context.InventoryItems.FindAsync(id);
            if (existingItem == null) return NotFound();

            // Update only the fields that should be modified
            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.Quantity = item.Quantity;
            existingItem.MinStockThreshold = item.MinStockThreshold;
            existingItem.Price = item.Price;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Inventory/low-stock
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetLowStockItems()
        {
            var lowStockItems = await _context.InventoryItems
                .Where(i => i.Quantity < i.MinStockThreshold)
                .OrderBy(i => i.Quantity)
                .ToListAsync();
            return Ok(lowStockItems);
        }
    }
}
