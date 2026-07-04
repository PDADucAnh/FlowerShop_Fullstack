# Phase 3: Frontend Bug Fixes Design

> Fix all critical and major frontend bugs without architecture changes.

## Packages

### Package A: Auth & Security

**A1 — Login broken** (🔴 Critical)
- File: `cms.frontend/src/pages/login/index.jsx:20`
- Bug: Calls `authService.login()` directly instead of `useAuth().login()`. Token never saved, auth state never set.
- Fix: Import `useAuth` context, call `login(username, password)` on submit. Remove direct `authService` call.

**A2 — XSS Blog Detail** (🔴 Critical)
- File: `cms.frontend/src/pages/blog-detail/index.jsx:114`
- Bug: `dangerouslySetInnerHTML` renders raw `post.content` without sanitization.
- Fix: Install `dompurify`, wrap content with `DOMPurify.sanitize(post.content)` before rendering.

**A3 — Hardcoded customerId** (🟠 Major)
- File: `cms.frontend/src/pages/checkout/index.jsx:35`
- Bug: `customerId: 1` hardcoded instead of using the authenticated user's ID.
- Fix: Get `user.id` from `useAuth()` context; pass as `customerId`.

**A4 — Hardcoded API URL fallback localhost** (🟠 Major)
- Files: `axiosClient.js:5`, `product-detail/index.jsx`, `blog-detail/index.jsx`, `CartTable.jsx`, `ProductCard.jsx`
- Bug: `https://localhost:7224` hardcoded as fallback, production fails if `.env` is missing.
- Fix: Add `getApiUrl()` utility that validates env var in production; consolidate image URL logic into a single `getImageUrl()` helper.

### Package B: UX Breaks

**B1 — Footer newsletter form reloads page** (🔴 Critical)
- File: `cms.frontend/src/components/Footer.jsx:21`
- Bug: `<form>` has no `onSubmit` → browser submits POST, reloads page.
- Fix: Add `onSubmit={(e) => e.preventDefault()}`.

**B2 — 401 redirect loses state** (🟠 Major)
- File: `cms.frontend/src/api/axiosClient.js:29`
- Bug: `window.location.href = '/login'` causes full browser navigation → all React state lost.
- Fix: Instead of `window.location.href`, dispatch a custom event or redirect via `window.location.hash` pattern. Or use a global `eventEmitter` to trigger navigation in App.jsx.

**B3 — 404 page uses `<a>` instead of `<Link>`** (🟠 Major)
- File: `cms.frontend/src/App.jsx:82`
- Bug: `<a href="/">` causes full page reload.
- Fix: `<Link to="/">`.

### Package C: Functional Correctness

**C1 — Cart ignores discountPrice** (🟠 Major)
- File: `cms.frontend/src/pages/cart/CartTable.jsx:42,62`
- Bug: Line total uses `item.price` even when `item.discountPrice` exists.
- Fix: Use `(item.discountPrice || item.price)` for unit price and line total.

**C2 — Related products button missing onClick** (🟠 Major)
- File: `cms.frontend/src/pages/product-detail/index.jsx:281`
- Bug: "Add to cart" button in related products section has no handler.
- Fix: Wire `addToCart(rp)` to `onClick`.

**C3 — Image gallery shows 3 identical images** (🟠 Major)
- File: `cms.frontend/src/pages/product-detail/index.jsx:107-126`
- Bug: All three `<img>` tags use `product.imageUrl`. Backend returns single image.
- Fix: Collapse to a single main image + remove placeholder thumbnails. If backend adds gallery support later, implement proper multi-image then.

**C4 — sort() mutates original array** (🟠 Major)
- File: `cms.frontend/src/pages/home/LatestBlog.jsx:14`
- Bug: `data.sort(...)` sorts in place, mutating API response.
- Fix: `[...data].sort(...)`.

### Package D: Dead Code & Dependencies

**D1 — Remove unused packages** (🟡 Minor)
- `react-transition-group`, `@mui/material`, `@emotion/react`, `@emotion/styled` are never imported.
- Fix: `npm uninstall` each.

**D2 — Fix index.html metadata** (🟡 Minor)
- Title from "React App" to "AnhCMS.Fashion".
- Canonical URL from `localhost` to production domain.
- Copyright year 2024 → dynamic `new Date().getFullYear()`.

## Excluded (deferred to Phase 4)
- Global layout (Header/Footer in App.jsx)
- Toast notification system
- Vite migration
- TypeScript migration
- React Query / data fetching pattern
- Form validation library
- Error boundaries
- Scroll restoration
- Shared component refactoring
