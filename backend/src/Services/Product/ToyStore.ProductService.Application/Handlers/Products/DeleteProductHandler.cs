using MediatR;
using Microsoft.Extensions.Logging;
using ToyStore.EventBus.Abstractions;
using ToyStore.EventBus.Events;
using ToyStore.ProductService.Application.Commands.Products;
using ToyStore.ProductService.Domain.Repositories;
using ToyStore.Shared.Models;
using ToyStore.Shared.Services;

namespace ToyStore.ProductService.Application.Handlers.Products;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeleteProductHandler> _logger;
    private readonly IEventBus _eventBus;
    private readonly ICacheService _cacheService;

    public DeleteProductHandler(
        IProductRepository productRepository,
        ILogger<DeleteProductHandler> logger,
        IEventBus eventBus,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _logger = logger;
        _eventBus = eventBus;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing product
            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                return ApiResponse<bool>.ErrorResult("Product not found", 404);
            }

            // Soft delete the product
            existingProduct.IsDeleted = true;
            existingProduct.UpdatedBy = request.UserId;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(existingProduct);

            // Clear cache
            await _cacheService.RemoveAsync($"product_{request.Id}");
            await _cacheService.RemovePatternAsync("products_*");
            await _cacheService.RemovePatternAsync("featured_products_*");

            // Publish integration event
            var productDeletedEvent = new ProductDeletedEvent
            {
                ProductId = existingProduct.Id
            };

            await _eventBus.PublishAsync(productDeletedEvent);

            _logger.LogInformation("Product deleted successfully with ID: {ProductId}", request.Id);

            return ApiResponse<bool>.SuccessResult(true, "Product deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID: {ProductId}", request.Id);
            return ApiResponse<bool>.ErrorResult("An error occurred while deleting the product", 500);
        }
    }
}
