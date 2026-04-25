using ProductManagement.Interfaces;
using ProductManagement.Models;

namespace ProductManagement.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<Product>> GetAllAsync(string userId, int page)
        {
            return await _repo.GetAllAsync(userId, page,10);
        }
        public async Task<List<Product>> GetAllForExportAsync(string userId)
        {
            return await _repo.GetAllForExportAsync(userId);
        }
        public async Task<int> CountAsync(string userId)
        {
            return await _repo.CountAsync(userId);
        }
        public async Task<Product?> GetByIdAsync(int id, string userId)
        {
            return await _repo.GetByIdAsync(id, userId);
        }
        public async Task CreateAsync(Product product)
        {
            var yearMonth = DateTime.Now.ToString("yyyyMM");
            var count = await _repo.CountByMonthAsync(yearMonth, product.UserId);
            product.ProductCode = $"{yearMonth}-{(count + 1):000}";

            await _repo.AddAsync(product);
            await _repo.SaveAsync();
        }
        public async Task UpdateAsync(Product product)
        {
            await _repo.UpdateAsync(product);
            await _repo.SaveAsync();
        }
        public async Task DeleteAsync(Product product)
        {
            await _repo.DeleteAsync(product);
            await _repo.SaveAsync();
        }
    }
}
