using MediatR;
using ToyStore.Shared.Models;

namespace ToyStore.ProductService.Application.Commands.Products;

public class DeleteProductCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
}
