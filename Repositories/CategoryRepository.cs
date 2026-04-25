using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Interfaces;
using ProductManagement.Models;

namespace ProductManagement.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) 
        { 
            _context = context;
        }
        public async Task<List<Category>> GetAllAsync(string userId)
        {
            return await _context.Categories.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id, string userId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
        }

        public async Task AddAsync(Category category) { 
        await _context.Categories.AddAsync(category);
        }

        public Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            return Task.CompletedTask;
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsCodeAsync(string userId, string code)
        {
            return await _context.Categories
                .AnyAsync(c => c.UserId == userId && c.CategoryCode == code);
        }
        public async Task<bool> ExistsCodeForOtherAsync(int categoryId, string userId, string code)
        {
            return await _context.Categories.AnyAsync(c =>
                c.CategoryId != categoryId &&
                c.UserId == userId &&
                c.CategoryCode == code);
        }
    }
}
