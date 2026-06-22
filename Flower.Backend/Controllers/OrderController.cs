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

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAll();
            return View(orders);
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

            // Admin creates order shell — items added later via OrderDetails admin
            var (success, message, orderId) = await _orderService.CreateOrder(
                model.CustomerId, model.Notes, new List<OrderItemInput>());

            if (success && model.Status != OrderStatus.Pending)
            {
                await _orderService.Update(orderId, new UpdateOrderDTO
                {
                    Id = orderId,
                    Status = model.Status,
                    Notes = model.Notes
                });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _orderService.Delete(id);
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
                Notes = order.Notes
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
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetDetail(id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}