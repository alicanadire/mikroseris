using Microsoft.EntityFrameworkCore;
using ToyStore.ProductService.Domain.Entities;
using ToyStore.ProductService.Domain.Repositories;
using ToyStore.ProductService.Infrastructure.Data;

namespace ToyStore.ProductService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Reviews.Where(r => r.IsApproved))
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => !p.IsDeleted && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && !p.IsDeleted && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetFeaturedAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsFeatured && !p.IsDeleted && p.IsActive)
            .OrderByDescending(p => p.Rating)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? brand = null,
        bool? inStock = null,
        string? sortBy = null,
        string? sortOrder = null)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => !p.IsDeleted && p.IsActive);

        // Apply filters
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || 
                                   p.Description.Contains(searchTerm) || 
                                   p.Brand.Contains(searchTerm));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (!string.IsNullOrEmpty(brand))
        {
            query = query.Where(p => p.Brand == brand);
        }

        if (inStock.HasValue && inStock.Value)
        {
            query = query.Where(p => p.StockQuantity > 0);
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "price" => sortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            "rating" => query.OrderByDescending(p => p.Rating),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            "name" => sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            _ => query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync();
        
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Products
            .AnyAsync(p => p.Id == id && !p.IsDeleted);
    }
}
