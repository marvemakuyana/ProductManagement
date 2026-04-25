using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProductManagement.Models;
using ProductManagement.Services;
using System.Security.Claims;

namespace ProductManagement.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly CategoryService _service;

        public CategoriesController(CategoryService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized();
            
            var categories = await _service.GetAllAsync(userId);
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if(!ModelState.IsValid)
                return View(category);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if(await _service.CodeExistsAsync(userId, category.CategoryCode))
            {
                ModelState.AddModelError("CategoryCode", "This category code already esists.");
                return View(category);
            }

            category.UserId = userId;
            category.CreatedBy = userId;
            category.CreatedDate = DateTime.Now;

            await _service.CreateAsync(category);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var category = await _service.GetByIdAsync(id, userId);

            if (category == null) 
                return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if(!ModelState.IsValid) 
                return View(category);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if(await _service.CodeExistsForOtherAsync(category.CategoryId, userId, category.CategoryCode))
            {
                ModelState.AddModelError("CategoryCode", "This category code already exists");
                return View(category);
            }
            var existing = await _service.GetByIdAsync(category.CategoryId, userId);
            if (existing == null)
                return NotFound();


            existing.Name = category.Name;
            existing.CategoryCode = category.CategoryCode;
            existing.IsActive = category.IsActive;

            existing.UpdatedBy = userId;
            existing.UpdatedDate = DateTime.Now;

            await _service.UpdateAsync(existing);
            return RedirectToAction(nameof(Index));
        }
    }
}
