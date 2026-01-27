using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryAssetManagementSystem.Api.Controllers;
using InventoryAssetManagementSystem.Api.Models;

namespace InventoryAssetManagementSystem.Tests;

public class InventoryControllerTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetInventoryItems_ReturnsAllItems()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.InventoryItems.AddRange(
            new InventoryItem { Id = 1, Name = "Item 1", Quantity = 10, MinStockThreshold = 5, Price = 100 },
            new InventoryItem { Id = 2, Name = "Item 2", Quantity = 20, MinStockThreshold = 10, Price = 200 }
        );
        await context.SaveChangesAsync();

        var controller = new InventoryController(context);

        // Act
        var result = await controller.GetInventoryItems();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IEnumerable<InventoryItem>>(okResult.Value);
        Assert.Equal(2, items.Count());
    }

    [Fact]
    public async Task GetInventoryItem_ExistingId_ReturnsItem()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem { Id = 1, Name = "Test Item", Quantity = 10, MinStockThreshold = 5, Price = 100 };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new InventoryController(context);

        // Act
        var result = await controller.GetInventoryItem(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItem = Assert.IsType<InventoryItem>(okResult.Value);
        Assert.Equal("Test Item", returnedItem.Name);
    }

    [Fact]
    public async Task GetInventoryItem_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new InventoryController(context);

        // Act
        var result = await controller.GetInventoryItem(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateInventoryItem_ValidItem_ReturnsCreatedItem()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new InventoryController(context);
        var newItem = new InventoryItem 
        { 
            Name = "New Item", 
            Description = "Test Description",
            Quantity = 15, 
            MinStockThreshold = 5, 
            Price = 150 
        };

        // Act
        var result = await controller.CreateInventoryItem(newItem);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdItem = Assert.IsType<InventoryItem>(createdResult.Value);
        Assert.Equal("New Item", createdItem.Name);
        Assert.Equal(15, createdItem.Quantity);
        Assert.True(createdItem.Id > 0);
    }

    [Fact]
    public async Task UpdateInventoryItem_ExistingItem_ReturnsNoContent()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var existingItem = new InventoryItem 
        { 
            Id = 1, 
            Name = "Original Name", 
            Quantity = 10, 
            MinStockThreshold = 5, 
            Price = 100 
        };
        context.InventoryItems.Add(existingItem);
        await context.SaveChangesAsync();

        var controller = new InventoryController(context);
        var updatedItem = new InventoryItem 
        { 
            Id = 1, 
            Name = "Updated Name", 
            Quantity = 20, 
            MinStockThreshold = 10, 
            Price = 200 
        };

        // Act
        var result = await controller.UpdateInventoryItem(1, updatedItem);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var itemInDb = await context.InventoryItems.FindAsync(1);
        Assert.NotNull(itemInDb);
        Assert.Equal("Updated Name", itemInDb.Name);
        Assert.Equal(20, itemInDb.Quantity);
    }

    [Fact]
    public async Task UpdateInventoryItem_NonExistingItem_ReturnsNotFound()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new InventoryController(context);
        var updatedItem = new InventoryItem 
        { 
            Id = 999, 
            Name = "Updated Name", 
            Quantity = 20, 
            MinStockThreshold = 10, 
            Price = 200 
        };

        // Act
        var result = await controller.UpdateInventoryItem(999, updatedItem);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteInventoryItem_ExistingItem_ReturnsNoContent()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem { Id = 1, Name = "To Delete", Quantity = 10, MinStockThreshold = 5, Price = 100 };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new InventoryController(context);

        // Act
        var result = await controller.DeleteInventoryItem(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var deletedItem = await context.InventoryItems.FindAsync(1);
        Assert.Null(deletedItem);
    }

    [Fact]
    public async Task DeleteInventoryItem_NonExistingItem_ReturnsNotFound()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new InventoryController(context);

        // Act
        var result = await controller.DeleteInventoryItem(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetLowStockItems_ReturnsItemsBelowThreshold()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.InventoryItems.AddRange(
            new InventoryItem { Id = 1, Name = "Low Stock Item", Quantity = 3, MinStockThreshold = 5, Price = 100 },
            new InventoryItem { Id = 2, Name = "Normal Stock Item", Quantity = 20, MinStockThreshold = 10, Price = 200 },
            new InventoryItem { Id = 3, Name = "Another Low Stock", Quantity = 2, MinStockThreshold = 5, Price = 150 }
        );
        await context.SaveChangesAsync();

        var controller = new InventoryController(context);

        // Act
        var result = await controller.GetLowStockItems();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IEnumerable<InventoryItem>>(okResult.Value);
        Assert.Equal(2, items.Count());
        Assert.All(items, item => Assert.True(item.Quantity < item.MinStockThreshold));
    }

    [Fact]
    public async Task GetLowStockItems_NoLowStock_ReturnsEmptyList()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.InventoryItems.AddRange(
            new InventoryItem { Id = 1, Name = "Item 1", Quantity = 10, MinStockThreshold = 5, Price = 100 },
            new InventoryItem { Id = 2, Name = "Item 2", Quantity = 20, MinStockThreshold = 10, Price = 200 }
        );
        await context.SaveChangesAsync();

        var controller = new InventoryController(context);

        // Act
        var result = await controller.GetLowStockItems();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IEnumerable<InventoryItem>>(okResult.Value);
        Assert.Empty(items);
    }
}
