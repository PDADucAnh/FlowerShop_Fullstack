using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IContactService contactService, ILogger<ContactController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _contactService.GetAll();
            return View(items);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var item = await _contactService.GetById(id);
            if (item == null) return NotFound();

            if (!item.IsRead)
            {
                await _contactService.MarkRead(id, true);
            }

            return View(item);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _contactService.Delete(id);
            TempData["Success"] = "Liên hệ đã được xóa.";
            return RedirectToAction("Index");
        }
    }
}
