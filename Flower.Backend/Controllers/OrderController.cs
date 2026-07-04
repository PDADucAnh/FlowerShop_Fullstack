using Flower.Backend.Models.DTOs;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;

        public OrderController(IOrderService orderService, ICustomerService customerService)
        {
            _orderService = orderService;
            _customerService = customerService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            var paged = await _orderService.GetPaged(page, pageSize);
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["CurrentPage"] = paged.Page;
            ViewData["TotalCount"] = paged.TotalCount;
            ViewData["PageSize"] = paged.PageSize;
            return View(paged.Items);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var customers = await _customerService.GetAll();
            ViewBag.CustomerList = new SelectList(customers, "Id", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDTO model)
        {
            if (!ModelState.IsValid)
            {
                var customers = await _customerService.GetAll();
                ViewBag.CustomerList = new SelectList(customers, "Id", "FullName", model.CustomerId);
                return View(model);
            }

            var (success, message, orderId) = await _orderService.CreateOrder(
                model.CustomerId, model.Notes, new List<OrderItemInput>(),
                model.OrderDate, model.Status);

            TempData["Success"] = "Đơn hàng đã được tạo thành công.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.Delete(id);
            TempData["Success"] = "Đơn hàng đã được xóa.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetDetail(id);
            if (order == null) return NotFound();

            var customers = await _customerService.GetAll();
            ViewBag.CustomerList = new SelectList(customers, "Id", "FullName", order.CustomerId);

            var model = new UpdateOrderDTO
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Notes = order.Notes,
                DeliveryDate = order.DeliveryDate,
                DeliveryTimeSlot = order.DeliveryTimeSlot,
                DeliveryDistrict = order.DeliveryDistrict,
                DeliveryAddress = order.DeliveryAddress
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateOrderDTO model)
        {
            if (!ModelState.IsValid)
            {
                var customers = await _customerService.GetAll();
                ViewBag.CustomerList = new SelectList(customers, "Id", "FullName", model.CustomerId);
                return View(model);
            }

            await _orderService.Update(model.Id, model);
            TempData["Success"] = "Đơn hàng đã được cập nhật.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCOD(int id)
        {
            var (success, message) = await _orderService.ProcessCODOrder(id);
            if (success)
            {
                TempData["Success"] = "Đơn hàng đã được xác nhận thành công.";
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetDetail(id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}