using ProductManagement.Models;

namespace ProductManagement.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(string userId, int page, int pageSize);
        Task<List<Product>> GetAllForExportAsync(string userId);
        Task<int> CountAsync(string userId);
        Task<Product?> GetByIdAsync(int  id, string userId);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<int> CountByMonthAsync(string yearMonth, string userId);
        Task SaveAsync();
    }
}
