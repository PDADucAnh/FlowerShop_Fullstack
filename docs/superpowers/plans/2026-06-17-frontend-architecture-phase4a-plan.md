# Phase 4a: Frontend Architecture — Vite + TypeScript + Global Layout Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Migrate from CRA 5 + JSX to Vite + React-TS, add backend-matching TypeScript types, and move Header/Footer to global layout.

**Architecture:** Scaffold a fresh Vite React-TS project, copy config files, batch-rename all `.js/.jsx` to `.ts/.tsx`, add type definitions for all backend DTOs, restructure `App.tsx` to wrap routes in Header/Footer, and remove per-page Header/Footer imports.

**Tech Stack:** Vite 6, React 19, TypeScript 5, React Router v7, Axios, Tailwind CSS (CDN), Bootstrap 4 (CDN)

---

## File Structure

| File | Action | Purpose |
|---|---|---|
| `vite.config.ts` | Create | Vite config with React plugin + `@/` alias |
| `tsconfig.json` | Create | Root TS config referencing app + node configs |
| `tsconfig.app.json` | Create | App-level TS strict config with path aliases |
| `tsconfig.node.json` | Create | Node config for Vite |
| `index.html` | Create (root) | Vite entry point with CDN links |
| `src/vite-env.d.ts` | Create | Vite client + env type declarations |
| `src/main.tsx` | Create | App entry point (replaces index.js) |
| `src/types/category.ts` | Create | Category DTO interfaces |
| `src/types/product.ts` | Create | Product DTO interfaces |
| `src/types/post.ts` | Create | Post interface |
| `src/types/customer.ts` | Create | Customer DTO interfaces |
| `src/types/order.ts` | Create | Order types + OrderStatus enum |
| `src/types/user.ts` | Create | User + Login types |
| `src/types/api.ts` | Create | ApiListResponse + ApiError wrappers |
| `src/types/context.ts` | Create | AuthState + CartState + CartItem types |
| `src/App.tsx` | Create | Global layout with Header/Footer wrapping |
| `src/utils/apiUtils.ts` | Modify | Use `import.meta.env.VITE_API_URL` |
| `src/api/axiosClient.ts` | Modify | Use `import.meta.env.VITE_API_URL` |
| `src/utils/eventEmitter.ts` | Rename | .js → .ts |
| `src/context/AuthContext.tsx` | Rename | .js → .tsx, add types |
| `src/context/CartContext.tsx` | Rename | .js → .tsx, add types |
| `src/services/*.ts` (5 files) | Rename | .js → .ts |
| `src/components/*.tsx` (5 files) | Rename | .jsx → .tsx |
| `src/pages/**/index.tsx` (9 dirs) | Rename | .jsx → .tsx, remove Header/Footer |
| `src/pages/cart/CartTable.tsx` | Rename | .jsx → .tsx |
| `src/pages/home/LatestBlog.tsx` | Rename | .jsx → .tsx |
| `src/pages/blog/BlogSidebar.tsx` | Rename | .jsx → .tsx |
| `package.json` | Modify | Replace CRA deps with Vite deps |
| `public/index.html` | Remove | Replaced by root index.html |
| `src/index.js` | Remove | Replaced by main.tsx |
| `src/reportWebVitals.js` | Remove | CRA-specific |
| `src/App.test.js` | Remove | CRA-specific |
| `src/setupTests.js` | Remove | CRA-specific |

---

### Task 1: Scaffold Vite Project + Create Config Files

**Files:**
- Create: `vite.config.ts`
- Create: `tsconfig.json`
- Create: `tsconfig.app.json`
- Create: `tsconfig.node.json`
- Create: `index.html` (root, replaces public/index.html)

- [ ] **Step 1: Scaffold temp Vite project**

```bash
mkdir temp-vite
cd temp-vite
npm create vite@latest . -- --template react-ts
```

Run: `npm create vite@latest temp-vite -- --template react-ts`

- [ ] **Step 2: Copy Vite config files into cms.frontend/**

```bash
# From temp-vite/ to cms.frontend/
cp temp-vite/vite.config.ts cms.frontend/vite.config.ts
cp temp-vite/tsconfig.json cms.frontend/tsconfig.json
cp temp-vite/tsconfig.app.json cms.frontend/tsconfig.app.json
cp temp-vite/tsconfig.node.json cms.frontend/tsconfig.node.json
cp temp-vite/src/vite-env.d.ts cms.frontend/src/vite-env.d.ts
```

- [ ] **Step 3: Update vite.config.ts with path alias + proxy**

Write `cms.frontend/vite.config.ts`:
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
        target: 'https://localhost:7224',
        changeOrigin: true,
      },
    },
  },
})
```

- [ ] **Step 4: Update tsconfig.app.json with path alias**

Edit `cms.frontend/tsconfig.app.json` to add:
```json
"compilerOptions": {
  "baseUrl": ".",
  "paths": {
    "@/*": ["./src/*"]
  }
}
```

Keep all existing compiler options from the scaffold template.

- [ ] **Step 5: Write root index.html**

Write `cms.frontend/index.html`:
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <link rel="icon" href="/favicon.ico" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="theme-color" content="#000000" />
    <meta name="description" content="AnhCMS.Fashion - Modern fashion e-commerce platform" />
    <link rel="apple-touch-icon" href="/apple-touch-icon.png" />
    <link rel="manifest" href="/manifest.json" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">
    <link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;500;600;700;800&display=swap" rel="stylesheet">
    <script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&display=swap" rel="stylesheet" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&family=Playfair+Display:wght@600;700&display=swap" rel="stylesheet" />
    <script id="tailwind-config">tailwind.config={"darkMode":"class","theme":{"extend":{"colors":{"on-tertiary-container":"#848484","surface-dim":"#dadada","on-secondary-fixed-variant":"#454747","secondary-fixed-dim":"#c6c6c7","secondary-fixed":"#e2e2e2","surface-variant":"#e2e2e2","secondary":"#5d5f5f","on-primary-fixed-variant":"#474747","inverse-surface":"#2f3131","surface-container":"#eeeeee","primary-fixed":"#e2e2e2","on-error-container":"#93000a","tertiary-container":"#1b1b1b","outline-variant":"#cfc4c5","inverse-primary":"#c6c6c6","on-background":"#1a1c1c","on-tertiary":"#ffffff","background":"#f9f9f9","outline":"#7e7576","error":"#ba1a1a","on-primary":"#ffffff","on-tertiary-fixed":"#1b1b1b","surface-container-high":"#e8e8e8","primary":"#000000","on-primary-fixed":"#1b1b1b","primary-container":"#1b1b1b","surface-container-low":"#f3f3f3","on-surface":"#1a1c1c","on-secondary":"#ffffff","on-secondary-container":"#616363","surface":"#f9f9f9","on-primary-container":"#848484","surface-tint":"#5e5e5e","tertiary-fixed":"#e2e2e2","on-surface-variant":"#4c4546","inverse-on-surface":"#f1f1f1","surface-container-highest":"#e2e2e2","surface-container-lowest":"#ffffff","primary-fixed-dim":"#c6c6c6","on-tertiary-fixed-variant":"#474747","surface-bright":"#f9f9f9","tertiary-fixed-dim":"#c6c6c6","on-secondary-fixed":"#1a1c1c","error-container":"#ffdad6","on-error":"#ffffff","secondary-container":"#dfe0e0","tertiary":"#000000"},"borderRadius":{"DEFAULT":"0px","lg":"0px","xl":"0px","full":"9999px"},"spacing":{"lg":"2rem","md":"1.5rem","xs":"0.5rem","margin":"4rem","sm":"1rem","xl":"4rem","base":"4px","gutter":"1.5rem"},"fontFamily":{"display-xl":["Playfair Display"],"display-xl-mobile":["Playfair Display"],"body-md":["Inter"],"headline-sm":["Inter"],"label-sm":["Inter"],"headline-lg":["Playfair Display"],"body-lg":["Inter"]},"fontSize":{"display-xl":["64px",{"lineHeight":"1.1","letterSpacing":"-0.02em","fontWeight":"700"}],"display-xl-mobile":["40px",{"lineHeight":"1.2","fontWeight":"700"}],"body-md":["16px",{"lineHeight":"1.6","fontWeight":"400"}],"headline-sm":["20px",{"lineHeight":"1.4","letterSpacing":"0.05em","fontWeight":"600"}],"label-sm":["12px",{"lineHeight":"1","letterSpacing":"0.1em","fontWeight":"500"}],"headline-lg":["32px",{"lineHeight":"1.2","fontWeight":"600"}],"body-lg":["18px",{"lineHeight":"1.6","fontWeight":"400"}]}}}}</script>
    <title>AnhCMS.Fashion</title>
</head>
<body>
    <noscript>You need to enable JavaScript to run this app.</noscript>
    <div id="root"></div>
    <script type="module" src="/src/main.tsx"></script>
</body>
</html>
```

- [ ] **Step 6: Remove old public/index.html**

```bash
rm cms.frontend/public/index.html
```

- [ ] **Step 7: Copy public/ assets from temp Vite (favicon, etc.)**

```bash
cp temp-vite/public/* cms.frontend/public/ 2>/dev/null || true
```

- [ ] **Step 8: Clean up temp directory**

```bash
rm -rf temp-vite
```

- [ ] **Step 9: Commit**

```bash
git add cms.frontend/vite.config.ts cms.frontend/tsconfig.json cms.frontend/tsconfig.app.json cms.frontend/tsconfig.node.json cms.frontend/index.html cms.frontend/src/vite-env.d.ts cms.frontend/public/
git commit -m "feat: scaffold Vite React-TS config with path alias and proxy"
```

---

### Task 2: Create TypeScript Type Definitions

**Files:**
- Create: `src/types/category.ts`
- Create: `src/types/product.ts`
- Create: `src/types/post.ts`
- Create: `src/types/customer.ts`
- Create: `src/types/order.ts`
- Create: `src/types/user.ts`
- Create: `src/types/api.ts`
- Create: `src/types/context.ts`

- [ ] **Step 1: Create types directory and category types**

```bash
mkdir -p cms.frontend/src/types
```

Write `cms.frontend/src/types/category.ts`:
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

- [ ] **Step 2: Create product types**

Write `cms.frontend/src/types/product.ts`:
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

- [ ] **Step 3: Create post types**

Write `cms.frontend/src/types/post.ts`:
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

- [ ] **Step 4: Create customer types**

Write `cms.frontend/src/types/customer.ts`:
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

- [ ] **Step 5: Create order types**

Write `cms.frontend/src/types/order.ts`:
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

- [ ] **Step 6: Create user types**

Write `cms.frontend/src/types/user.ts`:
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

- [ ] **Step 7: Create API response wrapper types**

Write `cms.frontend/src/types/api.ts`:
```ts
export interface ApiListResponse<T> {
  $values: T[];
  $id?: string;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}
```

- [ ] **Step 8: Create context types**

Write `cms.frontend/src/types/context.ts`:
```ts
import type { Product } from './product';

export interface AuthUser {
  id: number;
  username: string;
  fullName: string;
  role: string;
}

export interface AuthState {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  loading: boolean;
}

export interface CartItem extends Product {
  quantity: number;
}
```

- [ ] **Step 9: Commit**

```bash
git add cms.frontend/src/types/
git commit -m "feat: add TypeScript type definitions matching backend DTOs"
```

---

### Task 3: Create main.tsx Entry Point

**Files:**
- Create: `src/main.tsx`
- Remove: `src/index.js`, `src/reportWebVitals.js`, `src/App.test.js`, `src/setupTests.js`

- [ ] **Step 1: Write main.tsx**

Write `cms.frontend/src/main.tsx`:
```tsx
import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
```

- [ ] **Step 2: Remove CRA entry files**

```bash
rm cms.frontend/src/index.js
rm cms.frontend/src/reportWebVitals.js
rm cms.frontend/src/App.test.js
rm cms.frontend/src/setupTests.js
```

- [ ] **Step 3: Commit**

```bash
git add cms.frontend/src/main.tsx
git rm cms.frontend/src/index.js cms.frontend/src/reportWebVitals.js cms.frontend/src/App.test.js cms.frontend/src/setupTests.js
git commit -m "feat: add Vite entry point main.tsx, remove CRA-specific files"
```

---

### Task 4: Convert Utility Files to TypeScript

**Files:**
- Modify: `src/utils/apiUtils.ts` (already .ts, update env vars)
- Rename: `src/utils/eventEmitter.js` → `src/utils/eventEmitter.ts`
- Rename: `src/services/*.js` → `src/services/*.ts`
- Rename: `src/api/axiosClient.js` → `src/api/axiosClient.ts`

- [ ] **Step 1: Update apiUtils.ts for Vite env vars**

Read `cms.frontend/src/utils/apiUtils.ts` and update to use `import.meta.env.VITE_API_URL`:

```ts
const getBaseUrl = () => {
  const url = import.meta.env.VITE_API_URL;
  if (!url) {
    if (import.meta.env.PROD) {
      throw new Error('VITE_API_URL environment variable is required in production');
    }
    return 'https://localhost:7224';
  }
  return url;
};

export const API_BASE_URL = getBaseUrl();

export const getImageUrl = (path?: string): string => {
  if (!path) return 'https://via.placeholder.com/400x400?text=No+Image';
  if (path.startsWith('http')) return path;
  return `${API_BASE_URL}${path.startsWith('/') ? '' : '/'}${path}`;
};
```

- [ ] **Step 2: Update vite-env.d.ts with env types**

Write `cms.frontend/src/vite-env.d.ts`:
```ts
/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
```

- [ ] **Step 3: Rename eventEmitter.js → eventEmitter.ts**

```bash
git mv cms.frontend/src/utils/eventEmitter.js cms.frontend/src/utils/eventEmitter.ts
```

- [ ] **Step 4: Rename all service files .js → .ts**

```bash
for f in cms.frontend/src/services/*.js; do
  git mv "$f" "${f%.js}.ts"
done
```

- [ ] **Step 5: Rename axiosClient.js → axiosClient.ts**

```bash
git mv cms.frontend/src/api/axiosClient.js cms.frontend/src/api/axiosClient.ts
```

Read `cms.frontend/src/api/axiosClient.ts` and update to use Vite env:
```ts
import axios from 'axios';
import { API_BASE_URL } from '../utils/apiUtils';
import { authEvents } from '../utils/eventEmitter';

const axiosClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' },
});

axiosClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('auth_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

axiosClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('auth_token');
      localStorage.removeItem('auth_user');
      authEvents.emit('unauthorized');
    }
    return Promise.reject(error);
  }
);

export default axiosClient;
```

- [ ] **Step 6: Update imports in service files**

For each `.ts` file in `src/services/`, ensure the `import axiosClient from` path uses `.ts` extension or just the module name (Vite resolves extensions automatically — no changes needed if import paths didn't include `.js` extensions).

Run a quick check:
```bash
rg "from.*\.js" cms.frontend/src/services/ cms.frontend/src/api/
```
If any `.js` extension imports exist, replace them with no extension (e.g., `from './foo.js'` → `from './foo'`).

- [ ] **Step 7: Commit**

```bash
git add cms.frontend/src/utils/ cms.frontend/src/services/ cms.frontend/src/api/ cms.frontend/src/vite-env.d.ts
git commit -m "feat: convert utilities/services/api to TS, update to Vite env vars"
```

---

### Task 5: Convert Context Files to TypeScript

**Files:**
- Rename: `src/context/AuthContext.js` → `src/context/AuthContext.tsx`
- Rename: `src/context/CartContext.js` → `src/context/CartContext.tsx`

- [ ] **Step 1: Rename AuthContext and add types**

```bash
git mv cms.frontend/src/context/AuthContext.js cms.frontend/src/context/AuthContext.tsx
```

Read `cms.frontend/src/context/AuthContext.tsx` and add type imports:
```tsx
import React, { createContext, useContext, useState, useEffect, useCallback, type ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';
import type { AuthUser, AuthState } from '../types/context';
```
Also ensure the provider and hook are properly typed (return types).

- [ ] **Step 2: Rename CartContext and add types**

```bash
git mv cms.frontend/src/context/CartContext.js cms.frontend/src/context/CartContext.tsx
```

Read `cms.frontend/src/context/CartContext.tsx` and add type imports:
```tsx
import React, { createContext, useContext, useState, useCallback, type ReactNode } from 'react';
import type { Product } from '../types/product';
import type { CartItem } from '../types/context';
```

- [ ] **Step 3: Build to check for type errors**

```bash
cd cms.frontend && npx tsc --noEmit 2>&1 | head -50
```

Fix any type errors found (likely missing `ReactNode` imports or `children` typing).

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/context/
git commit -m "feat: convert context files to TypeScript with typed interfaces"
```

---

### Task 6: Convert Components to TypeScript

**Files:**
- Rename: `src/components/Header.jsx` → `src/components/Header.tsx`
- Rename: `src/components/Footer.jsx` → `src/components/Footer.tsx`
- Rename: `src/components/ProtectedRoute.jsx` → `src/components/ProtectedRoute.tsx`
- Rename: `src/components/ProductCard.jsx` → `src/components/ProductCard.tsx`
- Rename: `src/components/PostCard.jsx` → `src/components/PostCard.tsx`
- Rename: `src/pages/cart/CartTable.jsx` → `src/pages/cart/CartTable.tsx`
- Rename: `src/pages/home/LatestBlog.jsx` → `src/pages/home/LatestBlog.tsx`
- Rename: `src/pages/blog/BlogSidebar.jsx` → `src/pages/blog/BlogSidebar.tsx`

- [ ] **Step 1: Rename all component files**

```bash
git mv cms.frontend/src/components/Header.jsx cms.frontend/src/components/Header.tsx
git mv cms.frontend/src/components/Footer.jsx cms.frontend/src/components/Footer.tsx
git mv cms.frontend/src/components/ProtectedRoute.jsx cms.frontend/src/components/ProtectedRoute.tsx
git mv cms.frontend/src/components/ProductCard.jsx cms.frontend/src/components/ProductCard.tsx
git mv cms.frontend/src/components/PostCard.jsx cms.frontend/src/components/PostCard.tsx
git mv cms.frontend/src/pages/cart/CartTable.jsx cms.frontend/src/pages/cart/CartTable.tsx
git mv cms.frontend/src/pages/home/LatestBlog.jsx cms.frontend/src/pages/home/LatestBlog.tsx
git mv cms.frontend/src/pages/blog/BlogSidebar.jsx cms.frontend/src/pages/blog/BlogSidebar.tsx
```

- [ ] **Step 2: Add typing to ProtectedRoute**

Read `cms.frontend/src/components/ProtectedRoute.tsx` and add `children` typing:
```tsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();
  if (loading) return <div className="text-center py-5"><div className="spinner-border" /></div>;
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  return <>{children}</>;
};

export default ProtectedRoute;
```

- [ ] **Step 3: Add typing to ProductCard**

Read `cms.frontend/src/components/ProductCard.tsx` and add prop interface:
```tsx
import type { Product } from '../types/product';

interface ProductCardProps {
  item: Product;
}

const ProductCard: React.FC<ProductCardProps> = ({ item }) => {
  // ... rest unchanged
};
```

- [ ] **Step 4: Add typing to PostCard**

Read `cms.frontend/src/components/PostCard.tsx` and add prop interface:
```tsx
import type { Post } from '../types/post';

interface PostCardProps {
  post: Post;
}

const PostCard: React.FC<PostCardProps> = ({ post }) => {
  // ... rest unchanged
};
```

- [ ] **Step 5: Build to check for type errors**

```bash
cd cms.frontend && npx tsc --noEmit 2>&1 | head -50
```

Fix any type errors found.

- [ ] **Step 6: Commit**

```bash
git add cms.frontend/src/components/ cms.frontend/src/pages/cart/CartTable.tsx cms.frontend/src/pages/home/LatestBlog.tsx cms.frontend/src/pages/blog/BlogSidebar.tsx
git commit -m "feat: convert components to TypeScript with typed props"
```

---

### Task 7: Restructure App.tsx with Global Layout

**Files:**
- Create: `src/App.tsx` (replaces App.jsx)

- [ ] **Step 1: Remove old App.jsx**

```bash
git rm cms.frontend/src/App.jsx
```

- [ ] **Step 2: Write App.tsx with global Header/Footer layout**

Write `cms.frontend/src/App.tsx`:
```tsx
import React, { lazy, Suspense, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, useNavigate, Link } from 'react-router-dom';
import Header from './components/Header';
import Footer from './components/Footer';
import ProtectedRoute from './components/ProtectedRoute';
import { AuthProvider } from './context/AuthContext';
import { CartProvider } from './context/CartContext';
import { authEvents } from './utils/eventEmitter';

const Home = lazy(() => import('./pages/home/index'));
const Shop = lazy(() => import('./pages/shop/index'));
const ProductDetail = lazy(() => import('./pages/product-detail/index'));
const Blog = lazy(() => import('./pages/blog/index'));
const BlogDetail = lazy(() => import('./pages/blog-detail/index'));
const Cart = lazy(() => import('./pages/cart/index'));
const Checkout = lazy(() => import('./pages/checkout/index'));
const Login = lazy(() => import('./pages/login/index'));
const Register = lazy(() => import('./pages/register/index'));

const PageLoader: React.FC = () => (
  <div className="d-flex justify-content-center align-items-center min-vh-100 bg-light">
    <div className="text-center">
      <div className="spinner-border text-dark mb-3" role="status" style={{ width: '2rem', height: '2rem' }}>
        <span className="visually-hidden">Loading...</span>
      </div>
      <p className="text-muted font-body-sm text-[11px] tracking-widest text-uppercase">ANHCMS NARRATIVE...</p>
    </div>
  </div>
);

const AuthRedirectHandler: React.FC = () => {
  const navigate = useNavigate();
  useEffect(() => {
    const unsubscribe = authEvents.on('unauthorized', () => navigate('/login'));
    return unsubscribe;
  }, [navigate]);
  return null;
};

const NotFound: React.FC = () => (
  <div className="container text-center py-5 my-5">
    <img
      src="https://cdn-icons-png.flaticon.com/512/580/580185.png"
      alt="404"
      className="mb-4"
      style={{ width: '100px', opacity: 0.6 }}
    />
    <h2 className="fw-bold text-secondary">404 - KHÔNG TÌM THẤY TRANG</h2>
    <p className="text-muted">Đường dẫn bạn truy cập không tồn tại trên hệ thống AnhCMS.</p>
    <Link to="/" className="btn btn-dark btn-sm mt-2 text-decoration-none">Quay lại Trang Chủ</Link>
  </div>
);

const App: React.FC = () => {
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
};

export default App;
```

- [ ] **Step 3: Commit**

```bash
git add cms.frontend/src/App.tsx
git rm cms.frontend/src/App.jsx
git commit -m "feat: add global Header/Footer layout, extract NotFound component, typed App"
```

---

### Task 8: Remove Header/Footer from All Pages

**Files:**
- Modify: `src/pages/home/index.tsx`
- Modify: `src/pages/shop/index.tsx`
- Modify: `src/pages/product-detail/index.tsx`
- Modify: `src/pages/blog/index.tsx`
- Modify: `src/pages/blog-detail/index.tsx`
- Modify: `src/pages/cart/index.tsx`
- Modify: `src/pages/checkout/index.tsx`
- Modify: `src/pages/login/index.tsx`
- Modify: `src/pages/register/index.tsx`

- [ ] **Step 1: Rename all page files .jsx → .tsx**

```bash
for f in cms.frontend/src/pages/*/index.jsx; do
  git mv "$f" "${f%.jsx}.tsx"
done
for f in cms.frontend/src/pages/*/index.jsx cms.frontend/src/pages/*/index.tsx 2>/dev/null; do
  # handle any remaining
done
```

- [ ] **Step 2: Remove Header import/usage from all pages**

For each page file, run a sed command to remove `import Header` and `import Footer` lines and their JSX usage. Since the pattern is the same across all pages:

```bash
# Remove import Header lines
rg -l "import Header" cms.frontend/src/pages/ | ForEach-Object { 
  (Get-Content $_) -replace "import Header from '.*'`r?`n", "" | Set-Content $_
}

# Remove import Footer lines
rg -l "import Footer" cms.frontend/src/pages/ | ForEach-Object { 
  (Get-Content $_) -replace "import Footer from '.*'`r?`n", "" | Set-Content $_
}

# Remove <Header /> usage (typically right inside return div)
rg -l "<Header />" cms.frontend/src/pages/ | ForEach-Object {
  (Get-Content $_) -replace "<Header />`r?`n", "" | Set-Content $_
}

# Remove <Footer /> usage
rg -l "<Footer />" cms.frontend/src/pages/ | ForEach-Object {
  (Get-Content $_) -replace "<Footer />`r?`n", "" | Set-Content $_
}
```

After sed, manually verify each file has no remaining Header/Footer references:
```bash
rg "Header|Footer" cms.frontend/src/pages/
```
Expected: No matches (Header/Footer only in App.tsx, components/).

- [ ] **Step 3: Commit**

```bash
git add cms.frontend/src/pages/
git commit -m "refactor: remove per-page Header/Footer imports (now global in App.tsx)"
```

---

### Task 9: Update package.json and Install Dependencies

**Files:**
- Modify: `package.json`

- [ ] **Step 1: Update package.json scripts and deps**

Update `cms.frontend/package.json`:
```json
{
  "name": "cms.frontend",
  "version": "0.1.0",
  "private": true,
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc -b && vite build",
    "preview": "vite preview",
    "typecheck": "tsc --noEmit"
  },
  "dependencies": {
    "axios": "^1.17.0",
    "dompurify": "^3.4.10",
    "react": "^19.2.6",
    "react-dom": "^19.2.6",
    "react-icons": "^5.6.0",
    "react-router-dom": "^7.17.0"
  },
  "devDependencies": {
    "@types/dompurify": "^3.2.0",
    "@types/react": "^19.1.0",
    "@types/react-dom": "^19.1.0",
    "@vitejs/plugin-react": "^4.4.0",
    "typescript": "^5.8.0",
    "vite": "^6.3.0"
  }
}
```

- [ ] **Step 2: Delete node_modules and reinstall with Vite**

```bash
cd cms.frontend
rm -rf node_modules package-lock.json
npm install
```

- [ ] **Step 3: Commit**

```bash
git add cms.frontend/package.json cms.frontend/package-lock.json
git commit -m "chore: migrate from CRA to Vite deps, update scripts"
```

---

### Task 10: Build and Fix TypeScript Errors

- [ ] **Step 1: Run TypeScript check**

```bash
cd cms.frontend && npx tsc --noEmit 2>&1
```

- [ ] **Step 2: Fix all type errors iteratively**

Common errors to expect and fix:
- `Cannot find module 'X' or its corresponding type declarations` → install `@types/X` or add `declare module 'X'`
- `Property 'X' does not exist on type ...` → add missing property to interface
- `JSX element type 'X' does not have any construct or call signatures` → ensure function component returns JSX
- `'children' is missing in props` → add `children: React.ReactNode` to component props
- `Object is possibly 'undefined'` → add optional chaining or null check
- `Type 'string | undefined' is not assignable to type 'string'` → add default value or non-null assertion

Fix errors iteratively, running `npx tsc --noEmit` after each batch.

- [ ] **Step 3: Run Vite build**

```bash
cd cms.frontend && npm run build 2>&1
```

Expected: Build succeeds with 0 errors.

- [ ] **Step 4: Commit any type fixes**

```bash
git add -A
git commit -m "fix: resolve TypeScript errors after migration"
```

---

### Task 11: Final Verification

- [ ] **Step 1: Backend tests still pass**

```bash
cd cms.frontend && dotnet test CMS.Tests --configuration Release --no-restore 2>&1
```
Expected: 27/27 pass.

- [ ] **Step 2: Frontend build succeeds**

```bash
cd cms.frontend && npm run build 2>&1
```
Expected: 0 errors.

- [ ] **Step 3: Verify no leftover Header/Footer in pages**

```bash
rg "import Header|import Footer" cms.frontend/src/pages/
```
Expected: No matches.

- [ ] **Step 4: Verify no CRA references remain**

```bash
rg "react-scripts|REACT_APP_" cms.frontend/package.json cms.frontend/src/
```
Expected: No matches (only `VITE_API_URL` in env).

- [ ] **Step 5: Push**

```bash
git push origin Buoi_09
```
