using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Interfaces;
using ProductManagement.Models;

namespace ProductManagement.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Product>> GetAllAsync(string userId, int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.ProductId)
                .Skip((page -1)* pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<List<Product>> GetAllForExportAsync(string userId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.ProductId)
                .ToListAsync();
        }
        public async Task<int> CountAsync(string userId)
        {
            return await _context.Products
                .CountAsync(p => p.UserId == userId);
        }

        public async Task<Product?> GetByIdAsync(int id, string userId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id && p.UserId == userId);
        }
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }
        public Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            return Task.CompletedTask;
        }
        public async Task<int> CountByMonthAsync(string yearMonth, string userId)
        {
            return await _context.Products
                .CountAsync(p =>
                p.UserId == userId &&
                p.ProductCode.StartsWith(yearMonth));
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
