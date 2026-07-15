using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        private readonly INotificationService _notificationService;

        public CouponController(ICouponService couponService, INotificationService notificationService)
        {
            _couponService = couponService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _couponService.GetAll();
            return View(items);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CreateCouponDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            try
            {
                await _couponService.Create(model);
                await _notificationService.NotifyEntityChanged("Coupon");
                TempData["Success"] = "Mã giảm giá đã được tạo thành công.";
                return RedirectToAction("Index");
            }
            catch (System.InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _couponService.GetById(id);
            if (item == null) return NotFound();

            var model = new UpdateCouponDTO
            {
                Id = item.Id,
                Code = item.Code,
                Description = item.Description,
                DiscountType = item.DiscountType,
                DiscountValue = item.DiscountValue,
                MinimumOrderAmount = item.MinimumOrderAmount,
                MaximumDiscountAmount = item.MaximumDiscountAmount,
                UsageLimit = item.UsageLimit,
                UsagePerCustomer = item.UsagePerCustomer,
                CustomerId = item.CustomerId,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                IsPublic = item.IsPublic,
                IsActive = item.IsActive
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(UpdateCouponDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            var existing = await _couponService.GetById(model.Id);
            if (existing == null)
            {
                TempData["Error"] = "Mã giảm giá không tồn tại.";
                return RedirectToAction("Index");
            }

            var updated = await _couponService.Update(model.Id, model);
            if (!updated)
            {
                TempData["Error"] = "Không thể cập nhật mã giảm giá. Vui lòng thử lại.";
                return View(model);
            }

            await _notificationService.NotifyEntityChanged("Coupon");
            TempData["Success"] = "Mã giảm giá đã được cập nhật.";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _couponService.Delete(id);
                await _notificationService.NotifyEntityChanged("Coupon");
                TempData["Success"] = "Mã giảm giá đã được xóa.";
            }
            catch (System.InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var item = await _couponService.GetById(id);
            if (item == null) return NotFound();

            await _couponService.SetActive(id, !item.IsActive);
            await _notificationService.NotifyEntityChanged("Coupon");
            TempData["Success"] = item.IsActive ? "Đã tắt mã giảm giá." : "Đã bật mã giảm giá.";
            return RedirectToAction("Index");
        }
    }
}
