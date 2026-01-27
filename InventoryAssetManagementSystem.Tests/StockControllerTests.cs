using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryAssetManagementSystem.Api.Controllers;
using InventoryAssetManagementSystem.Api.Models;
using System.Reflection;

namespace InventoryAssetManagementSystem.Tests;

public class StockControllerTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task StockIn_ValidRequest_AddsStockAndCreatesTransaction()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Test Item", 
            Quantity = 10, 
            MinStockThreshold = 5, 
            Price = 100 
        };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new StockController(context);
        var request = new StockInRequest { Quantity = 5, Notes = "Adding stock" };

        // Act
        var result = await controller.StockIn(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var updatedItem = await context.InventoryItems.FindAsync(1);
        Assert.NotNull(updatedItem);
        Assert.Equal(15, updatedItem.Quantity);
        
        var transaction = await context.StockTransactions.FirstOrDefaultAsync();
        Assert.NotNull(transaction);
        Assert.Equal("In", transaction.Type);
        Assert.Equal(5, transaction.Quantity);
        Assert.Equal("Adding stock", transaction.Notes);
    }

    [Fact]
    public async Task StockIn_NonExistingItem_ReturnsNotFound()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new StockController(context);
        var request = new StockInRequest { Quantity = 5, Notes = "Adding stock" };

        // Act
        var result = await controller.StockIn(999, request);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task StockIn_ItemBelowThreshold_ReturnsLowStockAlert()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Test Item", 
            Quantity = 2, 
            MinStockThreshold = 10, 
            Price = 100 
        };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new StockController(context);
        var request = new StockInRequest { Quantity = 3, Notes = "Adding stock" };

        // Act
        var result = await controller.StockIn(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        var valueType = okResult.Value.GetType();
        var newQuantityProp = valueType.GetProperty("newQuantity");
        var isLowStockProp = valueType.GetProperty("isLowStock");
        
        Assert.NotNull(newQuantityProp);
        Assert.NotNull(isLowStockProp);
        
        var newQuantity = (int)newQuantityProp.GetValue(okResult.Value)!;
        var isLowStock = (bool)isLowStockProp.GetValue(okResult.Value)!;
        
        Assert.Equal(5, newQuantity);
        Assert.True(isLowStock);
    }

    [Fact]
    public async Task StockOut_ValidRequest_RemovesStockAndCreatesTransaction()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Test Item", 
            Quantity = 20, 
            MinStockThreshold = 5, 
            Price = 100 
        };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new StockController(context);
        var request = new StockOutRequest { Quantity = 5, Notes = "Removing stock" };

        // Act
        var result = await controller.StockOut(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var updatedItem = await context.InventoryItems.FindAsync(1);
        Assert.NotNull(updatedItem);
        Assert.Equal(15, updatedItem.Quantity);
        
        var transaction = await context.StockTransactions.FirstOrDefaultAsync();
        Assert.NotNull(transaction);
        Assert.Equal("Out", transaction.Type);
        Assert.Equal(5, transaction.Quantity);
        Assert.Equal("Removing stock", transaction.Notes);
    }

    [Fact]
    public async Task StockOut_InsufficientStock_ReturnsBadRequest()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Test Item", 
            Quantity = 5, 
            MinStockThreshold = 2, 
            Price = 100 
        };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new StockController(context);
        var request = new StockOutRequest { Quantity = 10, Notes = "Removing stock" };

        // Act
        var result = await controller.StockOut(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
        Assert.Contains("Insufficient stock", badRequestResult.Value.ToString()!);
    }

    [Fact]
    public async Task StockOut_NonExistingItem_ReturnsNotFound()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new StockController(context);
        var request = new StockOutRequest { Quantity = 5, Notes = "Removing stock" };

        // Act
        var result = await controller.StockOut(999, request);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task StockOut_ResultsInLowStock_ReturnsLowStockAlert()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Test Item", 
            Quantity = 10, 
            MinStockThreshold = 5, 
            Price = 100 
        };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new StockController(context);
        var request = new StockOutRequest { Quantity = 8, Notes = "Removing stock" };

        // Act
        var result = await controller.StockOut(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        var valueType = okResult.Value.GetType();
        var newQuantityProp = valueType.GetProperty("newQuantity");
        var lowStockAlertProp = valueType.GetProperty("lowStockAlert");
        
        Assert.NotNull(newQuantityProp);
        Assert.NotNull(lowStockAlertProp);
        
        var newQuantity = (int)newQuantityProp.GetValue(okResult.Value)!;
        var lowStockAlert = (string)lowStockAlertProp.GetValue(okResult.Value)!;
        
        Assert.Equal(2, newQuantity);
        Assert.Contains("Low stock alert", lowStockAlert);
    }

    [Fact]
    public async Task GetTransactions_ExistingItem_ReturnsTransactions()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Test Item", 
            Quantity = 10, 
            MinStockThreshold = 5, 
            Price = 100 
        };
        context.InventoryItems.Add(item);
        
        context.StockTransactions.AddRange(
            new StockTransaction { Id = 1, InventoryItemId = 1, Type = "In", Quantity = 10, Notes = "Initial stock" },
            new StockTransaction { Id = 2, InventoryItemId = 1, Type = "Out", Quantity = 5, Notes = "Sale" },
            new StockTransaction { Id = 3, InventoryItemId = 1, Type = "In", Quantity = 5, Notes = "Restock" }
        );
        await context.SaveChangesAsync();

        var controller = new StockController(context);

        // Act
        var result = await controller.GetTransactions(1);

        // Assert
        var transactions = Assert.IsAssignableFrom<IEnumerable<StockTransaction>>(result.Value);
        Assert.Equal(3, transactions.Count());
    }

    [Fact]
    public async Task GetTransactions_NoTransactions_ReturnsEmptyList()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Test Item", 
            Quantity = 10, 
            MinStockThreshold = 5, 
            Price = 100 
        };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new StockController(context);

        // Act
        var result = await controller.GetTransactions(1);

        // Assert
        var transactions = Assert.IsAssignableFrom<IEnumerable<StockTransaction>>(result.Value);
        Assert.Empty(transactions);
    }

    [Fact]
    public async Task GetTransactions_MultipleItems_ReturnsOnlySpecificItemTransactions()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item1 = new InventoryItem { Id = 1, Name = "Item 1", Quantity = 10, MinStockThreshold = 5, Price = 100 };
        var item2 = new InventoryItem { Id = 2, Name = "Item 2", Quantity = 20, MinStockThreshold = 10, Price = 200 };
        context.InventoryItems.AddRange(item1, item2);
        
        context.StockTransactions.AddRange(
            new StockTransaction { Id = 1, InventoryItemId = 1, Type = "In", Quantity = 10 },
            new StockTransaction { Id = 2, InventoryItemId = 2, Type = "In", Quantity = 20 },
            new StockTransaction { Id = 3, InventoryItemId = 1, Type = "Out", Quantity = 5 }
        );
        await context.SaveChangesAsync();

        var controller = new StockController(context);

        // Act
        var result = await controller.GetTransactions(1);

        // Assert
        var transactions = Assert.IsAssignableFrom<IEnumerable<StockTransaction>>(result.Value);
        Assert.Equal(2, transactions.Count());
        Assert.All(transactions, t => Assert.Equal(1, t.InventoryItemId));
    }

    [Fact]
    public async Task StockIn_WithNullNotes_CreatesTransactionSuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Test Item", 
            Quantity = 10, 
            MinStockThreshold = 5, 
            Price = 100 
        };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new StockController(context);
        var request = new StockInRequest { Quantity = 5, Notes = null };

        // Act
        var result = await controller.StockIn(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var transaction = await context.StockTransactions.FirstOrDefaultAsync();
        Assert.NotNull(transaction);
        Assert.Null(transaction.Notes);
    }

    [Fact]
    public async Task StockOut_WithNullNotes_CreatesTransactionSuccessfully()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var item = new InventoryItem 
        { 
            Id = 1, 
            Name = "Test Item", 
            Quantity = 10, 
            MinStockThreshold = 5, 
            Price = 100 
        };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var controller = new StockController(context);
        var request = new StockOutRequest { Quantity = 5, Notes = null };

        // Act
        var result = await controller.StockOut(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var transaction = await context.StockTransactions.FirstOrDefaultAsync();
        Assert.NotNull(transaction);
        Assert.Null(transaction.Notes);
    }
}
