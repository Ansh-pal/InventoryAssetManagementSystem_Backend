using Microsoft.EntityFrameworkCore;

namespace InventoryAssetManagementSystem.Api.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<StockTransaction>()
                .HasOne(s => s.InventoryItem)
                .WithMany()
                .HasForeignKey(s => s.InventoryItemId);
        }
    }
}
