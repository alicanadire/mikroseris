using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ToyStore.ProductService.Domain.Entities;

namespace ToyStore.ProductService.Infrastructure.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.ShortDescription).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.AgeRange).HasMaxLength(50);
            entity.Property(e => e.Rating).HasColumnType("decimal(3,2)");

            // Store lists as JSON
            entity.Property(e => e.ImageUrls)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

            entity.Property(e => e.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

            // Relationships
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Brand);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsFeatured);
            entity.HasIndex(e => new { e.Price, e.IsActive });
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            // Indexes
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
        });

        // ProductReview configuration
        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Rating).IsRequired();

            // Constraints
            entity.HasCheckConstraint("CK_ProductReview_Rating", "[Rating] >= 1 AND [Rating] <= 5");

            // Indexes
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsApproved);
            entity.HasIndex(e => new { e.ProductId, e.IsApproved });
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var categoryIds = new[]
        {
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Guid.Parse("44444444-4444-4444-4444-444444444444"),
            Guid.Parse("55555555-5555-5555-5555-555555555555"),
            Guid.Parse("66666666-6666-6666-6666-666666666666")
        };

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = categoryIds[0],
                Name = "Action Figures",
                Description = "Superhero and character action figures",
                Slug = "action-figures",
                ImageUrl = "/images/categories/action-figures.jpg",
                SortOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = categoryIds[1],
                Name = "Building Blocks",
                Description = "LEGO and other building sets",
                Slug = "building-blocks",
                ImageUrl = "/images/categories/building-blocks.jpg",
                SortOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = categoryIds[2],
                Name = "Dolls",
                Description = "Barbie, baby dolls, and fashion dolls",
                Slug = "dolls",
                ImageUrl = "/images/categories/dolls.jpg",
                SortOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = categoryIds[3],
                Name = "Educational Toys",
                Description = "Learning and STEM toys",
                Slug = "educational-toys",
                ImageUrl = "/images/categories/educational.jpg",
                SortOrder = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = categoryIds[4],
                Name = "Remote Control",
                Description = "RC cars, drones, and robots",
                Slug = "remote-control",
                ImageUrl = "/images/categories/rc-toys.jpg",
                SortOrder = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = categoryIds[5],
                Name = "Board Games",
                Description = "Family games and puzzles",
                Slug = "board-games",
                ImageUrl = "/images/categories/board-games.jpg",
                SortOrder = 6,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed Products
        var productIds = new[]
        {
            Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
            Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
            Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
            Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
            Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"),
            Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")
        };

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = productIds[0],
                Name = "Super Hero Action Figure Set",
                Description = "Complete set of 6 superhero action figures with accessories and display stand. Perfect for imaginative play and collecting.",
                ShortDescription = "Set of 6 superhero action figures with accessories",
                Price = 89.99m,
                OriginalPrice = 119.99m,
                Brand = "Hero Toys",
                AgeRange = "6-12 years",
                StockQuantity = 25,
                Rating = 4.8m,
                ReviewCount = 142,
                ImageUrls = new List<string> { "/images/products/superhero-set-1.jpg", "/images/products/superhero-set-2.jpg" },
                Tags = new List<string> { "superhero", "action", "collectible" },
                IsFeatured = true,
                IsActive = true,
                CategoryId = categoryIds[0],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = productIds[1],
                Name = "Ultimate Building Castle Set",
                Description = "Massive 2000-piece castle building set with knights, dragons, and medieval accessories. Includes detailed instruction manual.",
                ShortDescription = "2000-piece medieval castle building set",
                Price = 149.99m,
                Brand = "BlockMaster",
                AgeRange = "8+ years",
                StockQuantity = 12,
                Rating = 4.9m,
                ReviewCount = 89,
                ImageUrls = new List<string> { "/images/products/castle-set-1.jpg", "/images/products/castle-set-2.jpg" },
                Tags = new List<string> { "building", "castle", "medieval" },
                IsFeatured = true,
                IsActive = true,
                CategoryId = categoryIds[1],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
            // Add more seed products as needed...
        );
    }
}
