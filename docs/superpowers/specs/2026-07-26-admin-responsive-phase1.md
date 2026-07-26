# Phase 1 — Admin (Razor Views) Responsive Optimization

**Date:** 2026-07-26
**Goal:** Make the admin panel fully usable on mobile devices (<768px) without layout breakage.

---

## 1. Sidebar → Mobile Drawer

### Current state
- Sidebar is `fixed left-0 top-0 h-screen w-64` — always visible, no collapse
- Main content is `ml-64` — permanently offset 256px
- On mobile (<768px), the sidebar occupies a third of the screen, crushing content

### Target state
| Viewport | Sidebar | Main content |
|----------|---------|--------------|
| ≥768px (`md:`) | `fixed w-64` as today | `ml-64` as today |
| <768px | Off-canvas drawer: hidden by default (`-translate-x-full`), slides in when toggled | `ml-0` full width |

### Changes in `_LayoutAdmin.cshtml`

**A. Add hamburger button** inside header (line 291, next to the mobile brand title):
- `<button>` with `material-symbols-outlined` icon `menu` / `close`
- Visible only `<md` (`md:hidden`)
- Toggles class `sidebar-open` on `<body>` via JS

**B. Add backdrop overlay:**
- `<div id="sidebar-backdrop">` — fixed fullscreen, `bg-black/40`, `hidden` by default
- Click closes sidebar

**C. Sidebar `<nav>` responsive classes:**
- Current: `fixed left-0 top-0 h-full ... w-64 z-20`
- New: Add `-translate-x-full md:translate-x-0 transition-transform duration-300 ease-in-out`
- When `body.sidebar-open`: `translate-x-0` (overrides `-translate-x-full`)

**D. Main content margin:**
- Change `ml-64` to `md:ml-64` (no left margin on mobile)

**E. Close on Escape key** via JS.

### JavaScript (inline script block, vanilla JS)
```js
// Sidebar toggle
const sidebar = document.getElementById('sidebar');
const backdrop = document.getElementById('sidebar-backdrop');
const toggleBtn = document.getElementById('sidebar-toggle');

function openSidebar() {
  document.body.classList.add('sidebar-open');
  backdrop.classList.remove('hidden');
}
function closeSidebar() {
  document.body.classList.remove('sidebar-open');
  backdrop.classList.add('hidden');
}
toggleBtn.addEventListener('click', openSidebar);
backdrop.addEventListener('click', closeSidebar);
document.addEventListener('keydown', (e) => {
  if (e.key === 'Escape') closeSidebar();
});
```

### Additional CSS
- `.sidebar-open #sidebar { transform: translateX(0); }`
- `#sidebar-backdrop` transition for fade-in/out

---

## 2. Touch-friendly Dropdowns

### Audit result
- Notification bell dropdown (lines 311–322): already uses click toggle via JS (lines 568–574) ✓
- User avatar (line 331): static display element, no dropdown
- No `group-hover` dropdowns exist in admin layout ✓

**No changes needed.** Admin dropdowns already work on touch devices.

---

## 3. Tables & Forms Responsive

### Tables — check & fix overflow-x-auto

All admin Index views with `<table>` must wrap in `<div class="overflow-x-auto">`.

| View file | Has `overflow-x-auto`? |
|-----------|------------------------|
| `Views/Order/Index.cshtml` | ✅ Yes (line 60) |
| `Views/Product/Index.cshtml` | ✅ No table (card grid) |
| `Views/User/Index.cshtml` | ❓ Need check |
| `Views/Coupon/Index.cshtml` | ❓ Need check |
| `Views/Promotion/Index.cshtml` | ❓ Need check |
| `Views/FlashSale/Index.cshtml` | ❓ Need check |
| `Views/Post/Index.cshtml` | ❓ Need check |
| `Views/Category/Index.cshtml` | ❓ Need check |
| `Views/CategoryProduct/Index.cshtml` | ❓ Need check |
| `Views/OrderDetail/Index.cshtml` | ❓ Need check |
| `Views/Customer/Index.cshtml` | ❓ Need check |
| `Views/Contact/Index.cshtml` | ❓ Need check |
| `Views/Advertisement/Index.cshtml` | ❓ Need check |
| `Views/Notification/Index.cshtml` | ❓ Need check |
| `Views/Layout/Index.cshtml` | ❓ Need check |
| `Views/Page/Index.cshtml` | ❓ Need check |

**Action:** For each view missing `overflow-x-auto`, add the wrapper div and ensure all `<th>` columns are appropriately sized.

### Forms — verify grid pattern

Most Create/Edit forms already use `grid-cols-1 md:grid-cols-3 gap-xl`. This is correct for mobile stacking.

**Action:** Spot-check 2–3 forms for any hardcoded widths or missing responsive grid classes. Fix if found.

### Pagination — mobile spacing

`_Pagination.cshtml` (line 36–68) already uses responsive text and wraps buttons gracefully. Currently uses `flex gap-1` for page buttons — may need `flex-wrap` on very small screens.

**Action:** Add `flex-wrap` to pagination button container.

---

## 4. Files Changed

| File | Change |
|------|--------|
| `Views/Shared/_LayoutAdmin.cshtml` | Sidebar drawer, hamburger, backdrop, main content margin, inline JS + CSS |
| `Views/Shared/_Pagination.cshtml` | Add `flex-wrap` to button container |
| `Views/*/Index.cshtml` (each table view) | Add `overflow-x-auto` wrapper if missing |

---

## 5. Success Criteria

1. Open admin on mobile (<768px): sidebar hidden by default, hamburger icon visible
2. Tap hamburger: sidebar slides in from left, backdrop appears
3. Tap backdrop or Escape: sidebar slides out
4. Tables scroll horizontally without breaking layout
5. Forms stack in single column on mobile
6. All existing desktop behavior unchanged
7. Build succeeds (`dotnet build`)
