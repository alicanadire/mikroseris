using ToyStore.ProductService.Domain.Entities;

namespace ToyStore.ProductService.Domain.Repositories;

public interface IProductReviewRepository
{
    Task<ProductReview?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId, bool approvedOnly = true);
    Task<(IEnumerable<ProductReview> Reviews, int TotalCount)> GetPagedByProductIdAsync(
        Guid productId, 
        int page, 
        int pageSize, 
        bool approvedOnly = true);
    Task<ProductReview> AddAsync(ProductReview review);
    Task<ProductReview> UpdateAsync(ProductReview review);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> UserHasReviewedProductAsync(string userId, Guid productId);
    Task<double> GetAverageRatingAsync(Guid productId);
    Task<int> GetReviewCountAsync(Guid productId, bool approvedOnly = true);
}
