using System.ComponentModel.DataAnnotations;

namespace ToyStore.InventoryService.DTOs;

public class InventoryDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => Quantity - ReservedQuantity;
    public string Location { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public int ReorderLevel { get; set; }
    public int MaxStock { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty; // InStock, LowStock, OutOfStock
}

public class CreateInventoryDto
{
    [Required]
    [StringLength(50)]
    public string ProductId { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string WarehouseId { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; } = 10;
    
    [Range(1, int.MaxValue)]
    public int MaxStock { get; set; } = 1000;
    
    [Range(0, double.MaxValue)]
    public decimal UnitCost { get; set; }
}

public class UpdateInventoryDto
{
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    
    [StringLength(100)]
    public string? Location { get; set; }
    
    [StringLength(50)]
    public string? WarehouseId { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? ReorderLevel { get; set; }
    
    [Range(1, int.MaxValue)]
    public int? MaxStock { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? UnitCost { get; set; }
    
    public bool? IsActive { get; set; }
}

public class StockMovementDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string MovementType { get; set; } = string.Empty; // In, Out, Transfer, Adjustment
    public int Quantity { get; set; }
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty; // Order ID, Transfer ID, etc.
    public string Location { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CreateStockMovementDto
{
    [Required]
    [StringLength(50)]
    public string ProductId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string MovementType { get; set; } = string.Empty;
    
    public int Quantity { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Reason { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Reference { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;
}

public class StockReservationDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public int ReservedQuantity { get; set; }
    public DateTime ReservedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Status { get; set; } = string.Empty; // Reserved, Confirmed, Released, Expired
    public string ReservedBy { get; set; } = string.Empty;
}

public class CreateStockReservationDto
{
    [Required]
    [StringLength(50)]
    public string ProductId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string OrderId { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue)]
    public int ReservedQuantity { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
}

public class LowStockAlertDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public string Location { get; set; } = string.Empty;
    public string WarehouseId { get; set; } = string.Empty;
    public DateTime AlertDate { get; set; }
    public string Status { get; set; } = string.Empty; // New, Acknowledged, Resolved
}

public class InventoryReportDto
{
    public string ReportId { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty; // Stock, Movements, Alerts
    public DateTime GeneratedAt { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public List<InventoryDto> Items { get; set; } = new();
}

public class BulkInventoryUpdateDto
{
    public List<BulkInventoryItemDto> Items { get; set; } = new();
    public string Reason { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}

public class BulkInventoryItemDto
{
    [Required]
    [StringLength(50)]
    public string ProductId { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    
    [StringLength(20)]
    public string Operation { get; set; } = "Set"; // Set, Add, Subtract
}

public class WarehouseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateWarehouseDto
{
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
    
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
    
    [Phone]
    public string ContactPhone { get; set; } = string.Empty;
}
