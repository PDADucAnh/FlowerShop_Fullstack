using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/layout")]
    [ApiController]
    public class LayoutApiController : ControllerBase
    {
        private readonly ISystemSettingService _settingService;

        public LayoutApiController(ISystemSettingService settingService)
        {
            _settingService = settingService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetLayout()
        {
            var header = await _settingService.GetSetting<HeaderLayoutDTO>("HeaderLayout");
            if (header == null)
            {
                header = GetDefaultHeader();
            }
            else
            {
                header.Zones ??= new ZonesDTO();
                header.Zones.Left ??= new List<string>();
                header.Zones.Center ??= new List<string>();
                header.Zones.Right ??= new List<string>();
            }

            var footer = await _settingService.GetSetting<List<FooterColumnDTO>>("FooterLayout");
            footer ??= GetDefaultFooter();

            var storeInfo = await _settingService.GetSetting<StoreInfoSettings>("StoreInfo")
                            ?? new StoreInfoSettings();

            return Ok(new { header, footer, storeInfo });
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
}
