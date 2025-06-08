using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToyStore.EventBus.Abstractions;
using ToyStore.EventBus.Events;
using ToyStore.OrderService.Data;
using ToyStore.OrderService.DTOs;
using ToyStore.OrderService.Models;
using ToyStore.Shared.Models;

namespace ToyStore.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        OrderDbContext context, 
        IEventBus eventBus, 
        ILogger<OrdersController> logger)
    {
        _context = context;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        
        var query = _context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId && !o.IsDeleted)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt,
                ItemCount = o.Items.Count
            })
            .ToListAsync();

        var result = PaginatedResponse<OrderDto>.Create(orders, totalCount, pageSize, page);
        return Ok(ApiResponse<PaginatedResponse<OrderDto>>.SuccessResult(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var userId = GetUserId();
        
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId && !o.IsDeleted);

        if (order == null)
        {
            return NotFound(ApiResponse<OrderDetailDto>.ErrorResult("Order not found", 404));
        }

        var orderDto = new OrderDetailDto
        {
            Id = order.Id,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            PaymentMethod = order.PaymentMethod,
            ShippingAddress = new AddressDto
            {
                FirstName = order.ShippingFirstName,
                LastName = order.ShippingLastName,
                Street = order.ShippingStreet,
                City = order.ShippingCity,
                State = order.ShippingState,
                ZipCode = order.ShippingZipCode,
                Country = order.ShippingCountry,
                Phone = order.ShippingPhone
            },
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImageUrl = i.ProductImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList(),
            CreatedAt = order.CreatedAt
        };

        return Ok(ApiResponse<OrderDetailDto>.SuccessResult(orderDto));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        var userId = GetUserId();
        var userEmail = GetUserEmail();

        try
        {
            // Get user's cart
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return BadRequest(ApiResponse<OrderDto>.ErrorResult("Cart is empty", 400));
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                UserEmail = userEmail,
                TotalAmount = cart.Items.Sum(i => i.TotalPrice),
                PaymentMethod = createOrderDto.PaymentMethod,
                ShippingFirstName = createOrderDto.ShippingAddress.FirstName,
                ShippingLastName = createOrderDto.ShippingAddress.LastName,
                ShippingStreet = createOrderDto.ShippingAddress.Street,
                ShippingCity = createOrderDto.ShippingAddress.City,
                ShippingState = createOrderDto.ShippingAddress.State,
                ShippingZipCode = createOrderDto.ShippingAddress.ZipCode,
                ShippingCountry = createOrderDto.ShippingAddress.Country,
                ShippingPhone = createOrderDto.ShippingAddress.Phone
            };

            // Add order items
            foreach (var cartItem in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.ProductName,
                    ProductImageUrl = cartItem.ProductImageUrl,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice
                });
            }

            _context.Orders.Add(order);

            // Clear cart
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();

            // Publish order created event
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = userId,
                TotalAmount = order.TotalAmount,
                CustomerEmail = userEmail,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _eventBus.PublishAsync(orderCreatedEvent);

            var orderDto = new OrderDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                ItemCount = order.Items.Count
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, 
                ApiResponse<OrderDto>.SuccessResult(orderDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for user {UserId}", userId);
            return StatusCode(500, ApiResponse<OrderDto>.ErrorResult("Error creating order", 500));
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private string GetUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value ?? "";
    }
}
