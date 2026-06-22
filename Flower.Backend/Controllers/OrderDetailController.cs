using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrderDetailController(IOrderDetailService orderDetailService, IOrderService orderService, IProductService productService)
        {
            _orderDetailService = orderDetailService;
            _orderService = orderService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var orderDetails = await _orderDetailService.GetAll();
            return View(orderDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var orders = await _orderService.GetAll();
            var products = await _productService.GetAll();
            ViewBag.OrderList = new SelectList(orders, "Id", "Id");
            ViewBag.ProductList = new SelectList(products, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderDetailDTO model)
        {
            if (!ModelState.IsValid)
            {
                var orders = await _orderService.GetAll();
                var products = await _productService.GetAll();
                ViewBag.OrderList = new SelectList(orders, "Id", "Id", model.OrderId);
                ViewBag.ProductList = new SelectList(products, "Id", "Name", model.ProductId);
                return View(model);
            }

            await _orderDetailService.Create(model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _orderDetailService.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var orderDetail = await _orderDetailService.GetById(id);
            if (orderDetail == null) return NotFound();

            var orders = await _orderService.GetAll();
            var products = await _productService.GetAll();

            ViewBag.OrderList = new SelectList(orders, "Id", "Id", orderDetail.OrderId);
            ViewBag.ProductList = new SelectList(products, "Id", "Name", orderDetail.ProductId);
            return View(orderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OrderDetailDTO model)
        {
            if (!ModelState.IsValid)
            {
                var orders = await _orderService.GetAll();
                var products = await _productService.GetAll();
                ViewBag.OrderList = new SelectList(orders, "Id", "Id", model.OrderId);
                ViewBag.ProductList = new SelectList(products, "Id", "Name", model.ProductId);
                return View(model);
            }

            await _orderDetailService.Update(model.Id, model);
            return RedirectToAction("Index");
        }
    }
}