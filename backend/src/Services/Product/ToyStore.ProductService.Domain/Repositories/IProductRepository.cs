using ToyStore.ProductService.Domain.Entities;

namespace ToyStore.ProductService.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
    Task<IEnumerable<Product>> GetFeaturedAsync();
    Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? brand = null,
        bool? inStock = null,
        string? sortBy = null,
        string? sortOrder = null);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetBySlugAsync(string slug);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> AddAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

public interface IProductReviewRepository
{
    Task<ProductReview?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId);
    Task<(IEnumerable<ProductReview> Reviews, int TotalCount)> GetPagedByProductIdAsync(
        Guid productId, int page, int pageSize);
    Task<ProductReview> AddAsync(ProductReview review);
    Task<ProductReview> UpdateAsync(ProductReview review);
    Task DeleteAsync(Guid id);
    Task<double> GetAverageRatingAsync(Guid productId);
    Task<int> GetReviewCountAsync(Guid productId);
}
