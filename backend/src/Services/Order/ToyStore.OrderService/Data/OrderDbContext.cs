using Microsoft.EntityFrameworkCore;
using ToyStore.OrderService.Models;

namespace ToyStore.OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentIntentId).HasMaxLength(200);
            
            entity.Property(e => e.ShippingFirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingLastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingStreet).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ShippingCity).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingState).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingZipCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ShippingCountry).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingPhone).HasMaxLength(20);

            entity.HasMany(e => e.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProductImageUrl).HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);
        });

        // Cart configuration
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasMany(e => e.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId).IsUnique();
        });

        // CartItem configuration
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProductImageUrl).HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            
            entity.HasIndex(e => e.CartId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => new { e.CartId, e.ProductId }).IsUnique();
        });
    }
}
