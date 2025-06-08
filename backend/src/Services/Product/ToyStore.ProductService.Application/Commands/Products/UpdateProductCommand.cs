using MediatR;
using ToyStore.ProductService.Application.DTOs;
using ToyStore.Shared.Models;

namespace ToyStore.ProductService.Application.Commands.Products;

public class UpdateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public Guid Id { get; set; }
    public ProductUpdateDto Product { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
}
