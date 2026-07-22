using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Utils;
using Flower.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryProductService _categoryProductService;
        private readonly INotificationService _notificationService;
        private readonly IApplicationDbContext _context;
        private readonly IPhotoService _photoService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ICategoryProductService categoryProductService, INotificationService notificationService, IApplicationDbContext context, IPhotoService photoService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _categoryProductService = categoryProductService;
            _notificationService = notificationService;
            _context = context;
            _photoService = photoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            var paged = await _productService.GetPaged(page, pageSize);
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["CurrentPage"] = paged.Page;
            ViewData["TotalCount"] = paged.TotalCount;
            ViewData["PageSize"] = paged.PageSize;
            return View(paged.Items);
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
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                var categories = await _categoryProductService.GetAll();
                ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", model.CategoryProductId);
                return View(model);
            }

            _logger.LogInformation("Create POST: uploadImage={IsNull}, uploadImageLength={Length}, ModelStateValid={Valid}",
                uploadImage == null ? "null" : "not null",
                uploadImage?.Length ?? 0,
                ModelState.IsValid);

            if (uploadImage != null && uploadImage.Length > 0)
            {
                try
                {
                    var validateStream = uploadImage.OpenReadStream();
                    using var _ = Image.Load(validateStream);
                    validateStream.Position = 0;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Image validation failed");
                    ModelState.AddModelError("uploadImage", "File không hợp lệ. Chỉ chấp nhận file ảnh.");
                    TempData["Error"] = "File ảnh không hợp lệ.";
                    var categories = await _categoryProductService.GetAll();
                    ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", model.CategoryProductId);
                    return View(model);
                }

                model.ImageUrl = await _photoService.UploadPhotoAsync(uploadImage);
                if (string.IsNullOrEmpty(model.ImageUrl))
                {
                    _logger.LogWarning("UploadPhotoAsync returned null for file {FileName}", uploadImage.FileName);
                    ModelState.AddModelError("uploadImage", "Upload ảnh thất bại. Vui lòng kiểm tra cấu hình Cloudinary.");
                    TempData["Error"] = "Upload ảnh thất bại. Vui lòng thử lại hoặc kiểm tra cấu hình Cloudinary.";
                    var categories = await _categoryProductService.GetAll();
                    ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", model.CategoryProductId);
                    return View(model);
                }
            }

            await _productService.Create(model);
            await _notificationService.NotifyEntityChanged("Product");
            TempData["Success"] = "Sản phẩm đã được tạo thành công.";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.Delete(id);
            await _notificationService.NotifyEntityChanged("Product");
            TempData["Success"] = "Sản phẩm đã được xóa.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var categories = await _categoryProductService.GetAll();
            ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var model = new UpdateProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Slug = product.Slug,
                Price = product.Price,
                Sku = product.Sku,
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
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                var categories = await _categoryProductService.GetAll();
                ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", model.CategoryProductId);
                return View(model);
            }

            if (uploadImage != null && uploadImage.Length > 0)
            {
                try
                {
                    var validateStream = uploadImage.OpenReadStream();
                    using var _ = Image.Load(validateStream);
                    validateStream.Position = 0;
                }
                catch
                {
                    ModelState.AddModelError("uploadImage", "File không hợp lệ. Chỉ chấp nhận file ảnh.");
                    TempData["Error"] = "File ảnh không hợp lệ.";
                    var categories = await _categoryProductService.GetAll();
                    ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", model.CategoryProductId);
                    return View(model);
                }

                model.ImageUrl = await _photoService.UploadPhotoAsync(uploadImage);
                if (string.IsNullOrEmpty(model.ImageUrl))
                {
                    ModelState.AddModelError("uploadImage", "Upload ảnh thất bại. Vui lòng kiểm tra cấu hình Cloudinary.");
                    TempData["Error"] = "Upload ảnh thất bại. Vui lòng thử lại hoặc kiểm tra cấu hình Cloudinary.";
                    var categories = await _categoryProductService.GetAll();
                    ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", model.CategoryProductId);
                    return View(model);
                }
            }
            else
            {
                var oldProduct = await _productService.GetDetail(model.Id);
                if (oldProduct != null && string.IsNullOrEmpty(model.ImageUrl))
                {
                    model.ImageUrl = oldProduct.ImageUrl ?? string.Empty;
                }
            }

            var updated = await _productService.Update(model.Id, model);
            if (!updated)
            {
                TempData["Error"] = "Không thể cập nhật sản phẩm. Vui lòng thử lại.";
                var categories = await _categoryProductService.GetAll();
                ViewBag.CategoryProductList = new SelectList(categories, "Id", "Name", model.CategoryProductId);
                return View(model);
            }

            await _notificationService.NotifyEntityChanged("Product");
            TempData["Success"] = "Sản phẩm đã được cập nhật.";
            return RedirectToAction("Index");
        }
    }
}