using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICategoryService _categoryService;
        private readonly INotificationService _notificationService;

        public PostController(IPostService postService, ICategoryService categoryService, INotificationService notificationService)
        {
            _postService = postService;
            _categoryService = categoryService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var posts = await _postService.GetAll();
            if (id != null)
            {
                posts = posts.Where(p => p.CategoryId == id.Value);
            }
            return View(posts);
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

            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAll();
                ViewBag.CategoryList = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            await _postService.Create(model);
            await _notificationService.NotifyEntityChanged("Post");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _postService.Delete(id);
            await _notificationService.NotifyEntityChanged("Post");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postService.GetById(id);
            if (post == null) return NotFound();

            var categories = await _categoryService.GetAll();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name", post.CategoryId);

            var model = new UpdatePostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Summary = post.Summary,
                ImageUrl = post.ImageUrl,
                CategoryId = post.CategoryId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePostDTO model, IFormFile uploadImage)
        {
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

            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAll();
                ViewBag.CategoryList = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }

            await _postService.Update(model.Id, model);
            await _notificationService.NotifyEntityChanged("Post");
            return RedirectToAction("Index");
        }
    }
}
