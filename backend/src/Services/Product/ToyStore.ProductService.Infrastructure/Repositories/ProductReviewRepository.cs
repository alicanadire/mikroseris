using Microsoft.EntityFrameworkCore;
using ToyStore.ProductService.Domain.Entities;
using ToyStore.ProductService.Domain.Repositories;
using ToyStore.ProductService.Infrastructure.Data;

namespace ToyStore.ProductService.Infrastructure.Repositories;

public class ProductReviewRepository : IProductReviewRepository
{
    private readonly ProductDbContext _context;

    public ProductReviewRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<ProductReview?> GetByIdAsync(Guid id)
    {
        return await _context.ProductReviews
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(Guid productId, bool approvedOnly = true)
    {
        var query = _context.ProductReviews
            .Where(r => r.ProductId == productId && !r.IsDeleted);

        if (approvedOnly)
        {
            query = query.Where(r => r.IsApproved);
        }

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<ProductReview> Reviews, int TotalCount)> GetPagedByProductIdAsync(
        Guid productId, 
        int page, 
        int pageSize, 
        bool approvedOnly = true)
    {
        var query = _context.ProductReviews
            .Where(r => r.ProductId == productId && !r.IsDeleted);

        if (approvedOnly)
        {
            query = query.Where(r => r.IsApproved);
        }

        var totalCount = await query.CountAsync();

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reviews, totalCount);
    }

    public async Task<ProductReview> AddAsync(ProductReview review)
    {
        _context.ProductReviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<ProductReview> UpdateAsync(ProductReview review)
    {
        review.UpdatedAt = DateTime.UtcNow;
        _context.ProductReviews.Update(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task DeleteAsync(Guid id)
    {
        var review = await _context.ProductReviews.FindAsync(id);
        if (review != null)
        {
            review.IsDeleted = true;
            review.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.ProductReviews
            .AnyAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<bool> UserHasReviewedProductAsync(string userId, Guid productId)
    {
        return await _context.ProductReviews
            .AnyAsync(r => r.UserId == userId && r.ProductId == productId && !r.IsDeleted);
    }

    public async Task<double> GetAverageRatingAsync(Guid productId)
    {
        var reviews = await _context.ProductReviews
            .Where(r => r.ProductId == productId && r.IsApproved && !r.IsDeleted)
            .ToListAsync();

        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }

    public async Task<int> GetReviewCountAsync(Guid productId, bool approvedOnly = true)
    {
        var query = _context.ProductReviews
            .Where(r => r.ProductId == productId && !r.IsDeleted);

        if (approvedOnly)
        {
            query = query.Where(r => r.IsApproved);
        }

        return await query.CountAsync();
    }
}
