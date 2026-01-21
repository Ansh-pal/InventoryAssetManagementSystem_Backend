using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAssetManagementSystem.Api.Models
{
    public class StockTransaction
    {
        [Key]
        public int Id { get; set; }

        public int InventoryItemId { get; set; }

        [ForeignKey("InventoryItemId")]
        public InventoryItem InventoryItem { get; set; } = null!;

        [Required]
        public string Type { get; set; } = "In"; // "In" or "Out"

        [Required]
        public int Quantity { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }
    }
}
