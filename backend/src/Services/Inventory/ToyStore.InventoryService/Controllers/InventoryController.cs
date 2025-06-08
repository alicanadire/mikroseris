using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToyStore.Shared.Models;

namespace ToyStore.InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(InventoryDbContext context, ILogger<InventoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ApiResponse<InventoryDto>>> GetInventory(string productId)
    {
        try
        {
            var inventory = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            if (inventory == null)
            {
                return NotFound(new ApiResponse<InventoryDto>
                {
                    Success = false,
                    Message = "Inventory not found",
                    StatusCode = 404
                });
            }

            var dto = new InventoryDto
            {
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                Location = inventory.Location,
                LastUpdated = inventory.LastUpdated,
                ReorderLevel = 10,
                MaxStock = 1000
            };

            return Ok(new ApiResponse<InventoryDto>
            {
                Success = true,
                Data = dto,
                Message = "Inventory retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory for product {ProductId}", productId);
            return StatusCode(500, new ApiResponse<InventoryDto>
            {
                Success = false,
                Message = "Error retrieving inventory",
                StatusCode = 500
            });
        }
    }

    [HttpPost("update")]
    public async Task<ActionResult<ApiResponse<InventoryDto>>> UpdateInventory([FromBody] UpdateInventoryRequest request)
    {
        try
        {
            var inventory = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == request.ProductId);

            if (inventory == null)
            {
                inventory = new InventoryItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Location = request.Location ?? "Main Warehouse",
                    LastUpdated = DateTime.UtcNow
                };
                _context.InventoryItems.Add(inventory);
            }
            else
            {
                inventory.Quantity = request.Quantity;
                inventory.Location = request.Location ?? inventory.Location;
                inventory.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            var dto = new InventoryDto
            {
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                Location = inventory.Location,
                LastUpdated = inventory.LastUpdated,
                ReorderLevel = 10,
                MaxStock = 1000
            };

            return Ok(new ApiResponse<InventoryDto>
            {
                Success = true,
                Data = dto,
                Message = "Inventory updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory for product {ProductId}", request.ProductId);
            return StatusCode(500, new ApiResponse<InventoryDto>
            {
                Success = false,
                Message = "Error updating inventory",
                StatusCode = 500
            });
        }
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<ApiResponse<List<InventoryDto>>>> GetLowStock()
    {
        try
        {
            var lowStockItems = await _context.InventoryItems
                .Where(i => i.Quantity < 10)
                .ToListAsync();

            var dtos = lowStockItems.Select(inventory => new InventoryDto
            {
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                Location = inventory.Location,
                LastUpdated = inventory.LastUpdated,
                ReorderLevel = 10,
                MaxStock = 1000
            }).ToList();

            return Ok(new ApiResponse<List<InventoryDto>>
            {
                Success = true,
                Data = dtos,
                Message = "Low stock items retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving low stock items");
            return StatusCode(500, new ApiResponse<List<InventoryDto>>
            {
                Success = false,
                Message = "Error retrieving low stock items",
                StatusCode = 500
            });
        }
    }
}

public class InventoryDto
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public int ReorderLevel { get; set; }
    public int MaxStock { get; set; }
}

public class UpdateInventoryRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Location { get; set; }
}
