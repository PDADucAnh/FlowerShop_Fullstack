using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryProductService _categoryProductService;
        private readonly INotificationService _notificationService;

        public ProductController(IProductService productService, ICategoryProductService categoryProductService, INotificationService notificationService)
        {
            _productService = productService;
            _categoryProductService = categoryProductService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAll();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryProductService.GetAll();
            ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDTO model, IFormFile uploadImage)
        {
            if (uploadImage != null && uploadImage.Length > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadImage.CopyToAsync(stream);
                }

                model.ImageUrl = "/uploads/products/" + fileName;
            }

            if (!ModelState.IsValid)
            {
                var categories = await _categoryProductService.GetAll();
                ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", model.CategoryProductId);
                return View(model);
            }

            await _productService.Create(model);
            await _notificationService.NotifyEntityChanged("Product");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _productService.Delete(id);
            await _notificationService.NotifyEntityChanged("Product");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetDetail(id);
            if (product == null) return NotFound();

            var categories = await _categoryProductService.GetAll();
            ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", product.CategoryProductId);

            var model = new UpdateProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl ?? string.Empty,
                CategoryProductId = product.CategoryProductId
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetDetail(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProductDTO model, IFormFile uploadImage)
        {
            if (uploadImage != null && uploadImage.Length > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadImage.CopyToAsync(stream);
                }

                model.ImageUrl = "/uploads/products/" + fileName;
            }
            else
            {
                var oldProduct = await _productService.GetDetail(model.Id);
                if (oldProduct != null && string.IsNullOrEmpty(model.ImageUrl))
                {
                    model.ImageUrl = oldProduct.ImageUrl ?? string.Empty;
                }
            }

            if (!ModelState.IsValid)
            {
                var categories = await _categoryProductService.GetAll();
                ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", model.CategoryProductId);
                return View(model);
            }

            await _productService.Update(model.Id, model);
            await _notificationService.NotifyEntityChanged("Product");
            return RedirectToAction("Index");
        }
    }
}