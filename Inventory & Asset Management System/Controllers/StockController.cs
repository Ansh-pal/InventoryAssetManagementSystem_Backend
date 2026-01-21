using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryAssetManagementSystem.Api.Models;

namespace InventoryAssetManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("stock-in/{itemId}")]
        public async Task<ActionResult> StockIn(int itemId, [FromBody] StockInRequest request)
        {
            var item = await _context.InventoryItems.FindAsync(itemId);
            if (item == null) return NotFound("Item not found");

            // Add stock
            item.Quantity += request.Quantity;

            // Log transaction
            var transaction = new StockTransaction
            {
                InventoryItemId = itemId,
                Type = "In",
                Quantity = request.Quantity,
                Notes = request.Notes
            };
            _context.StockTransactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Stock added successfully",
                newQuantity = item.Quantity,
                isLowStock = item.Quantity < item.MinStockThreshold
            });
        }

        [HttpPost("stock-out/{itemId}")]
        public async Task<ActionResult> StockOut(int itemId, [FromBody] StockOutRequest request)
        {
            var item = await _context.InventoryItems.FindAsync(itemId);
            if (item == null) return NotFound("Item not found");

            if (item.Quantity < request.Quantity)
                return BadRequest($"Insufficient stock. Available: {item.Quantity}");

            // Remove stock
            item.Quantity -= request.Quantity;
            bool wasLowStock = item.Quantity < item.MinStockThreshold;

            // Log transaction
            var transaction = new StockTransaction
            {
                InventoryItemId = itemId,
                Type = "Out",
                Quantity = request.Quantity,
                Notes = request.Notes
            };
            _context.StockTransactions.Add(transaction);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Stock removed successfully",
                newQuantity = item.Quantity,
                lowStockAlert = wasLowStock ? "⚠️ Low stock alert!" : "Stock OK"
            });
        }

        [HttpGet("transactions/{itemId}")]
        public async Task<ActionResult<IEnumerable<StockTransaction>>> GetTransactions(int itemId)
        {
            return await _context.StockTransactions
                .Where(t => t.InventoryItemId == itemId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
    }

    // Request DTOs
    public class StockInRequest
    {
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }

    public class StockOutRequest
    {
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
