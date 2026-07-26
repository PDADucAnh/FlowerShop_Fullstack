# Home Page Mobile Optimization Design

## Goal

Optimize the React frontend Home page to deliver a premium mobile experience matching the Floraison design reference — slide-in drawer navigation, horizontal-scroll best sellers, 2-column product grid, stacked blog cards, and scroll-reveal animations.

## Scope

All changes are within `Flower-shop.frontend/src/`. No backend, no API, no data-model changes.

## File Changes

### 1. `assets/css/index.css` — Add CSS utilities

Add three utility classes:
- `.no-scrollbar` — hide scrollbar for horizontally scrolling sections
- `.drawer-overlay` — fixed fullscreen backdrop with blur, opacity transition
- `.drawer-content` — slide-in from left with cubic-bezier transform

### 2. New: `components/DrawerNav.tsx` — Slide-in drawer navigation

Extracted from Header.tsx mobile nav. Renders as a fixed overlay:

- **Desktop** (`md:`): `display: none` entirely
- **Mobile**: Full-height aside from left (`w-80`), rounded right corners, shadow
- Backdrop: `fixed inset-0 bg-black/30 backdrop-blur-sm`, click to close
- Nav items: list with Material Symbols icons, active state highlighted with `bg-secondary-container`
- Brand name at top: "Floraison Boutique" (or store name from settings)
- Close on `window.innerWidth >= 768` resize event
- Props: `isOpen: boolean`, `onClose: () => void`

### 3. `components/Header.tsx` — Simplify mobile nav

- Remove the inline `mobileNavOpen` list (`fixed inset-x-0 top-[72px] ...`)
- Mobile hamburger button toggles `DrawerNav` instead
- Layout on mobile: `[menu icon] [centered logo] [cart icon]`
- Keep desktop nav unchanged (dropdown menus, search, icons, user menu)
- Remove `renderMobileMenuItem` function (moved to DrawerNav)

### 4. `pages/home/HeroBanner.tsx` — Minor polish

- Update fallback title to "Artistry in Every Bloom" (bilingual: English primary, Vietnamese subtitle)
- Add `active:scale-95` transition to CTA button
- Keep carousel logic, API calls, loading/error/empty states intact

### 5. `pages/home/BestSellingProducts.tsx` — Horizontal scroll

Replace grid with horizontal flex scroll:

```tsx
<div className="flex gap-stack-md overflow-x-auto no-scrollbar pb-4 -mx-margin-mobile px-margin-mobile">
```

Each card: `w-64 flex-shrink-0 group` with `aspect-[4/5]` image, hover scale, name + price below.

Use the same data (`useBestSellingProducts(3)`) but render inline card JSX instead of `<ProductCard>` so aspect ratio can differ from the shared component's variants. The inline card shows: image with hover zoom, product name (headline-sm), price (label-md), and a tertiary color.

Add "View All" link next to section title.

### 6. `pages/home/ProductGrid.tsx` — 2-column mobile grid

Change grid classes from:
```
grid-cols-1 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4
```
to:
```
grid-cols-2 md:grid-cols-3 xl:grid-cols-4
```

Rename section title from "Tuyệt Tác Ngàn Hoa" to "Full Collection" with subtitle "Tinh hoa nghệ thuật cắm hoa" and a decorative separator.

Keep pagination, loading, empty states, and `ProductCard variant="standard"` intact.

### 7. `pages/home/LatestBlog.tsx` — Stacked vertical layout

Change from 3-column grid to vertical stack:

```tsx
<div className="space-y-stack-md">
```

Each blog card becomes a horizontal-image card:
- Image: `h-48 w-full object-cover rounded-t-xl`
- Content: tag (label-sm, primary, uppercase), title (headline-sm), excerpt (body-md, 2-line clamp)
- Card wrapper: `bg-surface-container-low rounded-xl overflow-hidden petal-shadow`

Keep the same data (3 latest posts sorted by date).

### 8. `pages/home/index.tsx` — Add scroll-reveal animations

Wrap section containers with `useScrollReveal` hook (already exists at `hooks/useScrollReveal.ts`):

```tsx
const { ref, isVisible } = useScrollReveal<HTMLElement>();
```

Apply `transition-all duration-700` + conditional `opacity-0 translate-y-4` / `opacity-100 translate-y-0` classes based on `isVisible`.

## What stays the same

- All API calls, services, hooks, types
- `ProductCard.tsx` (shared component) — unchanged except it continues to work for ProductGrid
- `Footer.tsx` — already matches the template style
- All routes, router config, context providers
- Tailwind config, color palette, fonts

## Files Modified (7 files)

1. `Flower-shop.frontend/src/assets/css/index.css` — CSS utilities
2. `Flower-shop.frontend/src/components/Header.tsx` — Simplify mobile, use DrawerNav
3. `Flower-shop.frontend/src/components/DrawerNav.tsx` — NEW file
4. `Flower-shop.frontend/src/pages/home/HeroBanner.tsx` — Minor polish
5. `Flower-shop.frontend/src/pages/home/BestSellingProducts.tsx` — Horizontal scroll
6. `Flower-shop.frontend/src/pages/home/ProductGrid.tsx` — 2-column grid
7. `Flower-shop.frontend/src/pages/home/LatestBlog.tsx` — Stacked layout
8. `Flower-shop.frontend/src/pages/home/index.tsx` — Scroll reveal wrapper

## Verification

- `npm run build` must pass (Vite build, no TypeScript errors)
- Test mobile viewport (375px-428px): drawer slides, horizontal scroll works, 2-column grid renders correctly
- Test desktop viewport (1024px+): no regression — header nav, hero, grid all look same as before
