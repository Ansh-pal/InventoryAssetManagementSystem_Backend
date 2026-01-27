using InventoryAssetManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add OpenAPI
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Map root endpoint to show API info
app.MapGet("/", () => Results.Json(new
{
    name = "Inventory & Asset Management System API",
    version = "1.0",
    status = "Running",
    documentation = "/scalar/v1",
    endpoints = new
    {
        inventory = "/api/Inventory",
        lowStock = "/api/Inventory/low-stock",
        stock = "/api/Stock"
    },
    description = "Backend API for Inventory and Asset Management System"
})).ExcludeFromDescription();

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
