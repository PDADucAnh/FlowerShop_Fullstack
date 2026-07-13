using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IApplicationDbContext _context;

        public CategoryController(ICategoryService categoryService, IApplicationDbContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            var paged = await _categoryService.GetPaged(page, pageSize);
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["CurrentPage"] = paged.Page;
            ViewData["TotalCount"] = paged.TotalCount;
            ViewData["PageSize"] = paged.PageSize;
            return View(paged.Items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetById(id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.Create(model);
            TempData["Success"] = "Danh mục blog đã được tạo thành công.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.Delete(id);
            TempData["Success"] = "Danh mục blog đã được xóa.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            var model = new UpdateCategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCategoryDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.Update(model.Id, model);
            TempData["Success"] = "Danh mục blog đã được cập nhật.";
            return RedirectToAction("Index");
        }
    }
}
