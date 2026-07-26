# Auth Loop Fix & Cart Selection — Implementation Plan

> **For agentic workers:** Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix infinite login loop on checkout and add checkbox selection to cart.

**Architecture:** Fix JWT key resolution mismatch between token creation and validation on backend; skip unnecessary SessionValidationMiddleware for JWT API calls; preserve navigation state on frontend for login redirect; add cart checkbox selection as local UI state.

**Tech Stack:** ASP.NET Core 8, React 18, TypeScript, Axios

## Global Constraints

- JWT secret key must be resolved in same order: `JWT_SECRET_KEY` env var → `Jwt:SecretKey` config → throw
- SessionValidationMiddleware must only affect Cookie auth (admin), not JWT API calls
- Cart selection state is local UI only — not stored in CartContext
- Checkout page must accept optional selected items from navigate state (backward compatible)

---

### Task 1: Fix AuthController JWT key resolution

**Files:**
- Modify: `Flower.Backend/Controllers/Api/AuthController.cs:56-57` (Login)
- Modify: `Flower.Backend/Controllers/Api/AuthController.cs:145-146` (UpdateProfile)

**Interfaces:**
- Consumes: `IConfiguration`, `Environment.GetEnvironmentVariable`
- Produces: JWT tokens signed with key consistent with validation in Program.cs

- [ ] **Change `Login` method key resolution**

```csharp
// BEFORE (line 56-57):
var jwtKey = _configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

// AFTER:
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? _configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
```

- [ ] **Change `UpdateProfile` method key resolution**

```csharp
// BEFORE (line 145-146):
var jwtKey = _configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

// AFTER:
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? _configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
```

- [ ] **Build backend**

Run: `dotnet build Flower.Backend/Flower.Backend.csproj`
Expected: Build succeeded

- [ ] **Commit**

```bash
git add Flower.Backend/Controllers/Api/AuthController.cs
git commit -m "fix: align JWT secret key resolution with Program.cs priority (env var first)"
```

---

### Task 2: Skip SessionValidationMiddleware for JWT API requests

**Files:**
- Modify: `Flower.Backend/Middleware/SessionValidationMiddleware.cs:22-23`

**Interfaces:**
- Consumes: `HttpContext`
- Produces: Skip middleware for `/api` paths, allowing JWT to be validated by JwtBearerHandler alone

- [ ] **Add guard at top of `InvokeAsync`**

```csharp
// BEFORE (line 22-23):
public async Task InvokeAsync(HttpContext context, IAuthService authService, IMemoryCache cache, IApplicationDbContext dbContext)
{
    if (context.Request.Path.StartsWithSegments("/hubs"))

// AFTER:
public async Task InvokeAsync(HttpContext context, IAuthService authService, IMemoryCache cache, IApplicationDbContext dbContext)
{
    // Skip middleware for JWT API requests — only applies to Cookie auth (admin panel)
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        await _next(context);
        return;
    }

    if (context.Request.Path.StartsWithSegments("/hubs"))
```

- [ ] **Build backend**

Run: `dotnet build Flower.Backend/Flower.Backend.csproj`
Expected: Build succeeded

- [ ] **Commit**

```bash
git add Flower.Backend/Middleware/SessionValidationMiddleware.cs
git commit -m "fix: skip SessionValidationMiddleware for JWT API requests"
```

---

### Task 3: Preserve navigation state on frontend 401

**Files:**
- Modify: `Flower-shop.frontend/src/api/axiosClient.ts:28-30`
- Modify: `Flower-shop.frontend/src/pages/login/index.tsx:23-24`

**Interfaces:**
- Consumes: `sessionStorage`, `window.location`, `useNavigate`
- Produces: Redirect back to original page after login

- [ ] **Save current URL before 401 redirect in axiosClient**

```typescript
// BEFORE (line 28-30):
if (error.response?.status === 401) {
    tokenService.removeToken();
    authEvents.emit('unauthorized');
}

// AFTER:
if (error.response?.status === 401) {
    tokenService.removeToken();
    sessionStorage.setItem('redirectAfterLogin', window.location.pathname + window.location.search);
    authEvents.emit('unauthorized');
}
```

- [ ] **Read redirect URL after login in LoginPage**

```typescript
// BEFORE (line 23-24):
await login(data.username, data.password, data.rememberMe || false);
navigate('/');

// AFTER:
await login(data.username, data.password, data.rememberMe || false);
const redirectTo = sessionStorage.getItem('redirectAfterLogin');
sessionStorage.removeItem('redirectAfterLogin');
navigate(redirectTo || '/');
```

- [ ] **Build frontend**

Run: `npm run build` in `Flower-shop.frontend/`
Expected: Build succeeds, no TS errors

- [ ] **Commit**

```bash
git add Flower-shop.frontend/src/api/axiosClient.ts Flower-shop.frontend/src/pages/login/index.tsx
git commit -m "feat: preserve navigation state before 401 redirect back to login"
```

---

### Task 4: Add checkbox selection to CartTable

**Files:**
- Modify: `Flower-shop.frontend/src/pages/cart/CartTable.tsx`

**Interfaces:**
- Produces: `selectedIds`, `onSelectionChange`, `onDeleteSelected` props

- [ ] **Update CartTable props interface**

```typescript
interface CartTableProps {
  items: CartItem[];
  selectedIds: Set<number>;
  onUpdateQuantity: (id: number, qty: number) => void;
  onRemove: (id: number) => void;
  onSelectionChange: (ids: Set<number>) => void;
  onDeleteSelected: () => void;
}
```

- [ ] **Add checkbox column in grid header (desktop)**

```tsx
// Before the "Sản phẩm" column:
<div className="col-span-1 flex items-center justify-center">
  <input
    type="checkbox"
    checked={items.length > 0 && selectedIds.size === items.length}
    onChange={() => {
      if (selectedIds.size === items.length) {
        onSelectionChange(new Set());
      } else {
        onSelectionChange(new Set(items.map(i => i.id)));
      }
    }}
    className="w-4 h-4 text-primary rounded border-outline-variant focus:ring-primary cursor-pointer"
  />
</div>
```

- [ ] **Add checkbox per row**

```tsx
// Before the product image column in each item row:
<div className="col-span-1 flex items-center justify-center">
  <input
    type="checkbox"
    checked={selectedIds.has(item.id)}
    onChange={() => {
      const next = new Set(selectedIds);
      if (next.has(item.id)) {
        next.delete(item.id);
      } else {
        next.add(item.id);
      }
      onSelectionChange(next);
    }}
    className="w-4 h-4 text-primary rounded border-outline-variant focus:ring-primary cursor-pointer"
  />
</div>
```

- [ ] **Update grid column spans** (adjust from 12-column grid: checkbox takes 1, product takes 5 instead of 6)

- [ ] **Build frontend**

Run: `npm run build` in `Flower-shop.frontend/`
Expected: Build succeeds

- [ ] **Commit**

```bash
git add Flower-shop.frontend/src/pages/cart/CartTable.tsx
git commit -m "feat: add checkbox selection to CartTable"
```

---

### Task 5: Integrate checkbox with Cart page (checkout/delete)

**Files:**
- Modify: `Flower-shop.frontend/src/pages/cart/index.tsx`
- Modify: `Flower-shop.frontend/src/pages/checkout/index.tsx`

**Interfaces:**
- Consumes: `CartTable` props
- Produces: Selected-items-only checkout flow

- [ ] **Add selectedIds state and handlers to cart page**

```typescript
const [selectedIds, setSelectedIds] = useState<Set<number>>(new Set());

const handleCheckout = () => {
  if (selectedIds.size === 0) {
    toast.error('Vui lòng chọn sản phẩm để thanh toán');
    return;
  }
  const selectedItems = cartItems.filter(item => selectedIds.has(item.id));
  navigate('/checkout', { state: { selectedItems } });
};

const handleDeleteSelected = () => {
  selectedIds.forEach(id => removeFromCart(id));
  setSelectedIds(new Set());
};
```

- [ ] **Pass new props to CartTable**

```tsx
<CartTable
  items={cartItems}
  selectedIds={selectedIds}
  onUpdateQuantity={updateQuantity}
  onRemove={removeFromCart}
  onSelectionChange={setSelectedIds}
  onDeleteSelected={handleDeleteSelected}
/>
```

- [ ] **Update checkout button**

```tsx
<button
  className="w-full bg-primary text-on-primary py-4 rounded-lg font-label-md text-label-md interactive-lift hover:opacity-90 transition-all flex items-center justify-center space-x-2 group border-0 cursor-pointer"
  onClick={handleCheckout}
>
  <span>TIẾN HÀNH THANH TOÁN ({selectedIds.size})</span>
  <span className="material-symbols-outlined group-hover:translate-x-1 transition-transform">arrow_forward</span>
</button>
```

- [ ] **Add Delete Selected button**

```tsx
{selectedIds.size > 0 && (
  <button
    onClick={handleDeleteSelected}
    className="text-error font-label-sm flex items-center gap-1 bg-transparent border-0 cursor-pointer hover:underline"
  >
    <span className="material-symbols-outlined text-[16px]">delete</span>
    Xoá đã chọn ({selectedIds.size})
  </button>
)}
```

- [ ] **Read selected items in checkout page**

```typescript
// At the top of CheckoutPage component, after useCart:
const location = useLocation();
const { state } = location;
const checkoutItems = state?.selectedItems || cartItems;
```

Replace `cartItems` references with `checkoutItems` where appropriate (or filter cartItems at submission time).

- [ ] **Build frontend**

Run: `npm run build` in `Flower-shop.frontend/`
Expected: Build succeeds

- [ ] **Commit**

```bash
git add Flower-shop.frontend/src/pages/cart/index.tsx Flower-shop.frontend/src/pages/checkout/index.tsx
git commit -m "feat: integrate cart selection with checkout flow"
```
