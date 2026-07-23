# Dynamic Header & Footer Layout Management

> **Date:** 2026-07-23
> **Status:** Approved Design

## Goal

Replace hardcoded Header/Footer in frontend with dynamic layout configuration managed via Admin UI. Admin can customize menu items, footer columns, component positions (zones), toggle visibility, and configure component-specific settings — all without redeploying code.

## Architecture Overview

```
Admin UI (ASP.NET MVC)  ──save──>  SystemSetting (JSONB)  ──read──>  API  ──>  Frontend (React)
      │                              │
      └── SortableJS/Nestable2       │
          (drag-and-drop)            │
                                     │
   Backend cache (30-min) ───────────┘
   + Frontend fetches on every load
```

- **Storage:** Existing `SystemSetting` table, keys `"HeaderLayout"` and `"FooterLayout"` (JSON string)
- **API:** `GET /api/layout` returns `{ header, footer, storeInfo }` in one call
- **Admin:** `LayoutController` with 2 tabs (Trình đơn / Chân trang), uses SortableJS for drag-and-drop
- **Frontend:** `layoutService.ts` fetches layout; Header/Footer components consume dynamic data

## Data Model — JSON Structures

### HeaderLayout — key `"HeaderLayout"`

```json
{
  "topBar": {
    "isActive": true,
    "text": "Giao hoa tận nơi trong 2 giờ tại TP.HCM",
    "url": null
  },
  "zones": {
    "left": ["hotline", "menu"],
    "center": ["logo"],
    "right": ["search", "wishlist", "cart", "account", "notification"]
  },
  "ctaButton": {
    "isActive": false,
    "text": "Đặt hoa theo yêu cầu",
    "url": "/custom-order",
    "variant": "filled"
  },
  "hotline": {
    "useDefault": true,
    "customText": null
  },
  "search": {
    "mode": "popup"
  },
  "menuItems": [
    {
      "label": "Trang chủ",
      "url": "/",
      "isExternal": false,
      "children": []
    },
    {
      "label": "Cửa hàng",
      "url": "/shop",
      "isExternal": false,
      "children": [
        {
          "label": "Hoa cưới",
          "url": "/shop?cat=hoa-cuoi",
          "isExternal": false,
          "children": []
        }
      ]
    }
  ]
}
```

### FooterLayout — key `"FooterLayout"`

```json
[
  {
    "title": "Về chúng tôi",
    "align": "left",
    "sortOrder": 1,
    "type": "links",
    "isActive": true,
    "links": [
      { "label": "Giới thiệu", "type": "page", "pageId": 1 },
      { "label": "Facebook", "type": "custom", "url": "https://facebook.com/flowershop" }
    ]
  },
  {
    "title": "Kết nối",
    "align": "center",
    "sortOrder": 3,
    "type": "social_icons",
    "isActive": true,
    "links": []
  }
]
```

### Available Component Types

| Type | Description | Configurable in Admin |
|------|-------------|----------------------|
| `logo` | Store logo/name | Toggle isActive only |
| `menu` | Navigation menu (tree) | Full menu item editor |
| `search` | Search bar | mode: "popup" \| "input" |
| `cart` | Cart icon with badge | Toggle isActive only |
| `wishlist` | Wishlist heart icon | Toggle isActive only |
| `account` | Login/dropdown | Toggle isActive only |
| `notification` | NotificationBell | Toggle isActive only |
| `hotline` | Phone quick-call | useDefault / customText |
| `ctaButton` | CTA button | text, url, variant |

### Footer Column Types

| Type | Render |
|------|--------|
| `links` | List of links |
| `social_icons` | Facebook/Zalo icon row |
| `text_block` | Rich text paragraph |

## Backend Implementation

### DTOs — new file `Flower.Backend/Models/DTOs/LayoutDTOs.cs`

- `HeaderLayoutDTO` — maps to HeaderLayout JSON structure
  - `TopBar` (bool isActive, string? text, string? url)
  - `Zones` (string[] left, string[] center, string[] right)
  - `CtaButton` (bool isActive, string? text, string? url, string? variant)
  - `HotlineConfig` (bool useDefault, string? customText)
  - `SearchConfig` (string mode)
  - `MenuItems` (List<MenuItemDTO>)
- `MenuItemDTO` — string id, string label, string url, bool isExternal, List<MenuItemDTO> children
- `FooterColumnDTO` — string title, string align, int sortOrder, string type, bool isActive, List<FooterLinkDTO> links
- `FooterLinkDTO` — string id, string label, string type ("page"|"custom"|"text_block"), int? pageId, string? url

Khi `type === "text_block"`, link được render dưới dạng plain text (`<p>` hoặc `<span>`, KHÔNG dùng `<a>`) — bỏ qua url, dùng làm đoạn mô tả/brand text trong footer column. Tránh tạo link rỗng ảnh hưởng SEO.

### API Controller — `Flower.Backend/Controllers/Api/LayoutApiController.cs`

- `[AllowAnonymous] GET /api/layout` — reads HeaderLayout + FooterLayout from `SystemSettingService`, merges StoreInfoSettings, returns `{ header, footer, storeInfo }`
- If keys don't exist, returns default config mirroring current hardcoded layout (backward compatibility)
- **Defensive:** If `zones.left/center/right` is `null` after deserialization, fallback to `[]` to prevent Frontend `.map()` crash
- Uses `SystemSettingService.GetSetting<HeaderLayoutDTO>("HeaderLayout")` and `GetSetting<List<FooterColumnDTO>>("FooterLayout")`

### Admin Controller — `Flower.Backend/Controllers/LayoutController.cs`

- `[Authorize(Policy = "StaffOnly")] GET Index()` — renders Layout management page with 2 tabs
- `[Authorize(Policy = "AdminOnly")] POST SaveHeader(HeaderLayoutDTO model)` — saves to `"HeaderLayout"` key, clears cache, writes audit log
- `[Authorize(Policy = "AdminOnly")] POST SaveFooter(List<FooterColumnDTO> model)` — saves to `"FooterLayout"` key, clears cache, writes audit log

### Admin Views

- `Views/Layout/Index.cshtml` — tab container, loads `_HeaderTab.cshtml` and `_FooterTab.cshtml` as partials
- `Views/Layout/_HeaderTab.cshtml` — 4 sections:
  1. Top Bar (checkbox + text + url)
  2. Zones (3 sortable lists with add/remove component)
  3. Component configs (hotline, search, CTA button)
  4. Menu tree (Nestable2 or SortableJS hierarchical)
- `Views/Layout/_FooterTab.cshtml` — column cards with inline link list editor
- Uses **SortableJS** for flat list drag-and-drop (zone components, footer links) and **Nestable2** or SortableJS nestable config for menu tree
- Before form submit: JavaScript serializes all DOM state into `<input type="hidden" name="jsonPayload">` as JSON string

### Sidebar — `_LayoutAdmin.cshtml`

Add new section **"GIAO DIỆN"** with 2 items:
- `Trình đơn` → Layout/Index (tab Header)
- `Chân trang` → Layout/Index (tab Footer)

## Frontend Implementation

### New Service — `Flower-shop.frontend/src/services/layoutService.ts`

```typescript
export interface HeaderLayout { /* matches HeaderLayout JSON */ }
export interface FooterColumn { /* matches FooterColumn JSON */ }
export interface MenuItem { /* recursive */ }

export const layoutService = {
  getLayout: () => axiosClient.get<{
    header: HeaderLayout;
    footer: FooterColumn[];
    storeInfo: StoreInfo;
  }>('/layout'),
};
```

### Refactored `Header.tsx`

- Call `layoutService.getLayout()` on mount (parallel with `settingsService.getStoreInfo()`)
- Render **TopBar** if `header.topBar.isActive` — above main header
- Render each **Zone** (`left`, `center`, `right`) via `.map()` of component type strings:
  - `"logo"` → Logo component (existing)
  - `"menu"` → Menu nav (now from `header.menuItems`, recursive)
  - `"search"` → Search with mode from `header.search.mode`
  - `"hotline"` → Hotline button (use storeInfo or customText)
  - `"cart"`, `"wishlist"`, `"account"`, `"notification"` → existing components
  - `"ctaButton"` → CTA button if `header.ctaButton.isActive`
- Missing component types → silently skip (graceful fallback)
- `/order-confirmation` → simplified layout remains (bypass dynamic rendering)

### Refactored `Footer.tsx`

- Call `layoutService.getLayout()` on mount
- Replace 3 hardcoded columns with `footer.map()`:
  - CSS grid: `grid-cols-1 md:grid-cols-{footer.length}`
  - Skip inactive columns (`!column.isActive`)
  - Column `type === "links"` → render link list
  - Column `type === "social_icons"` → icon row
  - Column `type === "text_block"` → text content
- Links with `type === "page"` → attempt to resolve via lookup; if page not found, hide link gracefully (no broken UI)

## Edge Cases & Guards

### 1. Cache Invalidation
When Admin saves Header/Footer config:
- Backend clears in-memory cache for `"HeaderLayout"` / `"FooterLayout"` (via existing `SystemSettingService` pattern)
- Consider adding revalidation trigger to Next.js if ISR is used (or rely on `revalidate: 60` at Fetch level)

### 2. Orphaned Page Links
If a Page is deleted but referenced in FooterLayout JSON:
- Frontend renders a `type: "page"` link by performing a client-side lookup against loaded pages
- If the page slug is not found, the link is **silently hidden** (not rendered) rather than breaking the UI

### 3. Menu Max Depth
Admin UI limits menu tree nesting to **3 levels max**:
- Enforced client-side via Nestable2 `maxDepth: 3` option (or SortableJS validation)
- Prevents overflow/collapse on mobile viewports

## Default Configurations

> Backend tự sinh `Guid.NewGuid().ToString()` cho trường `id` của mỗi menu item và footer link khi khởi tạo default config (nếu chưa có trong DB).

### Default Header (mirrors current hardcoded layout)

```json
{
  "topBar": { "isActive": false, "text": "", "url": null },
  "zones": { "left": [], "center": ["logo"], "right": ["search", "cart", "account"] },
  "ctaButton": { "isActive": false },
  "hotline": { "useDefault": true },
  "search": { "mode": "popup" },
  "menuItems": [
    { "label": "Trang chủ", "url": "/", "isExternal": false, "children": [] },
    { "label": "Cửa hàng", "url": "/shop", "isExternal": false, "children": [] },
    { "label": "Bài viết", "url": "/blog", "isExternal": false, "children": [] },
    { "label": "Giới thiệu", "url": "/about", "isExternal": false, "children": [] },
    { "label": "Liên hệ", "url": "/contact", "isExternal": false, "children": [] }
  ]
}
```

### Default Footer (mirrors current hardcoded layout)

```json
[
  {
    "title": "Về chúng tôi",
    "align": "left", "sortOrder": 1, "type": "links", "isActive": true,
    "links": [
      { "label": "Thương hiệu hoa tươi nghệ thuật...", "type": "text_block" },
      { "label": "Giới thiệu", "type": "page", "pageId": 1 }
    ]
  },
  {
    "title": "Chính sách",
    "align": "left", "sortOrder": 2, "type": "links", "isActive": true,
    "links": [
      { "label": "Giao hàng", "type": "custom", "url": "/delivery-policy" },
      { "label": "Đổi trả", "type": "custom", "url": "/return-policy" },
      { "label": "Bảo mật", "type": "custom", "url": "/privacy-policy" }
    ]
  },
  {
    "title": "Kết nối",
    "align": "left", "sortOrder": 3, "type": "social_icons", "isActive": true,
    "links": []
  }
]
```

## Files Summary

### New Files — Backend
- `Flower.Backend/Models/DTOs/LayoutDTOs.cs` — HeaderLayoutDTO, FooterColumnDTO, MenuItemDTO, FooterLinkDTO
- `Flower.Backend/Controllers/Api/LayoutApiController.cs` — `GET /api/layout`
- `Flower.Backend/Controllers/LayoutController.cs` — admin MVC
- `Flower.Backend/Views/Layout/Index.cshtml`
- `Flower.Backend/Views/Layout/_HeaderTab.cshtml`
- `Flower.Backend/Views/Layout/_FooterTab.cshtml`

### Modified Files — Backend
- `Flower.Backend/Views/Shared/_LayoutAdmin.cshtml` — add "GIAO DIỆN" sidebar section

### New Files — Frontend
- `Flower-shop.frontend/src/services/layoutService.ts` — layout API service + interfaces

### Modified Files — Frontend
- `Flower-shop.frontend/src/components/Header.tsx` — dynamic menu + zones + TopBar + CTA
- `Flower-shop.frontend/src/components/Footer.tsx` — dynamic columns
