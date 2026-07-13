using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            var paged = await _userService.GetPaged(page, pageSize);
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["CurrentPage"] = paged.Page;
            ViewData["TotalCount"] = paged.TotalCount;
            ViewData["PageSize"] = paged.PageSize;
            return View(paged.Items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDTO model)
        {
            var checkExist = await _userService.UserExistsAsync(model.Username);
            if (checkExist)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã có người dùng!");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            await _userService.Create(model);
            TempData["Success"] = "Người dùng đã được tạo thành công.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null) return NotFound();

            var model = new UpdateUserDTO
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _userService.Update(model.Id, model);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật người dùng.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Người dùng đã được cập nhật.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var targetUser = await _userService.GetById(id);
            if (targetUser != null && targetUser.Username == currentUsername)
            {
                TempData["Error"] = "Bạn không thể tự xóa tài khoản của chính mình";
                return RedirectToAction("Index");
            }

            await _userService.Delete(id);
            TempData["Success"] = "Người dùng đã được xóa.";
            return RedirectToAction("Index");
        }
    }
}
