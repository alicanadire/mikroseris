using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ToyStore.ProductService.Application.DTOs;
using ToyStore.ProductService.Application.Queries.Products;
using ToyStore.ProductService.Domain.Repositories;
using ToyStore.Shared.Models;
using ToyStore.Shared.Services;

namespace ToyStore.ProductService.Application.Handlers.Products;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, ApiResponse<PaginatedResponse<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsHandler> _logger;
    private readonly ICacheService _cacheService;

    public GetProductsHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetProductsHandler> logger,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<PaginatedResponse<ProductDto>>> Handle(
        GetProductsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Create cache key
            var cacheKey = $"products_{request.Page}_{request.PageSize}_{request.SearchTerm}_{request.CategoryId}_{request.MinPrice}_{request.MaxPrice}_{request.Brand}_{request.InStock}_{request.SortBy}_{request.SortOrder}";
            
            // Try to get from cache first
            var cachedResult = await _cacheService.GetAsync<PaginatedResponse<ProductDto>>(cacheKey);
            if (cachedResult != null)
            {
                return ApiResponse<PaginatedResponse<ProductDto>>.SuccessResult(cachedResult);
            }

            // Get products from repository
            var (products, totalCount) = await _productRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.CategoryId,
                request.MinPrice,
                request.MaxPrice,
                request.Brand,
                request.InStock,
                request.SortBy,
                request.SortOrder);

            // Map to DTOs
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            // Create paginated response
            var paginatedResponse = PaginatedResponse<ProductDto>.Create(
                productDtos,
                totalCount,
                request.PageSize,
                request.Page);

            // Cache the result for 5 minutes
            await _cacheService.SetAsync(cacheKey, paginatedResponse, TimeSpan.FromMinutes(5));

            return ApiResponse<PaginatedResponse<ProductDto>>.SuccessResult(paginatedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return ApiResponse<PaginatedResponse<ProductDto>>.ErrorResult(
                "An error occurred while retrieving products", 500);
        }
    }
}

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdHandler> _logger;
    private readonly ICacheService _cacheService;

    public GetProductByIdHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetProductByIdHandler> logger,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"product_{request.Id}";
            
            // Try cache first
            var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
            if (cachedProduct != null)
            {
                return ApiResponse<ProductDto>.SuccessResult(cachedProduct);
            }

            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return ApiResponse<ProductDto>.ErrorResult("Product not found", 404);
            }

            var productDto = _mapper.Map<ProductDto>(product);
            
            // Cache for 10 minutes
            await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(10));

            return ApiResponse<ProductDto>.SuccessResult(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product with ID: {ProductId}", request.Id);
            return ApiResponse<ProductDto>.ErrorResult("An error occurred while retrieving the product", 500);
        }
    }
}

public class GetFeaturedProductsHandler : IRequestHandler<GetFeaturedProductsQuery, ApiResponse<List<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetFeaturedProductsHandler> _logger;
    private readonly ICacheService _cacheService;

    public GetFeaturedProductsHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetFeaturedProductsHandler> logger,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<List<ProductDto>>> Handle(
        GetFeaturedProductsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"featured_products_{request.Count}";
            
            var cachedProducts = await _cacheService.GetAsync<List<ProductDto>>(cacheKey);
            if (cachedProducts != null)
            {
                return ApiResponse<List<ProductDto>>.SuccessResult(cachedProducts);
            }

            var products = await _productRepository.GetFeaturedAsync();
            var featuredProducts = products.Take(request.Count).ToList();
            
            var productDtos = _mapper.Map<List<ProductDto>>(featuredProducts);
            
            // Cache for 30 minutes
            await _cacheService.SetAsync(cacheKey, productDtos, TimeSpan.FromMinutes(30));

            return ApiResponse<List<ProductDto>>.SuccessResult(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured products");
            return ApiResponse<List<ProductDto>>.ErrorResult("An error occurred while retrieving featured products", 500);
        }
    }
}
