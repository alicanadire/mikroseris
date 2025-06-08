using System.ComponentModel.DataAnnotations;
using ToyStore.Shared.Models;

namespace ToyStore.InventoryService.Models;

public class InventoryItem : BaseEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string ProductId { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    public int Quantity { get; set; }
    
    public int ReservedQuantity { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string WarehouseId { get; set; } = string.Empty;
    
    public int ReorderLevel { get; set; } = 10;
    
    public int MaxStock { get; set; } = 1000;
    
    public decimal UnitCost { get; set; }
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Warehouse? Warehouse { get; set; }
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public virtual ICollection<StockReservation> StockReservations { get; set; } = new List<StockReservation>();
}

public class StockMovement : BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [StringLength(50)]
    public string ProductId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string MovementType { get; set; } = string.Empty; // In, Out, Transfer, Adjustment
    
    public int Quantity { get; set; }
    
    public int PreviousQuantity { get; set; }
    
    public int NewQuantity { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Reason { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Reference { get; set; } = string.Empty; // Order ID, Transfer ID, etc.
    
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string PerformedBy { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    // Navigation properties
    public virtual InventoryItem? InventoryItem { get; set; }
}

public class StockReservation : BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [StringLength(50)]
    public string ProductId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string OrderId { get; set; } = string.Empty;
    
    public int ReservedQuantity { get; set; }
    
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime ExpiresAt { get; set; }
    
    [StringLength(20)]
    public string Status { get; set; } = "Reserved"; // Reserved, Confirmed, Released, Expired
    
    [StringLength(50)]
    public string ReservedBy { get; set; } = string.Empty;

    // Navigation properties
    public virtual InventoryItem? InventoryItem { get; set; }
}

public class LowStockAlert : BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [StringLength(50)]
    public string ProductId { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    public int CurrentQuantity { get; set; }
    
    public int ReorderLevel { get; set; }
    
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string WarehouseId { get; set; } = string.Empty;
    
    public DateTime AlertDate { get; set; } = DateTime.UtcNow;
    
    [StringLength(20)]
    public string Status { get; set; } = "New"; // New, Acknowledged, Resolved
    
    [StringLength(50)]
    public string AcknowledgedBy { get; set; } = string.Empty;
    
    public DateTime? AcknowledgedAt { get; set; }
    
    public DateTime? ResolvedAt { get; set; }
}

public class Warehouse : BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string City { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string Country { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string ManagerName { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string ContactEmail { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string ContactPhone { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}

public class InventoryTransaction : BaseEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [StringLength(20)]
    public string TransactionType { get; set; } = string.Empty; // Bulk, Single, Transfer
    
    [StringLength(100)]
    public string Reference { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string PerformedBy { get; set; } = string.Empty;
    
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    [StringLength(20)]
    public string Status { get; set; } = "Completed"; // Pending, Completed, Failed
    
    public int TotalItems { get; set; }
    
    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;
}
