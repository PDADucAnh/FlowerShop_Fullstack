# Task 1: Backend — Add Pagination + Write Endpoints

## Global Constraints (from plan)
- No modification to existing MVC controllers or non-API controllers.
- API responses are raw objects — no wrapper.
- AllSystemSettingsViewModel (already exists in SystemSettingsDTOs.cs) contains Store, Smtp, VNPay, Shipping, Order, Cloudinary — Cloudinary NOT editable.
- ISystemSettingService.GetAllSettings() / SaveAllSettings() already exist.
- DTOs already exist for all entities — no new DTOs needed.
- PagedResult<T> already exists at Flower.Backend/Models/DTOs/PagedResult.cs.

**Files to modify:**
- `Flower.Backend/Services/Interfaces/IAdvertisementService.cs`
- `Flower.Backend/Services/Interfaces/IPageService.cs`
- `Flower.Backend/Services/Interfaces/IPromotionService.cs`
- `Flower.Backend/Services/Interfaces/ICouponService.cs`
- `Flower.Backend/Services/Interfaces/IPostService.cs`
- `Flower.Backend/Services/AdvertisementService.cs`
- `Flower.Backend/Services/PageService.cs`
- `Flower.Backend/Services/PromotionService.cs`
- `Flower.Backend/Services/CouponService.cs`
- `Flower.Backend/Services/PostService.cs`
- `Flower.Backend/Controllers/Api/AdvertisementsController.cs`
- `Flower.Backend/Controllers/Api/PagesController.cs`
- `Flower.Backend/Controllers/Api/PromotionsController.cs`
- `Flower.Backend/Controllers/Api/CouponsController.cs`
- `Flower.Backend/Controllers/Api/PostsController.cs`
- `Flower.Backend/Controllers/Api/LayoutApiController.cs`
- `Flower.Backend/Controllers/Api/SettingsApiController.cs`

## Implementation Steps

### Step 1: Add GetPaged to 4 service interfaces

**IAdvertisementService.cs** — add after `GetAll()`:
```csharp
Task<PagedResult<AdvertisementDTO>> GetPaged(int page, int pageSize);
```

**IPageService.cs** — add after `GetAll()`:
```csharp
Task<PagedResult<PageDTO>> GetPaged(int page, int pageSize);
```

**IPromotionService.cs** — add after `GetAll()`:
```csharp
Task<PagedResult<PromotionCampaignDTO>> GetPaged(int page, int pageSize);
```

**ICouponService.cs** — add after `GetAll()`:
```csharp
Task<PagedResult<CouponDTO>> GetPaged(int page, int pageSize);
```

### Step 2: Update IPostService — add search param

Change:
```csharp
Task<PagedResult<PostDTO>> GetPaged(int page, int pageSize);
```
To:
```csharp
Task<PagedResult<PostDTO>> GetPaged(int page, int pageSize, string? search = null);
```

### Step 3: Implement GetPaged in AdvertisementService

Open `Flower.Backend/Services/AdvertisementService.cs`. Add after `GetAll()`:

```csharp
public async Task<PagedResult<AdvertisementDTO>> GetPaged(int page, int pageSize)
{
    var query = _context.Advertisements.OrderByDescending(a => a.SortOrder);
    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    return new PagedResult<AdvertisementDTO>
    {
        Items = items.Select(a => a.ToDTO()).ToList(),
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

### Step 4: Implement GetPaged in PageService

Open `Flower.Backend/Services/PageService.cs`. Add after `GetAllActive()`:

```csharp
public async Task<PagedResult<PageDTO>> GetPaged(int page, int pageSize)
{
    var query = _context.Pages.OrderByDescending(p => p.Id);
    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    return new PagedResult<PageDTO>
    {
        Items = items.Select(p => p.ToDTO()).ToList(),
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

### Step 5: Implement GetPaged in PromotionService

Open `Flower.Backend/Services/PromotionService.cs`. Add after `GetAll()`:

```csharp
public async Task<PagedResult<PromotionCampaignDTO>> GetPaged(int page, int pageSize)
{
    var query = _context.PromotionCampaigns.OrderByDescending(p => p.CreatedAt);
    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    return new PagedResult<PromotionCampaignDTO>
    {
        Items = items.Select(p => p.ToDTO()).ToList(),
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

### Step 6: Implement GetPaged in CouponService

Open `Flower.Backend/Services/CouponService.cs`. Add after `GetAll()`:

```csharp
public async Task<PagedResult<CouponDTO>> GetPaged(int page, int pageSize)
{
    var query = _context.Coupons.OrderByDescending(c => c.CreatedAt);
    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    return new PagedResult<CouponDTO>
    {
        Items = items.Select(c => c.ToDTO()).ToList(),
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

### Step 7: Add search filter to PostService.GetPaged

Replace existing `GetPaged` with:

```csharp
public async Task<PagedResult<PostDTO>> GetPaged(int page, int pageSize, string? search = null)
{
    var query = _context.Posts
        .Include(p => p.Category)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(p => p.Title.Contains(search) || (p.Summary != null && p.Summary.Contains(search)));
    }

    query = query.OrderByDescending(p => p.Id);

    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<PostDTO>
    {
        Items = items.Select(p => p.ToDTO()).ToList(),
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

### Step 8: Add GetPaged action to AdvertisementsController

After `GetAll()`:
```csharp
[Authorize(Policy = "StaffOnly")]
[HttpGet("paged")]
public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    var result = await _advertisementService.GetPaged(page, pageSize);
    return Ok(result);
}
```

### Step 9: Add GetPaged action to PagesController

After `GetAll()`:
```csharp
[Authorize(Policy = "StaffOnly")]
[HttpGet("paged")]
public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    var result = await _pageService.GetPaged(page, pageSize);
    return Ok(result);
}
```

### Step 10: Add GetPaged action to PromotionsController

After `GetAll()`:
```csharp
[Authorize(Policy = "AdminOnly")]
[HttpGet("paged")]
public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    var result = await _promotionService.GetPaged(page, pageSize);
    return Ok(result);
}
```

### Step 11: Add GetPaged action to CouponsController

After `GetAll()`:
```csharp
[Authorize(Policy = "StaffOnly")]
[HttpGet("paged")]
public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    var result = await _couponService.GetPaged(page, pageSize);
    return Ok(result);
}
```

### Step 12: Add search query param to PostsController.GetPaged

Change the existing action:
```csharp
[AllowAnonymous]
[HttpGet("paged")]
public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 6)
```
To:
```csharp
[AllowAnonymous]
[HttpGet("paged")]
public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 6, [FromQuery] string? search = null)
```
And update the call from `_postService.GetPaged(page, pageSize)` to `_postService.GetPaged(page, pageSize, search)`.

### Step 13: Add layout write endpoints to LayoutApiController

Add before closing brace:
```csharp
[Authorize(Policy = "StaffOnly")]
[HttpPut("header")]
public async Task<IActionResult> SaveHeader([FromBody] HeaderLayoutDTO dto)
{
    var username = User.Identity?.Name ?? "System";
    await _settingService.SaveSetting("HeaderLayout", dto, username);
    return NoContent();
}

[Authorize(Policy = "StaffOnly")]
[HttpPut("footer")]
public async Task<IActionResult> SaveFooter([FromBody] List<FooterColumnDTO> dto)
{
    var username = User.Identity?.Name ?? "System";
    await _settingService.SaveSetting("FooterLayout", dto, username);
    return NoContent();
}
```

### Step 14: Replace SettingsApiController entirely

Replace the entire file with the version that has:
- Existing `GetStoreInfo()` and `GetCheckoutSettings()` (keep these)
- New `GetAll()` — `[Authorize(Policy = "StaffOnly")] GET /api/settings` returning `_settingService.GetAllSettings()`
- New `SaveStoreInfo()` — `PUT /api/settings/store-info`
- New `SaveSmtp()` — `PUT /api/settings/smtp`
- New `SaveVnPay()` — `PUT /api/settings/vnpay`
- New `SaveShipping()` — `PUT /api/settings/shipping`
- New `SaveOrder()` — `PUT /api/settings/order`
- All new PUT endpoints use `User.Identity?.Name ?? "System"` and return NoContent()

### Step 15: Build and verify

Run `dotnet build` — expect 0 errors.

### Step 16: Commit

Stage all changed files and commit with message `feat(backend): add pagination + settings write endpoints for Phase 4`.
