using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class LayoutController : Controller
    {
        private readonly ISystemSettingService _settingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutController(ISystemSettingService settingService, IHttpContextAccessor httpContextAccessor)
        {
            _settingService = settingService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string tab = "header")
        {
            var header = await _settingService.GetSetting<HeaderLayoutDTO>("HeaderLayout");
            var footer = await _settingService.GetSetting<List<FooterColumnDTO>>("FooterLayout");
            var pages = await GetPagesAsync();

            ViewBag.ActiveTab = tab;
            ViewBag.Pages = pages;
            return View(new LayoutViewModel
            {
                Header = header ?? GetDefaultHeader(),
                Footer = footer ?? GetDefaultFooter()
            });
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveHeader([FromForm] string jsonPayload)
        {
            if (string.IsNullOrEmpty(jsonPayload))
                return BadRequest("Payload is empty");

            var header = System.Text.Json.JsonSerializer.Deserialize<HeaderLayoutDTO>(jsonPayload, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (header == null)
                return BadRequest("Invalid header data");

            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
            await _settingService.SaveSetting("HeaderLayout", header, username);
            TempData["Success"] = "Đã lưu cấu hình Header thành công!";
            return RedirectToAction("Index", new { tab = "header" });
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFooter([FromForm] string jsonPayload)
        {
            if (string.IsNullOrEmpty(jsonPayload))
                return BadRequest("Payload is empty");

            var footer = System.Text.Json.JsonSerializer.Deserialize<List<FooterColumnDTO>>(jsonPayload, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (footer == null)
                return BadRequest("Invalid footer data");

            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
            await _settingService.SaveSetting("FooterLayout", footer, username);
            TempData["Success"] = "Đã lưu cấu hình Footer thành công!";
            return RedirectToAction("Index", new { tab = "footer" });
        }

        private async Task<List<PageDTO>> GetPagesAsync()
        {
            var dbContext = HttpContext.RequestServices.GetRequiredService<Flower.Data.IApplicationDbContext>();
            return await dbContext.Pages
                .Select(p => new PageDTO { Id = p.Id, Title = p.Title, Slug = p.Slug })
                .ToListAsync();
        }

        private static HeaderLayoutDTO GetDefaultHeader()
        {
            return new HeaderLayoutDTO
            {
                TopBar = new TopBarDTO { IsActive = false },
                Zones = new ZonesDTO
                {
                    Left = new List<string>(),
                    Center = new List<string> { "logo" },
                    Right = new List<string> { "search", "cart", "account" }
                },
                CtaButton = new CtaButtonDTO { IsActive = false },
                Hotline = new HotlineConfigDTO { UseDefault = true },
                Search = new SearchConfigDTO { Mode = "popup" },
                MenuItems = new List<MenuItemDTO>
                {
                    new() { Label = "Trang chủ", Url = "/" },
                    new() { Label = "Cửa hàng", Url = "/shop" },
                    new() { Label = "Bài viết", Url = "/blog" },
                    new() { Label = "Giới thiệu", Url = "/about" },
                    new() { Label = "Liên hệ", Url = "/contact" }
                }
            };
        }

        private static List<FooterColumnDTO> GetDefaultFooter()
        {
            return new List<FooterColumnDTO>
            {
                new()
                {
                    Title = "Về chúng tôi", Align = "left", SortOrder = 1, Type = "links",
                    Links = new List<FooterLinkDTO>
                    {
                        new() { Label = "Thương hiệu hoa tươi nghệ thuật...", Type = "text_block" },
                        new() { Label = "Giới thiệu", Type = "page", PageId = 1 }
                    }
                },
                new()
                {
                    Title = "Chính sách", Align = "left", SortOrder = 2, Type = "links",
                    Links = new List<FooterLinkDTO>
                    {
                        new() { Label = "Giao hàng", Type = "custom", Url = "/delivery-policy" },
                        new() { Label = "Đổi trả", Type = "custom", Url = "/return-policy" },
                        new() { Label = "Bảo mật", Type = "custom", Url = "/privacy-policy" }
                    }
                },
                new()
                {
                    Title = "Kết nối", Align = "left", SortOrder = 3, Type = "social_icons",
                    Links = new List<FooterLinkDTO>()
                }
            };
        }
    }

    public class LayoutViewModel
    {
        public HeaderLayoutDTO Header { get; set; } = new();
        public List<FooterColumnDTO> Footer { get; set; } = new();
    }
}
