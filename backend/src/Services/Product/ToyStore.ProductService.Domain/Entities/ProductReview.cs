using ToyStore.Shared.Models;

namespace ToyStore.ProductService.Domain.Entities;

public class ProductReview : BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public bool IsVerifiedPurchase { get; set; } = false;
    public bool IsApproved { get; set; } = false;

    // Navigation properties
    public Product Product { get; set; } = null!;
}
