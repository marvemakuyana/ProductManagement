using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Models;
using ProductManagement.Services;
using System.Security.Claims;

namespace ProductManagement.Controllers.Api
{
    [Authorize]
    [Route("api/categories")]
    [ApiController]
    public class CategoriesApiController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoriesApiController(CategoryService service)
        {
            _service = service;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var categories = await _service.GetAllAsync(userId);

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var category = await _service.GetByIdAsync(id, userId);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            category.UserId = userId;
            category.CreatedBy = userId;
            category.CreatedDate = DateTime.Now;

            await _service.CreateAsync(category);

            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (id != category.CategoryId)
                return BadRequest();

            var existing = await _service.GetByIdAsync(id, userId);

            if (existing == null)
                return NotFound();

            existing.Name = category.Name;
            existing.CategoryCode = category.CategoryCode;
            existing.IsActive = category.IsActive;
            existing.UpdatedBy = userId;
            existing.UpdatedDate = DateTime.Now;

            await _service.UpdateAsync(existing);

            return Ok(existing);
        }
    }
}