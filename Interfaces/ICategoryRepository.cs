using ProductManagement.Models;

namespace ProductManagement.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(string userId);
        Task<Category?> GetByIdAsync(int id, string userId);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task SaveAsync();
        Task<bool> ExistsCodeAsync(string userId, string code);
        Task<bool> ExistsCodeForOtherAsync(int categoryId, string userId, string code);
    }
}
