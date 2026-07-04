# Phase 1: Security & Authentication — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add JWT authentication to the API layer, fix frontend auth flow, and resolve critical security vulnerabilities.

**Architecture:** Dual auth (Cookie for MVC admin + JWT for React SPA). Policy-based authorization. Frontend AuthContext with token management.

**Tech Stack:** ASP.NET Core 8, React 18, Axios, JWT Bearer, Cookie Auth

---

### Task 1: Backend — JWT Authentication Setup

**Files:**
- Modify: `CMS.Backend/Program.cs`
- Create: `CMS.Backend/Models/ApiErrorResponse.cs`
- Modify: `CMS.Backend/appsettings.json`

- [ ] **Step 1: Install JWT NuGet package**

```bash
cd CMS.Backend
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

- [ ] **Step 2: Add JWT config to appsettings.json**

```json
{
  "Jwt": {
    "Issuer": "AnhCMS",
    "Audience": "AnhCMS.SPA",
    "SecretKey": "AnhCMS-SuperSecret-Key-256bit-Minimum-Length-Required!!",
    "ExpiryMinutes": 60
  }
}
```

- [ ] **Step 3: Create ApiErrorResponse model**

```csharp
// CMS.Backend/Models/ApiErrorResponse.cs
namespace CMS.Backend.Models;

public class ApiErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
}
```

- [ ] **Step 4: Configure JWT + Global Exception Middleware in Program.cs**

```csharp
// After existing using statements, add:
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CMS.Backend.Models;
using System.Text.Json;

// Inside builder.Services block, after AddCookie:

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin", "Administrator"));
    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("Admin", "Administrator", "Editor"));
});

// Before app.Run(), add global exception middleware for API:
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var error = new ApiErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An internal error occurred"
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    }
    else
    {
        await next();
    }
});
```

- [ ] **Step 5: Build and verify no errors**

```bash
dotnet build
```

Expected: Build succeeds with no errors

- [ ] **Step 6: Commit**

```bash
git add CMS.Backend/
git add -A
git commit -m "feat: add JWT authentication and global exception middleware"
```

---

### Task 2: Backend — AuthController Returns JWT on Login

**Files:**
- Modify: `CMS.Backend/Controllers/Api/AuthController.cs`

- [ ] **Step 1: Inject IConfiguration and add JWT generation**

Add using at top:
```csharp
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
```

Add field and update constructor:
```csharp
private readonly IAuthService _authService;
private readonly IConfiguration _configuration;

public AuthController(IAuthService authService, IConfiguration configuration)
{
    _authService = authService;
    _configuration = configuration;
}
```

Replace the Login method body after the cookie sign-in block (keep the cookie auth, add JWT generation after it):
```csharp
// After: await HttpContext.SignInAsync(...)

// Generate JWT
var jwtKey = _configuration["Jwt:SecretKey"]!;
var issuer = _configuration["Jwt:Issuer"]!;
var audience = _configuration["Jwt:Audience"]!;
var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

var tokenHandler = new JwtSecurityTokenHandler();
var key = Encoding.UTF8.GetBytes(jwtKey);
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("FullName", user.FullName)
    }),
    Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
    Issuer = issuer,
    Audience = audience,
    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature)
};
var token = tokenHandler.CreateToken(tokenDescriptor);
var tokenString = tokenHandler.WriteToken(token);

return Ok(new
{
    token = tokenString,
    expiresAt = tokenDescriptor.Expires,
    username = user.Username,
    fullName = user.FullName,
    role = user.Role,
    message = "Đăng nhập thành công"
});
```

- [ ] **Step 2: Build and verify**

```bash
dotnet build
```

Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add CMS.Backend/Controllers/Api/AuthController.cs
git commit -m "feat: AuthController returns JWT token on login"
```

---

### Task 3: Backend — Remove AuthService.Login Entity Mutation

**Files:**
- Modify: `CMS.Backend/Services/AuthService.cs`

- [ ] **Step 1: Remove `user.PasswordHash = null` mutation**

In `Services/AuthService.cs`, line 56: delete line `user.PasswordHash = null;`

The method should return the user entity with its PasswordHash intact. The hash is never sent to the client — `AuthController.Login` builds the response object manually.

- [ ] **Step 2: Build and verify**

```bash
dotnet build
```

- [ ] **Step 3: Commit**

```bash
git add CMS.Backend/Services/AuthService.cs
git commit -m "fix: remove dangerous PasswordHash null mutation in AuthService.Login"
```

---

### Task 4: Backend — Authorization on All API Controllers

**Files:**
- Modify: `CMS.Backend/Controllers/Api/ProductsController.cs`
- Modify: `CMS.Backend/Controllers/Api/CategoriesController.cs`
- Modify: `CMS.Backend/Controllers/Api/PostsController.cs`
- Modify: `CMS.Backend/Controllers/Api/CategoriesProductsController.cs`
- Modify: `CMS.Backend/Controllers/Api/OrdersController.cs`
- Modify: `CMS.Backend/Controllers/Api/OrderDetailsController.cs`
- Modify: `CMS.Backend/Controllers/Api/CustomersController.cs`
- Modify: `CMS.Backend/Controllers/Api/UsersController.cs`
- Modify: `CMS.Backend/Controllers/Api/AuthController.cs`

- [ ] **Step 1: Add `[AllowAnonymous]` to AuthController class**

In `AuthController.cs`, add attribute above class:
```csharp
[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
```

- [ ] **Step 2: Add `[Authorize(Policy = "AdminOnly")]` to UsersController class**

```csharp
[Authorize(Policy = "AdminOnly")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
```

- [ ] **Step 3: CustomersController — `[Authorize(Policy = "AdminOnly")]` on class**

- [ ] **Step 4: ProductsController — `[AllowAnonymous]` on class for GET, `[Authorize(Policy = "StaffOnly")]` on write methods**

Since `[AllowAnonymous]` on class allows everything by default, use this pattern:

```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "StaffOnly")]
public class ProductsController : ControllerBase
```

Then add `[AllowAnonymous]` on the GET endpoints:
```csharp
[AllowAnonymous]
[HttpGet]
public async Task<IActionResult> GetAll() { ... }

[AllowAnonymous]
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id) { ... }
```

- [ ] **Step 5: Same pattern for CategoriesController, PostsController, CategoriesProductsController**

```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "StaffOnly")]
public class CategoriesController : ControllerBase
```

Add `[AllowAnonymous]` on `[HttpGet]` and `[HttpGet("{id}")]` methods.

Same for `PostsController` and `CategoriesProductsController`.

- [ ] **Step 6: OrdersController — `[Authorize]` on class**

```csharp
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
```

Same for `OrderDetailsController`.

- [ ] **Step 7: Build and verify**

```bash
dotnet build
```

Expected: Build succeeds with no errors

- [ ] **Step 8: Commit**

```bash
git add CMS.Backend/Controllers/Api/
git commit -m "feat: add policy-based authorization to all API controllers"
```

---

### Task 5: Backend — Add Missing `[Authorize]` on MVC Controllers

**Files:**
- Modify: `CMS.Backend/Controllers/CategoryProductController.cs`
- Modify: `CMS.Backend/Controllers/CustomerController.cs`
- Modify: `CMS.Backend/Controllers/ProductController.cs`

- [ ] **Step 1: Add `[Authorize]` to CategoryProductController class**

```csharp
[Authorize]
public class CategoryProductController : Controller
```

- [ ] **Step 2: Add `[Authorize]` to CustomerController class**

```csharp
[Authorize]
public class CustomerController : Controller
```

- [ ] **Step 3: Add `[Authorize]` to ProductController class**

```csharp
[Authorize]
public class ProductController : Controller
```

- [ ] **Step 4: Build and verify**

```bash
dotnet build
```

- [ ] **Step 5: Commit**

```bash
git add CMS.Backend/Controllers/CategoryProductController.cs CMS.Backend/Controllers/CustomerController.cs CMS.Backend/Controllers/ProductController.cs
git commit -m "fix: add missing [Authorize] to MVC CategoryProduct, Customer, Product controllers"
```

---

### Task 6: Backend — Rename Customer.Password to PasswordHash

**Files:**
- Modify: `CMS.Data/Entities/Customer.cs`
- Modify: `CMS.Backend/Services/CustomerService.cs`
- Modify: `CMS.Backend/Models/DTOs/MappingExtensions.cs`
- Modify: `CMS.Backend/Models/DTOs/CustomerDTOs.cs`

- [ ] **Step 1: Update Customer entity**

```csharp
// CMS.Data/Entities/Customer.cs
// Rename property:
public string PasswordHash { get; set; } = string.Empty; // Lưu mật khẩu đã hash (PBKDF2)
```

- [ ] **Step 2: Update CustomerService.cs:39 reference**

Change `customer.Password` to `customer.PasswordHash`:
```csharp
// Line ~39
customer.PasswordHash = _passwordHasher.HashPassword(customer, register.Password);
```

- [ ] **Step 3: Update MappingExtensions.cs:158 reference**

Change `Password` to `PasswordHash` in the Customer mapping.

- [ ] **Step 4: Update CustomerDTOs.cs Password references**

In `Models/DTOs/CustomerDTOs.cs`, rename both occurrences:
- Line 18: `public string Password { get; set; }` → `public string PasswordHash { get; set; }`
- Line 28: `public string? Password { get; set; }` → `public string? PasswordHash { get; set; }`

- [ ] **Step 5: Build and verify**

```bash
dotnet build
```

Expected: No errors

- [ ] **Step 6: Commit**

```bash
git add CMS.Data/Entities/Customer.cs CMS.Backend/Services/CustomerService.cs CMS.Backend/Models/DTOs/MappingExtensions.cs CMS.Backend/Models/DTOs/CustomerDTOs.cs
git commit -m "refactor: rename Customer.Password to PasswordHash for consistency"
```

---

### Task 7: Backend — OrderController Empty Items Clarification

**Files:**
- Modify: `CMS.Backend/Controllers/OrderController.cs`

Note: `OrderService.CreateOrder` already handles empty items (`if (items != null && items.Count > 0)` at line 58). The code works correctly — this task just adds a comment for clarity.

- [ ] **Step 1: Add comment clarifying admin use case**

In `Controllers/OrderController.cs` line 45-46:
```csharp
// Admin creates order shell — items added later via OrderDetails admin
var (success, message, orderId) = await _orderService.CreateOrder(
    model.CustomerId, model.Notes, new List<OrderItemInput>());
```

- [ ] **Step 2: Build and verify**

```bash
dotnet build
```

- [ ] **Step 3: Commit**

```bash
git add CMS.Backend/Controllers/OrderController.cs
git commit -m "docs: clarify intentional empty items in OrderController.Create"
```

---

### Task 8: Frontend — AuthContext + Token Service

**Files:**
- Create: `cms.frontend/src/context/AuthContext.js`
- Create: `cms.frontend/src/services/tokenService.js`
- Modify: `cms.frontend/src/services/authService.js`

- [ ] **Step 1: Create tokenService.js**

```javascript
// src/services/tokenService.js
const TOKEN_KEY = 'anhcms_token';

const tokenService = {
    getToken: () => localStorage.getItem(TOKEN_KEY),
    setToken: (token) => localStorage.setItem(TOKEN_KEY, token),
    removeToken: () => localStorage.removeItem(TOKEN_KEY),
    hasToken: () => !!localStorage.getItem(TOKEN_KEY),
};

export default tokenService;
```

- [ ] **Step 2: Update authService.js to return token**

No change needed — `authService.login` already returns `response.data || response`, and the backend now returns `{ token, expiresAt, username, fullName, role }`. The response shape is already correct.

- [ ] **Step 3: Create AuthContext.js**

```javascript
// src/context/AuthContext.js
import React, { createContext, useState, useContext, useEffect, useCallback } from 'react';
import authService from '../services/authService';
import tokenService from '../services/tokenService';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = tokenService.getToken();
        if (token) {
            try {
                const payload = JSON.parse(atob(token.split('.')[1]));
                setUser({
                    username: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload.unique_name,
                    fullName: payload.FullName,
                    role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role,
                });
            } catch {
                tokenService.removeToken();
            }
        }
        setLoading(false);
    }, []);

    const login = useCallback(async (username, password) => {
        const response = await authService.login(username, password);
        if (response.token) {
            tokenService.setToken(response.token);
            setUser({
                username: response.username,
                fullName: response.fullName,
                role: response.role,
            });
        }
        return response;
    }, []);

    const logout = useCallback(() => {
        tokenService.removeToken();
        setUser(null);
    }, []);

    const isAuthenticated = !!user;

    return (
        <AuthContext.Provider value={{ user, login, logout, isAuthenticated, loading }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth must be used within AuthProvider');
    return context;
};
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/context/AuthContext.js cms.frontend/src/services/tokenService.js
git commit -m "feat: add AuthContext and token service for frontend auth"
```

---

### Task 9: Frontend — Axios Interceptor + AuthProvider in App

**Files:**
- Modify: `cms.frontend/src/api/axiosClient.js`
- Modify: `cms.frontend/src/App.jsx`

- [ ] **Step 1: Add request interceptor to axiosClient**

```javascript
// src/api/axiosClient.js
import axios from 'axios';
import tokenService from '../services/tokenService';

const axiosClient = axios.create({
    baseURL: process.env.REACT_APP_API_URL ? `${process.env.REACT_APP_API_URL}/api` : 'https://localhost:7224/api',
    headers: {
        'Content-Type': 'application/json',
    },
    timeout: 10000,
});

// Request interceptor — attach token
axiosClient.interceptors.request.use(
    (config) => {
        const token = tokenService.getToken();
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

axiosClient.interceptors.response.use(
    (response) => response.data,
    (error) => {
        if (error.response?.status === 401) {
            tokenService.removeToken();
            window.location.href = '/login';
        }
        console.error('API Error:', error.response || error.message);
        return Promise.reject(error);
    }
);

export default axiosClient;
```

- [ ] **Step 2: Wrap App with AuthProvider**

```javascript
// src/App.jsx
// At the top, add import:
import { AuthProvider } from './context/AuthContext';

// In the JSX, wrap CartProvider with AuthProvider:
function App() {
    return (
        <AuthProvider>
            <CartProvider>
                <Router>
                    {/* ... rest unchanged ... */}
                </Router>
            </CartProvider>
        </AuthProvider>
    );
}
```

- [ ] **Step 3: Verify frontend builds**

```bash
cd cms.frontend
npm run build 2>&1 | tail -20
```

If `npm run build` fails, try `npx react-scripts build`.

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/api/axiosClient.js cms.frontend/src/App.jsx
git commit -m "feat: add Bearer token interceptor and AuthProvider to frontend"
```

---

### Task 10: Frontend — ProtectedRoute + Header Auth State

**Files:**
- Create: `cms.frontend/src/components/ProtectedRoute.jsx`
- Modify: `cms.frontend/src/components/Header.jsx`
- Modify: `cms.frontend/src/App.jsx`

- [ ] **Step 1: Create ProtectedRoute component**

```javascript
// src/components/ProtectedRoute.jsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const ProtectedRoute = ({ children }) => {
    const { isAuthenticated, loading } = useAuth();

    if (loading) {
        return (
            <div className="d-flex justify-content-center align-items-center min-vh-100">
                <div className="spinner-border text-dark" role="status" />
            </div>
        );
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    return children;
};

export default ProtectedRoute;
```

- [ ] **Step 2: Update Header to show user state**

In `Header.jsx`, the login button is a person icon in the right-side div (line 21). Replace it with conditional auth state:

```javascript
// Add import at top:
import { useAuth } from '../context/AuthContext';

// Inside component, add:
const { user, isAuthenticated, logout } = useAuth();

// Replace line 21 (login icon link) with:
{isAuthenticated ? (
    <div className="flex items-center gap-2">
        <span className="text-xs uppercase tracking-wider hidden md:inline">{user?.fullName || user?.username}</span>
        <button onClick={logout} className="cursor-pointer transition-transform duration-200 active:scale-95 hover:bg-surface-container-low dark:hover:bg-tertiary-container transition-all duration-300 p-2 bg-transparent border-0 text-primary">
            <span className="material-symbols-outlined">logout</span>
        </button>
    </div>
) : (
    <Link className="cursor-pointer transition-transform duration-200 active:scale-95 hover:bg-surface-container-low dark:hover:bg-tertiary-container transition-all duration-300 p-2 text-primary" to="/login">
        <span className="material-symbols-outlined">person</span>
    </Link>
)}

- [ ] **Step 3: Add ProtectedRoute around Checkout in App.jsx**

```javascript
// src/App.jsx
// At top, add import:
import ProtectedRoute from './components/ProtectedRoute';

// In Routes, wrap Checkout:
<Route path="/checkout" element={
    <ProtectedRoute>
        <Checkout />
    </ProtectedRoute>
} />
```

- [ ] **Step 4: Verify frontend builds**

```bash
cd cms.frontend
npm run build 2>&1 | tail -20
```

- [ ] **Step 5: Commit**

```bash
git add cms.frontend/src/components/ProtectedRoute.jsx cms.frontend/src/App.jsx
git add cms.frontend/src/components/Header.jsx
git commit -m "feat: add ProtectedRoute, Header auth state, protect /checkout"
```

---

### Task 11: Frontend — Critical Bug Fixes (CartTable Port + Material Symbols)

**Files:**
- Modify: `cms.frontend/src/pages/cart/CartTable.jsx`
- Modify: `cms.frontend/public/index.html`

- [ ] **Step 1: Fix CartTable image port**

In `src/pages/cart/CartTable.jsx`, line 4:
```javascript
// Change from:
const IMAGE_BASE_URL = 'https://localhost:7111';
// To:
const IMAGE_BASE_URL = 'https://localhost:7224';
```

- [ ] **Step 2: Add Material Symbols font to index.html**

In `public/index.html`, inside `<head>`:
```html
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" />
```

- [ ] **Step 3: Verify**

```bash
cd cms.frontend
npm run build 2>&1 | tail -10
```

Expected: Build succeeds

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/pages/cart/CartTable.jsx cms.frontend/public/index.html
git commit -m "fix: correct CartTable image port and add Material Symbols font"
```

---

### Task 12: Full Build Verification

- [ ] **Step 1: Build backend**

```bash
cd CMS.Backend
dotnet build
```

Expected: Build succeeded

- [ ] **Step 2: Build frontend**

```bash
cd cms.frontend
npm run build 2>&1 | tail -20
```

Expected: The build is successful

- [ ] **Step 3: Run backend tests**

```bash
dotnet test CMS.Tests
```

Expected: All tests pass

- [ ] **Step 4: Commit remaining changes**

```bash
git add -A
git status
git commit -m "chore: final verification and cleanup for Phase 1 security"
```
