using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Models;
using ProductManagement.Services;
using System.Security.Claims;

namespace ProductManagement.Controllers.Api
{
    [Route("api/products")]
    [ApiController]
    [Authorize]
    public class ProductsApiController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsApiController(ProductService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var products = await _service.GetAllAsync(userId,page);
            
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _service.GetByIdAsync(id, userId!);

            if(product == null)
                return NotFound();
           
            return Ok(product);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            product.UserId = userId!;
            product.CreatedBy = userId!;
            product.CreatedDate = DateTime.Now;

            await _service.CreateAsync(product);

            return Ok(product);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (id != product.ProductId)
                return BadRequest("ID mismatch.");

            var existing = await _service.GetByIdAsync(id, userId);

            if (existing == null)
                return NotFound();

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.CategoryId = product.CategoryId;

            existing.UpdatedBy = userId;
            existing.UpdatedDate = DateTime.Now;

            await _service.UpdateAsync(existing);

            return Ok(existing);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await _service.GetByIdAsync(id, userId!);

            if (product == null)
                return NotFound();

            await _service.DeleteAsync(product);

            return Ok();

        }
    }
}
