# Phase 4: Content, Marketing & System Settings Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use subagent-driven-development (recommended) or executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace placeholder /content, /marketing, /system pages with tabbed single-page admin UIs for Banner management, Blog Posts, Static Pages, Layout config, Promotion campaigns, Coupons, and all 5 System Setting groups.

**Architecture:** Backend-first: add pagination and write endpoints to existing API controllers + service interfaces. Then frontend types/API modules. Then 3 tabbed pages + their sub-routes. Finally routing and build verification.

**Tech Stack:** .NET 8 / ASP.NET Core (backend), React 18 + TypeScript + Vite (frontend), React Query, shadcn/ui, sonner, React Router v6

## Global Constraints

- No modification to existing MVC controllers or non-API controllers.
- API responses are raw objects — the frontend calls `.then(r => r.data)` on Axios responses.
- All UI text in Vietnamese.
- Follow existing patterns: API controllers inject `I{Entity}Service`, frontend `@/api/` modules export an object with methods, types in `@/types/`.
- `AllSystemSettingsViewModel` (already exists in `SystemSettingsDTOs.cs`) contains `Store`, `Smtp`, `VNPay`, `Shipping`, `Order`, `Cloudinary` — Cloudinary NOT editable from admin UI.
- Cloudinary credentials kept in `appsettings.json` only — no admin UI exposure.
- Flash Sales NOT included in Phase 4 scope (only Promotions + Coupons for Marketing).
- `PaginatedResponse<T>` already exists at `@/types/api.ts` — reuse for all new paginated endpoints.
- `ISystemSettingService.GetAllSettings()` / `SaveAllSettings()` already exist.
- DTOs already exist for all entities (Advertisements, Posts, Pages, Layout, Settings, Coupons, Promotions) — no new DTOs needed.
- Active tab tracked by React state (no URL params for tabs). Full-page sub-routes (`/content/posts/new`) use React Router.

---

## File Structure

### Backend — Modified files

| File | Change |
|------|--------|
| `Flower.Backend/Services/Interfaces/IAdvertisementService.cs` | Add `GetPaged(int, int)` signature |
| `Flower.Backend/Services/Interfaces/IPageService.cs` | Add `GetPaged(int, int)` signature |
| `Flower.Backend/Services/Interfaces/IPromotionService.cs` | Add `GetPaged(int, int)` signature |
| `Flower.Backend/Services/Interfaces/ICouponService.cs` | Add `GetPaged(int, int)` signature |
| `Flower.Backend/Services/Interfaces/IPostService.cs` | Update `GetPaged(int, int, string?)` — add optional `search` param |
| `Flower.Backend/Services/AdvertisementService.cs` | Implement `GetPaged` |
| `Flower.Backend/Services/PageService.cs` | Implement `GetPaged` |
| `Flower.Backend/Services/PromotionService.cs` | Implement `GetPaged` |
| `Flower.Backend/Services/CouponService.cs` | Implement `GetPaged` |
| `Flower.Backend/Services/PostService.cs` | Add `search` filter to `GetPaged` |
| `Flower.Backend/Controllers/Api/AdvertisementsController.cs` | Add `GetPaged` action |
| `Flower.Backend/Controllers/Api/PagesController.cs` | Add `GetPaged` action |
| `Flower.Backend/Controllers/Api/PromotionsController.cs` | Add `GetPaged` action |
| `Flower.Backend/Controllers/Api/CouponsController.cs` | Add `GetPaged` action |
| `Flower.Backend/Controllers/Api/PostsController.cs` | Add `search` query param to existing `GetPaged` |
| `Flower.Backend/Controllers/Api/LayoutApiController.cs` | Add `PUT header` + `PUT footer` endpoints |
| `Flower.Backend/Controllers/Api/SettingsApiController.cs` | Add `GET /` (returns `AllSystemSettingsViewModel`), add `PUT store-info`, `PUT smtp`, `PUT vnpay`, `PUT shipping`, `PUT order` |

### Frontend — Created and Modified Files

| File | Type | Responsibility |
|------|------|----------------|
| `src/types/advertisement.ts` | Create | `AdvertisementDTO`, `CreateAdvertisementDTO`, `UpdateAdvertisementDTO` |
| `src/types/page.ts` | Create | `PageDTO`, `CreatePageDTO`, `UpdatePageDTO` |
| `src/types/layout.ts` | Create | `HeaderLayoutDTO`, `TopBarDTO`, `ZonesDTO`, `CtaButtonDTO`, `HotlineConfigDTO`, `SearchConfigDTO`, `MenuItemDTO`, `FooterColumnDTO`, `FooterLinkDTO` |
| `src/types/settings.ts` | Create | `StoreInfoSettings`, `SmtpSettings`, `VNPaySettings`, `ShippingSettings`, `OrderSettings`, `AllSystemSettings` |
| `src/types/post.ts` | Create | `PostDTO`, `CreatePostDTO`, `UpdatePostDTO` |
| `src/types/promotion.ts` | Create | `PromotionCampaignDTO`, `ActivePromotionDTO` |
| `src/types/coupon.ts` | Create | `CouponDTO`, `CouponUsageDTO`, `CreateCouponDTO`, `UpdateCouponDTO` |
| `src/api/advertisements.ts` | Create | `getPaged`, `getById`, `create`, `update`, `delete` |
| `src/api/pages.ts` | Create | `getPaged`, `getById`, `getBySlug`, `create`, `update`, `delete` |
| `src/api/layout.ts` | Create | `getLayout`, `saveHeader`, `saveFooter` |
| `src/api/settings.ts` | Create | `getAll`, `saveStoreInfo`, `saveSmtp`, `saveVnPay`, `saveShipping`, `saveOrder` |
| `src/api/promotions.ts` | Create | `getPaged`, `getById`, `create`, `update`, `delete`, `enable`, `disable`, `addProduct`, `removeProduct` |
| `src/api/coupons.ts` | Create | `getPaged`, `getById`, `create`, `update`, `delete`, `enable`, `disable`, `getUsages` |
| `src/api/posts.ts` | Create | `getPaged`, `getById`, `create`, `update`, `delete` |
| `src/pages/ContentPage.tsx` | Create | Tabbed wrapper: Banner / Bài viết / Trang tĩnh / Giao diện tabs |
| `src/pages/MarketingPage.tsx` | Create | Tabbed wrapper: Khuyến mãi / Mã giảm giá tabs |
| `src/pages/SystemSettingsPage.tsx` | Create | Tabbed wrapper: Thông tin cửa hàng / SMTP / VNPay / Vận chuyển / Đơn hàng tabs |
| `src/pages/content/BannersTab.tsx` | Create | DataTable + Dialog for CRUD |
| `src/pages/content/PostsTab.tsx` | Create | DataTable + link to create/edit full-page forms |
| `src/pages/content/PagesTab.tsx` | Create | DataTable + link to create/edit full-page forms |
| `src/pages/content/LayoutTab.tsx` | Create | Inline forms for header/footer config |
| `src/pages/content/PostFormPage.tsx` | Create | Full-page editor (title, content textarea, category, image, slug) |
| `src/pages/content/PageFormPage.tsx` | Create | Full-page editor (title, slug, content textarea, active toggle) |
| `src/pages/marketing/PromotionsTab.tsx` | Create | DataTable + create/edit dialogs + enable/disable |
| `src/pages/marketing/CouponsTab.tsx` | Create | DataTable + dialog for CRUD + enable/disable + usage list |
| `src/pages/system/StoreInfoTab.tsx` | Create | Inline form for store settings |
| `src/pages/system/SmtpTab.tsx` | Create | Inline form for SMTP settings |
| `src/pages/system/VnPayTab.tsx` | Create | Inline form for VNPay settings |
| `src/pages/system/ShippingTab.tsx` | Create | Inline form for shipping settings |
| `src/pages/system/OrderTab.tsx` | Create | Inline form for order settings |
| `src/App.tsx` | Modify | Replace placeholder imports, add sub-routes for PostFormPage + PageFormPage |
| `src/pages/PlaceholderPages.tsx` | Modify | Remove `ContentPage`, `MarketingPage`, `SystemPage` exports |

---

### Task 1: Backend — Add Pagination + Write Endpoints

**Files:**
- Modify: `Flower.Backend/Services/Interfaces/IAdvertisementService.cs`
- Modify: `Flower.Backend/Services/Interfaces/IPageService.cs`
- Modify: `Flower.Backend/Services/Interfaces/IPromotionService.cs`
- Modify: `Flower.Backend/Services/Interfaces/ICouponService.cs`
- Modify: `Flower.Backend/Services/Interfaces/IPostService.cs`
- Modify: `Flower.Backend/Services/AdvertisementService.cs`
- Modify: `Flower.Backend/Services/PageService.cs`
- Modify: `Flower.Backend/Services/PromotionService.cs`
- Modify: `Flower.Backend/Services/CouponService.cs`
- Modify: `Flower.Backend/Services/PostService.cs`
- Modify: `Flower.Backend/Controllers/Api/AdvertisementsController.cs`
- Modify: `Flower.Backend/Controllers/Api/PagesController.cs`
- Modify: `Flower.Backend/Controllers/Api/PromotionsController.cs`
- Modify: `Flower.Backend/Controllers/Api/CouponsController.cs`
- Modify: `Flower.Backend/Controllers/Api/PostsController.cs`
- Modify: `Flower.Backend/Controllers/Api/LayoutApiController.cs`
- Modify: `Flower.Backend/Controllers/Api/SettingsApiController.cs`

**Interfaces:**
- Consumes: Existing `PagedResult<T>`, `AllSystemSettingsViewModel`, `HeaderLayoutDTO`, `FooterColumnDTO`, `ISystemSettingService.GetAllSettings()` / `SaveAllSettings()`
- Produces:
  - `IAdvertisementService.GetPaged(int, int) → Task<PagedResult<AdvertisementDTO>>`
  - `IPageService.GetPaged(int, int) → Task<PagedResult<PageDTO>>`
  - `IPromotionService.GetPaged(int, int) → Task<PagedResult<PromotionCampaignDTO>>`
  - `ICouponService.GetPaged(int, int) → Task<PagedResult<CouponDTO>>`
  - `IPostService.GetPaged(int, int, string?) → Task<PagedResult<PostDTO>>` (search param added)
  - `GET /api/advertisements/paged?page=1&pageSize=10 → PagedResult<AdvertisementDTO>`
  - `GET /api/pages/paged?page=1&pageSize=10 → PagedResult<PageDTO>`
  - `GET /api/promotions/paged?page=1&pageSize=10 → PagedResult<PromotionCampaignDTO>`
  - `GET /api/coupons/paged?page=1&pageSize=10 → PagedResult<CouponDTO>`
  - `GET /api/posts/paged?page=1&pageSize=10&search= → PagedResult<PostDTO>` (search added)
  - `PUT /api/layout/header` — accepts `HeaderLayoutDTO`, saves to "HeaderLayout" key
  - `PUT /api/layout/footer` — accepts `List<FooterColumnDTO>`, saves to "FooterLayout" key
  - `GET /api/settings` — returns `AllSystemSettingsViewModel`
  - `PUT /api/settings/store-info` — body `StoreInfoSettings`, saves "StoreInfo"
  - `PUT /api/settings/smtp` — body `SmtpSettings`, saves "Smtp"
  - `PUT /api/settings/vnpay` — body `VNPaySettings`, saves "VNPay"
  - `PUT /api/settings/shipping` — body `ShippingSettings`, saves "Shipping"
  - `PUT /api/settings/order` — body `OrderSettings`, saves "Order"

**Implementation steps:**

- [ ] **Step 1: Add `GetPaged` to 4 service interfaces (IAdvertisementService, IPageService, IPromotionService, ICouponService)**

  Add this line after the existing `GetAll()` method in each:
  ```csharp
  Task<PagedResult<T>> GetPaged(int page, int pageSize);
  ```

  For IAdvertisementService:
  ```csharp
  Task<PagedResult<AdvertisementDTO>> GetPaged(int page, int pageSize);
  ```

  For IPageService:
  ```csharp
  Task<PagedResult<PageDTO>> GetPaged(int page, int pageSize);
  ```

  For IPromotionService:
  ```csharp
  Task<PagedResult<PromotionCampaignDTO>> GetPaged(int page, int pageSize);
  ```

  For ICouponService:
  ```csharp
  Task<PagedResult<CouponDTO>> GetPaged(int page, int pageSize);
  ```

- [ ] **Step 2: Update IPostService — add search param to `GetPaged`**

  Change:
  ```csharp
  Task<PagedResult<PostDTO>> GetPaged(int page, int pageSize);
  ```
  To:
  ```csharp
  Task<PagedResult<PostDTO>> GetPaged(int page, int pageSize, string? search = null);
  ```

- [ ] **Step 3: Implement `GetPaged` in AdvertisementService**

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
  Ensure the file has `using Microsoft.EntityFrameworkCore;` at the top.

- [ ] **Step 4: Implement `GetPaged` in PageService**

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

- [ ] **Step 5: Implement `GetPaged` in PromotionService**

  Open `Flower.Backend/Services/PromotionService.cs`. Add after `GetAll()`:
  ```csharp
  public async Task<PagedResult<PromotionCampaignDTO>> GetPaged(int page, int pageSize)
  {
      var query = _context.PromotionCampaigns
          .OrderByDescending(p => p.CreatedAt);

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

- [ ] **Step 6: Implement `GetPaged` in CouponService**

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

- [ ] **Step 7: Add `search` filter to PostService.GetPaged**

  Replace the existing `GetPaged` method signature and add the search filter:
  ```csharp
  public async Task<PagedResult<PostDTO>> GetPaged(int page, int pageSize, string? search = null)
  {
      var query = _context.Posts
          .Include(p => p.Category)
          .OrderByDescending(p => p.Id);

      if (!string.IsNullOrWhiteSpace(search))
      {
          query = (IOrderedQueryable<Post>)query
              .Where(p => p.Title.Contains(search) || (p.Summary != null && p.Summary.Contains(search)));
      }

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
  Note: The cast `(IOrderedQueryable<Post>)` handles the IQueryable→IOrderedQueryable issue after Where(). An alternative is to use `IQueryable<Post>` for `query` and call `OrderByDescending` after the optional Where.

  **Better implementation without cast:**
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

- [ ] **Step 8: Add `GetPaged` action to AdvertisementsController**

  Add after the existing `GetAll()` action (around line 36):
  ```csharp
  [Authorize(Policy = "StaffOnly")]
  [HttpGet("paged")]
  public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
  {
      var result = await _advertisementService.GetPaged(page, pageSize);
      return Ok(result);
  }
  ```

- [ ] **Step 9: Add `GetPaged` action to PagesController**

  Add after the existing `GetAll()` action:
  ```csharp
  [Authorize(Policy = "StaffOnly")]
  [HttpGet("paged")]
  public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
  {
      var result = await _pageService.GetPaged(page, pageSize);
      return Ok(result);
  }
  ```

- [ ] **Step 10: Add `GetPaged` action to PromotionsController**

  Add after the existing `GetAll()` action:
  ```csharp
  [Authorize(Policy = "AdminOnly")]
  [HttpGet("paged")]
  public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
  {
      var result = await _promotionService.GetPaged(page, pageSize);
      return Ok(result);
  }
  ```

- [ ] **Step 11: Add `GetPaged` action to CouponsController**

  Add after the existing `GetAll()` action:
  ```csharp
  [Authorize(Policy = "StaffOnly")]
  [HttpGet("paged")]
  public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
  {
      var result = await _couponService.GetPaged(page, pageSize);
      return Ok(result);
  }
  ```

- [ ] **Step 12: Add `search` query param to PostsController.GetPaged**

  Change existing `GetPaged` action:
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
  And change the service call from:
  ```csharp
  var result = await _postService.GetPaged(page, pageSize);
  ```
  To:
  ```csharp
  var result = await _postService.GetPaged(page, pageSize, search);
  ```

- [ ] **Step 13: Add layout write endpoints to LayoutApiController**

  Add before the closing brace of the class:
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

- [ ] **Step 14: Add settings read + write endpoints to SettingsApiController**

  Replace the existing file content with:
  ```csharp
  using Flower.Backend.Models.DTOs;
  using Flower.Backend.Services.Interfaces;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using System.Threading.Tasks;

  namespace Flower.Backend.Controllers.Api
  {
      [Route("api/settings")]
      [ApiController]
      public class SettingsApiController : ControllerBase
      {
          private readonly ISystemSettingService _settingService;

          public SettingsApiController(ISystemSettingService settingService)
          {
              _settingService = settingService;
          }

          [AllowAnonymous]
          [HttpGet("store-info")]
          public async Task<IActionResult> GetStoreInfo()
          {
              var storeInfo = await _settingService.GetSetting<StoreInfoSettings>("StoreInfo");
              return Ok(storeInfo);
          }

          [AllowAnonymous]
          [HttpGet("checkout")]
          public async Task<IActionResult> GetCheckoutSettings()
          {
              var shipping = await _settingService.GetSetting<ShippingSettings>("Shipping") ?? new ShippingSettings();
              var order = await _settingService.GetSetting<OrderSettings>("Order") ?? new OrderSettings();
              return Ok(new { shipping, order });
          }

          [Authorize(Policy = "StaffOnly")]
          [HttpGet]
          public async Task<IActionResult> GetAll()
          {
              var settings = await _settingService.GetAllSettings();
              return Ok(settings);
          }

          [Authorize(Policy = "StaffOnly")]
          [HttpPut("store-info")]
          public async Task<IActionResult> SaveStoreInfo([FromBody] StoreInfoSettings dto)
          {
              var username = User.Identity?.Name ?? "System";
              await _settingService.SaveSetting("StoreInfo", dto, username);
              return NoContent();
          }

          [Authorize(Policy = "StaffOnly")]
          [HttpPut("smtp")]
          public async Task<IActionResult> SaveSmtp([FromBody] SmtpSettings dto)
          {
              var username = User.Identity?.Name ?? "System";
              await _settingService.SaveSetting("Smtp", dto, username);
              return NoContent();
          }

          [Authorize(Policy = "StaffOnly")]
          [HttpPut("vnpay")]
          public async Task<IActionResult> SaveVnPay([FromBody] VNPaySettings dto)
          {
              var username = User.Identity?.Name ?? "System";
              await _settingService.SaveSetting("VNPay", dto, username);
              return NoContent();
          }

          [Authorize(Policy = "StaffOnly")]
          [HttpPut("shipping")]
          public async Task<IActionResult> SaveShipping([FromBody] ShippingSettings dto)
          {
              var username = User.Identity?.Name ?? "System";
              await _settingService.SaveSetting("Shipping", dto, username);
              return NoContent();
          }

          [Authorize(Policy = "StaffOnly")]
          [HttpPut("order")]
          public async Task<IActionResult> SaveOrder([FromBody] OrderSettings dto)
          {
              var username = User.Identity?.Name ?? "System";
              await _settingService.SaveSetting("Order", dto, username);
              return NoContent();
          }
      }
  }
  ```

- [ ] **Step 15: Build backend and verify compilation**

  Run: `dotnet build`
  Expected: Build succeeded with 0 errors.

- [ ] **Step 16: Commit**

  ```bash
  git add Flower.Backend/Controllers/Api/AdvertisementsController.cs Flower.Backend/Controllers/Api/PagesController.cs Flower.Backend/Controllers/Api/PromotionsController.cs Flower.Backend/Controllers/Api/CouponsController.cs Flower.Backend/Controllers/Api/PostsController.cs Flower.Backend/Controllers/Api/LayoutApiController.cs Flower.Backend/Controllers/Api/SettingsApiController.cs Flower.Backend/Services/Interfaces/IAdvertisementService.cs Flower.Backend/Services/Interfaces/IPageService.cs Flower.Backend/Services/Interfaces/IPromotionService.cs Flower.Backend/Services/Interfaces/ICouponService.cs Flower.Backend/Services/Interfaces/IPostService.cs Flower.Backend/Services/AdvertisementService.cs Flower.Backend/Services/PageService.cs Flower.Backend/Services/PromotionService.cs Flower.Backend/Services/CouponService.cs Flower.Backend/Services/PostService.cs
  git commit -m "feat(backend): add pagination + settings write endpoints for Phase 4"
  ```

---

### Task 2: Frontend — Types + API Modules

**Files:**
- Create: All `src/types/*.ts` files listed above
- Create: All `src/api/*.ts` files listed above

**Interfaces:**
- Consumes: Backend endpoints from Task 1; existing `PaginatedResponse<T>` at `@/types/api`
- Produces: Type and API interfaces that Task 3–5 consume

- [ ] **Step 1: Create `src/types/advertisement.ts`**

  ```typescript
  export interface AdvertisementDTO {
    id: number
    title: string
    subtitle?: string
    imageUrl?: string
    linkUrl?: string
    sortOrder: number
    isActive: boolean
    createdAt: string
  }

  export interface CreateAdvertisementDTO {
    title: string
    subtitle?: string
    imageUrl?: string
    linkUrl?: string
    sortOrder: number
    isActive?: boolean
  }

  export interface UpdateAdvertisementDTO {
    id: number
    title: string
    subtitle?: string
    imageUrl?: string
    linkUrl?: string
    sortOrder: number
    isActive?: boolean
  }
  ```

- [ ] **Step 2: Create `src/types/page.ts`**

  ```typescript
  export interface PageDTO {
    id: number
    title: string
    slug?: string
    content: string
    isActive: boolean
    createdAt: string
    updatedAt?: string
  }

  export interface CreatePageDTO {
    title: string
    slug?: string
    content: string
    isActive?: boolean
  }

  export interface UpdatePageDTO {
    id: number
    title: string
    slug?: string
    content: string
    isActive?: boolean
  }
  ```

- [ ] **Step 3: Create `src/types/post.ts`**

  ```typescript
  export interface PostDTO {
    id: number
    title: string
    content: string
    summary?: string
    slug?: string
    imageUrl: string
    createdDate: string
    categoryId: number
    categoryName?: string
  }

  export interface CreatePostDTO {
    title: string
    content: string
    summary?: string
    slug?: string
    imageUrl?: string
    categoryId: number
  }

  export interface UpdatePostDTO {
    id: number
    title: string
    content: string
    summary?: string
    slug?: string
    imageUrl?: string
    categoryId: number
  }
  ```

- [ ] **Step 4: Create `src/types/layout.ts`**

  ```typescript
  export interface TopBarDTO {
    isActive: boolean
    text?: string
    url?: string
  }

  export interface ZonesDTO {
    left: string[]
    center: string[]
    right: string[]
  }

  export interface CtaButtonDTO {
    isActive: boolean
    text?: string
    url?: string
    variant?: string
  }

  export interface HotlineConfigDTO {
    useDefault: boolean
    customText?: string
  }

  export interface SearchConfigDTO {
    mode: string
  }

  export interface MenuItemDTO {
    id: string
    label: string
    url: string
    isExternal?: boolean
    children?: MenuItemDTO[]
  }

  export interface FooterLinkDTO {
    id: string
    label: string
    type: string
    pageId?: number
    url?: string
  }

  export interface FooterColumnDTO {
    title: string
    align: string
    sortOrder: number
    type: string
    isActive: boolean
    links: FooterLinkDTO[]
  }

  export interface HeaderLayoutDTO {
    topBar: TopBarDTO
    zones: ZonesDTO
    ctaButton: CtaButtonDTO
    hotline: HotlineConfigDTO
    search: SearchConfigDTO
    menuItems: MenuItemDTO[]
  }

  export interface LayoutResponse {
    header: HeaderLayoutDTO
    footer: FooterColumnDTO[]
    storeInfo: StoreInfoSettings
  }

  // Re-export from settings for LayoutResponse.storeInfo
  import type { StoreInfoSettings } from './settings'
  export type { StoreInfoSettings }
  ```

- [ ] **Step 5: Create `src/types/settings.ts`**

  ```typescript
  export interface StoreInfoSettings {
    storeName: string
    logo: string
    hotline: string
    email: string
    address: string
    facebook?: string
    zalo?: string
    openHours?: string
    googleMapsEmbedUrl?: string
  }

  export interface SmtpSettings {
    host: string
    port: number
    username: string
    password: string
    senderName: string
    senderEmail: string
  }

  export interface VNPaySettings {
    tmnCode: string
    hashSecret: string
    returnUrl: string
    isSandbox: boolean
    enablePayment: boolean
  }

  export interface ShippingSettings {
    defaultFee: number
    freeShipFrom: number
    maxDistance: number
    deliveryTime: string
  }

  export interface OrderSettings {
    autoCancelMinutes: number
    enableCOD: boolean
    enableOnlinePayment: boolean
  }

  export interface AllSystemSettings {
    store: StoreInfoSettings
    smtp: SmtpSettings
    vnPay: VNPaySettings
    shipping: ShippingSettings
    order: OrderSettings
  }
  ```

- [ ] **Step 6: Create `src/types/promotion.ts`**

  ```typescript
  export type PromotionType = 'Automatic' | 'Manual'
  export type DiscountType = 'Percentage' | 'FixedAmount'

  export interface PromotionCampaignDTO {
    id: number
    name: string
    description?: string
    promotionType: PromotionType
    discountType: DiscountType
    discountValue: number
    startDate: string
    endDate: string
    priority: number
    bannerImage?: string
    isStackable: boolean
    isActive: boolean
    createdAt: string
    updatedAt?: string
    productIds?: number[]
  }
  ```

- [ ] **Step 7: Create `src/types/coupon.ts`**

  ```typescript
  export type DiscountType = 'Percentage' | 'FixedAmount'

  export interface CouponDTO {
    id: number
    code: string
    description?: string
    discountType: DiscountType
    discountValue: number
    minimumOrderAmount?: number
    maximumDiscountAmount?: number
    usageLimit?: number
    usedCount: number
    usagePerCustomer?: number
    customerId?: number
    startDate?: string
    endDate?: string
    isPublic: boolean
    isActive: boolean
    createdAt: string
    updatedAt?: string
  }

  export interface CreateCouponDTO {
    code: string
    description?: string
    discountType: DiscountType
    discountValue: number
    minimumOrderAmount?: number
    maximumDiscountAmount?: number
    usageLimit?: number
    usagePerCustomer?: number
    customerId?: number
    startDate?: string
    endDate?: string
    isPublic?: boolean
    isActive?: boolean
  }

  export interface UpdateCouponDTO {
    id: number
    code: string
    description?: string
    discountType: DiscountType
    discountValue: number
    minimumOrderAmount?: number
    maximumDiscountAmount?: number
    usageLimit?: number
    usagePerCustomer?: number
    customerId?: number
    startDate?: string
    endDate?: string
    isPublic?: boolean
    isActive?: boolean
  }

  export interface CouponUsageDTO {
    id: number
    couponId: number
    customerId: number
    orderId: number
    discountAmount: number
    usedAt: string
    couponCode?: string
    customerName?: string
  }
  ```

- [ ] **Step 8: Create `src/api/advertisements.ts`**

  ```typescript
  import { apiClient } from './client'
  import type { PaginatedResponse } from '@/types/api'
  import type { AdvertisementDTO, CreateAdvertisementDTO, UpdateAdvertisementDTO } from '@/types/advertisement'

  export const advertisementsApi = {
    getPaged(page = 1, pageSize = 10) {
      return apiClient.get<PaginatedResponse<AdvertisementDTO>>('/api/advertisements/paged', { params: { page, pageSize } })
    },
    getById(id: number) {
      return apiClient.get<AdvertisementDTO>(`/api/advertisements/${id}`)
    },
    create(dto: CreateAdvertisementDTO) {
      return apiClient.post<AdvertisementDTO>('/api/advertisements', dto)
    },
    update(id: number, dto: UpdateAdvertisementDTO) {
      return apiClient.put(`/api/advertisements/${id}`, dto)
    },
    delete(id: number) {
      return apiClient.delete(`/api/advertisements/${id}`)
    },
  }
  ```

- [ ] **Step 9: Create `src/api/pages.ts`**

  ```typescript
  import { apiClient } from './client'
  import type { PaginatedResponse } from '@/types/api'
  import type { PageDTO, CreatePageDTO, UpdatePageDTO } from '@/types/page'

  export const pagesApi = {
    getPaged(page = 1, pageSize = 10) {
      return apiClient.get<PaginatedResponse<PageDTO>>('/api/pages/paged', { params: { page, pageSize } })
    },
    getById(id: number) {
      return apiClient.get<PageDTO>(`/api/pages/${id}`)
    },
    getBySlug(slug: string) {
      return apiClient.get<PageDTO>(`/api/pages/slug/${slug}`)
    },
    create(dto: CreatePageDTO) {
      return apiClient.post<PageDTO>('/api/pages', dto)
    },
    update(id: number, dto: UpdatePageDTO) {
      return apiClient.put(`/api/pages/${id}`, dto)
    },
    delete(id: number) {
      return apiClient.delete(`/api/pages/${id}`)
    },
  }
  ```

- [ ] **Step 10: Create `src/api/layout.ts`**

  ```typescript
  import { apiClient } from './client'
  import type { HeaderLayoutDTO, FooterColumnDTO, LayoutResponse } from '@/types/layout'

  export const layoutApi = {
    getLayout() {
      return apiClient.get<LayoutResponse>('/api/layout')
    },
    saveHeader(dto: HeaderLayoutDTO) {
      return apiClient.put('/api/layout/header', dto)
    },
    saveFooter(dto: FooterColumnDTO[]) {
      return apiClient.put('/api/layout/footer', dto)
    },
  }
  ```

- [ ] **Step 11: Create `src/api/settings.ts`**

  ```typescript
  import { apiClient } from './client'
  import type { AllSystemSettings, StoreInfoSettings, SmtpSettings, VNPaySettings, ShippingSettings, OrderSettings } from '@/types/settings'

  export const settingsApi = {
    getAll() {
      return apiClient.get<AllSystemSettings>('/api/settings')
    },
    saveStoreInfo(dto: StoreInfoSettings) {
      return apiClient.put('/api/settings/store-info', dto)
    },
    saveSmtp(dto: SmtpSettings) {
      return apiClient.put('/api/settings/smtp', dto)
    },
    saveVnPay(dto: VNPaySettings) {
      return apiClient.put('/api/settings/vnpay', dto)
    },
    saveShipping(dto: ShippingSettings) {
      return apiClient.put('/api/settings/shipping', dto)
    },
    saveOrder(dto: OrderSettings) {
      return apiClient.put('/api/settings/order', dto)
    },
  }
  ```

- [ ] **Step 12: Create `src/api/posts.ts`**

  ```typescript
  import { apiClient } from './client'
  import type { PaginatedResponse } from '@/types/api'
  import type { PostDTO, CreatePostDTO, UpdatePostDTO } from '@/types/post'

  export interface PostsPagedParams {
    page?: number
    pageSize?: number
    search?: string
  }

  export const postsApi = {
    getPaged(params: PostsPagedParams = {}) {
      return apiClient.get<PaginatedResponse<PostDTO>>('/api/posts/paged', { params })
    },
    getById(id: number) {
      return apiClient.get<PostDTO>(`/api/posts/${id}`)
    },
    create(dto: CreatePostDTO) {
      return apiClient.post<PostDTO>('/api/posts', dto)
    },
    update(id: number, dto: UpdatePostDTO) {
      return apiClient.put(`/api/posts/${id}`, dto)
    },
    delete(id: number) {
      return apiClient.delete(`/api/posts/${id}`)
    },
  }
  ```

- [ ] **Step 13: Create `src/api/promotions.ts`**

  ```typescript
  import { apiClient } from './client'
  import type { PaginatedResponse } from '@/types/api'
  import type { PromotionCampaignDTO } from '@/types/promotion'
  import type { CreatePromotionCampaignDTO, UpdatePromotionCampaignDTO } from '@/types/promotion'

  export const promotionsApi = {
    getPaged(page = 1, pageSize = 10) {
      return apiClient.get<PaginatedResponse<PromotionCampaignDTO>>('/api/promotions/paged', { params: { page, pageSize } })
    },
    getById(id: number) {
      return apiClient.get<PromotionCampaignDTO>(`/api/promotions/${id}`)
    },
    create(dto: CreatePromotionCampaignDTO) {
      return apiClient.post<PromotionCampaignDTO>('/api/promotions', dto)
    },
    update(id: number, dto: UpdatePromotionCampaignDTO) {
      return apiClient.put(`/api/promotions/${id}`, dto)
    },
    delete(id: number) {
      return apiClient.delete(`/api/promotions/${id}`)
    },
    enable(id: number) {
      return apiClient.patch(`/api/promotions/${id}/enable`)
    },
    disable(id: number) {
      return apiClient.patch(`/api/promotions/${id}/disable`)
    },
    addProduct(id: number, productId: number) {
      return apiClient.post(`/api/promotions/${id}/products`, { productId })
    },
    removeProduct(id: number, productId: number) {
      return apiClient.delete(`/api/promotions/${id}/products/${productId}`)
    },
  }
  ```

  Note: The `CreatePromotionCampaignDTO` and `UpdatePromotionCampaignDTO` types need to be added to `src/types/promotion.ts`. Let's update the type file to include them.

  **Update Step 6: Add DTOs to promotion.ts**

  ```typescript
  export interface CreatePromotionCampaignDTO {
    name: string
    description?: string
    promotionType: PromotionType
    discountType: DiscountType
    discountValue: number
    startDate: string
    endDate: string
    priority: number
    bannerImage?: string
    isStackable: boolean
    isActive?: boolean
    productIds?: number[]
  }

  export interface UpdatePromotionCampaignDTO {
    id: number
    name: string
    description?: string
    promotionType: PromotionType
    discountType: DiscountType
    discountValue: number
    startDate: string
    endDate: string
    priority: number
    bannerImage?: string
    isStackable: boolean
    isActive?: boolean
    productIds?: number[]
  }
  ```

- [ ] **Step 14: Create `src/api/coupons.ts`**

  ```typescript
  import { apiClient } from './client'
  import type { PaginatedResponse } from '@/types/api'
  import type { CouponDTO, CreateCouponDTO, UpdateCouponDTO, CouponUsageDTO } from '@/types/coupon'

  export const couponsApi = {
    getPaged(page = 1, pageSize = 10) {
      return apiClient.get<PaginatedResponse<CouponDTO>>('/api/coupons/paged', { params: { page, pageSize } })
    },
    getById(id: number) {
      return apiClient.get<CouponDTO>(`/api/coupons/${id}`)
    },
    create(dto: CreateCouponDTO) {
      return apiClient.post<CouponDTO>('/api/coupons', dto)
    },
    update(id: number, dto: UpdateCouponDTO) {
      return apiClient.put(`/api/coupons/${id}`, dto)
    },
    delete(id: number) {
      return apiClient.delete(`/api/coupons/${id}`)
    },
    enable(id: number) {
      return apiClient.patch(`/api/coupons/${id}/enable`)
    },
    disable(id: number) {
      return apiClient.patch(`/api/coupons/${id}/disable`)
    },
    getUsages(id: number) {
      return apiClient.get<CouponUsageDTO[]>(`/api/coupons/${id}/usages`)
    },
  }
  ```

- [ ] **Step 15: Build frontend and verify compilation**

  Run: `npm run build` (or `npx tsc --noEmit`)
  Expected: Build succeeds with 0 errors.

- [ ] **Step 16: Commit**

  ```bash
  git add flower-admin.frontend/src/types/advertisement.ts flower-admin.frontend/src/types/page.ts flower-admin.frontend/src/types/post.ts flower-admin.frontend/src/types/layout.ts flower-admin.frontend/src/types/settings.ts flower-admin.frontend/src/types/promotion.ts flower-admin.frontend/src/types/coupon.ts flower-admin.frontend/src/api/advertisements.ts flower-admin.frontend/src/api/pages.ts flower-admin.frontend/src/api/layout.ts flower-admin.frontend/src/api/settings.ts flower-admin.frontend/src/api/posts.ts flower-admin.frontend/src/api/promotions.ts flower-admin.frontend/src/api/coupons.ts
  git commit -m "feat(frontend): types + API modules for Phase 4"
  ```

---

### Task 3: Frontend — Content Page (Banners, Posts, Pages, Layout)

**Files:**
- Create: `src/pages/ContentPage.tsx`
- Create: `src/pages/content/BannersTab.tsx`
- Create: `src/pages/content/PostsTab.tsx`
- Create: `src/pages/content/PagesTab.tsx`
- Create: `src/pages/content/LayoutTab.tsx`
- Create: `src/pages/content/PostFormPage.tsx`
- Create: `src/pages/content/PageFormPage.tsx`

**Interfaces:**
- Consumes: API modules from Task 2 (advertisementsApi, pagesApi, postsApi, layoutApi)
- Produces: Complete /content page with all 4 tabs + sub-routes for Post/Page forms

- [ ] **Step 1: Create `src/pages/content/BannersTab.tsx`**

  ```typescript
  import { useState } from 'react'
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { advertisementsApi } from '@/api/advertisements'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import {
    Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
    DialogFooter, DialogClose,
  } from '@/components/ui/dialog'
  import { Card, CardContent } from '@/components/ui/card'
  import { Loader2, AlertCircle, Plus, Pencil, Trash2 } from 'lucide-react'
  import { toast } from 'sonner'
  import type { AdvertisementDTO, CreateAdvertisementDTO, UpdateAdvertisementDTO } from '@/types/advertisement'

  export function BannersTab() {
    const [page, setPage] = useState(1)
    const [editItem, setEditItem] = useState<AdvertisementDTO | null>(null)
    const [dialogOpen, setDialogOpen] = useState(false)
    const queryClient = useQueryClient()

    const { data, isLoading, error } = useQuery({
      queryKey: ['advertisements', page],
      queryFn: () => advertisementsApi.getPaged(page).then((r) => r.data),
    })

    const createMutation = useMutation({
      mutationFn: (dto: CreateAdvertisementDTO) => advertisementsApi.create(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['advertisements'] }); setDialogOpen(false); toast.success('Đã thêm banner') },
      onError: () => toast.error('Không thể thêm banner'),
    })

    const updateMutation = useMutation({
      mutationFn: ({ id, dto }: { id: number; dto: UpdateAdvertisementDTO }) => advertisementsApi.update(id, dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['advertisements'] }); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật banner') },
      onError: () => toast.error('Không thể cập nhật banner'),
    })

    const deleteMutation = useMutation({
      mutationFn: (id: number) => advertisementsApi.delete(id),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['advertisements'] }); toast.success('Đã xóa banner') },
      onError: () => toast.error('Không thể xóa banner'),
    })

    const openCreate = () => { setEditItem(null); setDialogOpen(true) }
    const openEdit = (item: AdvertisementDTO) => { setEditItem(item); setDialogOpen(true) }

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault()
      const form = e.currentTarget
      const formData = new FormData(form)
      const title = formData.get('title') as string
      if (!title) return

      if (editItem) {
        updateMutation.mutate({
          id: editItem.id,
          dto: {
            id: editItem.id,
            title,
            subtitle: formData.get('subtitle') as string || undefined,
            imageUrl: formData.get('imageUrl') as string || undefined,
            linkUrl: formData.get('linkUrl') as string || undefined,
            sortOrder: Number(formData.get('sortOrder')) || 0,
            isActive: formData.get('isActive') === 'on',
          },
        })
      } else {
        createMutation.mutate({
          title,
          subtitle: formData.get('subtitle') as string || undefined,
          imageUrl: formData.get('imageUrl') as string || undefined,
          linkUrl: formData.get('linkUrl') as string || undefined,
          sortOrder: Number(formData.get('sortOrder')) || 0,
        })
      }
    }

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải banner</p></div>

    return (
      <div className="space-y-4">
        <div className="flex justify-end">
          <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
            <DialogTrigger asChild>
              <Button size="sm" onClick={openCreate}><Plus className="mr-1 size-4" />Thêm banner</Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader><DialogTitle>{editItem ? 'Sửa banner' : 'Thêm banner'}</DialogTitle></DialogHeader>
              <form onSubmit={handleSubmit} className="space-y-4">
                <input name="title" defaultValue={editItem?.title ?? ''} placeholder="Tiêu đề" required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <input name="subtitle" defaultValue={editItem?.subtitle ?? ''} placeholder="Phụ đề (tùy chọn)" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <input name="imageUrl" defaultValue={editItem?.imageUrl ?? ''} placeholder="URL hình ảnh" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <input name="linkUrl" defaultValue={editItem?.linkUrl ?? ''} placeholder="URL liên kết (tùy chọn)" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <input name="sortOrder" type="number" defaultValue={editItem?.sortOrder ?? 0} placeholder="Thứ tự" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <label className="flex items-center gap-2 text-sm">
                  <input name="isActive" type="checkbox" defaultChecked={editItem?.isActive ?? true} />
                  Kích hoạt
                </label>
                <DialogFooter>
                  <DialogClose asChild><Button variant="outline" type="button">Hủy</Button></DialogClose>
                  <Button type="submit">{editItem ? 'Cập nhật' : 'Thêm'}</Button>
                </DialogFooter>
              </form>
            </DialogContent>
          </Dialog>
        </div>

        <Card>
          <CardContent className="p-0">
            {data && data.items.length > 0 ? (
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b text-left text-muted-foreground">
                    <th className="px-4 py-3 font-medium">Tiêu đề</th>
                    <th className="px-4 py-3 font-medium">Thứ tự</th>
                    <th className="px-4 py-3 font-medium">Trạng thái</th>
                    <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.map((item) => (
                    <tr key={item.id} className="border-b last:border-0">
                      <td className="px-4 py-3">{item.title}</td>
                      <td className="px-4 py-3">{item.sortOrder}</td>
                      <td className="px-4 py-3">
                        <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                          {item.isActive ? 'Hoạt động' : 'Ẩn'}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-right">
                        <Button variant="ghost" size="icon" onClick={() => openEdit(item)}><Pencil className="size-4" /></Button>
                        <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa banner này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có banner nào</div>
            )}
          </CardContent>
        </Card>

        {data && (data.totalPages ?? 0) > 1 && (
          <div className="flex items-center justify-between text-sm">
            <span className="text-muted-foreground">Trang {data.page} / {data.totalPages}</span>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
              <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
            </div>
          </div>
        )}
      </div>
    )
  }
  ```

- [ ] **Step 2: Create `src/pages/content/PostsTab.tsx`**

  ```typescript
  import { useState } from 'react'
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { useNavigate } from 'react-router-dom'
  import { postsApi } from '@/api/posts'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import { Card, CardContent } from '@/components/ui/card'
  import { Search, Loader2, AlertCircle, Plus, Pencil, Trash2 } from 'lucide-react'
  import { toast } from 'sonner'

  export function PostsTab() {
    const [page, setPage] = useState(1)
    const [search, setSearch] = useState('')
    const navigate = useNavigate()
    const queryClient = useQueryClient()

    const { data, isLoading, error } = useQuery({
      queryKey: ['posts', page, search],
      queryFn: () => postsApi.getPaged({ page, pageSize: 10, search: search || undefined }).then((r) => r.data),
    })

    const deleteMutation = useMutation({
      mutationFn: (id: number) => postsApi.delete(id),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['posts'] }); toast.success('Đã xóa bài viết') },
      onError: () => toast.error('Không thể xóa bài viết'),
    })

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải bài viết</p></div>

    return (
      <div className="space-y-4">
        <div className="flex items-center justify-between gap-4">
          <div className="relative flex-1 max-w-sm">
            <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
            <Input placeholder="Tìm kiếm bài viết…" className="pl-9" value={search} onChange={(e) => { setSearch(e.target.value); setPage(1) }} />
          </div>
          <Button size="sm" onClick={() => navigate('/content/posts/new')}><Plus className="mr-1 size-4" />Thêm bài viết</Button>
        </div>

        <Card>
          <CardContent className="p-0">
            {data && data.items.length > 0 ? (
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b text-left text-muted-foreground">
                    <th className="px-4 py-3 font-medium">Tiêu đề</th>
                    <th className="px-4 py-3 font-medium">Danh mục</th>
                    <th className="px-4 py-3 font-medium">Ngày tạo</th>
                    <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.map((item) => (
                    <tr key={item.id} className="border-b last:border-0">
                      <td className="px-4 py-3">{item.title}</td>
                      <td className="px-4 py-3 text-muted-foreground">{item.categoryName}</td>
                      <td className="px-4 py-3 text-muted-foreground">{new Date(item.createdDate).toLocaleDateString('vi-VN')}</td>
                      <td className="px-4 py-3 text-right">
                        <Button variant="ghost" size="icon" onClick={() => navigate(`/content/posts/${item.id}/edit`)}><Pencil className="size-4" /></Button>
                        <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa bài viết này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có bài viết nào</div>
            )}
          </CardContent>
        </Card>

        {data && (data.totalPages ?? 0) > 1 && (
          <div className="flex items-center justify-between text-sm">
            <span className="text-muted-foreground">Trang {data.page} / {data.totalPages}</span>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
              <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
            </div>
          </div>
        )}
      </div>
    )
  }
  ```

- [ ] **Step 3: Create `src/pages/content/PagesTab.tsx`**

  ```typescript
  import { useState } from 'react'
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { useNavigate } from 'react-router-dom'
  import { pagesApi } from '@/api/pages'
  import { Button } from '@/components/ui/button'
  import { Card, CardContent } from '@/components/ui/card'
  import { Loader2, AlertCircle, Plus, Pencil, Trash2 } from 'lucide-react'
  import { toast } from 'sonner'

  export function PagesTab() {
    const [page, setPage] = useState(1)
    const navigate = useNavigate()
    const queryClient = useQueryClient()

    const { data, isLoading, error } = useQuery({
      queryKey: ['pages', page],
      queryFn: () => pagesApi.getPaged(page).then((r) => r.data),
    })

    const deleteMutation = useMutation({
      mutationFn: (id: number) => pagesApi.delete(id),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['pages'] }); toast.success('Đã xóa trang') },
      onError: () => toast.error('Không thể xóa trang'),
    })

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải trang</p></div>

    return (
      <div className="space-y-4">
        <div className="flex justify-end">
          <Button size="sm" onClick={() => navigate('/content/pages/new')}><Plus className="mr-1 size-4" />Thêm trang</Button>
        </div>

        <Card>
          <CardContent className="p-0">
            {data && data.items.length > 0 ? (
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b text-left text-muted-foreground">
                    <th className="px-4 py-3 font-medium">Tiêu đề</th>
                    <th className="px-4 py-3 font-medium">Slug</th>
                    <th className="px-4 py-3 font-medium">Trạng thái</th>
                    <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.map((item) => (
                    <tr key={item.id} className="border-b last:border-0">
                      <td className="px-4 py-3">{item.title}</td>
                      <td className="px-4 py-3 text-muted-foreground">/{item.slug}</td>
                      <td className="px-4 py-3">
                        <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                          {item.isActive ? 'Hiển thị' : 'Ẩn'}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-right">
                        <Button variant="ghost" size="icon" onClick={() => navigate(`/content/pages/${item.id}/edit`)}><Pencil className="size-4" /></Button>
                        <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa trang này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có trang nào</div>
            )}
          </CardContent>
        </Card>

        {data && (data.totalPages ?? 0) > 1 && (
          <div className="flex items-center justify-between text-sm">
            <span className="text-muted-foreground">Trang {data.page} / {data.totalPages}</span>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
              <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
            </div>
          </div>
        )}
      </div>
    )
  }
  ```

- [ ] **Step 4: Create `src/pages/content/LayoutTab.tsx`**

  A simple form that loads and saves header/footer layout config as JSON. The layout structure is complex, so this simplifies to a JSON editor approach for MVP:

  ```typescript
  import { useState } from 'react'
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { layoutApi } from '@/api/layout'
  import { Button } from '@/components/ui/button'
  import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
  import { Loader2, AlertCircle, Save } from 'lucide-react'
  import { toast } from 'sonner'

  export function LayoutTab() {
    const queryClient = useQueryClient()

    const { data, isLoading, error } = useQuery({
      queryKey: ['layout'],
      queryFn: () => layoutApi.getLayout().then((r) => r.data),
    })

    const [headerJson, setHeaderJson] = useState('')
    const [footerJson, setFooterJson] = useState('')

    // Sync state when data loads
    if (data && !headerJson && !footerJson) {
      if (headerJson === '') setHeaderJson(JSON.stringify(data.header, null, 2))
      if (footerJson === '') setFooterJson(JSON.stringify(data.footer, null, 2))
    }

    const saveHeaderMutation = useMutation({
      mutationFn: (dto: any) => layoutApi.saveHeader(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['layout'] }); toast.success('Đã lưu header') },
      onError: () => toast.error('Không thể lưu header'),
    })

    const saveFooterMutation = useMutation({
      mutationFn: (dto: any) => layoutApi.saveFooter(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['layout'] }); toast.success('Đã lưu footer') },
      onError: () => toast.error('Không thể lưu footer'),
    })

    const handleSaveHeader = () => {
      try {
        const parsed = JSON.parse(headerJson)
        saveHeaderMutation.mutate(parsed)
      } catch {
        toast.error('JSON header không hợp lệ')
      }
    }

    const handleSaveFooter = () => {
      try {
        const parsed = JSON.parse(footerJson)
        saveFooterMutation.mutate(parsed)
      } catch {
        toast.error('JSON footer không hợp lệ')
      }
    }

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cấu hình giao diện</p></div>

    return (
      <div className="space-y-6">
        <Card>
          <CardHeader><CardTitle className="text-base">Header</CardTitle></CardHeader>
          <CardContent className="space-y-3">
            <textarea
              className="w-full h-64 rounded-md border bg-background p-3 font-mono text-xs"
              value={headerJson}
              onChange={(e) => setHeaderJson(e.target.value)}
            />
            <Button size="sm" onClick={handleSaveHeader} disabled={saveHeaderMutation.isPending}>
              <Save className="mr-1 size-4" />Lưu header
            </Button>
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle className="text-base">Footer</CardTitle></CardHeader>
          <CardContent className="space-y-3">
            <textarea
              className="w-full h-64 rounded-md border bg-background p-3 font-mono text-xs"
              value={footerJson}
              onChange={(e) => setFooterJson(e.target.value)}
            />
            <Button size="sm" onClick={handleSaveFooter} disabled={saveFooterMutation.isPending}>
              <Save className="mr-1 size-4" />Lưu footer
            </Button>
          </CardContent>
        </Card>
      </div>
    )
  }
  ```

- [ ] **Step 5: Create `src/pages/content/PostFormPage.tsx`**

  ```typescript
  import { useState, useEffect } from 'react'
  import { useNavigate, useParams } from 'react-router-dom'
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { postsApi } from '@/api/posts'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
  import { Loader2, ArrowLeft, Save } from 'lucide-react'
  import { toast } from 'sonner'
  import type { CreatePostDTO, UpdatePostDTO } from '@/types/post'

  export function PostFormPage() {
    const { id } = useParams()
    const isEdit = !!id
    const navigate = useNavigate()
    const queryClient = useQueryClient()

    const { data: post, isLoading } = useQuery({
      queryKey: ['post', id],
      queryFn: () => postsApi.getById(Number(id)).then((r) => r.data),
      enabled: isEdit,
    })

    const [title, setTitle] = useState('')
    const [content, setContent] = useState('')
    const [summary, setSummary] = useState('')
    const [slug, setSlug] = useState('')
    const [imageUrl, setImageUrl] = useState('')
    const [categoryId, setCategoryId] = useState(0)

    useEffect(() => {
      if (post) {
        setTitle(post.title)
        setContent(post.content)
        setSummary(post.summary ?? '')
        setSlug(post.slug ?? '')
        setImageUrl(post.imageUrl)
        setCategoryId(post.categoryId)
      }
    }, [post])

    const createMutation = useMutation({
      mutationFn: (dto: CreatePostDTO) => postsApi.create(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['posts'] }); toast.success('Đã tạo bài viết'); navigate('/content') },
      onError: () => toast.error('Không thể tạo bài viết'),
    })

    const updateMutation = useMutation({
      mutationFn: (dto: UpdatePostDTO) => postsApi.update(Number(id), dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['posts'] }); toast.success('Đã cập nhật bài viết'); navigate('/content') },
      onError: () => toast.error('Không thể cập nhật bài viết'),
    })

    const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault()
      if (!title || !content || !categoryId) { toast.error('Vui lòng điền đầy đủ thông tin'); return }

      if (isEdit) {
        updateMutation.mutate({ id: Number(id), title, content, summary: summary || undefined, slug: slug || undefined, imageUrl: imageUrl || undefined, categoryId })
      } else {
        createMutation.mutate({ title, content, summary: summary || undefined, slug: slug || undefined, imageUrl: imageUrl || undefined, categoryId })
      }
    }

    if (isEdit && isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>

    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={() => navigate('/content')}><ArrowLeft className="size-5" /></Button>
          <h1 className="text-2xl font-semibold">{isEdit ? 'Sửa bài viết' : 'Thêm bài viết'}</h1>
        </div>

        <Card>
          <CardHeader><CardTitle className="text-base">Thông tin bài viết</CardTitle></CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="text-sm font-medium">Tiêu đề *</label>
                <Input value={title} onChange={(e) => setTitle(e.target.value)} required />
              </div>
              <div>
                <label className="text-sm font-medium">Tóm tắt</label>
                <Input value={summary} onChange={(e) => setSummary(e.target.value)} />
              </div>
              <div>
                <label className="text-sm font-medium">Slug</label>
                <Input value={slug} onChange={(e) => setSlug(e.target.value)} placeholder="tu-khoa-tieng-viet" />
              </div>
              <div>
                <label className="text-sm font-medium">URL hình ảnh</label>
                <Input value={imageUrl} onChange={(e) => setImageUrl(e.target.value)} />
              </div>
              <div>
                <label className="text-sm font-medium">ID danh mục *</label>
                <Input type="number" value={categoryId || ''} onChange={(e) => setCategoryId(Number(e.target.value))} required />
              </div>
              <div>
                <label className="text-sm font-medium">Nội dung *</label>
                <textarea
                  className="w-full min-h-[300px] rounded-md border bg-background p-3 text-sm"
                  value={content}
                  onChange={(e) => setContent(e.target.value)}
                  required
                />
              </div>
              <div className="flex gap-3">
                <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                  <Save className="mr-1 size-4" />{isEdit ? 'Cập nhật' : 'Tạo bài viết'}
                </Button>
                <Button variant="outline" type="button" onClick={() => navigate('/content')}>Hủy</Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    )
  }
  ```

- [ ] **Step 6: Create `src/pages/content/PageFormPage.tsx`**

  ```typescript
  import { useState, useEffect } from 'react'
  import { useNavigate, useParams } from 'react-router-dom'
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { pagesApi } from '@/api/pages'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
  import { Loader2, ArrowLeft, Save } from 'lucide-react'
  import { toast } from 'sonner'
  import type { CreatePageDTO, UpdatePageDTO } from '@/types/page'

  export function PageFormPage() {
    const { id } = useParams()
    const isEdit = !!id
    const navigate = useNavigate()
    const queryClient = useQueryClient()

    const { data: page, isLoading } = useQuery({
      queryKey: ['page', id],
      queryFn: () => pagesApi.getById(Number(id)).then((r) => r.data),
      enabled: isEdit,
    })

    const [title, setTitle] = useState('')
    const [slug, setSlug] = useState('')
    const [content, setContent] = useState('')
    const [isActive, setIsActive] = useState(true)

    useEffect(() => {
      if (page) {
        setTitle(page.title)
        setSlug(page.slug ?? '')
        setContent(page.content)
        setIsActive(page.isActive)
      }
    }, [page])

    const createMutation = useMutation({
      mutationFn: (dto: CreatePageDTO) => pagesApi.create(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['pages'] }); toast.success('Đã tạo trang'); navigate('/content') },
      onError: () => toast.error('Không thể tạo trang'),
    })

    const updateMutation = useMutation({
      mutationFn: (dto: UpdatePageDTO) => pagesApi.update(Number(id), dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['pages'] }); toast.success('Đã cập nhật trang'); navigate('/content') },
      onError: () => toast.error('Không thể cập nhật trang'),
    })

    const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault()
      if (!title || !content) { toast.error('Vui lòng điền đầy đủ thông tin'); return }

      if (isEdit) {
        updateMutation.mutate({ id: Number(id), title, slug: slug || undefined, content, isActive })
      } else {
        createMutation.mutate({ title, slug: slug || undefined, content, isActive })
      }
    }

    if (isEdit && isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>

    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={() => navigate('/content')}><ArrowLeft className="size-5" /></Button>
          <h1 className="text-2xl font-semibold">{isEdit ? 'Sửa trang' : 'Thêm trang'}</h1>
        </div>

        <Card>
          <CardHeader><CardTitle className="text-base">Thông tin trang</CardTitle></CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="text-sm font-medium">Tiêu đề *</label>
                <Input value={title} onChange={(e) => setTitle(e.target.value)} required />
              </div>
              <div>
                <label className="text-sm font-medium">Slug</label>
                <Input value={slug} onChange={(e) => setSlug(e.target.value)} placeholder="gioi-thieu" />
              </div>
              <div>
                <label className="text-sm font-medium">Nội dung *</label>
                <textarea
                  className="w-full min-h-[300px] rounded-md border bg-background p-3 text-sm"
                  value={content}
                  onChange={(e) => setContent(e.target.value)}
                  required
                />
              </div>
              <label className="flex items-center gap-2 text-sm">
                <input type="checkbox" checked={isActive} onChange={(e) => setIsActive(e.target.checked)} />
                Hiển thị
              </label>
              <div className="flex gap-3">
                <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                  <Save className="mr-1 size-4" />{isEdit ? 'Cập nhật' : 'Tạo trang'}
                </Button>
                <Button variant="outline" type="button" onClick={() => navigate('/content')}>Hủy</Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    )
  }
  ```

- [ ] **Step 7: Create `src/pages/ContentPage.tsx`** — tabbed wrapper

  ```typescript
  import { useState } from 'react'
  import { Outlet, useLocation } from 'react-router-dom'
  import { BannersTab } from './content/BannersTab'
  import { PostsTab } from './content/PostsTab'
  import { PagesTab } from './content/PagesTab'
  import { LayoutTab } from './content/LayoutTab'

  const tabs = [
    { key: 'banners', label: 'Banner' },
    { key: 'posts', label: 'Bài viết' },
    { key: 'pages', label: 'Trang tĩnh' },
    { key: 'layout', label: 'Giao diện' },
  ]

  export function ContentPage() {
    const [activeTab, setActiveTab] = useState('banners')
    const location = useLocation()

    // If URL contains sub-route (/content/posts/new, /content/posts/:id/edit, etc.), render Outlet
    if (location.pathname !== '/content') {
      return <Outlet />
    }

    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-semibold">Nội dung</h1>
        <div className="flex flex-wrap gap-2">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              onClick={() => setActiveTab(tab.key)}
              className={`px-3 py-1.5 text-sm rounded-full border transition-colors ${
                activeTab === tab.key
                  ? 'bg-primary text-primary-foreground border-primary'
                  : 'bg-background text-muted-foreground border-border hover:bg-muted'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>
        {activeTab === 'banners' && <BannersTab />}
        {activeTab === 'posts' && <PostsTab />}
        {activeTab === 'pages' && <PagesTab />}
        {activeTab === 'layout' && <LayoutTab />}
      </div>
    )
  }
  ```

- [ ] **Step 8: Build and verify compilation**

  Run: `npm run build`
  Expected: Build succeeds with 0 errors.

- [ ] **Step 9: Commit**

  ```bash
  git add flower-admin.frontend/src/pages/ContentPage.tsx flower-admin.frontend/src/pages/content/
  git commit -m "feat(frontend): ContentPage with Banners, Posts, Pages, Layout tabs"
  ```

---

### Task 4: Frontend — Marketing Page (Promotions + Coupons)

**Files:**
- Create: `src/pages/MarketingPage.tsx`
- Create: `src/pages/marketing/PromotionsTab.tsx`
- Create: `src/pages/marketing/CouponsTab.tsx`

**Interfaces:**
- Consumes: promotionsApi, couponsApi from Task 2
- Produces: Complete /marketing page

- [ ] **Step 1: Create `src/pages/marketing/PromotionsTab.tsx`**

  ```typescript
  import { useState } from 'react'
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { promotionsApi } from '@/api/promotions'
  import { Button } from '@/components/ui/button'
  import {
    Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
    DialogFooter, DialogClose,
  } from '@/components/ui/dialog'
  import { Card, CardContent } from '@/components/ui/card'
  import { Loader2, AlertCircle, Plus, Pencil, Trash2, ToggleLeft, ToggleRight } from 'lucide-react'
  import { toast } from 'sonner'
  import type { PromotionCampaignDTO } from '@/types/promotion'

  export function PromotionsTab() {
    const [page, setPage] = useState(1)
    const [editItem, setEditItem] = useState<PromotionCampaignDTO | null>(null)
    const [dialogOpen, setDialogOpen] = useState(false)
    const queryClient = useQueryClient()

    const { data, isLoading, error } = useQuery({
      queryKey: ['promotions', page],
      queryFn: () => promotionsApi.getPaged(page).then((r) => r.data),
    })

    const createMutation = useMutation({
      mutationFn: (dto: any) => promotionsApi.create(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['promotions'] }); setDialogOpen(false); toast.success('Đã thêm khuyến mãi') },
      onError: () => toast.error('Không thể thêm khuyến mãi'),
    })

    const updateMutation = useMutation({
      mutationFn: ({ id, dto }: { id: number; dto: any }) => promotionsApi.update(id, dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['promotions'] }); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật khuyến mãi') },
      onError: () => toast.error('Không thể cập nhật khuyến mãi'),
    })

    const deleteMutation = useMutation({
      mutationFn: (id: number) => promotionsApi.delete(id),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['promotions'] }); toast.success('Đã xóa khuyến mãi') },
      onError: () => toast.error('Không thể xóa khuyến mãi'),
    })

    const toggleMutation = useMutation({
      mutationFn: ({ id, enable }: { id: number; enable: boolean }) => enable ? promotionsApi.enable(id) : promotionsApi.disable(id),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['promotions'] }); toast.success('Đã cập nhật trạng thái') },
    })

    const openCreate = () => { setEditItem(null); setDialogOpen(true) }
    const openEdit = (item: PromotionCampaignDTO) => { setEditItem(item); setDialogOpen(true) }

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault()
      const form = e.currentTarget
      const formData = new FormData(form)
      const name = formData.get('name') as string
      if (!name) return

      const dto: any = {
        name,
        description: formData.get('description') as string || undefined,
        discountType: formData.get('discountType') as string,
        discountValue: Number(formData.get('discountValue')),
        startDate: formData.get('startDate') as string,
        endDate: formData.get('endDate') as string,
        priority: Number(formData.get('priority')) || 0,
        isStackable: formData.get('isStackable') === 'on',
      }

      if (editItem) {
        dto.id = editItem.id
        updateMutation.mutate({ id: editItem.id, dto })
      } else {
        createMutation.mutate(dto)
      }
    }

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải khuyến mãi</p></div>

    return (
      <div className="space-y-4">
        <div className="flex justify-end">
          <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
            <DialogTrigger asChild>
              <Button size="sm" onClick={openCreate}><Plus className="mr-1 size-4" />Thêm khuyến mãi</Button>
            </DialogTrigger>
            <DialogContent className="max-w-lg">
              <DialogHeader><DialogTitle>{editItem ? 'Sửa khuyến mãi' : 'Thêm khuyến mãi'}</DialogTitle></DialogHeader>
              <form onSubmit={handleSubmit} className="space-y-3">
                <input name="name" defaultValue={editItem?.name ?? ''} placeholder="Tên khuyến mãi" required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <input name="description" defaultValue={editItem?.description ?? ''} placeholder="Mô tả" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <div className="grid grid-cols-2 gap-3">
                  <select name="discountType" defaultValue={editItem?.discountType ?? 'Percentage'} className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm">
                    <option value="Percentage">Phần trăm</option>
                    <option value="FixedAmount">Số tiền cố định</option>
                  </select>
                  <input name="discountValue" type="number" step="0.01" defaultValue={editItem?.discountValue ?? 0} required placeholder="Giá trị" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <input name="startDate" type="datetime-local" defaultValue={editItem?.startDate ? editItem.startDate.substring(0, 16) : ''} required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                  <input name="endDate" type="datetime-local" defaultValue={editItem?.endDate ? editItem.endDate.substring(0, 16) : ''} required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <input name="priority" type="number" defaultValue={editItem?.priority ?? 0} placeholder="Ưu tiên" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                  <label className="flex items-center gap-2 text-sm">
                    <input name="isStackable" type="checkbox" defaultChecked={editItem?.isStackable ?? false} />
                    Cộng dồn
                  </label>
                </div>
                <DialogFooter>
                  <DialogClose asChild><Button variant="outline" type="button">Hủy</Button></DialogClose>
                  <Button type="submit">{editItem ? 'Cập nhật' : 'Thêm'}</Button>
                </DialogFooter>
              </form>
            </DialogContent>
          </Dialog>
        </div>

        <Card>
          <CardContent className="p-0">
            {data && data.items.length > 0 ? (
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b text-left text-muted-foreground">
                    <th className="px-4 py-3 font-medium">Tên</th>
                    <th className="px-4 py-3 font-medium">Loại giảm</th>
                    <th className="px-4 py-3 font-medium">Giá trị</th>
                    <th className="px-4 py-3 font-medium">Ngày</th>
                    <th className="px-4 py-3 font-medium">Trạng thái</th>
                    <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.map((item) => (
                    <tr key={item.id} className="border-b last:border-0">
                      <td className="px-4 py-3">{item.name}</td>
                      <td className="px-4 py-3 text-muted-foreground">{item.discountType === 'Percentage' ? '%' : 'VNĐ'}</td>
                      <td className="px-4 py-3">{item.discountValue.toLocaleString()}</td>
                      <td className="px-4 py-3 text-muted-foreground text-xs">
                        {new Date(item.startDate).toLocaleDateString('vi-VN')} - {new Date(item.endDate).toLocaleDateString('vi-VN')}
                      </td>
                      <td className="px-4 py-3">
                        <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                          {item.isActive ? 'Kích hoạt' : 'Tắt'}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-right">
                        <Button variant="ghost" size="icon" onClick={() => toggleMutation.mutate({ id: item.id, enable: !item.isActive })}>
                          {item.isActive ? <ToggleRight className="size-4" /> : <ToggleLeft className="size-4" />}
                        </Button>
                        <Button variant="ghost" size="icon" onClick={() => openEdit(item)}><Pencil className="size-4" /></Button>
                        <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa khuyến mãi này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có khuyến mãi nào</div>
            )}
          </CardContent>
        </Card>

        {data && (data.totalPages ?? 0) > 1 && (
          <div className="flex items-center justify-between text-sm">
            <span className="text-muted-foreground">Trang {data.page} / {data.totalPages}</span>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
              <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
            </div>
          </div>
        )}
      </div>
    )
  }
  ```

- [ ] **Step 2: Create `src/pages/marketing/CouponsTab.tsx`**

  Similar to PromotionsTab but for coupons:
  ```typescript
  import { useState } from 'react'
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { couponsApi } from '@/api/coupons'
  import { Button } from '@/components/ui/button'
  import {
    Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
    DialogFooter, DialogClose,
  } from '@/components/ui/dialog'
  import { Card, CardContent } from '@/components/ui/card'
  import { Loader2, AlertCircle, Plus, Pencil, Trash2, ToggleLeft, ToggleRight, Eye } from 'lucide-react'
  import { toast } from 'sonner'
  import type { CouponDTO } from '@/types/coupon'

  export function CouponsTab() {
    const [page, setPage] = useState(1)
    const [editItem, setEditItem] = useState<CouponDTO | null>(null)
    const [dialogOpen, setDialogOpen] = useState(false)
    const [usagesItem, setUsagesItem] = useState<CouponDTO | null>(null)
    const [usagesOpen, setUsagesOpen] = useState(false)
    const queryClient = useQueryClient()

    const { data, isLoading, error } = useQuery({
      queryKey: ['coupons', page],
      queryFn: () => couponsApi.getPaged(page).then((r) => r.data),
    })

    const { data: usages } = useQuery({
      queryKey: ['coupon-usages', usagesItem?.id],
      queryFn: () => couponsApi.getUsages(usagesItem!.id).then((r) => r.data),
      enabled: !!usagesItem && usagesOpen,
    })

    const createMutation = useMutation({
      mutationFn: (dto: any) => couponsApi.create(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['coupons'] }); setDialogOpen(false); toast.success('Đã thêm mã giảm giá') },
      onError: () => toast.error('Không thể thêm mã giảm giá'),
    })

    const updateMutation = useMutation({
      mutationFn: ({ id, dto }: { id: number; dto: any }) => couponsApi.update(id, dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['coupons'] }); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật mã giảm giá') },
      onError: () => toast.error('Không thể cập nhật mã giảm giá'),
    })

    const deleteMutation = useMutation({
      mutationFn: (id: number) => couponsApi.delete(id),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['coupons'] }); toast.success('Đã xóa mã giảm giá') },
      onError: () => toast.error('Không thể xóa mã giảm giá'),
    })

    const toggleMutation = useMutation({
      mutationFn: ({ id, enable }: { id: number; enable: boolean }) => enable ? couponsApi.enable(id) : couponsApi.disable(id),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['coupons'] }); toast.success('Đã cập nhật trạng thái') },
    })

    const openCreate = () => { setEditItem(null); setDialogOpen(true) }
    const openEdit = (item: CouponDTO) => { setEditItem(item); setDialogOpen(true) }

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault()
      const form = e.currentTarget
      const formData = new FormData(form)
      const code = formData.get('code') as string
      if (!code) return

      const dto: any = {
        code,
        description: formData.get('description') as string || undefined,
        discountType: formData.get('discountType') as string,
        discountValue: Number(formData.get('discountValue')),
        minimumOrderAmount: Number(formData.get('minimumOrderAmount')) || undefined,
        maximumDiscountAmount: Number(formData.get('maximumDiscountAmount')) || undefined,
        usageLimit: Number(formData.get('usageLimit')) || undefined,
        usagePerCustomer: Number(formData.get('usagePerCustomer')) || undefined,
        isPublic: formData.get('isPublic') === 'on',
      }

      if (editItem) {
        dto.id = editItem.id
        updateMutation.mutate({ id: editItem.id, dto })
      } else {
        createMutation.mutate(dto)
      }
    }

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải mã giảm giá</p></div>

    return (
      <div className="space-y-4">
        <div className="flex justify-end">
          <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
            <DialogTrigger asChild>
              <Button size="sm" onClick={openCreate}><Plus className="mr-1 size-4" />Thêm mã giảm giá</Button>
            </DialogTrigger>
            <DialogContent className="max-w-lg">
              <DialogHeader><DialogTitle>{editItem ? 'Sửa mã giảm giá' : 'Thêm mã giảm giá'}</DialogTitle></DialogHeader>
              <form onSubmit={handleSubmit} className="space-y-3">
                <input name="code" defaultValue={editItem?.code ?? ''} placeholder="Mã giảm giá" required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm font-mono uppercase" />
                <input name="description" defaultValue={editItem?.description ?? ''} placeholder="Mô tả" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <div className="grid grid-cols-2 gap-3">
                  <select name="discountType" defaultValue={editItem?.discountType ?? 'Percentage'} className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm">
                    <option value="Percentage">Phần trăm</option>
                    <option value="FixedAmount">Số tiền cố định</option>
                  </select>
                  <input name="discountValue" type="number" step="0.01" defaultValue={editItem?.discountValue ?? 0} required placeholder="Giá trị" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <input name="minimumOrderAmount" type="number" defaultValue={editItem?.minimumOrderAmount ?? ''} placeholder="Đơn tối thiểu" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                  <input name="maximumDiscountAmount" type="number" defaultValue={editItem?.maximumDiscountAmount ?? ''} placeholder="Giảm tối đa" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <input name="usageLimit" type="number" defaultValue={editItem?.usageLimit ?? ''} placeholder="SL sử dụng" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                  <input name="usagePerCustomer" type="number" defaultValue={editItem?.usagePerCustomer ?? ''} placeholder="SL/khách" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                </div>
                <label className="flex items-center gap-2 text-sm">
                  <input name="isPublic" type="checkbox" defaultChecked={editItem?.isPublic ?? true} />
                  Công khai
                </label>
                <DialogFooter>
                  <DialogClose asChild><Button variant="outline" type="button">Hủy</Button></DialogClose>
                  <Button type="submit">{editItem ? 'Cập nhật' : 'Thêm'}</Button>
                </DialogFooter>
              </form>
            </DialogContent>
          </Dialog>
        </div>

        <Card>
          <CardContent className="p-0">
            {data && data.items.length > 0 ? (
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b text-left text-muted-foreground">
                    <th className="px-4 py-3 font-medium">Mã</th>
                    <th className="px-4 py-3 font-medium">Giảm</th>
                    <th className="px-4 py-3 font-medium">Đã dùng</th>
                    <th className="px-4 py-3 font-medium">Trạng thái</th>
                    <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {data.items.map((item) => (
                    <tr key={item.id} className="border-b last:border-0">
                      <td className="px-4 py-3 font-mono">{item.code}</td>
                      <td className="px-4 py-3">{item.discountType === 'Percentage' ? `${item.discountValue}%` : `${item.discountValue.toLocaleString()}₫`}</td>
                      <td className="px-4 py-3 text-muted-foreground">{item.usedCount}/{item.usageLimit ?? '∞'}</td>
                      <td className="px-4 py-3">
                        <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                          {item.isActive ? 'Kích hoạt' : 'Tắt'}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-right">
                        <Button variant="ghost" size="icon" onClick={() => { setUsagesItem(item); setUsagesOpen(true) }}><Eye className="size-4" /></Button>
                        <Button variant="ghost" size="icon" onClick={() => toggleMutation.mutate({ id: item.id, enable: !item.isActive })}>
                          {item.isActive ? <ToggleRight className="size-4" /> : <ToggleLeft className="size-4" />}
                        </Button>
                        <Button variant="ghost" size="icon" onClick={() => openEdit(item)}><Pencil className="size-4" /></Button>
                        <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa mã giảm giá này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có mã giảm giá nào</div>
            )}
          </CardContent>
        </Card>

        {/* Usages dialog */}
        <Dialog open={usagesOpen} onOpenChange={setUsagesOpen}>
          <DialogContent>
            <DialogHeader><DialogTitle>Lịch sử sử dụng: {usagesItem?.code}</DialogTitle></DialogHeader>
            {usages && usages.length > 0 ? (
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b text-left text-muted-foreground">
                    <th className="py-2 font-medium">Khách hàng</th>
                    <th className="py-2 font-medium">Đơn hàng</th>
                    <th className="py-2 font-medium">Giảm</th>
                    <th className="py-2 font-medium">Ngày</th>
                  </tr>
                </thead>
                <tbody>
                  {usages.map((u) => (
                    <tr key={u.id} className="border-b last:border-0">
                      <td className="py-2">{u.customerName ?? `#${u.customerId}`}</td>
                      <td className="py-2">#{u.orderId}</td>
                      <td className="py-2">{u.discountAmount.toLocaleString()}₫</td>
                      <td className="py-2 text-xs">{new Date(u.usedAt).toLocaleDateString('vi-VN')}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <p className="text-sm text-muted-foreground">Chưa có lượt sử dụng</p>
            )}
          </DialogContent>
        </Dialog>

        {data && (data.totalPages ?? 0) > 1 && (
          <div className="flex items-center justify-between text-sm">
            <span className="text-muted-foreground">Trang {data.page} / {data.totalPages}</span>
            <div className="flex gap-2">
              <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
              <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
            </div>
          </div>
        )}
      </div>
    )
  }
  ```

- [ ] **Step 3: Create `src/pages/MarketingPage.tsx`** — tabbed wrapper

  ```typescript
  import { useState } from 'react'
  import { PromotionsTab } from './marketing/PromotionsTab'
  import { CouponsTab } from './marketing/CouponsTab'

  const tabs = [
    { key: 'promotions', label: 'Khuyến mãi' },
    { key: 'coupons', label: 'Mã giảm giá' },
  ]

  export function MarketingPage() {
    const [activeTab, setActiveTab] = useState('promotions')

    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-semibold">Marketing</h1>
        <div className="flex flex-wrap gap-2">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              onClick={() => setActiveTab(tab.key)}
              className={`px-3 py-1.5 text-sm rounded-full border transition-colors ${
                activeTab === tab.key
                  ? 'bg-primary text-primary-foreground border-primary'
                  : 'bg-background text-muted-foreground border-border hover:bg-muted'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>
        {activeTab === 'promotions' && <PromotionsTab />}
        {activeTab === 'coupons' && <CouponsTab />}
      </div>
    )
  }
  ```

- [ ] **Step 4: Build and verify compilation**

  Run: `npm run build`
  Expected: Build succeeds with 0 errors.

- [ ] **Step 5: Commit**

  ```bash
  git add flower-admin.frontend/src/pages/MarketingPage.tsx flower-admin.frontend/src/pages/marketing/
  git commit -m "feat(frontend): MarketingPage with Promotions and Coupons tabs"
  ```

---

### Task 5: Frontend — System Settings Page

**Files:**
- Create: `src/pages/SystemSettingsPage.tsx`
- Create: `src/pages/system/StoreInfoTab.tsx`
- Create: `src/pages/system/SmtpTab.tsx`
- Create: `src/pages/system/VnPayTab.tsx`
- Create: `src/pages/system/ShippingTab.tsx`
- Create: `src/pages/system/OrderTab.tsx`

**Interfaces:**
- Consumes: settingsApi from Task 2
- Produces: Complete /system page with 5 setting tabs

- [ ] **Step 1: Create `src/pages/system/StoreInfoTab.tsx`**

  ```typescript
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { settingsApi } from '@/api/settings'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
  import { Loader2, AlertCircle, Save } from 'lucide-react'
  import { toast } from 'sonner'
  import { useState, useEffect } from 'react'
  import type { StoreInfoSettings } from '@/types/settings'

  export function StoreInfoTab() {
    const queryClient = useQueryClient()
    const { data: allSettings, isLoading, error } = useQuery({
      queryKey: ['settings'],
      queryFn: () => settingsApi.getAll().then((r) => r.data),
    })

    const [form, setForm] = useState<StoreInfoSettings>({} as StoreInfoSettings)

    useEffect(() => {
      if (allSettings?.store) setForm(allSettings.store)
    }, [allSettings])

    const mutation = useMutation({
      mutationFn: (dto: StoreInfoSettings) => settingsApi.saveStoreInfo(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu thông tin cửa hàng') },
      onError: () => toast.error('Không thể lưu thông tin'),
    })

    const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault()
      mutation.mutate(form)
    }

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

    return (
      <Card>
        <CardHeader><CardTitle className="text-base">Thông tin cửa hàng</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div><label className="text-sm font-medium">Tên cửa hàng</label><Input value={form.storeName || ''} onChange={(e) => setForm({ ...form, storeName: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">Hotline</label><Input value={form.hotline || ''} onChange={(e) => setForm({ ...form, hotline: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">Email</label><Input type="email" value={form.email || ''} onChange={(e) => setForm({ ...form, email: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">Địa chỉ</label><Input value={form.address || ''} onChange={(e) => setForm({ ...form, address: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">Logo (URL)</label><Input value={form.logo || ''} onChange={(e) => setForm({ ...form, logo: e.target.value })} /></div>
              <div><label className="text-sm font-medium">Facebook</label><Input value={form.facebook || ''} onChange={(e) => setForm({ ...form, facebook: e.target.value })} /></div>
              <div><label className="text-sm font-medium">Zalo</label><Input value={form.zalo || ''} onChange={(e) => setForm({ ...form, zalo: e.target.value })} /></div>
              <div><label className="text-sm font-medium">Giờ mở cửa</label><Input value={form.openHours || ''} onChange={(e) => setForm({ ...form, openHours: e.target.value })} /></div>
            </div>
            <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
          </form>
        </CardContent>
      </Card>
    )
  }
  ```

- [ ] **Step 2: Create `src/pages/system/SmtpTab.tsx`**

  ```typescript
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { settingsApi } from '@/api/settings'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
  import { Loader2, AlertCircle, Save } from 'lucide-react'
  import { toast } from 'sonner'
  import { useState, useEffect } from 'react'
  import type { SmtpSettings } from '@/types/settings'

  export function SmtpTab() {
    const queryClient = useQueryClient()
    const { data: allSettings, isLoading, error } = useQuery({
      queryKey: ['settings'],
      queryFn: () => settingsApi.getAll().then((r) => r.data),
    })

    const [form, setForm] = useState<SmtpSettings>({} as SmtpSettings)

    useEffect(() => {
      if (allSettings?.smtp) setForm(allSettings.smtp)
    }, [allSettings])

    const mutation = useMutation({
      mutationFn: (dto: SmtpSettings) => settingsApi.saveSmtp(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu cấu hình SMTP') },
      onError: () => toast.error('Không thể lưu cấu hình SMTP'),
    })

    const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault()
      mutation.mutate(form)
    }

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

    return (
      <Card>
        <CardHeader><CardTitle className="text-base">Cấu hình SMTP</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div><label className="text-sm font-medium">Host</label><Input value={form.host || ''} onChange={(e) => setForm({ ...form, host: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">Port</label><Input type="number" value={form.port || ''} onChange={(e) => setForm({ ...form, port: Number(e.target.value) })} required /></div>
              <div><label className="text-sm font-medium">Username</label><Input value={form.username || ''} onChange={(e) => setForm({ ...form, username: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">Password</label><Input type="password" value={form.password || ''} onChange={(e) => setForm({ ...form, password: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">Tên người gửi</label><Input value={form.senderName || ''} onChange={(e) => setForm({ ...form, senderName: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">Email người gửi</label><Input type="email" value={form.senderEmail || ''} onChange={(e) => setForm({ ...form, senderEmail: e.target.value })} required /></div>
            </div>
            <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
          </form>
        </CardContent>
      </Card>
    )
  }
  ```

- [ ] **Step 3: Create `src/pages/system/VnPayTab.tsx`**

  ```typescript
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { settingsApi } from '@/api/settings'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
  import { Loader2, AlertCircle, Save } from 'lucide-react'
  import { toast } from 'sonner'
  import { useState, useEffect } from 'react'
  import type { VNPaySettings } from '@/types/settings'

  export function VnPayTab() {
    const queryClient = useQueryClient()
    const { data: allSettings, isLoading, error } = useQuery({
      queryKey: ['settings'],
      queryFn: () => settingsApi.getAll().then((r) => r.data),
    })

    const [form, setForm] = useState<VNPaySettings>({} as VNPaySettings)

    useEffect(() => {
      if (allSettings?.vnPay) setForm(allSettings.vnPay)
    }, [allSettings])

    const mutation = useMutation({
      mutationFn: (dto: VNPaySettings) => settingsApi.saveVnPay(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu cấu hình VNPay') },
      onError: () => toast.error('Không thể lưu cấu hình VNPay'),
    })

    const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault()
      mutation.mutate(form)
    }

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

    return (
      <Card>
        <CardHeader><CardTitle className="text-base">Cấu hình VNPay</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div><label className="text-sm font-medium">TmnCode</label><Input value={form.tmnCode || ''} onChange={(e) => setForm({ ...form, tmnCode: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">HashSecret</label><Input type="password" value={form.hashSecret || ''} onChange={(e) => setForm({ ...form, hashSecret: e.target.value })} required /></div>
              <div><label className="text-sm font-medium">ReturnUrl</label><Input value={form.returnUrl || ''} onChange={(e) => setForm({ ...form, returnUrl: e.target.value })} required /></div>
              <div className="flex items-end gap-4">
                <label className="flex items-center gap-2 text-sm">
                  <input type="checkbox" checked={form.isSandbox ?? true} onChange={(e) => setForm({ ...form, isSandbox: e.target.checked })} />
                  Sandbox
                </label>
                <label className="flex items-center gap-2 text-sm">
                  <input type="checkbox" checked={form.enablePayment ?? true} onChange={(e) => setForm({ ...form, enablePayment: e.target.checked })} />
                  Bật thanh toán
                </label>
              </div>
            </div>
            <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
          </form>
        </CardContent>
      </Card>
    )
  }
  ```

- [ ] **Step 4: Create `src/pages/system/ShippingTab.tsx`**

  ```typescript
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { settingsApi } from '@/api/settings'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
  import { Loader2, AlertCircle, Save } from 'lucide-react'
  import { toast } from 'sonner'
  import { useState, useEffect } from 'react'
  import type { ShippingSettings } from '@/types/settings'

  export function ShippingTab() {
    const queryClient = useQueryClient()
    const { data: allSettings, isLoading, error } = useQuery({
      queryKey: ['settings'],
      queryFn: () => settingsApi.getAll().then((r) => r.data),
    })

    const [form, setForm] = useState<ShippingSettings>({} as ShippingSettings)

    useEffect(() => {
      if (allSettings?.shipping) setForm(allSettings.shipping)
    }, [allSettings])

    const mutation = useMutation({
      mutationFn: (dto: ShippingSettings) => settingsApi.saveShipping(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu cấu hình vận chuyển') },
      onError: () => toast.error('Không thể lưu cấu hình vận chuyển'),
    })

    const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault()
      mutation.mutate(form)
    }

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

    return (
      <Card>
        <CardHeader><CardTitle className="text-base">Cấu hình vận chuyển</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div><label className="text-sm font-medium">Phí mặc định (₫)</label><Input type="number" value={form.defaultFee || ''} onChange={(e) => setForm({ ...form, defaultFee: Number(e.target.value) })} required /></div>
              <div><label className="text-sm font-medium">Miễn phí từ (₫)</label><Input type="number" value={form.freeShipFrom || ''} onChange={(e) => setForm({ ...form, freeShipFrom: Number(e.target.value) })} required /></div>
              <div><label className="text-sm font-medium">Khoảng cách tối đa (km)</label><Input type="number" step="0.1" value={form.maxDistance || ''} onChange={(e) => setForm({ ...form, maxDistance: Number(e.target.value) })} required /></div>
              <div><label className="text-sm font-medium">Thời gian giao</label><Input value={form.deliveryTime || ''} onChange={(e) => setForm({ ...form, deliveryTime: e.target.value })} /></div>
            </div>
            <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
          </form>
        </CardContent>
      </Card>
    )
  }
  ```

- [ ] **Step 5: Create `src/pages/system/OrderTab.tsx`**

  ```typescript
  import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
  import { settingsApi } from '@/api/settings'
  import { Button } from '@/components/ui/button'
  import { Input } from '@/components/ui/input'
  import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
  import { Loader2, AlertCircle, Save } from 'lucide-react'
  import { toast } from 'sonner'
  import { useState, useEffect } from 'react'
  import type { OrderSettings } from '@/types/settings'

  export function OrderTab() {
    const queryClient = useQueryClient()
    const { data: allSettings, isLoading, error } = useQuery({
      queryKey: ['settings'],
      queryFn: () => settingsApi.getAll().then((r) => r.data),
    })

    const [form, setForm] = useState<OrderSettings>({} as OrderSettings)

    useEffect(() => {
      if (allSettings?.order) setForm(allSettings.order)
    }, [allSettings])

    const mutation = useMutation({
      mutationFn: (dto: OrderSettings) => settingsApi.saveOrder(dto),
      onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu cấu hình đơn hàng') },
      onError: () => toast.error('Không thể lưu cấu hình đơn hàng'),
    })

    const handleSubmit = (e: React.FormEvent) => {
      e.preventDefault()
      mutation.mutate(form)
    }

    if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
    if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

    return (
      <Card>
        <CardHeader><CardTitle className="text-base">Cấu hình đơn hàng</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div><label className="text-sm font-medium">Tự động hủy sau (phút)</label><Input type="number" value={form.autoCancelMinutes || ''} onChange={(e) => setForm({ ...form, autoCancelMinutes: Number(e.target.value) })} required /></div>
              <div className="flex items-end gap-4">
                <label className="flex items-center gap-2 text-sm">
                  <input type="checkbox" checked={form.enableCOD ?? true} onChange={(e) => setForm({ ...form, enableCOD: e.target.checked })} />
                  Cho phép COD
                </label>
                <label className="flex items-center gap-2 text-sm">
                  <input type="checkbox" checked={form.enableOnlinePayment ?? true} onChange={(e) => setForm({ ...form, enableOnlinePayment: e.target.checked })} />
                  Cho phép thanh toán online
                </label>
              </div>
            </div>
            <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
          </form>
        </CardContent>
      </Card>
    )
  }
  ```

- [ ] **Step 6: Create `src/pages/SystemSettingsPage.tsx`** — tabbed wrapper

  ```typescript
  import { useState } from 'react'
  import { StoreInfoTab } from './system/StoreInfoTab'
  import { SmtpTab } from './system/SmtpTab'
  import { VnPayTab } from './system/VnPayTab'
  import { ShippingTab } from './system/ShippingTab'
  import { OrderTab } from './system/OrderTab'

  const tabs = [
    { key: 'store', label: 'Cửa hàng' },
    { key: 'smtp', label: 'SMTP' },
    { key: 'vnpay', label: 'VNPay' },
    { key: 'shipping', label: 'Vận chuyển' },
    { key: 'order', label: 'Đơn hàng' },
  ]

  export function SystemSettingsPage() {
    const [activeTab, setActiveTab] = useState('store')

    return (
      <div className="space-y-6">
        <h1 className="text-2xl font-semibold">Cài đặt hệ thống</h1>
        <div className="flex flex-wrap gap-2">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              onClick={() => setActiveTab(tab.key)}
              className={`px-3 py-1.5 text-sm rounded-full border transition-colors ${
                activeTab === tab.key
                  ? 'bg-primary text-primary-foreground border-primary'
                  : 'bg-background text-muted-foreground border-border hover:bg-muted'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>
        {activeTab === 'store' && <StoreInfoTab />}
        {activeTab === 'smtp' && <SmtpTab />}
        {activeTab === 'vnpay' && <VnPayTab />}
        {activeTab === 'shipping' && <ShippingTab />}
        {activeTab === 'order' && <OrderTab />}
      </div>
    )
  }
  ```

- [ ] **Step 7: Build and verify compilation**

  Run: `npm run build`
  Expected: Build succeeds with 0 errors.

- [ ] **Step 8: Commit**

  ```bash
  git add flower-admin.frontend/src/pages/SystemSettingsPage.tsx flower-admin.frontend/src/pages/system/
  git commit -m "feat(frontend): SystemSettingsPage with 5 settings tabs"
  ```

---

### Task 6: Routing + Cleanup + Full Build Verification

**Files:**
- Modify: `src/App.tsx` — replace placeholder imports, add sub-routes for PostFormPage + PageFormPage
- Modify: `src/pages/PlaceholderPages.tsx` — remove ContentPage, MarketingPage, SystemPage exports

**Interfaces:**
- Consumes: ContentPage, PostFormPage, PageFormPage, MarketingPage, SystemSettingsPage from Tasks 3–5
- Produces: Fully wired routes for all Phase 4 pages

- [ ] **Step 1: Remove old exports from `PlaceholderPages.tsx`**

  Remove these lines from the file:
  ```typescript
  export function ContentPage() {
    return <PlaceholderPage title="Nội dung" />
  }

  export function MarketingPage() {
    return <PlaceholderPage title="Marketing" />
  }

  export function SystemPage() {
    return <PlaceholderPage title="Hệ thống" />
  }
  ```

  Also remove `'/content', '/marketing', '/system'` from the `placeholderPages` array:
  ```typescript
  const placeholderPages = [
    { href: '/orders', title: 'Đơn hàng' },
    { href: '/products', title: 'Sản phẩm' },
  ]
  ```

- [ ] **Step 2: Update `App.tsx`**

  Replace the old imports:
  ```typescript
  import {
    ContentPage,
    MarketingPage,
    SystemPage,
  } from '@/pages/PlaceholderPages'
  ```

  With:
  ```typescript
  import { ContentPage } from '@/pages/ContentPage'
  import { PostFormPage } from '@/pages/content/PostFormPage'
  import { PageFormPage } from '@/pages/content/PageFormPage'
  import { MarketingPage } from '@/pages/MarketingPage'
  import { SystemSettingsPage } from '@/pages/SystemSettingsPage'
  ```

  Replace the old route entries:
  ```typescript
  <Route path="content" element={<ContentPage />} />
  <Route path="marketing" element={<MarketingPage />} />
  <Route path="system" element={<SystemPage />} />
  ```

  With:
  ```typescript
  <Route path="content" element={<ContentPage />}>
    <Route path="posts/new" element={<PostFormPage />} />
    <Route path="posts/:id/edit" element={<PostFormPage />} />
    <Route path="pages/new" element={<PageFormPage />} />
    <Route path="pages/:id/edit" element={<PageFormPage />} />
  </Route>
  <Route path="marketing" element={<MarketingPage />} />
  <Route path="system" element={<SystemSettingsPage />} />
  ```

  **Important:** The `<ContentPage>` uses `<Outlet />` to render sub-routes when the URL is not `/content`. The parent route declaration must match.

- [ ] **Step 3: Build and verify compilation**

  Run: `npm run build`
  Expected: Build succeeds with 0 errors.

- [ ] **Step 4: Run dotnet build to confirm backend still compiles**

  Run: `dotnet build`
  Expected: Build succeeded with 0 errors.

- [ ] **Step 5: Commit**

  ```bash
  git add flower-admin.frontend/src/App.tsx flower-admin.frontend/src/pages/PlaceholderPages.tsx
  git commit -m "feat: wire Phase 4 routes, cleanup placeholder exports"
  ```

---

## Self-Review Checklist

- [x] **Spec coverage:** All 7 sub-areas (Banners, Posts, Pages, Layout, Promotions, Coupons, Settings) mapped to tasks. Backend pagination for 4 entities, search for posts. Write endpoints for Layout + 5 setting groups.
- [x] **Placeholder scan:** No "TBD", "TODO", "implement later", "fill in details" in any step.
- [x] **Type consistency:** `PagedResult<T>` consistent between backend and `PaginatedResponse<T>` on frontend. `AllSystemSettings` frontend type maps to `AllSystemSettingsViewModel` backend DTO. All API modules use the correct type signatures.
- [x] **No missing dependencies:** Task 1 (backend) doesn't depend on frontend. Tasks 2 (types+API) depends on Task 1 endpoints only by convention (API URLs). Tasks 3-5 depend on Task 2 types+API. Task 6 depends on page components from Tasks 3-5.
