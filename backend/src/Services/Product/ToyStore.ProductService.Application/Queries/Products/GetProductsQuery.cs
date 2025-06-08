using MediatR;
using ToyStore.ProductService.Application.DTOs;
using ToyStore.Shared.Models;

namespace ToyStore.ProductService.Application.Queries.Products;

public class GetProductsQuery : IRequest<ApiResponse<PaginatedResponse<ProductDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Brand { get; set; }
    public bool? InStock { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

public class GetProductByIdQuery : IRequest<ApiResponse<ProductDto>>
{
    public Guid Id { get; set; }
}

public class GetFeaturedProductsQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public int Count { get; set; } = 8;
}
