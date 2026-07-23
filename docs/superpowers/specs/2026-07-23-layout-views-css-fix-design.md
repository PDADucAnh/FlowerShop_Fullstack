# Layout Views CSS Fix — Design

**Goal:** Fix Tailwind CSS rendering on admin Layout views (Trình đơn / Chân trang) so they match existing admin views (Page, Contact, etc.).

## Root Cause

`Flower.Backend/Views/Layout/Index.cshtml` is missing `Layout = "_LayoutAdmin"`. Without this, the page renders without the admin layout, which provides:
- Tailwind CSS CDN with custom design tokens (colors, spacing, fonts)
- Material Symbols font
- Admin sidebar + header + notification system
- `Scripts` section rendering

## Changes

### 1. Index.cshtml — Add layout + align spacing tokens

- Add `Layout = "_LayoutAdmin"` at line 3
- Replace raw spacing `px-6` → `px-lg`, `py-3` → `py-3` (keep, matches Page view)
- Keep existing structure (tabs via query param, partial views)

### 2. _HeaderTab.cshtml — Align spacing tokens

- `p-6` → `p-lg` (card padding)
- `mb-6` → `mb-stack-md` (card margin)
- `gap-4` → keep (matches existing)
- `px-3 py-2` → keep (input sizing is consistent)

### 3. _FooterTab.cshtml — Align tokens + fix bg color

- `bg-white` → `bg-surface` (primary fix)
- `p-6` → `p-lg`
- `px-3 py-2` → keep
- `text-white` → `text-on-primary` (admin token)

## Success Criteria

1. `Layout = "_LayoutAdmin"` is present in Index.cshtml
2. No `bg-white` remains — all use `bg-surface` or design system tokens
3. Spacing uses design tokens (`px-lg`, `p-lg`, `mb-stack-md`) not raw Tailwind
4. Build passes (`dotnet build`)
5. Admin layout pages render with proper Tailwind styling
