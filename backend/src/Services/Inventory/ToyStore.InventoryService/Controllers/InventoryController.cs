using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ToyStore.InventoryService.DTOs;
using ToyStore.InventoryService.Models;
using ToyStore.Shared.Models;
using ToyStore.Shared.Services;

namespace ToyStore.InventoryService.Controllers;

/// <summary>
/// Inventory management controller for stock operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[SwaggerTag("Inventory management including stock levels, reservations, and warehouse operations")]
[Produces("application/json")]
public class InventoryController : ControllerBase
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<InventoryController> _logger;
    private readonly ICacheService _cacheService;

    public InventoryController(InventoryDbContext context, ILogger<InventoryController> logger, ICacheService cacheService)
    {
        _context = context;
        _logger = logger;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Get inventory for a specific product
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <returns>Inventory information</returns>
    /// <response code="200">Inventory retrieved successfully</response>
    /// <response code="404">Inventory not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{productId}")]
    [SwaggerOperation(
        Summary = "Get product inventory",
        Description = "Retrieves inventory information for a specific product including available quantity and location.",
        OperationId = "GetInventory",
        Tags = new[] { "Inventory" })]
    [SwaggerResponse(200, "Inventory retrieved successfully", typeof(ApiResponse<InventoryDto>))]
    [SwaggerResponse(404, "Inventory not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetInventory([SwaggerParameter("Product identifier", Required = true)] string productId)
    {
        try
        {
            var cacheKey = $"inventory_{productId}";
            var cachedInventory = await _cacheService.GetAsync<InventoryDto>(cacheKey);
            if (cachedInventory != null)
            {
                return Ok(ApiResponse<InventoryDto>.SuccessResult(cachedInventory));
            }

            var inventory = await _context.InventoryItems
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(i => i.ProductId == productId && i.IsActive);

            if (inventory == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Inventory not found", 404));
            }

            var dto = MapToInventoryDto(inventory);
            
            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));

            return Ok(ApiResponse<InventoryDto>.SuccessResult(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory for product {ProductId}", productId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving inventory", 500));
        }
    }

    /// <summary>
    /// Get all inventory items with pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="warehouseId">Filter by warehouse</param>
    /// <param name="lowStock">Filter low stock items</param>
    /// <returns>Paginated inventory list</returns>
    /// <response code="200">Inventory list retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get inventory list",
        Description = "Retrieves a paginated list of inventory items with optional filtering.",
        OperationId = "GetInventoryList",
        Tags = new[] { "Inventory" })]
    [SwaggerResponse(200, "Inventory list retrieved successfully", typeof(ApiResponse<PaginatedResponse<InventoryDto>>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetInventoryList(
        [FromQuery, SwaggerParameter("Page number", Required = false)] int page = 1,
        [FromQuery, SwaggerParameter("Items per page", Required = false)] int pageSize = 20,
        [FromQuery, SwaggerParameter("Warehouse filter", Required = false)] string? warehouseId = null,
        [FromQuery, SwaggerParameter("Low stock filter", Required = false)] bool? lowStock = null)
    {
        try
        {
            var query = _context.InventoryItems
                .Include(i => i.Warehouse)
                .Where(i => i.IsActive);

            if (!string.IsNullOrEmpty(warehouseId))
            {
                query = query.Where(i => i.WarehouseId == warehouseId);
            }

            if (lowStock.HasValue && lowStock.Value)
            {
                query = query.Where(i => i.Quantity <= i.ReorderLevel);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(i => i.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = items.Select(MapToInventoryDto).ToList();
            var paginatedResult = PaginatedResponse<InventoryDto>.Create(dtos, totalCount, pageSize, page);

            return Ok(ApiResponse<PaginatedResponse<InventoryDto>>.SuccessResult(paginatedResult));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory list");
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving inventory list", 500));
        }
    }

    /// <summary>
    /// Create new inventory item
    /// </summary>
    /// <param name="request">Inventory creation data</param>
    /// <returns>Created inventory item</returns>
    /// <response code="201">Inventory created successfully</response>
    /// <response code="400">Invalid inventory data</response>
    /// <response code="409">Inventory already exists</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [SwaggerOperation(
        Summary = "Create inventory item",
        Description = "Creates a new inventory item for a product.",
        OperationId = "CreateInventory",
        Tags = new[] { "Inventory" })]
    [SwaggerResponse(201, "Inventory created successfully", typeof(ApiResponse<InventoryDto>))]
    [SwaggerResponse(400, "Invalid inventory data", typeof(ApiResponse<object>))]
    [SwaggerResponse(409, "Inventory already exists", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> CreateInventory([FromBody, SwaggerRequestBody("Inventory creation data", Required = true)] CreateInventoryDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid inventory data", 400));
            }

            var existingInventory = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == request.ProductId && i.IsActive);

            if (existingInventory != null)
            {
                return Conflict(ApiResponse<object>.ErrorResult("Inventory already exists for this product", 409));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

            var inventory = new InventoryItem
            {
                ProductId = request.ProductId,
                ProductName = request.ProductName,
                SKU = request.SKU,
                Quantity = request.Quantity,
                Location = request.Location,
                WarehouseId = request.WarehouseId,
                ReorderLevel = request.ReorderLevel,
                MaxStock = request.MaxStock,
                UnitCost = request.UnitCost,
                LastUpdated = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedBy = userId
            };

            _context.InventoryItems.Add(inventory);

            // Create initial stock movement
            var stockMovement = new StockMovement
            {
                ProductId = request.ProductId,
                MovementType = "Initial",
                Quantity = request.Quantity,
                PreviousQuantity = 0,
                NewQuantity = request.Quantity,
                Reason = "Initial inventory creation",
                Location = request.Location,
                PerformedBy = userId,
                CreatedBy = userId,
                UpdatedBy = userId
            };

            _context.StockMovements.Add(stockMovement);
            await _context.SaveChangesAsync();

            // Clear cache
            await _cacheService.RemoveAsync($"inventory_{request.ProductId}");

            var dto = MapToInventoryDto(inventory);

            _logger.LogInformation("Inventory created for product {ProductId} by user {UserId}", request.ProductId, userId);

            return CreatedAtAction(nameof(GetInventory), new { productId = request.ProductId }, 
                ApiResponse<InventoryDto>.SuccessResult(dto, "Inventory created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inventory for product {ProductId}", request.ProductId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error creating inventory", 500));
        }
    }

    /// <summary>
    /// Update inventory quantity and details
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="request">Update data</param>
    /// <returns>Updated inventory</returns>
    /// <response code="200">Inventory updated successfully</response>
    /// <response code="400">Invalid update data</response>
    /// <response code="404">Inventory not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{productId}")]
    [Authorize(Roles = "Admin,Manager")]
    [SwaggerOperation(
        Summary = "Update inventory",
        Description = "Updates inventory quantity and other details for a product.",
        OperationId = "UpdateInventory",
        Tags = new[] { "Inventory" })]
    [SwaggerResponse(200, "Inventory updated successfully", typeof(ApiResponse<InventoryDto>))]
    [SwaggerResponse(400, "Invalid update data", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Inventory not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> UpdateInventory(
        [SwaggerParameter("Product identifier", Required = true)] string productId,
        [FromBody, SwaggerRequestBody("Inventory update data", Required = true)] UpdateInventoryDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid update data", 400));
            }

            var inventory = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == productId && i.IsActive);

            if (inventory == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Inventory not found", 404));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var previousQuantity = inventory.Quantity;

            // Update fields
            inventory.Quantity = request.Quantity;
            if (!string.IsNullOrEmpty(request.Location))
                inventory.Location = request.Location;
            if (!string.IsNullOrEmpty(request.WarehouseId))
                inventory.WarehouseId = request.WarehouseId;
            if (request.ReorderLevel.HasValue)
                inventory.ReorderLevel = request.ReorderLevel.Value;
            if (request.MaxStock.HasValue)
                inventory.MaxStock = request.MaxStock.Value;
            if (request.UnitCost.HasValue)
                inventory.UnitCost = request.UnitCost.Value;
            if (request.IsActive.HasValue)
                inventory.IsActive = request.IsActive.Value;

            inventory.LastUpdated = DateTime.UtcNow;
            inventory.UpdatedBy = userId;

            // Create stock movement if quantity changed
            if (previousQuantity != request.Quantity)
            {
                var movementType = request.Quantity > previousQuantity ? "Adjustment-In" : "Adjustment-Out";
                var quantityDiff = Math.Abs(request.Quantity - previousQuantity);

                var stockMovement = new StockMovement
                {
                    ProductId = productId,
                    MovementType = movementType,
                    Quantity = quantityDiff,
                    PreviousQuantity = previousQuantity,
                    NewQuantity = request.Quantity,
                    Reason = "Inventory adjustment",
                    Location = inventory.Location,
                    PerformedBy = userId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.StockMovements.Add(stockMovement);
            }

            await _context.SaveChangesAsync();

            // Clear cache
            await _cacheService.RemoveAsync($"inventory_{productId}");

            var dto = MapToInventoryDto(inventory);

            _logger.LogInformation("Inventory updated for product {ProductId} by user {UserId}", productId, userId);

            return Ok(ApiResponse<InventoryDto>.SuccessResult(dto, "Inventory updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory for product {ProductId}", productId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error updating inventory", 500));
        }
    }

    /// <summary>
    /// Reserve stock for an order
    /// </summary>
    /// <param name="request">Stock reservation data</param>
    /// <returns>Reservation result</returns>
    /// <response code="200">Stock reserved successfully</response>
    /// <response code="400">Invalid reservation data or insufficient stock</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("reserve")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Reserve stock",
        Description = "Reserves stock for an order to prevent overselling.",
        OperationId = "ReserveStock",
        Tags = new[] { "Stock Management" })]
    [SwaggerResponse(200, "Stock reserved successfully", typeof(ApiResponse<StockReservationDto>))]
    [SwaggerResponse(400, "Invalid reservation data or insufficient stock", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Product not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> ReserveStock([FromBody, SwaggerRequestBody("Stock reservation data", Required = true)] CreateStockReservationDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid reservation data", 400));
            }

            var inventory = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == request.ProductId && i.IsActive);

            if (inventory == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Product not found", 404));
            }

            var availableQuantity = inventory.Quantity - inventory.ReservedQuantity;
            if (availableQuantity < request.ReservedQuantity)
            {
                return BadRequest(ApiResponse<object>.ErrorResult($"Insufficient stock. Available: {availableQuantity}, Requested: {request.ReservedQuantity}", 400));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
            var expiresAt = request.ExpiresAt ?? DateTime.UtcNow.AddHours(2); // Default 2 hours

            var reservation = new StockReservation
            {
                ProductId = request.ProductId,
                OrderId = request.OrderId,
                ReservedQuantity = request.ReservedQuantity,
                ExpiresAt = expiresAt,
                ReservedBy = userId,
                CreatedBy = userId,
                UpdatedBy = userId
            };

            // Update reserved quantity
            inventory.ReservedQuantity += request.ReservedQuantity;
            inventory.LastUpdated = DateTime.UtcNow;
            inventory.UpdatedBy = userId;

            _context.StockReservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Clear cache
            await _cacheService.RemoveAsync($"inventory_{request.ProductId}");

            var dto = new StockReservationDto
            {
                Id = reservation.Id,
                ProductId = reservation.ProductId,
                OrderId = reservation.OrderId,
                ReservedQuantity = reservation.ReservedQuantity,
                ReservedAt = reservation.ReservedAt,
                ExpiresAt = reservation.ExpiresAt,
                Status = reservation.Status,
                ReservedBy = reservation.ReservedBy
            };

            _logger.LogInformation("Stock reserved for product {ProductId}, order {OrderId}, quantity {Quantity}", 
                request.ProductId, request.OrderId, request.ReservedQuantity);

            return Ok(ApiResponse<StockReservationDto>.SuccessResult(dto, "Stock reserved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving stock for product {ProductId}", request.ProductId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error reserving stock", 500));
        }
    }

    /// <summary>
    /// Get low stock alerts
    /// </summary>
    /// <returns>List of low stock alerts</returns>
    /// <response code="200">Low stock alerts retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("low-stock")]
    [Authorize(Roles = "Admin,Manager")]
    [SwaggerOperation(
        Summary = "Get low stock alerts",
        Description = "Retrieves products that are at or below their reorder level.",
        OperationId = "GetLowStockAlerts",
        Tags = new[] { "Alerts" })]
    [SwaggerResponse(200, "Low stock alerts retrieved successfully", typeof(ApiResponse<List<LowStockAlertDto>>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetLowStockAlerts()
    {
        try
        {
            var lowStockItems = await _context.InventoryItems
                .Include(i => i.Warehouse)
                .Where(i => i.IsActive && i.Quantity <= i.ReorderLevel)
                .ToListAsync();

            var alerts = lowStockItems.Select(item => new LowStockAlertDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                SKU = item.SKU,
                CurrentQuantity = item.Quantity,
                ReorderLevel = item.ReorderLevel,
                Location = item.Location,
                WarehouseId = item.WarehouseId,
                AlertDate = DateTime.UtcNow,
                Status = "Active"
            }).ToList();

            return Ok(ApiResponse<List<LowStockAlertDto>>.SuccessResult(alerts, "Low stock alerts retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving low stock alerts");
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving low stock alerts", 500));
        }
    }

    /// <summary>
    /// Get stock movements for a product
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Paginated stock movements</returns>
    /// <response code="200">Stock movements retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{productId}/movements")]
    [Authorize(Roles = "Admin,Manager")]
    [SwaggerOperation(
        Summary = "Get stock movements",
        Description = "Retrieves the history of stock movements for a specific product.",
        OperationId = "GetStockMovements",
        Tags = new[] { "Stock Management" })]
    [SwaggerResponse(200, "Stock movements retrieved successfully", typeof(ApiResponse<PaginatedResponse<StockMovementDto>>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetStockMovements(
        [SwaggerParameter("Product identifier", Required = true)] string productId,
        [FromQuery, SwaggerParameter("Page number", Required = false)] int page = 1,
        [FromQuery, SwaggerParameter("Items per page", Required = false)] int pageSize = 20)
    {
        try
        {
            var query = _context.StockMovements
                .Where(m => m.ProductId == productId)
                .OrderByDescending(m => m.CreatedAt);

            var totalCount = await query.CountAsync();
            var movements = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = movements.Select(m => new StockMovementDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                MovementType = m.MovementType,
                Quantity = m.Quantity,
                PreviousQuantity = m.PreviousQuantity,
                NewQuantity = m.NewQuantity,
                Reason = m.Reason,
                Reference = m.Reference,
                Location = m.Location,
                PerformedBy = m.PerformedBy,
                CreatedAt = m.CreatedAt,
                Notes = m.Notes
            }).ToList();

            var paginatedResult = PaginatedResponse<StockMovementDto>.Create(dtos, totalCount, pageSize, page);

            return Ok(ApiResponse<PaginatedResponse<StockMovementDto>>.SuccessResult(paginatedResult));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stock movements for product {ProductId}", productId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error retrieving stock movements", 500));
        }
    }

    private InventoryDto MapToInventoryDto(InventoryItem inventory)
    {
        return new InventoryDto
        {
            ProductId = inventory.ProductId,
            ProductName = inventory.ProductName,
            SKU = inventory.SKU,
            Quantity = inventory.Quantity,
            ReservedQuantity = inventory.ReservedQuantity,
            Location = inventory.Location,
            WarehouseId = inventory.WarehouseId,
            ReorderLevel = inventory.ReorderLevel,
            MaxStock = inventory.MaxStock,
            UnitCost = inventory.UnitCost,
            LastUpdated = inventory.LastUpdated,
            IsActive = inventory.IsActive,
            Status = GetInventoryStatus(inventory)
        };
    }

    private string GetInventoryStatus(InventoryItem inventory)
    {
        var availableQuantity = inventory.Quantity - inventory.ReservedQuantity;
        
        if (availableQuantity <= 0)
            return "OutOfStock";
        if (availableQuantity <= inventory.ReorderLevel)
            return "LowStock";
        
        return "InStock";
    }
}
