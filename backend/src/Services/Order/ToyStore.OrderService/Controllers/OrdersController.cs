using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ToyStore.EventBus.Abstractions;
using ToyStore.EventBus.Events;
using ToyStore.OrderService.Data;
using ToyStore.OrderService.DTOs;
using ToyStore.OrderService.Models;
using ToyStore.Shared.Models;

namespace ToyStore.OrderService.Controllers;

/// <summary>
/// Order management controller for ToyStore e-commerce platform
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[SwaggerTag("Order management including cart operations, order processing, and order history")]
[Produces("application/json")]
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

    /// <summary>
    /// Retrieves a paginated list of user's orders
    /// </summary>
    /// <param name="page">Page number for pagination (starts from 1)</param>
    /// <param name="pageSize">Number of items per page (maximum 50)</param>
    /// <returns>Paginated list of user orders</returns>
    /// <response code="200">Returns the paginated order list</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get user orders with pagination",
        Description = "Retrieves a paginated list of orders for the authenticated user, ordered by creation date (newest first).",
        OperationId = "GetOrders",
        Tags = new[] { "Orders" })]
    [SwaggerResponse(200, "Successfully retrieved orders", typeof(ApiResponse<PaginatedResponse<OrderDto>>))]
    [SwaggerResponse(401, "Unauthorized - Missing or invalid token", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetOrders(
        [FromQuery, SwaggerParameter("Page number for pagination", Required = false)] int page = 1,
        [FromQuery, SwaggerParameter("Number of items per page (max 50)", Required = false)] int pageSize = 10)
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

    /// <summary>
    /// Retrieves detailed information about a specific order
    /// </summary>
    /// <param name="id">The unique identifier of the order</param>
    /// <returns>Detailed order information</returns>
    /// <response code="200">Order found and returned</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="404">Order not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get order details by ID",
        Description = "Retrieves detailed information about a specific order including items, shipping information, and payment details.",
        OperationId = "GetOrderById",
        Tags = new[] { "Orders" })]
    [SwaggerResponse(200, "Order found successfully", typeof(ApiResponse<OrderDetailDto>))]
    [SwaggerResponse(401, "Unauthorized - Missing or invalid token", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Order not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetOrder([SwaggerParameter("Order unique identifier", Required = true)] Guid id)
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

    /// <summary>
    /// Creates a new order from the user's cart
    /// </summary>
    /// <param name="createOrderDto">Order creation data including payment method and shipping address</param>
    /// <returns>Created order details</returns>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Invalid order data or empty cart</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new order",
        Description = "Creates a new order from the authenticated user's cart items. The cart will be cleared after successful order creation.",
        OperationId = "CreateOrder",
        Tags = new[] { "Orders" })]
    [SwaggerResponse(201, "Order created successfully", typeof(ApiResponse<OrderDto>))]
    [SwaggerResponse(400, "Invalid order data or empty cart", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized - Missing or invalid token", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> CreateOrder([FromBody, SwaggerRequestBody("Order creation data", Required = true)] CreateOrderDto createOrderDto)
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
