# Phase 4: Content Management, Marketing & System Settings — Design Spec

## Overview

Replace the 3 remaining placeholder pages (`/content`, `/marketing`, `/system`) in the admin SPA with tabbed management pages. Each route hosts multiple tabs, each tab is a CRUD module for one entity type.

## Routes & Tab Structure

| Route | Tabs |
|-------|------|
| `/content` | Banner | Bài viết | Trang tĩnh | Giao diện |
| `/marketing` | Khuyến mãi | Mã giảm giá |
| `/system` | Thông tin CH | SMTP | VNPay | Vận chuyển | Đơn hàng |

All tabs share the same wrapping layout — a pill-style tab bar above a `<div>` that conditionally renders the active tab's component. No sub-routing.

## Backend Changes

### 1. Advertisements — Add pagination

**Controller:** `AdvertisementsController.cs`
```
GET /api/advertisements/paged?page=1&pageSize=20
```
Returns `PaginatedResponse<AdvertisementDTO>`.

**Service:** `IAdvertisementService` / `AdvertisementService`
```csharp
Task<PagedResult<AdvertisementDTO>> GetPaged(int page, int pageSize);
```
Simple order by `SortOrder`, then `CreatedAt` desc.

### 2. Posts — Add search to existing paged endpoint

**Controller:** `PostsController.cs`
```
GET /api/posts/paged?page=1&pageSize=20&search=keyword
```
Existing `GetPaged` extended with optional `search` param.

**Service:** `IPostService` / `PostService`
```csharp
Task<PagedResult<PostDTO>> GetPaged(int page, int pageSize, string? search = null);
```
Filter: `Title.Contains(search) || Summary.Contains(search)`. Order by `CreatedDate` desc.

### 3. Pages — Add pagination

**Controller:** `PagesController.cs`
```
GET /api/pages/paged?page=1&pageSize=20
```
Returns `PaginatedResponse<PageDTO>`.

**Service:** `IPageService` / `PageService`
```csharp
Task<PagedResult<PageDTO>> GetPaged(int page, int pageSize);
```
Order by `CreatedAt` desc.

### 4. Promotions — Add pagination

**Controller:** `PromotionsController.cs`
```
GET /api/promotions/paged?page=1&pageSize=20&search=&status=active|inactive
```
Returns `PaginatedResponse<PromotionCampaignDTO>`.

**Service:** `IPromotionService` / `PromotionService`
```csharp
Task<PagedResult<PromotionCampaignDTO>> GetPaged(int page, int pageSize, string? search = null, bool? isActive = null);
```

### 5. Coupons — Add pagination

**Controller:** `CouponsController.cs`
```
GET /api/coupons/paged?page=1&pageSize=20&search=
```
Returns `PaginatedResponse<CouponDTO>`.

**Service:** `ICouponService` / `CouponService`
```csharp
Task<PagedResult<CouponDTO>> GetPaged(int page, int pageSize, string? search = null);
```

### 6. Layout — Add write endpoint

**Controller:** `LayoutApiController.cs`
```
PUT /api/layout/header  — body: HeaderLayoutDTO
PUT /api/layout/footer  — body: FooterColumnDTO[]
```
Both call `_settingService.SaveSetting<T>` which already exists.

### 7. Settings — Add write endpoints

**Controller:** `SettingsApiController.cs`
```
PUT /api/settings/store-info   — body: StoreInfoSettings
PUT /api/settings/smtp         — body: SmtpSettings
PUT /api/settings/vnpay        — body: VNPaySettings
PUT /api/settings/shipping     — body: ShippingSettings
PUT /api/settings/order        — body: OrderSettings
```
All:
```
[Authorize(Policy = "StaffOnly")]
```
Each calls `_settingService.SaveSetting(key, dto, username)`.

Add a single combined read endpoint to load all settings for the admin form:
```
GET /api/settings — returns AllSystemSettingsViewModel
```
This lets the frontend call once and split into the 5 tab forms on the client side. Existing `store-info` and `checkout` endpoints remain untouched for the customer-facing frontend.

## Frontend Structure

### New directories
```
src/pages/content/
  components/
    BannerTable.tsx
    BannerDialog.tsx
    PostTable.tsx
    PostForm.tsx
    PageTable.tsx
    PageForm.tsx
    LayoutHeaderForm.tsx
    LayoutFooterForm.tsx
  ContentPage.tsx

src/pages/marketing/
  components/
    PromotionTable.tsx
    PromotionDialog.tsx
    CouponTable.tsx
    CouponDialog.tsx
  MarketingPage.tsx

src/pages/system/
  components/
    StoreInfoForm.tsx
    SmtpForm.tsx
    VnPayForm.tsx
    ShippingForm.tsx
    OrderForm.tsx
  SystemPage.tsx
```

### New API modules

| File | Functions |
|------|-----------|
| `src/api/advertisements.ts` | `getPaged`, `getById`, `create`, `update`, `delete` |
| `src/api/pages.ts` | `getPaged`, `getById`, `getBySlug`, `create`, `update`, `delete` |
| `src/api/layout.ts` | `getLayout`, `saveHeader`, `saveFooter` |
| `src/api/settings.ts` | `getAll`, `saveStoreInfo`, `saveSmtp`, `saveVnPay`, `saveShipping`, `saveOrder` |
| `src/api/promotions.ts` | `getPaged`, `getById`, `create`, `update`, `delete`, `enable`, `disable`, `addProduct`, `removeProduct` |
| `src/api/coupons.ts` | `getPaged`, `getById`, `create`, `update`, `delete`, `enable`, `disable`, `getUsages` |

### New types

| File | Types |
|------|-------|
| `src/types/advertisement.ts` | `AdvertisementDTO`, `CreateAdvertisementDTO`, `UpdateAdvertisementDTO` |
| `src/types/page.ts` | `PageDTO`, `CreatePageDTO`, `UpdatePageDTO` |
| `src/types/layout.ts` | `HeaderLayoutDTO`, `TopBarDTO`, `ZonesDTO`, `CtaButtonDTO`, `HotlineConfigDTO`, `SearchConfigDTO`, `MenuItemDTO`, `FooterColumnDTO`, `FooterLinkDTO` |
| `src/types/settings.ts` | `StoreInfoSettings`, `SmtpSettings`, `VNPaySettings`, `ShippingSettings`, `OrderSettings` |

Existing `post.ts`, `promotion.ts`, `coupon.ts` types suffice with minor extensions (add `search` params to paged interfaces).

### UI patterns per tab

**Tabs with inline CRUD (Dialog-based):**
- Banners: DataTable → delete / edit opens Dialog → save closes + invalidates query
- Coupons: DataTable → edit opens Dialog → save → invalidate
- Settings: All 5 tabs — inline forms with save button (no DataTable per se)
- Layout: Header form + Footer form in same tab, save button per section

**Tabs with full-page forms:**
- Blog Posts: DataTable → click row → navigate to `/content/posts/:id/edit` or `/content/posts/new`
- Static Pages: DataTable → click row → navigate to `/content/pages/:id/edit` or `/content/pages/new`

These use sub-routing under `/content` for the create/edit flows.

## Sidebar Changes

Current sidebar already has `/content`, `/marketing`, `/system` entries. No changes needed.

## Task Breakdown

1. **Backend** — New service methods + controller actions for all 7 areas (pagination, settings write, layout write)
2. **Frontend types + API modules** — New types (advertisement, page, layout, settings) + extended existing API modules (add `getPaged` to promotions/coupons, add new modules)
3. **Content Page** — Banner tab (table + dialog) + Posts tab (table + sub-routing) + Pages tab (table + sub-routing) + Layout tab (forms) + `ContentPage` wrapper
4. **Marketing Page** — Promotions tab + Coupons tab + `MarketingPage` wrapper
5. **System Settings** — All 5 settings forms + `SystemPage` wrapper
6. **Routing + sidebar + build verification** — Add sub-routes for post/page create/edit, verify full build

## Design Decisions

- **Tab state via React state**, not URL params — simpler, avoids cluttering URL. Active tab is tracked with `useState`.
- **Rich text editor**: CKEditor 5 is already used in the MVC admin (`wwwroot/libs/ckeditor`). For the SPA, use a simple `<textarea>` with the existing `@uploadcare/ckeditor5` or plain `<textarea>` — defer full CKEditor integration. Plan uses `<textarea>` for post/page content with a note that CKEditor can be added later.
- **Layout settings stored as JSON** in `SystemSetting` table — reuse existing `ISystemSettingService.GetSetting<T>` / `SaveSetting<T>`.
- **Settings forms are uncontrolled or controlled** with a single "Lưu" button. Load current values on mount via `useQuery`, save via `useMutation`.
- **No separate delete confirmation for banners/coupons** — use inline `AlertDialog` (deleteTarget pattern from Phase 3). Posts and Pages delete from their DataTable rows.
- **Backend pagination follows Phase 3 pattern** — `<Entity>Controller.GetPaged` with `[FromQuery]` params, service returns `PagedResult<T>`, frontend receives `PaginatedResponse<T>`.

## Files Not Modified

- No existing MVC controllers
- No existing non-API controllers
- No database migrations needed (all column changes are read/query only)
