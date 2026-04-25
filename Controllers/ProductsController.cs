using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductManagement.Models;
using ProductManagement.Services;
using System.Security.Claims;
using OfficeOpenXml;

namespace ProductManagement.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        public ProductsController(ProductService productService, CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var products = await _productService.GetAllAsync(userId, page);
            var total = await _productService.CountAsync(userId);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / 10.0);

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(product);
            }
            if(imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);

                var forlderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images/products");

                var filePath = Path.Combine( forlderPath, fileName );

                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                product.ImagePath = "/images/products/" + fileName;
            }

            product.UserId = userId;
            product.CreatedBy = userId;
            product.CreatedDate = DateTime.Now;

            await _productService.CreateAsync(product);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _productService.GetByIdAsync(id, userId);

            if (product == null)
                return NotFound();

            await LoadCategories();

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(product);
            }

            var existing = await _productService.GetByIdAsync(product.ProductId, userId);

            if (existing == null)
                return NotFound();

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.CategoryId = product.CategoryId;

            existing.UpdatedBy = userId;
            existing.UpdatedDate = DateTime.Now;

            await _productService.UpdateAsync(existing);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _productService.GetByIdAsync(id, userId);

            if(product == null)
                return NotFound();

            await _productService.DeleteAsync(product);

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var categories = await _categoryService.GetAllAsync(userId!);

            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "Name");
        }
        public IActionResult UploadExcel()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (file == null || file.Length == 0)
                {
                    ViewBag.Error = "Please select an Excel file.";
                
                    return View();
                }
               

            ExcelPackage.License.SetNonCommercialPersonal("Marv");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            int rowCount = worksheet.Dimension.Rows;

            for(int row = 2; row <= rowCount; row++)
            {
                var product = new Product
                {
                    Name = worksheet.Cells[row, 1].Text,
                    Description = worksheet.Cells[row, 2].Text,
                    Price = decimal.Parse(worksheet.Cells[row, 3].Text),
                    CategoryId = int.Parse(worksheet.Cells[row, 4].Text),
                    UserId = userId,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now
                };
                await _productService.CreateAsync(product);
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ExportExcel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var products = await _productService.GetAllForExportAsync(userId);

            ExcelPackage.License.SetNonCommercialPersonal("Marv");

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Products");

            ws.Cells[1, 1].Value = "Code";
            ws.Cells[1, 2].Value = "Name";
            ws.Cells[1, 3].Value = "Price";
            ws.Cells[1, 4].Value = "CategoryId";

            int row = 2;

            foreach (var item in products)
            {
                ws.Cells[row, 1].Value = item.ProductCode;
                ws.Cells[row, 2].Value = item.Name;
                ws.Cells[row, 3].Value = item.Price;
                ws.Cells[row, 4].Value = item.CategoryId;
                row++;
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            var bytes = package.GetAsByteArray();

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Products.xlsx");
        }
    }
}
