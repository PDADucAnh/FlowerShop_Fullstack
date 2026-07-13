using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            var paged = await _customerService.GetPaged(page, pageSize);
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["CurrentPage"] = paged.Page;
            ViewData["TotalCount"] = paged.TotalCount;
            ViewData["PageSize"] = paged.PageSize;
            return View(paged.Items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _customerService.Create(model);
            TempData["Success"] = "Khách hàng đã được tạo thành công.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.Delete(id);
            TempData["Success"] = "Khách hàng đã được xóa.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetById(id);
            if (customer == null) return NotFound();

            var model = new UpdateCustomerDTO
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCustomerDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _customerService.Update(model.Id, model);
            TempData["Success"] = "Khách hàng đã được cập nhật.";
            return RedirectToAction("Index");
        }
    }
}