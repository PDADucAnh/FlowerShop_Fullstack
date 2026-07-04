# Phase 4a: Frontend Architecture — Vite + TypeScript + Global Layout

**Goal:** Migrate frontend from CRA 5 + JSX to Vite + React-TS, add TypeScript types matching backend DTOs, and move Header/Footer to global layout.

**Why split:** Phase 4 was too large for one spec. Sub-phase 4a handles the foundational migration (Vite, TypeScript, layout restructuring). Sub-phase 4b+c handles React Query, error boundaries, toast system, and form validation.

**Tech Stack (after migration):** Vite 6, React 19, TypeScript 5, React Router v7, Axios, Tailwind CSS (CDN), Bootstrap 4 (CDN), dompurify, react-icons.

---

## 1. Build System + TypeScript Architecture

### 1.1 Scaffold Strategy

Create a fresh Vite project with `react-ts` template, then copy existing source files into it and convert.

```bash
npm create vite@latest temp-frontend -- --template react-ts
```

Key files to copy from the scaffold into `cms.frontend/`:
- `vite.config.ts` — Vite config with `@vitejs/plugin-react`
- `tsconfig.json` — references `tsconfig.app.json` + `tsconfig.node.json`
- `tsconfig.app.json` — app-level TS config with `strict: true`
- `tsconfig.node.json` — Node config for Vite itself
- `src/vite-env.d.ts` — Vite client type declarations
- Root `index.html` — Vite uses this as entry point (moved from `public/`)

### 1.2 TypeScript Strictness

- `strict: true` in `tsconfig.app.json` (Vite react-ts template default)
- NO `any` for business logic — typed DTOs matching backend models
- `@types/react`, `@types/react-dom` installed automatically by the template
- Path alias `@/` → `src/` configured in both `vite.config.ts` and `tsconfig.app.json`

### 1.3 Environment Variables

- `VITE_API_URL` replaces `REACT_APP_API_URL` (Vite uses `VITE_` prefix)
- Update `apiUtils.ts` to read `import.meta.env.VITE_API_URL`
- `src/vite-env.d.ts` augmented with typed `ImportMetaEnv`:
```ts
interface ImportMetaEnv {
  readonly VITE_API_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
```

### 1.4 Directory Structure (unchanged from CRA)

```
src/
  api/         → apiUtils.ts, axiosClient.ts, eventEmitter.ts
  assets/
  components/
  context/
  pages/
  services/
  types/       → NEW: shared TypeScript interfaces/DTOs
  utils/
  App.tsx
  main.tsx     (replaces index.js)
  vite-env.d.ts
```

---

## 2. Migration Steps — CRA to Vite

### 2.1 File Conversion

| Before | After |
|---|---|
| `src/index.js` | `src/main.tsx` |
| `src/App.jsx` | `src/App.tsx` |
| `src/**/*.jsx` | `src/**/*.tsx` |
| `src/**/*.js` (under `src/`) | `src/**/*.ts` |
| `src/utils/eventEmitter.js` | `src/utils/eventEmitter.ts` |
| `src/services/*.js` | `src/services/*.ts` |
| `src/api/*.js` | `src/api/*.ts` |
| `src/context/*.js` | `src/context/*.tsx` |
| `src/components/*.jsx` | `src/components/*.tsx` |
| `.env` | `.env` (unchanged, but prefix changes to `VITE_`) |

**Exception:** `src/assets/` files (CSS, images) remain unchanged — no TypeScript conversion needed.

### 2.2 index.html Structure

Vite places `index.html` at project root (not `public/`). Contents:
```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <link rel="icon" type="image/svg+xml" href="/vite.svg" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <meta name="description" content="AnhCMS.Fashion - Modern fashion e-commerce platform" />
  <title>AnhCMS.Fashion</title>
  <!-- CDN links: Bootstrap, Font Awesome, Tailwind, Google Fonts, Material Symbols -->
  <script type="module" src="/src/main.tsx"></script>
</head>
<body>
  <div id="root"></div>
</body>
</html>
```

### 2.3 Package Changes

**Install (keep versions from current CRA setup):**
- `axios`, `dompurify`, `react-router-dom`, `react-icons` — reinstall in new Vite project
- `@types/dompurify` — TypeScript types for dompurify
- `@types/node` — TypeScript types for Node.js `path` module used in vite.config.ts

**Remove (CRA-specific, no longer needed):**
- `react-scripts` — entire CRA build system
- `@testing-library/dom`, `@testing-library/jest-dom`, `@testing-library/react`, `@testing-library/user-event` — testing will be re-evaluated later
- `web-vitals` — CRA-specific metric reporting

**Remove from src/:**
- `reportWebVitals.js`
- `App.test.js`
- `setupTests.js`

### 2.4 vite.config.ts

```ts
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src'),
    },
  },
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: process.env.VITE_API_URL || 'https://localhost:7224',
        changeOrigin: true,
      },
    },
  },
})
```

### 2.5 Environment Variable Migration

| CRA | Vite |
|---|---|
| `process.env.REACT_APP_API_URL` | `import.meta.env.VITE_API_URL` |
| `.env` with `REACT_APP_` prefix | `.env` with `VITE_` prefix |

Update `src/utils/apiUtils.js` → `src/utils/apiUtils.ts` to use `import.meta.env.VITE_API_URL` and export typed constants.

---

## 3. TypeScript Types — Backend DTO Mapping

Create `src/types/` directory with one file per DTO group. Each file exports:
- **Read interface** — matches backend GET response DTO
- **Input interface** — matches backend POST/PUT request DTO

### 3.1 Category Types (`src/types/category.ts`)

```ts
export interface Category {
  id: number;
  name: string;
  description?: string;
}

export interface CategoryInput {
  name: string;
  description?: string;
}
```

### 3.2 Product Types (`src/types/product.ts`)

```ts
export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  discountPrice?: number;
  imageUrl?: string;
  stockQuantity: number;
  categoryProductName?: string;
  createdDate?: string;
}

export interface ProductInput {
  name: string;
  description?: string;
  price: number;
  discountPrice?: number;
  imageUrl?: string;
  stockQuantity: number;
  categoryProductId?: number;
}
```

### 3.3 Post Types (`src/types/post.ts`)

```ts
export interface Post {
  id: number;
  title: string;
  content: string;
  imageUrl?: string;
  summary?: string;
  createdDate?: string;
  categoryName?: string;
  views?: number;
}
```

### 3.4 Customer Types (`src/types/customer.ts`)

```ts
export interface Customer {
  id: number;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
}

export interface CustomerInput {
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  password?: string;
}
```

### 3.5 Order Types (`src/types/order.ts`)

```ts
export type OrderStatus = 'Pending' | 'Shipping' | 'Completed';

export interface OrderDetail {
  id: number;
  orderId: number;
  productId: number;
  productName?: string;
  quantity: number;
  unitPrice: number;
  customerName?: string;
}

export interface Order {
  id: number;
  customerId: number;
  orderDate: string;
  status: OrderStatus;
  totalAmount: number;
  orderDetails?: OrderDetail[];
}

export interface OrderInput {
  customerId: number;
  items: { productId: number; quantity: number }[];
}
```

### 3.6 User Types (`src/types/user.ts`)

```ts
export interface User {
  id: number;
  username: string;
  fullName: string;
  role: string;
}

export interface UserInput {
  username: string;
  password: string;
  fullName: string;
  role: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  role: string;
  fullName: string;
}
```

### 3.7 API Response Wrapper

Backend wraps list responses with `$values`:

```ts
// src/types/api.ts
export interface ApiListResponse<T> {
  $values: T[];
  $id?: string;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}
```

### 3.8 Context Types

```ts
// src/types/context.ts
export interface AuthState {
  user: { id: number; username: string; fullName: string; role: string } | null;
  token: string | null;
  isAuthenticated: boolean;
  loading: boolean;
}

export interface CartItem extends Product {
  quantity: number;
}

export interface CartState {
  items: CartItem[];
  totalItems: number;
  totalPrice: number;
}
```

---

## 4. Global Layout

### 4.1 Current Problem

Every page component independently imports and renders `<Header />` and `<Footer />`. This causes:
- Duplicate imports across 8+ page files
- Inconsistent layout if one page forgets a component
- Impossible to add site-wide elements (e.g., toast container) without touching every page

### 4.2 Target Structure

`App.tsx` renders Header + content + Footer once:

```tsx
function App() {
  return (
    <AuthProvider>
      <CartProvider>
        <Router>
          <AuthRedirectHandler />
          <div className="d-flex flex-column min-vh-100 bg-light">
            <Header />
            <main className="flex-grow-1">
              <Suspense fallback={<PageLoader />}>
                <Routes>
                  <Route path="/" element={<Home />} />
                  <Route path="/shop" element={<Shop />} />
                  <Route path="/product/:id" element={<ProductDetail />} />
                  <Route path="/blog" element={<Blog />} />
                  <Route path="/blog/:id" element={<BlogDetail />} />
                  <Route path="/cart" element={<Cart />} />
                  <Route path="/checkout" element={<ProtectedRoute><Checkout /></ProtectedRoute>} />
                  <Route path="/login" element={<Login />} />
                  <Route path="/register" element={<Register />} />
                  <Route path="*" element={<NotFound />} />
                </Routes>
              </Suspense>
            </main>
            <Footer />
          </div>
        </Router>
      </CartProvider>
    </AuthProvider>
  );
}
```

### 4.3 Pages to Update

Remove `import Header` and `import Footer` + their JSX from these files:

| File | Removes Header | Removes Footer |
|---|---|---|
| `pages/home/index.tsx` | ✅ | ✅ |
| `pages/shop/index.tsx` | ✅ | ✅ |
| `pages/product-detail/index.tsx` | ✅ | ✅ |
| `pages/blog/index.tsx` | ✅ | ✅ |
| `pages/blog-detail/index.tsx` | ✅ | ✅ |
| `pages/cart/index.tsx` | ✅ | ✅ |
| `pages/checkout/index.tsx` | ✅ | ✅ |
| `pages/login/index.tsx` | ✅ | ✅ |
| `pages/register/index.tsx` | ✅ | ✅ |

### 4.4 Layout Verification

After migration, visually verify:
- Header appears on every route
- Footer stays at bottom (sticky footer via `flex-grow-1` on `<main>`)
- No double Header/Footer on any page
- Auth state in Header (login/logout buttons) works across navigation

---

## 5. Non-Goals (Deferred to Sub-phase 4b+c)

- **React Query** — server state management
- **Error boundaries** — crash resilience
- **Toast notification system** — user feedback
- **Form validation library** — form UX improvement
- Component refactoring beyond what TypeScript conversion requires
- CSS module migration or Tailwind changes

---

## 6. Testing & Verification

### 6.1 Build Verification

```bash
cd cms.frontend
npm run build
# Expected: 0 errors, all routes compile
```

### 6.2 Runtime Verification

- `npm run dev` — Vite dev server starts on port 3000
- Navigate to every route: homepage, shop, product detail, blog, blog detail, cart, checkout, login, register, 404
- Verify Header shows on all pages
- Verify Footer shows on all pages and stays at bottom
- Verify login/logout flow works
- Verify cart operations work

### 6.3 Backend Tests

```bash
dotnet test CMS.Tests
```
All 27 tests must continue to pass (no backend changes in Phase 4a).

---

## 7. Risk Mitigation

| Risk | Likelihood | Mitigation |
|---|---|---|
| Build errors during JSX→TSX conversion | High | Convert file-by-file, build after each group |
| Missing type definitions | Medium | Start with `any` for complex cases, refine in iteration |
| Vite dev server differs from CRA | Low | Same React/Router versions, proxy config matches CRA behavior |
| Global layout breaks a page | Medium | Visual check of all 9 routes before commit |
| Environment variable prefix change | Medium | Update all `process.env.REACT_APP_*` → `import.meta.env.VITE_*` |
