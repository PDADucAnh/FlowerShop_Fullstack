# Phase 1: Security & Authentication — Enterprise Design

## Overview

Add proper authentication and authorization to the AnhCMS API layer (JWT Bearer for React SPA), fix critical frontend auth flow, and resolve security vulnerabilities across the stack.

---

## 1. Authentication Architecture

### Dual Auth Scheme (Cookie + JWT)

| Scheme | Audience | Mechanism | Status |
|--------|----------|-----------|--------|
| Cookie Auth | MVC Admin pages | `HttpContext.SignInAsync` | Existing, keep as-is |
| JWT Bearer | React SPA API calls | `Authorization: Bearer <token>` | New |

Both schemes coexist in the same pipeline. Controllers choose via `[Authorize]` attribute.

### JWT Configuration

```json
{
  "Jwt": {
    "Issuer": "AnhCMS",
    "Audience": "AnhCMS.SPA",
    "SecretKey": "<256-bit key from User Secrets>",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

- Key stored in `secrets.json` (dev) / environment variable (prod), never in `appsettings.json`
- Algorithm: HMAC-SHA256
- Claims: `sub` (username), `role`, `fullName`, `iat`, `exp`

### AuthController Changes

**Login endpoint** — now returns both cookie (for admin) and JWT (for SPA):
```json
{
  "token": "<jwt>",
  "refreshToken": "<guid>",
  "expiresAt": "<ISO8601>",
  "username": "...",
  "fullName": "...",
  "role": "..."
}
```

**Register endpoint** — unchanged, but now returns JWT on success so user is immediately authenticated.

---

## 2. Authorization Policy Design

| Policy Name | Roles | Applied To |
|-------------|-------|------------|
| `AdminOnly` | Admin, Administrator | UsersController, CustomersController |
| `StaffOnly` | Admin, Administrator, Editor | POST/PUT/DELETE on Products, Categories, Posts |
| `Authenticated` | Any authenticated user | OrdersController, OrderDetailsController |
| `Public` | Anonymous | GET Products, Categories, Posts |

### API Controller Authorization Matrix

| Controller | Read (GET) | Write (POST/PUT/DELETE) |
|------------|-----------|------------------------|
| `ProductsController` | `[AllowAnonymous]` | `[Authorize(Policy = "StaffOnly")]` |
| `CategoriesController` | `[AllowAnonymous]` | `[Authorize(Policy = "StaffOnly")]` |
| `PostsController` | `[AllowAnonymous]` | `[Authorize(Policy = "StaffOnly")]` |
| `CategoriesProductsController` | `[AllowAnonymous]` | `[Authorize(Policy = "StaffOnly")]` |
| `OrdersController` | `[Authorize]` | `[Authorize]` |
| `OrderDetailsController` | `[Authorize]` | `[Authorize]` |
| `CustomersController` | `[Authorize(Policy = "AdminOnly")]` | `[Authorize(Policy = "AdminOnly")]` |
| `UsersController` | `[Authorize(Policy = "AdminOnly")]` | `[Authorize(Policy = "AdminOnly")]` |
| `AuthController` | — | `[AllowAnonymous]` |

Missing MVC controller auth will also be fixed:
| Controller | Fix |
|------------|-----|
| `CategoryProductController` | Add `[Authorize]` |
| `CustomerController` | Add `[Authorize]` |
| `ProductController` | Add `[Authorize]` |

---

## 3. Frontend Auth Flow

### AuthContext + useAuth Hook

```
src/
├── context/
│   └── AuthContext.js        # AuthProvider, useAuth hook
├── services/
│   ├── authService.js        # Updated: returns token
│   └── tokenService.js       # NEW: localStorage get/set/clear
├── api/
│   └── axiosClient.js        # Updated: Bearer interceptor
├── components/
│   ├── ProtectedRoute.jsx     # NEW: redirect to /login if not auth
│   └── Header.jsx            # Updated: show user name / logout
```

**Token storage:**
- Access token: in-memory (AuthProvider state) + localStorage fallback
- Refresh token: localStorage only
- On app load: hydrate from localStorage

**Axios interceptor flow:**
```
Request → Attach Authorization header
Response 401 → Try refresh token → Retry original request → Fail → Logout
```

### Protected Routes

| Route | Access |
|-------|--------|
| `/` (home) | Public |
| `/shop` | Public |
| `/product/:id` | Public |
| `/blog` | Public |
| `/blog/:id` | Public |
| `/cart` | Public |
| `/checkout` | Authenticated only |
| `/login` | Redirect to / if already auth |
| `/register` | Redirect to / if already auth |

---

## 4. Critical Bug Fixes

### 4.1 CartTable Image Port
- File: `src/pages/cart/CartTable.jsx` line 4
- Fix: Change `IMAGE_BASE_URL` from `https://localhost:7111` to `https://localhost:7224`

### 4.2 Material Symbols Font
- File: `public/index.html`
- Fix: Add `<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" />`

### 4.3 OrderController Empty Items Bug
- File: `Controllers/OrderController.cs` line 46
- Problem: `CreateOrder` POST always passes `new List<OrderItemInput>()` — orders created via MVC admin have zero items
- Fix: The MVC admin order form doesn't have an item selector, so allow order creation with zero items. Add a comment clarifying this is intentional for admin use. Backend `OrderService.CreateOrder` already validates items (checks products exist) — add a guard to skip item validation when items list is empty.

### 4.4 Customer PasswordHash Rename
- File: `CMS.Data/Entities/Customer.cs`
- Fix: Rename `Password` property → `PasswordHash` to match `User` entity convention
- Update `CustomerService`, `CustomerDTOs`, `MappingExtensions` references
- Keep the `[Column("Password")]` attribute (or add migration) so DB column name stays consistent

### 4.5 AuthService.Login — Remove Entity Mutation
- File: `Services/AuthService.cs` line 56
- Problem: `user.PasswordHash = null` mutates a tracked EF Core entity, risking state corruption
- Fix: Remove the `user.PasswordHash = null` line. The entity should never be mutated for presentation purposes. Instead, the `AuthController.Login` endpoint will construct the response object manually (it already does this with `new { username, fullName, role }`), so no entity data is leaked to the client regardless.

---

## 5. Infrastructure

### 5.1 Global Exception Middleware
Add structured JSON error responses for API:
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Username is required",
    "details": null
  }
}
```

### 5.2 Error Response Model
```csharp
public class ApiErrorResponse
{
    public string Code { get; set; }
    public string Message { get; set; }
    public object? Details { get; set; }
}
```

### 5.3 CORS (unchanged)
Keep existing CORS policy for localhost:3000. No changes needed.

---

## Scope

### In Scope
- JWT auth backend (Program.cs, AuthController, appsettings)
- AuthContext + token management (frontend)
- Axios interceptor for Bearer token
- Policy-based authorization on all API controllers
- `[Authorize]` on missing MVC controllers
- CartTable port fix
- Material Symbols font import
- Customer.Password → PasswordHash rename
- AuthService.Login side-effect removal
- Protected routes for checkout
- Global exception middleware

### Out of Scope (deferred to later phases)
- Pagination
- Generic base controller/service pattern
- Validation attributes on all DTOs
- Image upload service
- Rate limiting
- CRA → Vite migration
- Frontend error states
- Remaining architecture improvements
