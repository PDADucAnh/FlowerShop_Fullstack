# Task 8 Report — Dynamic Header Layout

**Commit:** `60c5494`

## Changes Made

Modified: `Flower-shop.frontend/src/components/Header.tsx`

### What was implemented

1. **Import `layoutService`** — added `import layoutService, { HeaderLayout, MenuItem } from '../services/layoutService'`

2. **State & data fetching** — added `HeaderLayout` state (`layout`) and `searchOpen` boolean; called `layoutService.getLayout()` in `useEffect` alongside the existing `settingsService.getStoreInfo()` call. Failure silently keeps default rendering.

3. **TopBar** — rendered as a full-width `<div>` above the sticky header (`bg-primary text-on-primary`) when `header.topBar.isActive` is true. Supports optional link via `url`.

4. **Dynamic nav menu** — replaced hardcoded `<Link>` rows with recursive `renderMenuItem(item, depth)`:
   - Max depth 3 (root → child → sub-child)
   - Root-level items with `children` render as dropdown triggers (using `group` hover pattern)
   - Second-level items with `children` render as nested flyout menus (`group/sub` with `left-full`)
   - Items without children render as direct `<Link>` with `navLinkClass()` for active state
   - External links get `target="_blank" rel="noopener noreferrer"`
   - Falls back to hardcoded links when `menuItems` is empty/null

5. **Mobile nav** — same dynamic tree via `renderMobileMenuItem()`, with `expand_more` indicator for parent items and nested `<Link>` rows that close the mobile drawer on click.

6. **CTA button** — rendered in the header right section when `header.ctaButton.isActive`. Supports `filled` (bg-primary text-on-primary) and `outlined` (border-primary text-primary) variants.

7. **Hotline** — reads `header.hotline.useDefault` to decide between `storeInfo.hotline` and `header.hotline.customText`.

8. **Search mode** — checks `header.search.mode`:
   - `"popup"` — renders a search toggle button; clicking opens a centered overlay with auto-focused input
   - `"input"` — renders the original inline search bar

9. **Zones** — renders `layout.zones.left` as text spans in the header right section.

### Order-confirmation path
Early return (line 37) bypasses all dynamic rendering — unchanged.

### Verification
- `npx tsc --noEmit` — passed
- `npm run build` — passed (only warnings from `@microsoft/signalr`, unrelated)
