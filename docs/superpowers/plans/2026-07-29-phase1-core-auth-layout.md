# Phase 1: Core Setup, Auth & Layout — Implementation Plan

**Goal:** Set up the standalone React admin SPA (Vite + shadcn/ui + Tailwind) with JWT auth and AppShell layout, plus backend JWT/CORS/auth endpoints.

**Architecture:** Standalone SPA (Vercel) + ASP.NET Core Web API (Render). JWT Bearer auth with refresh token rotation. Axios interceptor with refresh queue pattern. shadcn/ui library.

**Tech Stack:** React 19, TypeScript 6, Vite 8, Tailwind CSS, shadcn/ui, react-router-dom v7, axios, @tanstack/react-query, @microsoft/signalr (backend: ASP.NET Core 8, JWT Bearer, CORS)

## Global Constraints

- All UI text in Vietnamese (matching existing Razor views)
- API base URL configurable via `VITE_API_URL` env var
- JWT expiry: 60 min (existing config)
- Refresh token expiry: 30 days (existing config)
- CORS allowed: `http://localhost:5173`, `http://localhost:5174`, `https://flower-admin.vercel.app`, `https://flower-admin-*.vercel.app`
- Routes do NOT use `/admin` prefix (app is at admin subdomain)
- Refresh token returned in response body (stored in client localStorage)

---

## File Structure

### Backend (Flower.Backend)

| File | Action | Purpose |
|------|--------|---------|
| `Program.cs` | Modify | Add admin Vercel domain + localhost:5174 to CORS origins |
| `Controllers/Api/AuthController.cs` | Modify | Add refresh/logout/me endpoints, update login to return refreshToken |
| `Models/DTOs/AuthResult.cs` | Modify | Add `LoginResponseDTO` |
| `Models/DTOs/AuthDTOs.cs` | Modify | Add `RefreshTokenRequest` |

### Frontend (flower-admin.frontend)

| File | Action | Purpose |
|------|--------|---------|
| `package.json` | Modify | Add deps |
| `vite.config.ts` | Modify | Add Tailwind plugin, proxy |
| `tailwind.config.ts` | Create | Tailwind config |
| `src/index.css` | Modify | Tailwind directives + CSS vars |
| `src/lib/utils.ts` | Create | `cn()` helper |
| `components.json` | Create | shadcn/ui config |
| `src/types/api.ts` | Create | `ApiResponse<T>`, `Pagination` |
| `src/types/auth.ts` | Create | `User`, `LoginRequest`, `LoginResponse` |
| `src/api/client.ts` | Create | Axios instance + interceptors |
| `src/api/auth.ts` | Create | `login()`, `logout()`, `refreshToken()` |
| `src/context/AuthContext.tsx` | Create | AuthProvider + useAuth |
| `src/components/shared/ProtectedRoute.tsx` | Create | Route guard |
| `src/components/layout/AppShell.tsx` | Create | Sidebar + Header + Outlet |
| `src/components/layout/AppSidebar.tsx` | Create | Navigation menu |
| `src/components/layout/AppHeader.tsx` | Create | Header with user menu |
| `src/pages/login/index.tsx` | Create | Login page |
| `src/pages/dashboard/index.tsx` | Create | Placeholder dashboard |
| `src/App.tsx` | Modify | Router setup |
| `src/main.tsx` | Modify | Add QueryClientProvider + AuthProvider |

---

## Task 1: Backend — Update CORS & Auth Endpoints

**Files:**
- Modify: `Flower.Backend/Program.cs`
- Modify: `Flower.Backend/Controllers/Api/AuthController.cs`
- Modify: `Flower.Backend/Models/DTOs/AuthResult.cs`
- Modify: `Flower.Backend/Models/DTOs/AuthDTOs.cs`

**Interfaces:**
- Consumes: Existing `IAuthService.CreateRefreshTokenAsync`, `ValidateRefreshTokenAsync`, `RevokeTokenAsync`, `Login`
- Produces: `POST /api/auth/login` (now returns refreshToken), `POST /api/auth/refresh`, `POST /api/auth/logout`, `GET /api/auth/me`

- [ ] **Step 1: Add admin origins to CORS policy**

Edit `Program.cs` — add `"http://localhost:5174"` and `"https://flower-admin.vercel.app"` to the existing `AllowVercel` policy origins array.

- [ ] **Step 2: Add LoginResponseDTO to AuthResult.cs**

Append after existing `LoginResult` class:
```csharp
public class LoginResponseDTO
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string ExpiresAt { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = string.Empty;
}
```

- [ ] **Step 3: Add RefreshTokenRequest to AuthDTOs.cs**

Append after `ChangePasswordRequest`:
```csharp
public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
```

- [ ] **Step 4: Update login endpoint to return refreshToken**

In `AuthController.cs`, after creating the JWT token, add:
```csharp
var rawRefreshToken = await _authService.CreateRefreshTokenAsync(result.Id, "AdminSPA");
```
Change the response to return `accessToken`, `refreshToken`, `expiresAt`, and nested `user` object.

- [ ] **Step 5: Add refresh endpoint to AuthController**

```csharp
[AllowAnonymous]
[HttpPost("refresh")]
public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
{
    var userId = await _authService.ValidateRefreshTokenAsync(request.RefreshToken);
    if (userId == null)
        return Unauthorized(new { success = false, message = "Refresh token không hợp lệ hoặc đã hết hạn." });
    await _authService.RevokeTokenAsync(request.RefreshToken);
    var user = await _authService.GetProfile(userId.Value.ToString(), "User");
    if (user == null)
        return Unauthorized(new { success = false, message = "Người dùng không tồn tại." });
    // Generate new JWT + refresh token (same pattern as login)
    // Return { accessToken, refreshToken, expiresAt }
}
```

- [ ] **Step 6: Add logout endpoint to AuthController**

```csharp
[Authorize]
[HttpPost("logout")]
public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest? request)
{
    if (request != null && !string.IsNullOrEmpty(request.RefreshToken))
        await _authService.RevokeTokenAsync(request.RefreshToken);
    else {
        var userIdClaim = User.FindFirst("Id")?.Value;
        if (int.TryParse(userIdClaim, out var userId))
            await _authService.RevokeUserTokensAsync(userId);
    }
    return Ok(new { success = true, message = "Đã đăng xuất thành công." });
}
```

- [ ] **Step 7: Add /me endpoint to AuthController**

```csharp
[Authorize]
[HttpGet("me")]
public async Task<IActionResult> GetCurrentUser()
{
    var username = User.Identity?.Name;
    var authType = User.FindFirst("AuthType")?.Value ?? "User";
    if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = "Invalid token" });
    var result = await _authService.GetProfile(username, authType);
    if (result == null) return NotFound(new { message = "User not found" });
    return Ok(new { id = result.Id, username = result.Username, fullName = result.FullName,
        email = result.Email, phone = result.Phone, address = result.Address, role = result.Role, authType = result.AuthType });
}
```

- [ ] **Step 8: Build backend and verify**

```bash
cd Flower.Backend && dotnet build
```

---

## Task 2: Frontend — Init Tailwind, shadcn/ui & Install Dependencies

**Files:**
- Modify: `package.json`
- Create: `tailwind.config.ts`
- Modify: `vite.config.ts`
- Modify: `src/index.css`
- Create: `src/lib/utils.ts`
- Create: `components.json`

- [ ] **Step 1: Install tailwindcss**

```bash
cd flower-admin.frontend
npm install tailwindcss @tailwindcss/vite
```

- [ ] **Step 2: Init shadcn/ui**

```bash
npx shadcn@latest init -d --yes
```

- [ ] **Step 3: Install extra deps**

```bash
npm install react-router-dom axios @tanstack/react-query @microsoft/signalr lucide-react
npx shadcn@latest add button
```

- [ ] **Step 4: Add Tailwind to vite.config.ts**

```typescript
import { defineConfig } from 'vite'
import react, { reactCompilerPreset } from '@vitejs/plugin-react'
import babel from '@rolldown/plugin-babel'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [
    tailwindcss(),
    react(),
    babel({ presets: [reactCompilerPreset()] }),
  ],
  server: {
    port: 5173,
    proxy: { '/api': { target: 'http://localhost:5000', changeOrigin: true } },
  },
})
```

- [ ] **Step 5: Verify dev server starts**

```bash
npm run dev
```

---

## Task 3: Frontend — Type Definitions

**Files:**
- Create: `src/types/api.ts`
- Create: `src/types/auth.ts`

- [ ] **Step 1: Create src/types/api.ts**

```typescript
export interface ApiResponse<T> {
  success?: boolean;
  message?: string;
  data?: T;
}

export interface Pagination {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface PaginationParams {
  page?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
}
```

- [ ] **Step 2: Create src/types/auth.ts**

```typescript
export interface User {
  id: number;
  username: string;
  fullName: string;
  email: string | null;
  phone: string | null;
  address: string | null;
  role: string;
  authType?: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface AuthState {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}
```

---

## Task 4: Frontend — Axios Client with JWT Interceptors

**Files:**
- Create: `src/api/client.ts`
- Create: `src/api/auth.ts`

- [ ] **Step 1: Create src/api/client.ts**

Axios instance with:
- Base URL from `VITE_API_URL` env var (fallback `http://localhost:5000`)
- Request interceptor: attach `Authorization: Bearer <token>` from localStorage
- Response interceptor: on 401, use refresh queue pattern (isRefreshing flag + failedQueue), retry original request after token refresh. On refresh failure, clear tokens and redirect to `/login`.

Define `CustomAxiosRequestConfig extends InternalAxiosRequestConfig` with `_retry?: boolean`.

- [ ] **Step 2: Create src/api/auth.ts**

```typescript
import apiClient from './client';
import type { LoginRequest, LoginResponse, RefreshTokenResponse, User } from '../types/auth';

export const authApi = {
  login: (data: LoginRequest) => apiClient.post<LoginResponse>('/api/auth/login', data).then(r => r.data),
  logout: (refreshToken: string) => apiClient.post('/api/auth/logout', { refreshToken }),
  refreshToken: (refreshToken: string) => apiClient.post<RefreshTokenResponse>('/api/auth/refresh', { refreshToken }).then(r => r.data),
  getMe: () => apiClient.get<User>('/api/auth/me').then(r => r.data),
};
```

---

## Task 5: Frontend — Auth Context

**Files:**
- Create: `src/context/AuthContext.tsx`

- [ ] **Step 1: Create AuthContext**

Provider with state:
- `user: User | null` — fetched from `/api/auth/me` on mount if token exists
- `isAuthenticated: boolean` — derived from user + token existence
- `isLoading: boolean` — true during initial token validation
- `login(data: LoginRequest)` — calls API, stores tokens in localStorage, sets user
- `logout()` — calls `/api/auth/logout`, clears tokens and user, redirects to `/login`

Use `createContext` + `useContext` pattern. Export `AuthProvider` and `useAuth`.

---

## Task 6: Frontend — ProtectedRoute & Layout Components

**Files:**
- Create: `src/components/shared/ProtectedRoute.tsx`
- Create: `src/components/layout/AppShell.tsx`
- Create: `src/components/layout/AppSidebar.tsx`
- Create: `src/components/layout/AppHeader.tsx`

- [ ] **Step 1: Create ProtectedRoute**

If `isLoading` → spinner. If not `isAuthenticated` → `<Navigate to="/login">`. Else → `<Outlet />`.

- [ ] **Step 2: Create AppShell**

Sidebar (left, fixed on desktop) + Header (top) + `<Outlet />` (main content area). Use flex layout: `flex h-screen`.

- [ ] **Step 3: Create AppSidebar**

Full sidebar menu from `_LayoutAdmin.cshtml` — all Vietnamese labels. Menu groups: Bảng điều khiển, Danh mục sản phẩm (Sản phẩm/Danh mục sản phẩm/Nhập hàng loạt), Bán hàng (Đơn hàng/Khách hàng/Liên hệ), Nội dung (Bài viết/Danh mục blog/Trang), Giao diện (Bố cục Trang), Tiếp thị (Khuyến mãi/Mã giảm giá/Flash Sale/Quảng cáo), Hệ thống (Thông báo/Cấu hình). Active state based on `useLocation()`. Mobile responsive with toggle.

Use `lucide-react` icons.

- [ ] **Step 4: Create AppHeader**

Show:
- Notification bell icon (placeholder)
- Settings icon (link to /settings)
- User avatar (initials from fullName) + fullName
- Logout button

---

## Task 7: Frontend — Login Page & Router

**Files:**
- Create: `src/pages/login/index.tsx`
- Create: `src/pages/dashboard/index.tsx`
- Modify: `src/App.tsx`
- Modify: `src/main.tsx`

- [ ] **Step 1: Create Login page**

Form with username + password inputs. On submit: call `useAuth().login()`. On success: navigate to `/`. Show error message if login fails. Branding with Flower2 icon + "FlowerShop Admin" header.

- [ ] **Step 2: Create placeholder Dashboard page**

Simple page: "Bảng điều khiển" title + "Dashboard sẽ được triển khai ở Phase 4."

- [ ] **Step 3: Wire up App.tsx**

```typescript
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { Toaster } from 'sonner';
import ProtectedRoute from './components/shared/ProtectedRoute';
import AppShell from './components/layout/AppShell';
import LoginPage from './pages/login';
import DashboardPage from './pages/dashboard';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Toaster position="top-right" />
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route element={<ProtectedRoute />}>
            <Route element={<AppShell />}>
              <Route path="/" element={<DashboardPage />} />
            </Route>
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}
export default App;
```

- [ ] **Step 4: Update main.tsx**

```typescript
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import App from './App';
import './index.css';

const queryClient = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <App />
    </QueryClientProvider>
  </StrictMode>,
);
```

- [ ] **Step 5: Install sonner for toasts**

```bash
npm install sonner
```

---

## Task 8: Verify Frontend Build

- [ ] **Step 1: Build and check for errors**

```bash
cd flower-admin.frontend && npm run build
```

Expected: Build succeeds, output in `dist/` folder.

- [ ] **Step 2: Dev server smoke test**

```bash
npm run dev
```

Expected: `http://localhost:5173` shows login page. The backend proxy forwards `/api/*` to `http://localhost:5000`.
