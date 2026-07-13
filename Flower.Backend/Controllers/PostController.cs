using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Utils;
using Flower.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;
        private readonly INotificationService _notificationService;
        private readonly IApplicationDbContext _context;

        public PostController(IPostService postService, ICategoryService categoryService, INotificationService notificationService, IApplicationDbContext context)
        {
            _postService = postService;
            _categoryService = categoryService;
            _notificationService = notificationService;
            _context = context;
        }

        public async Task<IActionResult> Index(int? id, int page = 1, int pageSize = 12)
        {
            if (id != null)
            {
                var allPosts = await _postService.GetAll();
                var filtered = allPosts.Where(p => p.CategoryId == id.Value).ToList();
                ViewData["TotalPages"] = 1;
                ViewData["CurrentPage"] = 1;
                ViewData["TotalCount"] = filtered.Count;
                ViewData["PageSize"] = filtered.Count;
                return View(filtered);
            }

            var paged = await _postService.GetPaged(page, pageSize);
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["CurrentPage"] = paged.Page;
            ViewData["TotalCount"] = paged.TotalCount;
            ViewData["PageSize"] = paged.PageSize;
            return View(paged.Items);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload == null || upload.Length == 0)
                return Json(new { error = new { message = "No file uploaded." } });

            try
            {
                using var validateStream = upload.OpenReadStream();
                using var _ = Image.Load(validateStream);
            }
            catch
            {
                return Json(new { error = new { message = "File không hợp lệ. Chỉ chấp nhận file ảnh." } });
            }

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "ckeditor");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await upload.CopyToAsync(stream);
            }

            var url = "/uploads/ckeditor/" + fileName;
            return Json(new { url, uploaded = true });
        }

        public async Task<IActionResult> Details(int id)
        {
            var post = await _postService.GetById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAll();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostDTO model, IFormFile uploadImage)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAll();
                ViewBag.CategoryList = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            if (uploadImage != null && uploadImage.Length > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                try
                {
                    using var validateStream = uploadImage.OpenReadStream();
                    using var _ = Image.Load(validateStream);
                }
                catch
                {
                    ModelState.AddModelError("uploadImage", "File không hợp lệ. Chỉ chấp nhận file ảnh.");
                    var categories = await _categoryService.GetAll();
                    ViewBag.CategoryList = new SelectList(categories, "Id", "Name", model.CategoryId);
                    return View(model);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadImage.CopyToAsync(stream);
                }

                model.ImageUrl = "/uploads/" + fileName;
            }

            await _postService.Create(model);
            await _notificationService.NotifyEntityChanged("Post");
            TempData["Success"] = "Bài viết đã được tạo thành công.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _postService.Delete(id);
            await _notificationService.NotifyEntityChanged("Post");
            TempData["Success"] = "Bài viết đã được xóa.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var categories = await _categoryService.GetAll();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");

            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();

            var model = new UpdatePostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Summary = post.Summary,
                Slug = post.Slug,
                ImageUrl = post.ImageUrl,
                CategoryId = post.CategoryId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePostDTO model, IFormFile uploadImage)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                var categories = await _categoryService.GetAll();
                ViewBag.CategoryList = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            if (uploadImage != null && uploadImage.Length > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadImage.CopyToAsync(stream);
                }

                model.ImageUrl = "/uploads/" + fileName;
            }
            else
            {
                var oldPost = await _postService.GetById(model.Id);
                if (oldPost != null && string.IsNullOrEmpty(model.ImageUrl))
                {
                    model.ImageUrl = oldPost.ImageUrl;
                }
            }

            var updated = await _postService.Update(model.Id, model);
            if (!updated)
            {
                TempData["Error"] = "Không thể cập nhật bài viết. Vui lòng thử lại.";
                var categories = await _categoryService.GetAll();
                ViewBag.CategoryList = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            await _notificationService.NotifyEntityChanged("Post");
            TempData["Success"] = "Bài viết đã được cập nhật.";
            return RedirectToAction("Index");
        }
    }
}
