using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ToyStore.EventBus.Abstractions;
using ToyStore.EventBus.Events;
using ToyStore.ProductService.Application.Commands.Products;
using ToyStore.ProductService.Application.DTOs;
using ToyStore.ProductService.Domain.Entities;
using ToyStore.ProductService.Domain.Repositories;
using ToyStore.Shared.Models;

namespace ToyStore.ProductService.Application.Handlers.Products;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IEventBus _eventBus;

    public CreateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<CreateProductHandler> logger,
        IEventBus eventBus)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
        _eventBus = eventBus;
    }

    public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(request.Product.CategoryId);
            if (category == null)
            {
                return ApiResponse<ProductDto>.ErrorResult("Category not found", 404);
            }

            // Map DTO to entity
            var product = _mapper.Map<Product>(request.Product);
            product.CreatedBy = request.UserId;
            product.UpdatedBy = request.UserId;

            // Create product
            var createdProduct = await _productRepository.AddAsync(product);

            // Map back to DTO
            var productDto = _mapper.Map<ProductDto>(createdProduct);

            // Publish integration event
            var productCreatedEvent = new ProductCreatedEvent
            {
                ProductId = createdProduct.Id,
                Name = createdProduct.Name,
                Price = createdProduct.Price,
                StockQuantity = createdProduct.StockQuantity,
                CategoryId = createdProduct.CategoryId.ToString()
            };

            await _eventBus.PublishAsync(productCreatedEvent);

            _logger.LogInformation("Product created successfully with ID: {ProductId}", createdProduct.Id);

            return ApiResponse<ProductDto>.SuccessResult(productDto, "Product created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return ApiResponse<ProductDto>.ErrorResult("An error occurred while creating the product", 500);
        }
    }
}
