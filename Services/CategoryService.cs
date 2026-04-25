using ProductManagement.Interfaces;
using ProductManagement.Models;

namespace ProductManagement.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo) {  _repo = repo; }
        public async Task<List<Category>> GetAllAsync(string userId)
        {
            return await _repo.GetAllAsync(userId);
        }

        public async Task<Category?> GetByIdAsync(int id, string userId)
        {
            return await _repo.GetByIdAsync(id, userId);
        }

        public async Task CreateAsync(Category category)
        {
            await _repo.AddAsync(category);
            await _repo.SaveAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            await _repo.UpdateAsync(category);
            await _repo.SaveAsync();
        }
        public async Task<bool> CodeExistsAsync(string userId, string code)
        {
            return await _repo.ExistsCodeAsync(userId, code);
        }
        public async Task<bool> CodeExistsForOtherAsync(int categoryId, string userId, string code)
        {
            return await _repo.ExistsCodeForOtherAsync(categoryId, userId, code);
        }
    }
}
