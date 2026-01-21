using System.ComponentModel.DataAnnotations;

namespace InventoryAssetManagementSystem.Api.Models
{
    public class InventoryItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int MinStockThreshold { get; set; }

        [Required]
        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
