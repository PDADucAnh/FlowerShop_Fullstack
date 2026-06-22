using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _categoryService.GetAll();
            return View(data);
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
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetById(id);

            if (category == null) return NotFound();

            var model = new UpdateCategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCategoryDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.Update(model.Id, model);
            return RedirectToAction("Index");
        }
    }
}
