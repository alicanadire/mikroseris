using System.ComponentModel.DataAnnotations;

namespace ToyStore.UserService.DTOs;

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "en";
    public string TimeZone { get; set; } = "UTC";
}

public class UpdateUserProfileDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    public DateTime? DateOfBirth { get; set; }
    
    public string ProfileImageUrl { get; set; } = string.Empty;
    
    [StringLength(10)]
    public string PreferredLanguage { get; set; } = "en";
    
    [StringLength(50)]
    public string TimeZone { get; set; } = "UTC";
}

public class UserAddressDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateUserAddressDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Company { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string AddressLine1 { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string AddressLine2 { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string City { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string State { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string ZipCode { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Country { get; set; } = string.Empty;
    
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    public bool IsDefault { get; set; } = false;
}

public class UpdateUserAddressDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Company { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string AddressLine1 { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string AddressLine2 { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string City { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string State { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string ZipCode { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Country { get; set; } = string.Empty;
    
    [Phone]
    public string Phone { get; set; } = string.Empty;
    
    public bool IsDefault { get; set; }
    
    public bool IsActive { get; set; } = true;
}

public class UserPreferencesDto
{
    public string UserId { get; set; } = string.Empty;
    public bool EmailNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    public bool MarketingEmails { get; set; } = true;
    public bool OrderUpdates { get; set; } = true;
    public bool ProductRecommendations { get; set; } = true;
    public string Currency { get; set; } = "USD";
    public string Language { get; set; } = "en";
    public string TimeZone { get; set; } = "UTC";
    public DateTime UpdatedAt { get; set; }
}

public class UpdateUserPreferencesDto
{
    public bool EmailNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; } = false;
    public bool MarketingEmails { get; set; } = true;
    public bool OrderUpdates { get; set; } = true;
    public bool ProductRecommendations { get; set; } = true;
    
    [StringLength(10)]
    public string Currency { get; set; } = "USD";
    
    [StringLength(10)]
    public string Language { get; set; } = "en";
    
    [StringLength(50)]
    public string TimeZone { get; set; } = "UTC";
}

public class UserOrderHistoryDto
{
    public string OrderId { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public List<OrderItemSummaryDto> Items { get; set; } = new();
}

public class OrderItemSummaryDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class UserWishlistDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public bool IsInStock { get; set; }
    public DateTime AddedAt { get; set; }
}

public class AddToWishlistDto
{
    [Required]
    public string ProductId { get; set; } = string.Empty;
}

public class UserStatsDto
{
    public string UserId { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public int WishlistItems { get; set; }
    public int ReviewsWritten { get; set; }
    public DateTime LastOrderDate { get; set; }
    public DateTime JoinDate { get; set; }
    public string MembershipLevel { get; set; } = "Bronze";
    public int LoyaltyPoints { get; set; }
}
