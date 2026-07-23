# Dynamic Layout Management — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace hardcoded Header/Footer with dynamic layout configuration stored as JSON in SystemSetting, manageable via Admin UI drag-and-drop.

**Architecture:** Backend stores `HeaderLayout` and `FooterLayout` as JSON in `SystemSetting` table (keys `"HeaderLayout"` / `"FooterLayout"`). Public API `GET /api/layout` returns merged layout + StoreInfo. Admin MVC controller uses SortableJS/Nestable2 for drag-and-drop UI. Frontend fetches layout and renders dynamically via `.map()` loops.

**Tech Stack:** ASP.NET Core 8, EF Core, PostgreSQL, React 19 + TypeScript + Tailwind CSS, SortableJS / Nestable2

## Global Constraints

- All Vietnamese UI labels in admin views (consistent with existing _LayoutAdmin.cshtml)
- Use existing `SystemSettingService` for read/write; don't create new DB tables
- `id: string` (Guid) required on every MenuItemDTO and FooterLinkDTO
- Zones arrays must never be `null` — fallback to `[]` at API level
- Max menu depth: 3 levels (enforced client-side)
- Frontend: orphaned page links in footer → silently hide (not crash)
- Frontend: `text_block` link type → render as `<p>/<span>`, never `<a>`
- Backend: default config when DB keys don't exist (backward compatibility)
- Commits after every task; build must pass before commit

---

## File Structure

### New Files — Backend (6)
| File | Responsibility |
|------|---------------|
| `Flower.Backend/Models/DTOs/LayoutDTOs.cs` | HeaderLayoutDTO, FooterColumnDTO, MenuItemDTO, FooterLinkDTO |
| `Flower.Backend/Controllers/Api/LayoutApiController.cs` | `GET /api/layout` public endpoint |
| `Flower.Backend/Controllers/LayoutController.cs` | Admin MVC: Index, SaveHeader, SaveFooter |
| `Flower.Backend/Views/Layout/Index.cshtml` | Tab container layout |
| `Flower.Backend/Views/Layout/_HeaderTab.cshtml` | Header form: TopBar, Zones, Configs, MenuTree |
| `Flower.Backend/Views/Layout/_FooterTab.cshtml` | Footer form: column cards with link lists |

### Modified Files — Backend (1)
| File | Change |
|------|--------|
| `Flower.Backend/Views/Shared/_LayoutAdmin.cshtml` | Add "GIAO DIỆN" sidebar section |

### New Files — Frontend (1)
| File | Responsibility |
|------|---------------|
| `Flower-shop.frontend/src/services/layoutService.ts` | TypeScript interfaces + axios call |

### Modified Files — Frontend (2)
| File | Change |
|------|--------|
| `Flower-shop.frontend/src/components/Header.tsx` | Dynamic menu, zones, TopBar, CTA, search mode |
| `Flower-shop.frontend/src/components/Footer.tsx` | Dynamic columns from API |

---

### Task 1: Backend DTOs — LayoutDTOs.cs

**Files:**
- Create: `Flower.Backend/Models/DTOs/LayoutDTOs.cs`

**Produces:** `HeaderLayoutDTO`, `FooterColumnDTO`, `MenuItemDTO`, `FooterLinkDTO` — consumed by Tasks 2, 3, 4.

- [ ] **Step 1: Create LayoutDTOs.cs**

```csharp
using System.Text.Json.Serialization;

namespace Flower.Backend.Models.DTOs;

public class HeaderLayoutDTO
{
    public TopBarDTO TopBar { get; set; } = new();
    public ZonesDTO Zones { get; set; } = new();
    public CtaButtonDTO CtaButton { get; set; } = new();
    public HotlineConfigDTO Hotline { get; set; } = new();
    public SearchConfigDTO Search { get; set; } = new();
    public List<MenuItemDTO> MenuItems { get; set; } = new();
}

public class TopBarDTO
{
    public bool IsActive { get; set; }
    public string? Text { get; set; }
    public string? Url { get; set; }
}

public class ZonesDTO
{
    [JsonPropertyName("left")]
    public List<string> Left { get; set; } = new();
    [JsonPropertyName("center")]
    public List<string> Center { get; set; } = new();
    [JsonPropertyName("right")]
    public List<string> Right { get; set; } = new();
}

public class CtaButtonDTO
{
    public bool IsActive { get; set; }
    public string? Text { get; set; }
    public string? Url { get; set; }
    public string? Variant { get; set; }
}

public class HotlineConfigDTO
{
    public bool UseDefault { get; set; } = true;
    public string? CustomText { get; set; }
}

public class SearchConfigDTO
{
    public string Mode { get; set; } = "popup";
}

public class MenuItemDTO
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
    public List<MenuItemDTO> Children { get; set; } = new();
}

public class FooterColumnDTO
{
    public string Title { get; set; } = string.Empty;
    public string Align { get; set; } = "left";
    public int SortOrder { get; set; }
    public string Type { get; set; } = "links";
    public bool IsActive { get; set; } = true;
    public List<FooterLinkDTO> Links { get; set; } = new();
}

public class FooterLinkDTO
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "custom"; // "page" | "custom" | "text_block"
    public int? PageId { get; set; }
    public string? Url { get; set; }
}
```

- [ ] **Step 2: Build to verify compilation**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 3: Commit**

```bash
git add Flower.Backend/Models/DTOs/LayoutDTOs.cs
git commit -m "feat: add LayoutDTOs for dynamic header/footer"
```

---

### Task 2: Backend API — LayoutApiController

**Files:**
- Create: `Flower.Backend/Controllers/Api/LayoutApiController.cs`

**Consumes:** `HeaderLayoutDTO`, `FooterColumnDTO` (Task 1), `SystemSettingService`, `StoreInfoSettings`

- [ ] **Step 1: Create LayoutApiController.cs**

```csharp
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flower.Backend.Controllers.Api;

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
```

- [ ] **Step 2: Build to verify**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 3: Commit**

```bash
git add Flower.Backend/Controllers/Api/LayoutApiController.cs
git commit -m "feat: add GET /api/layout endpoint with default config fallback"
```

---

### Task 3: Admin MVC Controller — LayoutController

**Files:**
- Create: `Flower.Backend/Controllers/LayoutController.cs`

**Consumes:** `HeaderLayoutDTO`, `FooterColumnDTO` (Task 1), `SystemSettingService`

- [ ] **Step 1: Create LayoutController.cs**

```csharp
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flower.Backend.Controllers;

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
        var pages = await GetPagesAsync(); // for footer page link selector

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

        var header = System.Text.Json.JsonSerializer.Deserialize<HeaderLayoutDTO>(jsonPayload);
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

        var footer = System.Text.Json.JsonSerializer.Deserialize<List<FooterColumnDTO>>(jsonPayload);
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
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
            .Select(dbContext.Pages, p => new PageDTO { Id = p.Id, Title = p.Title, Slug = p.Slug })
            .ToListAsync();
    }

    // Default configs (same as Task 2 defaults) — reuse to avoid duplication
    private static HeaderLayoutDTO GetDefaultHeader() { /* copy from Task 2 */ }
    private static List<FooterColumnDTO> GetDefaultFooter() { /* copy from Task 2 */ }
}

public class LayoutViewModel
{
    public HeaderLayoutDTO Header { get; set; } = new();
    public List<FooterColumnDTO> Footer { get; set; } = new();
}
```

- [ ] **Step 2: Build to verify**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 3: Commit**

```bash
git add Flower.Backend/Controllers/LayoutController.cs
git commit -m "feat: add admin LayoutController with save actions"
```

---

### Task 4: Admin Views — Header Tab

**Files:**
- Create: `Flower.Backend/Views/Layout/Index.cshtml`
- Create: `Flower.Backend/Views/Layout/_HeaderTab.cshtml`

**Consumes:** `LayoutViewModel` (Task 3), `ViewBag.Pages`

- [ ] **Step 1: Create Index.cshtml**

```html
@model LayoutViewModel
@{
    ViewData["Title"] = "Quản lý giao diện";
    var activeTab = ViewBag.ActiveTab as string ?? "header";
}

<div class="max-w-6xl mx-auto py-6 px-4">
    <h1 class="text-3xl font-bold mb-6">Quản lý giao diện</h1>

    @if (TempData["Success"] != null)
    {
        <div class="bg-green-100 text-green-700 px-4 py-3 rounded-lg mb-4">@TempData["Success"]</div>
    }

    <!-- Tabs -->
    <div class="flex border-b border-outline-variant/30 mb-6">
        <a href="@Url.Action("Index", new { tab = "header" })"
           class="px-6 py-3 font-label-md @(activeTab == "header" ? "text-primary border-b-2 border-primary" : "text-on-surface-variant")">Trình đơn</a>
        <a href="@Url.Action("Index", new { tab = "footer" })"
           class="px-6 py-3 font-label-md @(activeTab == "footer" ? "text-primary border-b-2 border-primary" : "text-on-surface-variant")">Chân trang</a>
    </div>

    @if (activeTab == "header")
    {
        @await Html.PartialAsync("_HeaderTab", Model.Header)
    }
    else
    {
        @await Html.PartialAsync("_FooterTab", Model.Footer)
    }
</div>
```

- [ ] **Step 2: Create _HeaderTab.cshtml**

```html
@model HeaderLayoutDTO
@{
    var allComponents = new[] { "logo", "menu", "categories", "search", "cart", "wishlist", "account", "notification", "hotline", "ctaButton" };
    var zoneNames = new[] { "left", "center", "right" };
}

<form id="headerForm" method="post" action="@Url.Action("SaveHeader")">
    @Html.AntiForgeryToken()
    <input type="hidden" name="jsonPayload" id="headerJsonPayload" />

    <!-- Section 1: Top Bar -->
    <div class="bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 mb-6">
        <h3 class="text-lg font-bold mb-4">Thanh thông báo</h3>
        <label class="flex items-center gap-2 mb-4">
            <input type="checkbox" id="topBarActive" @(Model.TopBar.IsActive ? "checked" : "") class="w-4 h-4" />
            <span>Hiển thị thanh thông báo</span>
        </label>
        <div class="grid grid-cols-2 gap-4 topBar-fields" style="@(Model.TopBar.IsActive ? "" : "display:none")">
            <div>
                <label class="block font-label-md text-sm mb-1">Nội dung</label>
                <input type="text" id="topBarText" value="@Model.TopBar.Text" class="w-full px-3 py-2 border rounded-lg" />
            </div>
            <div>
                <label class="block font-label-md text-sm mb-1">Link (tùy chọn)</label>
                <input type="text" id="topBarUrl" value="@Model.TopBar.Url" class="w-full px-3 py-2 border rounded-lg" />
            </div>
        </div>
    </div>

    <!-- Section 2: Zones -->
    <div class="bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 mb-6">
        <h3 class="text-lg font-bold mb-4">Vùng hiển thị</h3>
        <div class="grid grid-cols-3 gap-6">
            @foreach (var zone in zoneNames)
            {
                var items = zone == "left" ? Model.Zones.Left : zone == "center" ? Model.Zones.Center : Model.Zones.Right;
                <div>
                    <label class="block font-label-md font-semibold mb-2">@(zone == "left" ? "Bên trái" : zone == "center" ? "Ở giữa" : "Bên phải")</label>
                    <div class="zone-sortable min-h-[120px] border-2 border-dashed border-outline-variant/30 rounded-lg p-3 space-y-2"
                         data-zone="@zone">
                        @foreach (var comp in items)
                        {
                            <div class="drag-item bg-primary-container text-primary px-3 py-2 rounded-lg text-sm cursor-move flex justify-between items-center" data-component="@comp">
                                <span>@comp</span>
                                <button type="button" class="remove-component text-red-500 hover:text-red-700">&times;</button>
                            </div>
                        }
                    </div>
                    <select class="add-component mt-2 w-full px-3 py-1.5 border rounded-lg text-sm">
                        <option value="">+ Thêm component</option>
                        @foreach (var comp in allComponents)
                        {
                            if (!items.Contains(comp))
                            {
                                <option value="@comp">@comp</option>
                            }
                        }
                    </select>
                </div>
            }
        </div>
    </div>

    <!-- Section 3: Component Configs -->
    <div class="bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 mb-6">
        <h3 class="text-lg font-bold mb-4">Cấu hình component</h3>

        <!-- Hotline config -->
        <div class="mb-4">
            <h4 class="font-semibold mb-2">Hotline</h4>
            <label class="flex items-center gap-2 mb-2">
                <input type="checkbox" id="hotlineUseDefault" @(Model.Hotline.UseDefault ? "checked" : "") class="w-4 h-4" />
                <span>Dùng số mặc định từ Thông tin cửa hàng</span>
            </label>
            <input type="text" id="hotlineCustom" value="@Model.Hotline.CustomText" placeholder="Số điện thoại riêng cho Header" class="w-full px-3 py-2 border rounded-lg" />
        </div>

        <!-- Search config -->
        <div class="mb-4">
            <h4 class="font-semibold mb-2">Tìm kiếm</h4>
            <label class="mr-4"><input type="radio" name="searchMode" value="popup" @(Model.Search.Mode == "popup" ? "checked" : "") class="mr-1" /> Popup</label>
            <label><input type="radio" name="searchMode" value="input" @(Model.Search.Mode == "input" ? "checked" : "") class="mr-1" /> Input inline</label>
        </div>

        <!-- CTA Button config -->
        <div class="mb-4">
            <h4 class="font-semibold mb-2">Button CTA</h4>
            <label class="flex items-center gap-2 mb-2">
                <input type="checkbox" id="ctaActive" @(Model.CtaButton.IsActive ? "checked" : "") class="w-4 h-4" />
                <span>Bật</span>
            </label>
            <div class="grid grid-cols-3 gap-4 cta-fields" style="@(Model.CtaButton.IsActive ? "" : "display:none")">
                <input type="text" id="ctaText" value="@Model.CtaButton.Text" placeholder="Nội dung" class="px-3 py-2 border rounded-lg" />
                <input type="text" id="ctaUrl" value="@Model.CtaButton.Url" placeholder="Link" class="px-3 py-2 border rounded-lg" />
                <select id="ctaVariant" class="px-3 py-2 border rounded-lg">
                    <option value="filled" @(Model.CtaButton.Variant == "filled" ? "selected" : "")>Filled</option>
                    <option value="outlined" @(Model.CtaButton.Variant == "outlined" ? "selected" : "")>Outlined</option>
                </select>
            </div>
        </div>
    </div>

    <!-- Section 4: Menu Tree -->
    <div class="bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 mb-6">
        <h3 class="text-lg font-bold mb-4">Menu điều hướng</h3>
        <div id="menuTree" class="dd">
            <ol class="dd-list">
                @foreach (var item in Model.MenuItems)
                {
                    <li class="dd-item" data-id="@item.Id">
                        <div class="dd-handle flex justify-between items-center px-3 py-2 border rounded-lg mb-1 bg-white">
                            <span class="menu-label">@item.Label</span>
                            <div class="flex gap-2">
                                <button type="button" class="edit-menu-item text-blue-500 text-sm">Sửa</button>
                                <button type="button" class="remove-menu-item text-red-500 text-sm">Xoá</button>
                            </div>
                        </div>
                        @if (item.Children.Any())
                        {
                            <ol class="dd-list">
                                @foreach (var child in item.Children)
                                {
                                    <li class="dd-item" data-id="@child.Id">
                                        <div class="dd-handle flex justify-between items-center px-3 py-2 border rounded-lg mb-1 bg-white ml-4">
                                            <span class="menu-label">@child.Label</span>
                                            <button type="button" class="remove-menu-item text-red-500 text-sm">Xoá</button>
                                        </div>
                                    </li>
                                }
                            </ol>
                        }
                    </li>
                }
            </ol>
        </div>
        <button type="button" id="addMenuItem" class="mt-3 px-4 py-2 bg-primary text-white rounded-lg text-sm">+ Thêm mục menu</button>
    </div>

    <div class="flex justify-end gap-3">
        <button type="reset" class="px-5 py-2.5 border border-outline rounded-lg text-primary bg-transparent">Làm lại</button>
        <button type="submit" class="px-5 py-2.5 bg-primary text-on-primary border-0 rounded-lg">Lưu cấu hình</button>
    </div>
</form>

@section Scripts {
    <script src="https://cdn.jsdelivr.net/npm/sortablejs@1.15.0/Sortable.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/nestable2@1.6.0/jquery.nestable.min.js"></script>
    <script>
        // SortableJS for zones
        document.querySelectorAll('.zone-sortable').forEach(el => {
            new Sortable(el, {
                group: 'zones',
                animation: 150,
                onEnd: updateAddComponentDropdowns
            });
        });

        // Add component to zone
        document.querySelectorAll('.add-component').forEach(select => {
            select.addEventListener('change', function() {
                const zone = this.closest('div').querySelector('.zone-sortable');
                const comp = this.value;
                if (!comp) return;
                const div = document.createElement('div');
                div.className = 'drag-item bg-primary-container text-primary px-3 py-2 rounded-lg text-sm cursor-move flex justify-between items-center';
                div.dataset.component = comp;
                div.innerHTML = '<span>' + comp + '</span><button type="button" class="remove-component text-red-500 hover:text-red-700">&times;</button>';
                zone.appendChild(div);
                div.querySelector('.remove-component').addEventListener('click', function() {
                    div.remove();
                    updateAddComponentDropdowns();
                });
                this.value = '';
                updateAddComponentDropdowns();
            });
        });

        // Remove component
        document.querySelectorAll('.remove-component').forEach(btn => {
            btn.addEventListener('click', function() {
                this.closest('.drag-item').remove();
                updateAddComponentDropdowns();
            });
        });

        function updateAddComponentDropdowns() {
            document.querySelectorAll('.zone-sortable').forEach(zone => {
                const select = zone.closest('div').querySelector('.add-component');
                const existing = new Set([...zone.querySelectorAll('.drag-item')].map(el => el.dataset.component));
                select.querySelectorAll('option').forEach(opt => {
                    if (opt.value) opt.style.display = existing.has(opt.value) ? 'none' : '';
                });
            });
        }

        // Nestable2 for menu tree
        $('#menuTree').nestable({
            maxDepth: 3,
            group: 1
        });

        // Serialize form on submit
        document.getElementById('headerForm').addEventListener('submit', function(e) {
            // Build JSON payload from DOM
            const payload = {
                topBar: { isActive: document.getElementById('topBarActive').checked, text: document.getElementById('topBarText').value, url: document.getElementById('topBarUrl').value || null },
                zones: {
                    left: [...document.querySelector('.zone-sortable[data-zone="left"]').querySelectorAll('.drag-item')].map(el => el.dataset.component),
                    center: [...document.querySelector('.zone-sortable[data-zone="center"]').querySelectorAll('.drag-item')].map(el => el.dataset.component),
                    right: [...document.querySelector('.zone-sortable[data-zone="right"]').querySelectorAll('.drag-item')].map(el => el.dataset.component)
                },
                ctaButton: { isActive: document.getElementById('ctaActive').checked, text: document.getElementById('ctaText').value || null, url: document.getElementById('ctaUrl').value || null, variant: document.getElementById('ctaVariant').value },
                hotline: { useDefault: document.getElementById('hotlineUseDefault').checked, customText: document.getElementById('hotlineCustom').value || null },
                search: { mode: document.querySelector('input[name="searchMode"]:checked').value },
                menuItems: buildMenuItemsFromTree()
            };
            document.getElementById('headerJsonPayload').value = JSON.stringify(payload);
        });

        function buildMenuItemsFromTree() {
            // Traverse Nestable2 tree and return array
            const items = [];
            document.querySelectorAll('#menuTree > ol.dd-list > li.dd-item').forEach(li => {
                items.push(buildMenuItem(li));
            });
            return items;
        }

        function buildMenuItem(li) {
            const labelEl = li.querySelector('.menu-label');
            const children = [];
            li.querySelectorAll(':scope > ol.dd-list > li.dd-item').forEach(childLi => {
                children.push(buildMenuItem(childLi));
            });
            return {
                id: li.dataset.id || crypto.randomUUID(),
                label: labelEl ? labelEl.textContent : '',
                url: li.dataset.url || '/',
                isExternal: li.dataset.external === 'true',
                children: children
            };
        }
    </script>
}
```

- [ ] **Step 3: Build to verify**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 4: Commit**

```bash
git add Flower.Backend/Views/Layout/
git commit -m "feat: add admin layout views (header tab) with SortableJS"
```

---

### Task 5: Admin View — Footer Tab

**Files:**
- Create: `Flower.Backend/Views/Layout/_FooterTab.cshtml`

**Consumes:** `List<FooterColumnDTO>` (Task 1), `ViewBag.Pages` (Task 3)

- [ ] **Step 1: Create _FooterTab.cshtml**

```html
@model List<FooterColumnDTO>
@{
    var pages = ViewBag.Pages as List<PageDTO> ?? new();
}

<form id="footerForm" method="post" action="@Url.Action("SaveFooter")">
    @Html.AntiForgeryToken()
    <input type="hidden" name="jsonPayload" id="footerJsonPayload" />

    <div id="footerColumns" class="space-y-4">
        @for (int i = 0; i < Model.Count; i++)
        {
            var col = Model[i];
            <div class="bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 footer-column" data-index="@i">
                <div class="flex justify-between items-center mb-4">
                    <div class="flex gap-4 items-center flex-1">
                        <span class="drag-handle cursor-move text-gray-400">⠿</span>
                        <input type="text" class="col-title font-bold px-3 py-2 border rounded-lg flex-1" value="@col.Title" placeholder="Tiêu đề cột" />
                        <select class="col-align px-3 py-2 border rounded-lg">
                            <option value="left" @(col.Align == "left" ? "selected" : "")>Trái</option>
                            <option value="center" @(col.Align == "center" ? "selected" : "")>Giữa</option>
                            <option value="right" @(col.Align == "right" ? "selected" : "")>Phải</option>
                        </select>
                        <input type="number" class="col-sort-order w-16 px-3 py-2 border rounded-lg" value="@col.SortOrder" placeholder="TT" />
                        <select class="col-type px-3 py-2 border rounded-lg">
                            <option value="links" @(col.Type == "links" ? "selected" : "")>Links</option>
                            <option value="social_icons" @(col.Type == "social_icons" ? "selected" : "")>Social Icons</option>
                            <option value="text_block" @(col.Type == "text_block" ? "selected" : "")>Text Block</option>
                        </select>
                        <label class="flex items-center gap-1 text-sm">
                            <input type="checkbox" class="col-active" @(col.IsActive ? "checked" : "") />
                            Bật
                        </label>
                    </div>
                    <button type="button" class="remove-column text-red-500 hover:text-red-700 ml-2">×</button>
                </div>

                <!-- Links list -->
                <div class="footer-links space-y-2 pl-8">
                    @foreach (var link in col.Links)
                    {
                        <div class="flex gap-2 items-center link-item" data-id="@link.Id">
                            <span class="drag-handle cursor-move text-gray-400">⠿</span>
                            <input type="text" class="link-label flex-1 px-3 py-2 border rounded-lg text-sm" value="@link.Label" placeholder="Nhãn" />
                            <select class="link-type px-3 py-2 border rounded-lg text-sm">
                                <option value="custom" @(link.Type == "custom" ? "selected" : "")>URL tùy chỉnh</option>
                                <option value="page" @(link.Type == "page" ? "selected" : "")>Bài viết</option>
                                <option value="text_block" @(link.Type == "text_block" ? "selected" : "")>Văn bản</option>
                            </select>
                            @if (link.Type == "page")
                            {
                                <select class="link-page-id px-3 py-2 border rounded-lg text-sm">
                                    <option value="">Chọn bài viết...</option>
                                    @foreach (var p in pages)
                                    {
                                        <option value="@p.Id" @(link.PageId == p.Id ? "selected" : "")>@p.Title</option>
                                    }
                                </select>
                            }
                            else
                            {
                                <input type="text" class="link-url flex-1 px-3 py-2 border rounded-lg text-sm" value="@(link.Url ?? "")" placeholder="URL" />
                            }
                            <button type="button" class="remove-link text-red-400 hover:text-red-600">×</button>
                        </div>
                    }
                </div>
                <button type="button" class="add-link mt-2 ml-8 text-sm text-primary hover:underline">+ Thêm link</button>
            </div>
        }
    </div>

    <button type="button" id="addColumn" class="mt-4 px-4 py-2 bg-primary text-white rounded-lg text-sm">+ Thêm cột mới</button>

    <div class="flex justify-end gap-3 mt-6 pt-4 border-t border-outline-variant/15">
        <button type="submit" class="px-5 py-2.5 bg-primary text-on-primary border-0 rounded-lg">Lưu cấu hình</button>
    </div>
</form>

@section Scripts {
    <script src="https://cdn.jsdelivr.net/npm/sortablejs@1.15.0/Sortable.min.js"></script>
    <script>
        // SortableJS for columns and links
        new Sortable(document.getElementById('footerColumns'), {
            handle: '.drag-handle',
            animation: 150
        });

        document.querySelectorAll('.footer-links').forEach(el => {
            new Sortable(el, {
                handle: '.drag-handle',
                animation: 150
            });
        });

        // Add column
        document.getElementById('addColumn').addEventListener('click', function() {
            const col = document.createElement('div');
            col.className = 'bg-white rounded-xl shadow-sm border border-outline-variant/20 p-6 footer-column';
            col.innerHTML = `
                <div class="flex justify-between items-center mb-4">
                    <div class="flex gap-4 items-center flex-1">
                        <span class="drag-handle cursor-move text-gray-400">⠿</span>
                        <input type="text" class="col-title font-bold px-3 py-2 border rounded-lg flex-1" placeholder="Tiêu đề cột" />
                        <select class="col-align px-3 py-2 border rounded-lg"><option value="left">Trái</option><option value="center">Giữa</option><option value="right">Phải</option></select>
                        <input type="number" class="col-sort-order w-16 px-3 py-2 border rounded-lg" value="0" />
                        <select class="col-type px-3 py-2 border rounded-lg"><option value="links">Links</option><option value="social_icons">Social Icons</option></select>
                        <label class="flex items-center gap-1 text-sm"><input type="checkbox" class="col-active" checked /> Bật</label>
                    </div>
                    <button type="button" class="remove-column text-red-500 hover:text-red-700 ml-2">×</button>
                </div>
                <div class="footer-links space-y-2 pl-8"></div>
                <button type="button" class="add-link mt-2 ml-8 text-sm text-primary hover:underline">+ Thêm link</button>`;
            document.getElementById('footerColumns').appendChild(col);
            col.querySelector('.remove-column').onclick = () => col.remove();
            col.querySelector('.add-link').onclick = addLinkHandler;
            new Sortable(col.querySelector('.footer-links'), { handle: '.drag-handle', animation: 150 });
        });

        // Add link
        const addLinkHandler = function() {
            const links = this.closest('.footer-column').querySelector('.footer-links');
            const div = document.createElement('div');
            div.className = 'flex gap-2 items-center link-item';
            div.dataset.id = crypto.randomUUID();
            div.innerHTML = `
                <span class="drag-handle cursor-move text-gray-400">⠿</span>
                <input type="text" class="link-label flex-1 px-3 py-2 border rounded-lg text-sm" placeholder="Nhãn" />
                <select class="link-type px-3 py-2 border rounded-lg text-sm" onchange="toggleLinkUrl(this)">
                    <option value="custom">URL tùy chỉnh</option>
                    <option value="page">Bài viết</option>
                    <option value="text_block">Văn bản</option>
                </select>
                <input type="text" class="link-url flex-1 px-3 py-2 border rounded-lg text-sm" placeholder="URL" />
                <button type="button" class="remove-link text-red-400 hover:text-red-600">×</button>`;
            links.appendChild(div);
            div.querySelector('.remove-link').onclick = () => div.remove();
        };
        document.querySelectorAll('.add-link').forEach(btn => {
            btn.addEventListener('click', addLinkHandler);
        });

        // Serialize on submit
        document.getElementById('footerForm').addEventListener('submit', function(e) {
            const columns = [];
            document.querySelectorAll('.footer-column').forEach(col => {
                const links = [];
                col.querySelectorAll('.link-item').forEach(link => {
                    const type = link.querySelector('.link-type').value;
                    links.push({
                        id: link.dataset.id,
                        label: link.querySelector('.link-label').value,
                        type: type,
                        pageId: type === 'page' ? parseInt(link.querySelector('.link-page-id').value) || null : null,
                        url: type !== 'page' ? link.querySelector('.link-url').value || null : null
                    });
                });
                columns.push({
                    title: col.querySelector('.col-title').value,
                    align: col.querySelector('.col-align').value,
                    sortOrder: parseInt(col.querySelector('.col-sort-order').value) || 0,
                    type: col.querySelector('.col-type').value,
                    isActive: col.querySelector('.col-active').checked,
                    links: links
                });
            });
            document.getElementById('footerJsonPayload').value = JSON.stringify(columns);
        });
    </script>
}
```

- [ ] **Step 2: Build to verify**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 3: Commit**

```bash
git add Flower.Backend/Views/Layout/_FooterTab.cshtml
git commit -m "feat: add admin layout footer tab with SortableJS column/link editor"
```

---

### Task 6: Sidebar — Add "GIAO DIỆN" Section

**Files:**
- Modify: `Flower.Backend/Views/Shared/_LayoutAdmin.cshtml`

- [ ] **Step 1: Add "GIAO DIỆN" section after existing sections (e.g. after "NỘI DUNG")**

Find the "NỘI DUNG" section or a suitable location in the sidebar and add:

```html
<!-- GIAO DIỆN -->
<div class="px-4 py-2 font-label-md text-sm text-on-surface-variant uppercase tracking-wider">Giao diện</div>
<a class="flex items-center gap-3 px-4 py-3 rounded-lg font-label-md text-label-md @(controller == "Layout" && ViewContext.RouteData.Values["action"]?.ToString() != "Index" ? ...)"
   asp-controller="Layout" asp-action="Index" asp-route-tab="header">
    <span class="material-symbols-outlined">menu</span>
    Trình đơn
</a>
<a class="flex items-center gap-3 px-4 py-3 rounded-lg font-label-md text-label-md @(controller == "Layout" && ViewContext.RouteData.Values["action"]?.ToString() == "Index" ? ...)"
   asp-controller="Layout" asp-action="Index" asp-route-tab="footer">
    <span class="material-symbols-outlined">grid_view</span>
    Chân trang
</a>
```

- [ ] **Step 2: Build to verify**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 3: Commit**

```bash
git add Flower.Backend/Views/Shared/_LayoutAdmin.cshtml
git commit -m "feat: add GIAO DIEN section to admin sidebar"
```

---

### Task 7: Frontend — layoutService

**Files:**
- Create: `Flower-shop.frontend/src/services/layoutService.ts`

**Produces:** Interfaces and service consumed by Tasks 8, 9.

- [ ] **Step 1: Create layoutService.ts**

```typescript
import axiosClient from '../api/axiosClient';

export interface TopBar {
  isActive: boolean;
  text?: string;
  url?: string | null;
}

export interface Zones {
  left: string[];
  center: string[];
  right: string[];
}

export interface CtaButton {
  isActive: boolean;
  text?: string | null;
  url?: string | null;
  variant?: string | null;
}

export interface HotlineConfig {
  useDefault: boolean;
  customText?: string | null;
}

export interface SearchConfig {
  mode: 'popup' | 'input';
}

export interface MenuItem {
  id: string;
  label: string;
  url: string;
  isExternal: boolean;
  children: MenuItem[];
}

export interface HeaderLayout {
  topBar: TopBar;
  zones: Zones;
  ctaButton: CtaButton;
  hotline: HotlineConfig;
  search: SearchConfig;
  menuItems: MenuItem[];
}

export interface FooterLink {
  id: string;
  label: string;
  type: 'page' | 'custom' | 'text_block';
  pageId?: number | null;
  url?: string | null;
}

export interface FooterColumn {
  title: string;
  align: 'left' | 'center' | 'right';
  sortOrder: number;
  type: 'links' | 'social_icons' | 'text_block';
  isActive: boolean;
  links: FooterLink[];
}

export interface LayoutResponse {
  header: HeaderLayout;
  footer: FooterColumn[];
  storeInfo: Record<string, unknown>;
}

const layoutService = {
  getLayout: () =>
    axiosClient.get<LayoutResponse>('/layout'),
};

export default layoutService;
```

- [ ] **Step 2: Build frontend to verify**

Run: `cd Flower-shop.frontend && npx tsc --noEmit`
Expected: No type errors

- [ ] **Step 3: Commit**

```bash
git add Flower-shop.frontend/src/services/layoutService.ts
git commit -m "feat: add frontend layoutService with TypeScript interfaces"
```

---

### Task 8: Frontend — Refactor Header.tsx

**Files:**
- Modify: `Flower-shop.frontend/src/components/Header.tsx`

**Consumes:** `layoutService` (Task 7), `settingsService.getStoreInfo()`

- [ ] **Step 1: Update Header.tsx to consume layoutService**

Key changes:
- Add `useEffect` to fetch `layoutService.getLayout()` on mount
- Add `layoutResponse` state (HeaderLayout | null)
- Replace hardcoded nav links with `layoutResponse.menuItems.map(renderMenuItem)` (recursive)
- Render TopBar before main header if `header.topBar.isActive`
- Render zones dynamically: `.map(componentType => { switch(componentType) {...} })`
- Hotline: read from `storeInfo` if `header.hotline.useDefault`, else `header.hotline.customText`
- Search: use `header.search.mode` to decide popup vs input
- CTA button: conditionally render if `header.ctaButton.isActive`
- Keep `/order-confirmation` simplified header (bypass dynamic rendering)

- [ ] **Step 2: Build frontend to verify**

Run: `cd Flower-shop.frontend && npx tsc --noEmit`
Expected: No type errors

- [ ] **Step 3: Run full frontend build**

Run: `cd Flower-shop.frontend && npm run build 2>&1 | Select-String -Pattern "error|Failed|built in"`

- [ ] **Step 4: Commit**

```bash
git add Flower-shop.frontend/src/components/Header.tsx
git commit -m "feat: dynamic header with zones, menu tree, TopBar, CTA, search mode"
```

---

### Task 9: Frontend — Refactor Footer.tsx

**Files:**
- Modify: `Flower-shop.frontend/src/components/Footer.tsx`

**Consumes:** `layoutService` (Task 7)

- [ ] **Step 1: Update Footer.tsx to consume layoutService**

Key changes:
- Fetch `layoutService.getLayout()` on mount
- Store `footer: FooterColumn[]` in state
- Replace hardcoded columns with `footer.filter(c => c.isActive).sort((a,b) => a.sortOrder - b.sortOrder).map(column => ...)`
- CSS grid: `grid-cols-1 md:grid-cols-{activeColumnCount}`
- Column type `links`: map links, skip `text_block` for `<a>` tag, `page` type resolves via lookup, `custom` type uses url directly
- Column type `social_icons`: render Facebook/Zalo icons from storeInfo
- Column type `text_block`: render `<p>` with column title + content
- `text_block` link in links list: render as `<span>` not `<a>`
- Orphaned page links: if page slug not found → hide silently

- [ ] **Step 2: Build frontend to verify**

Run: `cd Flower-shop.frontend && npx tsc --noEmit`
Expected: No type errors

- [ ] **Step 3: Run full frontend build**

Run: `cd Flower-shop.frontend && npm run build 2>&1 | Select-String -Pattern "error|Failed|built in"`

- [ ] **Step 4: Commit**

```bash
git add Flower-shop.frontend/src/components/Footer.tsx
git commit -m "feat: dynamic footer with columns, links, text_block, social_icons"
```

---

## Spec Coverage Check

| Spec Requirement | Task |
|-----------------|------|
| HeaderLayoutDTO / FooterColumnDTO / MenuItemDTO / FooterLinkDTO with `id` field | Task 1 |
| `GET /api/layout` with zones null safety + default fallback | Task 2 |
| Admin LayoutController with SaveHeader / SaveFooter + audit + cache invalidation | Task 3 |
| Admin Header tab: TopBar, Zones (SortableJS), Configs, MenuTree (Nestable2) | Task 4 |
| Admin Footer tab: column cards, inline link editor, SortableJS | Task 5 |
| Sidebar "GIAO DIỆN" section | Task 6 |
| Frontend layoutService.ts with TypeScript interfaces | Task 7 |
| Frontend Header refactor: dynamic menu, zones, TopBar, CTA, search mode, hotline | Task 8 |
| Frontend Footer refactor: dynamic columns, link types, text_block, social_icons | Task 9 |
| Max depth 3 (Nestable2 maxDepth option) | Task 4 (JS config) |
| Cache invalidation on save | Task 3 (via SystemSettingService) |
| Orphaned page link handling | Task 9 (optional future enhancement) |
