using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ToyStore.EventBus.Abstractions;
using ToyStore.EventBus.Events;
using ToyStore.ProductService.Application.Commands.Products;
using ToyStore.ProductService.Application.DTOs;
using ToyStore.ProductService.Domain.Repositories;
using ToyStore.Shared.Models;
using ToyStore.Shared.Services;

namespace ToyStore.ProductService.Application.Handlers.Products;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly IEventBus _eventBus;
    private readonly ICacheService _cacheService;

    public UpdateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<UpdateProductHandler> logger,
        IEventBus eventBus,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
        _eventBus = eventBus;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing product
            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                return ApiResponse<ProductDto>.ErrorResult("Product not found", 404);
            }

            // Validate category exists if changed
            if (request.Product.CategoryId != existingProduct.CategoryId)
            {
                var category = await _categoryRepository.GetByIdAsync(request.Product.CategoryId);
                if (category == null)
                {
                    return ApiResponse<ProductDto>.ErrorResult("Category not found", 404);
                }
            }

            // Update product properties
            existingProduct.Name = request.Product.Name;
            existingProduct.Description = request.Product.Description;
            existingProduct.ShortDescription = request.Product.ShortDescription;
            existingProduct.Price = request.Product.Price;
            existingProduct.OriginalPrice = request.Product.OriginalPrice;
            existingProduct.Brand = request.Product.Brand;
            existingProduct.AgeRange = request.Product.AgeRange;
            existingProduct.StockQuantity = request.Product.StockQuantity;
            existingProduct.ImageUrls = request.Product.ImageUrls;
            existingProduct.Tags = request.Product.Tags;
            existingProduct.IsFeatured = request.Product.IsFeatured;
            existingProduct.IsActive = request.Product.IsActive;
            existingProduct.CategoryId = request.Product.CategoryId;
            existingProduct.UpdatedBy = request.UserId;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            // Update product
            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);

            // Map back to DTO
            var productDto = _mapper.Map<ProductDto>(updatedProduct);

            // Clear cache
            await _cacheService.RemoveAsync($"product_{request.Id}");
            await _cacheService.RemovePatternAsync("products_*");
            await _cacheService.RemovePatternAsync("featured_products_*");

            // Publish integration event
            var productUpdatedEvent = new ProductUpdatedEvent
            {
                ProductId = updatedProduct.Id,
                Name = updatedProduct.Name,
                Price = updatedProduct.Price,
                StockQuantity = updatedProduct.StockQuantity
            };

            await _eventBus.PublishAsync(productUpdatedEvent);

            _logger.LogInformation("Product updated successfully with ID: {ProductId}", updatedProduct.Id);

            return ApiResponse<ProductDto>.SuccessResult(productDto, "Product updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID: {ProductId}", request.Id);
            return ApiResponse<ProductDto>.ErrorResult("An error occurred while updating the product", 500);
        }
    }
}
