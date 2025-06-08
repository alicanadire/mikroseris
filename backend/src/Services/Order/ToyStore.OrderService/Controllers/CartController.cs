using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToyStore.OrderService.Data;
using ToyStore.OrderService.DTOs;
using ToyStore.OrderService.Models;
using ToyStore.Shared.Models;

namespace ToyStore.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly ILogger<CartController> _logger;

    public CartController(OrderDbContext context, ILogger<CartController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var cartDto = new CartDto
        {
            Id = cart.Id,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImageUrl = i.ProductImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList(),
            TotalAmount = cart.Items.Sum(i => i.TotalPrice),
            TotalItems = cart.Items.Sum(i => i.Quantity)
        };

        return Ok(ApiResponse<CartDto>.SuccessResult(cartDto));
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
    {
        var userId = GetUserId();

        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == addToCartDto.ProductId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += addToCartDto.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = addToCartDto.ProductId,
                    ProductName = addToCartDto.ProductName,
                    ProductImageUrl = addToCartDto.ProductImageUrl,
                    Quantity = addToCartDto.Quantity,
                    UnitPrice = addToCartDto.UnitPrice
                });
            }

            await _context.SaveChangesAsync();

            var cartDto = new CartDto
            {
                Id = cart.Id,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImageUrl = i.ProductImageUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList(),
                TotalAmount = cart.Items.Sum(i => i.TotalPrice),
                TotalItems = cart.Items.Sum(i => i.Quantity)
            };

            return Ok(ApiResponse<CartDto>.SuccessResult(cartDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart for user {UserId}", userId);
            return StatusCode(500, ApiResponse<CartDto>.ErrorResult("Error adding item to cart", 500));
        }
    }

    [HttpPut("update/{itemId}")]
    public async Task<IActionResult> UpdateCartItem(Guid itemId, [FromBody] int quantity)
    {
        var userId = GetUserId();

        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound(ApiResponse<CartDto>.ErrorResult("Cart not found", 404));
            }

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                return NotFound(ApiResponse<CartDto>.ErrorResult("Cart item not found", 404));
            }

            if (quantity <= 0)
            {
                cart.Items.Remove(item);
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            var cartDto = new CartDto
            {
                Id = cart.Id,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImageUrl = i.ProductImageUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList(),
                TotalAmount = cart.Items.Sum(i => i.TotalPrice),
                TotalItems = cart.Items.Sum(i => i.Quantity)
            };

            return Ok(ApiResponse<CartDto>.SuccessResult(cartDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item {ItemId} for user {UserId}", itemId, userId);
            return StatusCode(500, ApiResponse<CartDto>.ErrorResult("Error updating cart item", 500));
        }
    }

    [HttpDelete("remove/{itemId}")]
    public async Task<IActionResult> RemoveCartItem(Guid itemId)
    {
        return await UpdateCartItem(itemId, 0);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserId();

        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "Cart cleared successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("Error clearing cart", 500));
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
