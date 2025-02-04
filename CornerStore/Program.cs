using CornerStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using CornerStore.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Set the JSON serializer options to handle circular references and datetime format issues.
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Enable legacy timestamp behavior for PostgreSQL.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add the PostgreSQL database context to the DI container.
builder.Services.AddNpgsql<CornerStoreDbContext>(builder.Configuration["CornerStoreDbConnectionString"] ?? "testing");

var app = builder.Build();

// Configure the HTTP request pipeline for development environment.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Define the API endpoints.

// Products API base URL
const string _productApiUrl = "/products";

// Get all products with optional search (GET)
app.MapGet(_productApiUrl, async (CornerStoreDbContext db, string? search) =>
{
    try
    {
        // Initialize the query with the product table and include related Category data
        IQueryable<Product> query = db.Products
            .Include(p => p.Category);   // Ensure Category is included

        // If a search term is provided, filter products by name, brand, or category name
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearch = search.ToLower();  // Make search case-insensitive
            query = query.Where(p => p.ProductName.ToLower().Contains(lowerSearch) ||
                                     p.Brand.ToLower().Contains(lowerSearch) ||
                                     p.Category.CategoryName.ToLower().Contains(lowerSearch));
        }

        // Execute the query and retrieve the results
        var products = await query.ToListAsync();

        // Map the result to a ProductDTO
        var productsDTO = products.Select(product => new ProductDTO
        {
            Id = product.Id,
            ProductName = product.ProductName,
            Price = product.Price,
            Brand = product.Brand,
            Category = new CategoryDTO{
                Id = product.CategoryId,
                CategoryName = product.Category?.CategoryName  // Ensure Category is not null
            }

        }).ToList();

        return Results.Ok(productsDTO); // Return the filtered and mapped products
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while retrieving products: " + ex.Message);
    }
});





// Add a product (POST)
app.MapPost(_productApiUrl, async (CornerStoreDbContext db, Product product) =>
{
    try
    {
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return Results.Created($"/products/{product.Id}", product); // Return the created product with its ID
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while adding the product: " + ex.Message);
    }
});

// Update a product (PUT)
app.MapPut(_productApiUrl + "/{id}", async (CornerStoreDbContext db, int id, Product product) =>
{
    try
    {
        var existingProduct = await db.Products.FindAsync(id);
        if (existingProduct is null)
        {
            return Results.NotFound("Product not found.");
        }

        // Update the product fields
        existingProduct.ProductName = product.ProductName;
        existingProduct.Price = product.Price;
        existingProduct.Brand = product.Brand;
        existingProduct.CategoryId = product.CategoryId;

        await db.SaveChangesAsync();

        return Results.NoContent(); // Return the updated product
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while updating the product: " + ex.Message);
    }
});

// Add a cashier
app.MapPost("/cashiers", async (CornerStoreDbContext db, Cashier cashier) =>
{
    try
    {
        db.Cashiers.Add(cashier);
        await db.SaveChangesAsync();
        return Results.Created($"/cashiers/{cashier.Id}", cashier);
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while adding the cashier: " + ex.Message);
    }
});

// Get a cashier by ID with their orders and the products in those orders
app.MapGet("/cashiers/{id}", async (CornerStoreDbContext db, int id) =>
{
    try
    {
        var cashier = await db.Cashiers
            .Include(c => c.Orders)
            .ThenInclude(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cashier is null)
        {
            return Results.NotFound("Cashier not found.");
        }

        return Results.Ok(cashier);
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while retrieving the cashier: " + ex.Message);
    }
});

// Get an order by ID, including cashier, products, and categories
app.MapGet("/orders/{id}", async (CornerStoreDbContext db, int id) =>
{
    try
    {
        var order = await db.Orders
            .Include(o => o.Cashier)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return Results.NotFound("Order not found.");
        }

        return Results.Ok(order);
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while retrieving the order: " + ex.Message);
    }
});

// Get all orders or filter by orderDate
// Get all orders or filter by orderDate
app.MapGet("/orders", async (CornerStoreDbContext db, DateTime? orderDate) =>
{
    try
    {
        IQueryable<Order> query = db.Orders
            .Include(o => o.Cashier)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .ThenInclude(p => p.Category);

        if (orderDate.HasValue)
        {
            query = query.Where(o => o.PaidOnDate.HasValue && o.PaidOnDate.Value.Date == orderDate.Value.Date);
        }

        var orders = await query.ToListAsync();

        // Return empty list if no orders, instead of NoContent
        return orders.Any() ? Results.Ok(orders) : Results.Ok(new List<Order>());
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while retrieving the orders: " + ex.Message);
    }
});


// Delete an order
app.MapDelete("/orders/{id}", async (CornerStoreDbContext db, int id) =>
{
    try
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
        {
            return Results.NotFound("Order not found.");
        }

        db.Orders.Remove(order);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while deleting the order: " + ex.Message);
    }
});


// Create an order with products
app.MapPost("/orders", async (CornerStoreDbContext db, Order order) =>
{
    try
    {
        if (order.OrderProducts == null || order.OrderProducts.Count == 0)
        {
            return Results.BadRequest("Order must include at least one product.");
        }

        // Ensure the Product is loaded for each OrderProduct before calculating Total
        foreach (var orderProduct in order.OrderProducts)
        {
            var product = await db.Products.FindAsync(orderProduct.ProductId);
            if (product == null)
            {
                return Results.BadRequest($"Product with ID {orderProduct.ProductId} not found.");
            }
            orderProduct.Product = product; // Load product into the orderProduct
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return Results.Created($"/orders/{order.Id}", order);
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while creating the order: " + ex.Message);
    }
});




app.Run();

// The Program class is required but should not be moved or changed.
public partial class Program { }
