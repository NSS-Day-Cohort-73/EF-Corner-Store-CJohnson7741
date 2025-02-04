using Microsoft.EntityFrameworkCore;
using CornerStore.Models;
public class CornerStoreDbContext : DbContext
{

    public DbSet<Cashier> Cashiers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }

    public CornerStoreDbContext(DbContextOptions<CornerStoreDbContext> context) : base(context)
    {

    }

    //allows us to configure the schema when migrating as well as seed data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    // Configure the composite primary key for OrderProduct
        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => new { op.OrderId, op.ProductId }); // Composite key using both OrderId and ProductId

               // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, CategoryName = "Electronics" },
            new Category { Id = 2, CategoryName = "Clothing" }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, ProductName = "Laptop", Price = 999.99m, Brand = "BrandA", CategoryId = 1 },
            new Product { Id = 2, ProductName = "Smartphone", Price = 699.99m, Brand = "BrandB", CategoryId = 1 },
            new Product { Id = 3, ProductName = "T-Shirt", Price = 19.99m, Brand = "BrandC", CategoryId = 2 },
            new Product { Id = 4, ProductName = "Jeans", Price = 49.99m, Brand = "BrandD", CategoryId = 2 }
        );

        // Seed Cashiers
        modelBuilder.Entity<Cashier>().HasData(
            new Cashier { Id = 1, FirstName = "John", LastName = "Doe" },
            new Cashier { Id = 2, FirstName = "Jane", LastName = "Smith" }
        );

        // Seed Orders
        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, CashierId = 1, PaidOnDate = DateTime.Now },
            new Order { Id = 2, CashierId = 2, PaidOnDate = DateTime.Now }
        );

        // Seed OrderProducts (many-to-many relationship between Orders and Products)
        modelBuilder.Entity<OrderProduct>().HasData(
            new OrderProduct { OrderId = 1, ProductId = 1, Quantity = 1 }, // Order 1, Product 1 (Laptop)
            new OrderProduct { OrderId = 1, ProductId = 2, Quantity = 2 }, // Order 1, Product 2 (Smartphone)
            new OrderProduct { OrderId = 2, ProductId = 3, Quantity = 3 }, // Order 2, Product 3 (T-Shirt)
            new OrderProduct { OrderId = 2, ProductId = 4, Quantity = 1 }  // Order 2, Product 4 (Jeans)
        );
    }
}