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
    [Authorize]
    public class CategoryProductController : Controller
    {
        private readonly ICategoryProductService _categoryProductService;
        private readonly INotificationService _notificationService;
        private readonly IApplicationDbContext _context;

        public CategoryProductController(ICategoryProductService categoryProductService, INotificationService notificationService, IApplicationDbContext context)
        {
            _categoryProductService = categoryProductService;
            _notificationService = notificationService;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            var paged = await _categoryProductService.GetPaged(page, pageSize);
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["CurrentPage"] = paged.Page;
            ViewData["TotalCount"] = paged.TotalCount;
            ViewData["PageSize"] = paged.PageSize;
            return View(paged.Items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryProductDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryProductService.Create(model);
            await _notificationService.NotifyEntityChanged("CategoryProduct");
            TempData["Success"] = "Danh mục sản phẩm đã được tạo thành công.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _categoryProductService.Delete(id);
            await _notificationService.NotifyEntityChanged("CategoryProduct");
            TempData["Success"] = "Danh mục sản phẩm đã được xóa.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.CategoriesProducts
                .FirstOrDefaultAsync(cp => cp.Id == id);
            if (category == null) return NotFound();

            var model = new UpdateCategoryProductDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCategoryProductDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryProductService.Update(model.Id, model);
            await _notificationService.NotifyEntityChanged("CategoryProduct");
            TempData["Success"] = "Danh mục sản phẩm đã được cập nhật.";
            return RedirectToAction("Index");
        }
    }
}