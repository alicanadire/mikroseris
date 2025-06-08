using ToyStore.Shared.Models;

namespace ToyStore.ProductService.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string AgeRange { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool InStock => StockQuantity > 0;
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public bool IsFeatured { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
}
