using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize]
    public class CategoryProductController : Controller
    {
        private readonly ICategoryProductService _categoryProductService;
        private readonly INotificationService _notificationService;

        public CategoryProductController(ICategoryProductService categoryProductService, INotificationService notificationService)
        {
            _categoryProductService = categoryProductService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryProductService.GetAll();
            return View(categories);
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
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _categoryProductService.Delete(id);
            await _notificationService.NotifyEntityChanged("CategoryProduct");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryProductService.GetById(id);
            if (category == null) return NotFound();

            var model = new UpdateCategoryProductDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
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
            return RedirectToAction("Index");
        }
    }
}