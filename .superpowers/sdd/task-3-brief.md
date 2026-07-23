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


