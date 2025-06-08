using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using ToyStore.ProductService.Application.Commands.Products;
using ToyStore.ProductService.Application.DTOs;
using ToyStore.ProductService.Application.Queries.Products;

namespace ToyStore.ProductService.API.Controllers;

/// <summary>
/// Products management controller for ToyStore e-commerce platform
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[SwaggerTag("Products management including CRUD operations, search, and categorization")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated list of products with advanced filtering and sorting options
    /// </summary>
    /// <param name="page">Page number for pagination (starts from 1)</param>
    /// <param name="pageSize">Number of items per page (maximum 50)</param>
    /// <param name="searchTerm">Search term to filter products by name or description</param>
    /// <param name="categoryId">Filter products by category ID</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="brand">Filter products by brand name</param>
    /// <param name="inStock">Filter products by stock availability</param>
    /// <param name="sortBy">Sort field (name, price, createdAt, rating)</param>
    /// <param name="sortOrder">Sort order (asc, desc)</param>
    /// <returns>Paginated list of products</returns>
    /// <response code="200">Returns the paginated product list</response>
    /// <response code="400">Bad request parameters</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get products with filtering and pagination",
        Description = "Retrieves products with advanced filtering, sorting, and pagination capabilities. Supports search by name, category filtering, price range, and stock status.",
        OperationId = "GetProducts",
        Tags = new[] { "Products" })]
    [SwaggerResponse(200, "Successfully retrieved products", typeof(ApiResponse<PaginatedResponse<ProductDto>>))]
    [SwaggerResponse(400, "Invalid request parameters", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetProducts(
        [FromQuery, SwaggerParameter("Page number for pagination", Required = false)] int page = 1,
        [FromQuery, SwaggerParameter("Number of items per page (max 50)", Required = false)] int pageSize = 12,
        [FromQuery, SwaggerParameter("Search term for product name or description", Required = false)] string? searchTerm = null,
        [FromQuery, SwaggerParameter("Category ID to filter products", Required = false)] Guid? categoryId = null,
        [FromQuery, SwaggerParameter("Minimum price filter", Required = false)] decimal? minPrice = null,
        [FromQuery, SwaggerParameter("Maximum price filter", Required = false)] decimal? maxPrice = null,
        [FromQuery, SwaggerParameter("Brand name filter", Required = false)] string? brand = null,
        [FromQuery, SwaggerParameter("Stock availability filter", Required = false)] bool? inStock = null,
        [FromQuery, SwaggerParameter("Sort field: name, price, createdAt, rating", Required = false)] string? sortBy = null,
        [FromQuery, SwaggerParameter("Sort order: asc, desc", Required = false)] string? sortOrder = null)
    {
        var query = new GetProductsQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Brand = brand,
            InStock = inStock,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific product by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the product</param>
    /// <returns>Product details</returns>
    /// <response code="200">Product found and returned</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get product by ID",
        Description = "Retrieves detailed information about a specific product using its unique identifier.",
        OperationId = "GetProductById",
        Tags = new[] { "Products" })]
    [SwaggerResponse(200, "Product found successfully", typeof(ApiResponse<ProductDetailDto>))]
    [SwaggerResponse(404, "Product not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetProduct([SwaggerParameter("Product unique identifier", Required = true)] Guid id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a list of featured products for homepage display
    /// </summary>
    /// <param name="count">Number of featured products to return (maximum 20)</param>
    /// <returns>List of featured products</returns>
    /// <response code="200">Featured products retrieved successfully</response>
    /// <response code="400">Invalid count parameter</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("featured")]
    [SwaggerOperation(
        Summary = "Get featured products",
        Description = "Retrieves a curated list of featured products typically displayed on the homepage or promotional sections.",
        OperationId = "GetFeaturedProducts",
        Tags = new[] { "Products" })]
    [SwaggerResponse(200, "Featured products retrieved successfully", typeof(ApiResponse<List<ProductDto>>))]
    [SwaggerResponse(400, "Invalid count parameter", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetFeaturedProducts([FromQuery, SwaggerParameter("Number of featured products (max 20)", Required = false)] int count = 8)
    {
        var query = new GetFeaturedProductsQuery { Count = count };
        var result = await _mediator.Send(query);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new product in the system
    /// </summary>
    /// <param name="productDto">Product creation data</param>
    /// <returns>Created product details</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid product data</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Create a new product",
        Description = "Creates a new product in the system. Requires admin role authorization.",
        OperationId = "CreateProduct",
        Tags = new[] { "Products" })]
    [SwaggerResponse(201, "Product created successfully", typeof(ApiResponse<ProductDto>))]
    [SwaggerResponse(400, "Invalid product data", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized - Missing or invalid token", typeof(ApiResponse<object>))]
    [SwaggerResponse(403, "Forbidden - Insufficient permissions", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> CreateProduct([FromBody, SwaggerRequestBody("Product creation data", Required = true)] ProductCreateDto productDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

        var command = new CreateProductCommand
        {
            Product = productDto,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }

        return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product unique identifier</param>
    /// <param name="productDto">Product update data</param>
    /// <returns>Updated product details</returns>
    /// <response code="200">Product updated successfully</response>
    /// <response code="400">Invalid product data or ID</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Update an existing product",
        Description = "Updates product information. Requires admin role authorization.",
        OperationId = "UpdateProduct",
        Tags = new[] { "Products" })]
    [SwaggerResponse(200, "Product updated successfully", typeof(ApiResponse<ProductDto>))]
    [SwaggerResponse(400, "Invalid product data or ID", typeof(ApiResponse<object>))]
    [SwaggerResponse(401, "Unauthorized - Missing or invalid token", typeof(ApiResponse<object>))]
    [SwaggerResponse(403, "Forbidden - Insufficient permissions", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Product not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> UpdateProduct(
        [SwaggerParameter("Product unique identifier", Required = true)] Guid id,
        [FromBody, SwaggerRequestBody("Product update data", Required = true)] ProductUpdateDto productDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

        var command = new UpdateProductCommand
        {
            Id = id,
            Product = productDto,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes a product from the system (soft delete)
    /// </summary>
    /// <param name="id">Product unique identifier</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">Product deleted successfully</response>
    /// <response code="401">Unauthorized access</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(
        Summary = "Delete a product",
        Description = "Soft deletes a product from the system. Requires admin role authorization.",
        OperationId = "DeleteProduct",
        Tags = new[] { "Products" })]
    [SwaggerResponse(204, "Product deleted successfully")]
    [SwaggerResponse(401, "Unauthorized - Missing or invalid token", typeof(ApiResponse<object>))]
    [SwaggerResponse(403, "Forbidden - Insufficient permissions", typeof(ApiResponse<object>))]
    [SwaggerResponse(404, "Product not found", typeof(ApiResponse<object>))]
    [SwaggerResponse(500, "Internal server error", typeof(ApiResponse<object>))]
    public async Task<IActionResult> DeleteProduct([SwaggerParameter("Product unique identifier", Required = true)] Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

        var command = new DeleteProductCommand
        {
            Id = id,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result);
        }

        return NoContent();
    }
}
