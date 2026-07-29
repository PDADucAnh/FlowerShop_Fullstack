# Admin SPA Migration — System Design

**Date:** 2026-07-29
**Project:** FlowerShop — Admin Panel SPA Migration
**Status:** Approved

---

## 1. Architecture Overview

### 1.1 Context

The current admin panel is an ASP.NET Core MVC application using Razor Views (`.cshtml`) served from `Flower.Backend/`. The goal is to extract it into a standalone React SPA deployed independently on Vercel, communicating with the ASP.NET backend (deployed on Render) via REST APIs with JWT Bearer authentication.

### 1.2 Deployment Topology

```
Vercel (flower-admin.vercel.app)                    Render (backend.flower.com)
┌──────────────────────────────┐      JWT Bearer     ┌──────────────────────────┐
│  React 19 + shadcn/ui SPA   │ ◄──────────────────► │  ASP.NET Core Web API   │
│  Vite + TypeScript + Tailwind│    CORS origin       │  /api/* endpoints       │
│  react-router-dom            │                      │  SignalR Hub            │
│  axios + interceptors        │                      │  JWT Auth Middleware     │
│  @tanstack/react-query       │                      │  CORS policy            │
└──────────────────────────────┘                      └──────────────────────────┘
```

### 1.3 Auth Flow

- User logs in via `POST /api/auth/login` → receives `{ accessToken, refreshToken, user }`
- `accessToken` (JWT, 15-30min expiry) stored in memory + `refreshToken` stored in `localStorage`
- Axios request interceptor attaches `Authorization: Bearer <accessToken>` to every request
- Axios response interceptor detects 401 → uses refresh queue pattern (see §3.3) to call `POST /api/auth/refresh` and retry failed requests
- `refreshToken` returned in API response body (not HttpOnly cookie) to avoid third-party cookie restrictions across origins
- On logout → `POST /api/auth/logout` + clear tokens from client
- `GET /api/auth/me` validates current token and returns user profile + roles

---

## 2. Project Structure

### 2.1 Directory Layout

```
flower-admin.frontend/
├── src/
│   ├── api/
│   │   ├── client.ts           # Axios instance + interceptors
│   │   ├── auth.ts
│   │   ├── products.ts
│   │   ├── categories.ts
│   │   ├── orders.ts
│   │   ├── customers.ts
│   │   ├── contacts.ts
│   │   ├── dashboard.ts
│   │   ├── posts.ts
│   │   ├── pages.ts
│   │   ├── layout.ts
│   │   ├── promotions.ts
│   │   ├── coupons.ts
│   │   ├── flashsales.ts
│   │   ├── advertisements.ts
│   │   ├── import.ts
│   │   ├── notifications.ts
│   │   ├── users.ts
│   │   └── settings.ts
│   ├── components/
│   │   ├── ui/                 # shadcn/ui primitives
│   │   ├── layout/
│   │   │   ├── AppShell.tsx
│   │   │   ├── AppSidebar.tsx
│   │   │   ├── AppHeader.tsx
│   │   │   └── AppFooter.tsx
│   │   └── shared/
│   │       ├── DataTable.tsx
│   │       ├── SearchInput.tsx
│   │       ├── Pagination.tsx
│   │       ├── StatusBadge.tsx
│   │       ├── ConfirmDialog.tsx
│   │       ├── EmptyState.tsx
│   │       └── ProtectedRoute.tsx
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   ├── useDebounce.ts
│   │   └── useRealtimeNotifications.ts
│   ├── lib/
│   │   ├── utils.ts
│   │   └── constants.ts
│   ├── types/
│   │   ├── api.ts               # ApiResponse<T>, Pagination, PaginationParams
│   │   ├── auth.ts               # User, LoginRequest, LoginResponse, AuthState
│   │   ├── product.ts            # Product, CreateProductDTO, UpdateProductDTO
│   │   ├── category.ts           # Category (product categories)
│   │   ├── order.ts              # Order, OrderDetail, OrderStatus
│   │   ├── customer.ts
│   │   ├── contact.ts
│   │   ├── post.ts               # BlogPost, BlogCategory
│   │   ├── page.ts
│   │   ├── promotion.ts
│   │   ├── coupon.ts
│   │   ├── flashsale.ts
│   │   ├── advertisement.ts
│   │   ├── notification.ts
│   │   ├── dashboard.ts          # DashboardSummary, DashboardCharts
│   │   ├── user.ts
│   │   └── settings.ts
│   ├── pages/
│   │   ├── login/
│   │   ├── dashboard/
│   │   ├── products/             # + categories-products
│   │   ├── orders/
│   │   ├── customers/
│   │   ├── contacts/
│   │   ├── posts/
│   │   ├── pages/
│   │   ├── layout/
│   │   ├── promotions/
│   │   ├── coupons/
│   │   ├── flashsales/
│   │   ├── advertisements/
│   │   ├── notifications/
│   │   ├── users/
│   │   └── settings/
│   ├── context/
│   │   ├── AuthContext.tsx
│   │   └── ThemeContext.tsx
│   ├── App.tsx
│   └── main.tsx
├── public/
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
└── tailwind.config.ts
```

### 2.2 Routes (no /admin prefix — app is already at admin subdomain)

| Path | Component | Protected |
|------|-----------|-----------|
| `/login` | LoginPage | No |
| `/` | DashboardPage | Yes |
| `/products` | ProductList | Yes (Staff+) |
| `/products/create` | ProductCreate | Yes (Staff+) |
| `/products/:id/edit` | ProductEdit | Yes (Staff+) |
| `/products/:id` | ProductDetail | Yes (Staff+) |
| `/categories-products` | CategoryProductList | Yes (Staff+) |
| `/orders` | OrderList | Yes (Staff+) |
| `/orders/:id` | OrderDetail | Yes (Staff+) |
| `/customers` | CustomerList | Yes (Staff+) |
| `/customers/:id/edit` | CustomerEdit | Yes (Staff+) |
| `/contacts` | ContactList | Yes (Staff+) |
| `/contacts/:id` | ContactDetail | Yes (Staff+) |
| `/posts` | PostList | Yes (Staff+) |
| `/posts/create` | PostCreate | Yes (Staff+) |
| `/posts/:id/edit` | PostEdit | Yes (Staff+) |
| `/categories` | BlogCategoryList | Yes (Staff+) |
| `/pages` | PageList | Yes (Staff+) |
| `/layout` | LayoutUI | Yes (Staff+) |
| `/import` | BulkImport | Yes (Staff+) |
| `/promotions` | PromotionList | Yes (Staff+) |
| `/coupons` | CouponList | Yes (Staff+) |
| `/flash-sales` | FlashSaleList | Yes (Staff+) |
| `/advertisements` | AdvertisementList | Yes (Staff+) |
| `/notifications` | NotificationList | Yes (Staff+) |
| `/users` | UserList | Yes (Admin only) |
| `/settings` | SettingsPage | Yes (Admin only) |

---

## 3. Backend Changes Required

### 3.1 JWT Authentication

Add to `Program.cs`:
- `Microsoft.AspNetCore.Authentication.JwtBearer` middleware
- Issuer, Audience, SigningKey configuration (from `appsettings.json` / env vars)
- Token validation parameters

### 3.2 CORS Configuration

Allow origins:
- `http://localhost:5173` (Vite dev)
- `http://localhost:5174` (Vite dev fallback)
- `https://flower-admin.vercel.app` (production)
- `https://flower-admin-*.vercel.app` (preview deploys)

### 3.3 Auth API Endpoints (new)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/login` | No | Login → returns `{ accessToken, refreshToken, user }` |
| POST | `/api/auth/refresh` | No | Refresh → returns `{ accessToken, refreshToken }` |
| POST | `/api/auth/logout` | Yes | Invalidate refresh token |
| GET | `/api/auth/me` | Yes | Current user profile + roles |

### 3.4 API Controllers (existing, mostly unchanged)

All existing `Controllers/Api/*` controllers serve as the data layer. No significant changes needed — they already return JSON. May need policy annotation adjustment to support JWT roles.

### 3.5 SignalR Hub

Existing notification hub (`/hubs/notifications`) continues to work. React client connects with `accessTokenFactory` to provide JWT for authentication.

---

## 4. Frontend Auth Implementation Detail

### 4.1 Axios Interceptor — Refresh Queue Pattern

```
Request fails with 401
        │
        ▼
 ┌── isRefreshing? ──yes──► queue request (promise)
 │      no
 │      ▼
 │  Set isRefreshing = true
 │      ▼
 │  POST /api/auth/refresh
 │      │
 │      ├── success ──► update tokens ──► replay queue ──► retry original
 │      └── fail    ──► clear tokens ──► redirect /login
```

### 4.2 SignalR Connection (React)

```typescript
const connection = new HubConnectionBuilder()
  .withUrl("https://backend.flower.com/hubs/notifications", {
    accessTokenFactory: () => getAccessToken()
  })
  .withAutomaticReconnect()
  .build();
```

---

## 5. Implementation Phases

### Phase 1 — Core Setup, Auth & Layout
**Files:** `client.ts`, `auth.ts`, `AuthContext.tsx`, `ProtectedRoute.tsx`, `AppShell.tsx`, `AppSidebar.tsx`, `AppHeader.tsx`, `App.tsx`, `src/types/auth.ts`, `src/types/api.ts`, `pages/login/`
**Backend:** JWT middleware, CORS policy, Auth API endpoints
**Pages:** Login, AppShell (empty dashboard placeholder)
**Deps:** shadcn/ui init, Tailwind, react-router, axios, react-query, signalr

### Phase 2 — Products & Categories
**Pages:** Products (List, Create, Edit, Detail), Categories-Products (List, Create, Edit)
**Files:** `src/api/products.ts`, `src/api/categories.ts`, `src/types/product.ts`, `src/types/category.ts`

### Phase 3 — Orders, Customers & Contacts
**Pages:** Orders (List, Detail), Customers (List, Edit), Contacts (List, Detail)
**Files:** `src/api/orders.ts`, `src/api/customers.ts`, `src/api/contacts.ts`, `src/types/order.ts`, `src/types/customer.ts`, `src/types/contact.ts`

### Phase 4 — Dashboard & Analytics
**Pages:** Dashboard with revenue charts, order stats, inventory distribution
**Files:** `src/api/dashboard.ts`, `src/types/dashboard.ts`

### Phase 5 — Content Management
**Pages:** Posts (List, Create, Edit, Detail), Blog Categories (List), Pages (List, Create, Edit), Layout UI, Bulk Import
**Files:** `src/api/posts.ts`, `src/api/pages.ts`, `src/api/layout.ts`, `src/api/import.ts`, `src/types/post.ts`, `src/types/page.ts`

### Phase 6 — Marketing
**Pages:** Promotions, Coupons, FlashSales, Advertisements (List, Create, Edit each)
**Files:** `src/api/promotions.ts`, `src/api/coupons.ts`, `src/api/flashsales.ts`, `src/api/advertisements.ts`

### Phase 7 — System & Settings
**Pages:** Users (List, Create, Edit), Settings, Notifications
**Files:** `src/api/users.ts`, `src/api/settings.ts`, `src/api/notifications.ts`

---

## 6. Key Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| UI Library | shadcn/ui + Tailwind CSS | Lightweight, accessible, matches project style |
| Auth | JWT Bearer + Refresh Token | Decoupled architecture, no cookie cross-origin issues |
| Refresh Token Storage | localStorage | Cross-origin compatible, controlled by client |
| Data Fetching | @tanstack/react-query | Caching, dedup, background refetch |
| State Management | React Context (auth) + React Query (server) | Minimal global state, server cache via query client |
| Routing | react-router-dom v7 | Standard, nested layouts |
| Realtime | SignalR with accessTokenFactory | Reuse existing hub, JWT auth for WebSocket |
| Routes | No /admin prefix | Already at admin subdomain |
| API Base URL | Configurable via `VITE_API_URL` env var | Dev → `http://localhost:5000`, Prod → `https://backend.flower.com` |

---

## 7. Migration Path & Risks

### 7.1 Cutover Strategy
1. Develop SPA alongside existing MVC views
2. Deploy SPA to Vercel staging URL for testing
3. When ready, flip DNS record → admin subdomain points to Vercel
4. Old MVC admin remains accessible via direct URL for rollback

### 7.2 Risks

| Risk | Mitigation |
|------|-----------|
| JWT token stolen from localStorage | Short expiry (15min), HTTPS only |
| CORS misconfiguration in production | Preflight testing in CI, staging env |
| SignalR connection failure | Automatic reconnect, graceful degradation |
| API response shape mismatch | TypeScript types mirror API DTOs, test coverage |
