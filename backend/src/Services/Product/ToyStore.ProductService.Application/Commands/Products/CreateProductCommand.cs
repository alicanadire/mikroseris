using MediatR;
using ToyStore.ProductService.Application.DTOs;
using ToyStore.Shared.Models;

namespace ToyStore.ProductService.Application.Commands.Products;

public class CreateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public ProductCreateDto Product { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
}
