# FlowerShop — Full Project Audit Report

**Generated:** 2026-07-05
**Scope:** Backend (.NET), Frontend (React/TypeScript), Database (SQL Server)
**Total Findings:** 83 (Critical: 12, High: 14, Medium: 34, Low: 23)
**Resolved:** 60 (12 Critical ✅, 14 High ✅, 34 Medium ✅)
**Remaining:** 23 (23 Low)

---

## Architecture Overview

The FlowerShop project is a full-stack e-commerce application for a florist, consisting of:

- **Backend:** ASP.NET Core Web API (`Flower.Backend`) with Entity Framework Core, JWT authentication, payment integration
- **Frontend:** React + TypeScript SPA (`Flower-shop.frontend`) with React Router, React Query, Tailwind CSS
- **Database:** SQL Server with EF Core code-first migrations (`Flower.Data`)

---

## Backend Findings (29)

### F-BE-001 [CRITICAL] Payment Webhook Authorization

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Controllers\Api\PaymentController.cs:10,27` |
| **Category** | Controllers/Authentication |
| **Impact** | Payment webhooks never process. Orders stuck in PendingVerification |
| **Description** | `PaymentController` has `[Authorize]` at class level but the Webhook endpoint MUST be accessible without authentication. Payment gateways cannot pass auth headers. |
| **Suggested Fix** | Add `[AllowAnonymous]` to the Webhook action |

---

### F-BE-002 [CRITICAL] Fire-and-Forget Email Task

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderService.cs:326-337` |
| **Category** | Async/Concurrency |
| **Impact** | OTP emails not reliably sent. Risk of process crash |
| **Description** | `_ = Task.Run(async () => { send email })` — email runs outside request scope. When the HTTP request completes, the DI scope is disposed. Any scoped service access in the task throws `ObjectDisposedException`. |
| **Suggested Fix** | Use `IHostedService`/Channel pattern or capture scope explicitly |

---

### F-BE-003 [CRITICAL] Fire-and-Forget Email Task (AuthService)

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\AuthService.cs:249-259` |
| **Category** | Async/Concurrency |
| **Impact** | Password reset emails not reliably sent |
| **Description** | Same fire-and-forget `Task.Run` pattern as F-BE-002 for forgot-password email delivery. |
| **Suggested Fix** | Same as F-BE-002 — use `IHostedService`/Channel pattern |

---

### F-BE-004 [CRITICAL] Hardcoded OTP Backdoor

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\FraudDetectionService.cs:52-64` |
| **Category** | Security |
| **Impact** | Anyone knowing `000000` can confirm any COD order |
| **Description** | OTP `'000000'` is hardcoded as a master code that bypasses all verification. This is a deliberate backdoor in the fraud detection system. |
| **Suggested Fix** | Remove the hardcoded `'000000'` check |

---

### F-BE-005 [CRITICAL] JWT Secret Key in appsettings.json

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\appsettings.json:16` |
| **Category** | Secrets Management |
| **Impact** | Anyone with repo access can forge JWT tokens with any role |
| **Description** | JWT `SecretKey` hardcoded in `appsettings.json`. This file is committed to source control. |
| **Suggested Fix** | Move to environment variables or user secrets |

---

### F-BE-006 [CRITICAL] Transaction Nesting / Double Save

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderService.cs:210-218` |
| **Category** | Data Integrity |
| **Impact** | Delivery slots permanently consumed on failed orders |
| **Description** | `DeliverySlotService.TryLockSlot()` calls `SaveChangesAsync()` INSIDE the outer `CreateOrder` transaction. If the outer transaction rolls back, the inner save is already committed. |
| **Suggested Fix** | Delegate slot operations to the same `DbContext` under the same transaction |

---

### F-BE-007 [HIGH] File Upload Validation

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Controllers\ProductController.cs:60-73`, `PostController.cs`, `AdvertisementController.cs` |
| **Category** | Security |
| **Impact** | RCE via malicious file upload disguised as `.jpg` |
| **Description** | No file content-type validation (magic bytes) — only file extension is trusted. An attacker can upload a `.exe` renamed to `.jpg`. |
| **Suggested Fix** | Validate magic bytes with `SixLabors.ImageSharp`, whitelist extensions |

---

### F-BE-008 [HIGH] Cookie Security

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Controllers\AccountController.cs:54` |
| **Category** | Security |
| **Impact** | MITM token interception |
| **Description** | RefreshToken cookie has `Secure = false` — it can be sent over unencrypted HTTP connections. |
| **Suggested Fix** | Set `Secure = true`, `SameSite = Lax` |

---

### F-BE-009 [HIGH] Webhook Secret Hardcoded

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\appsettings.json:36` |
| **Category** | Secrets Management |
| **Impact** | Attackers can forge valid webhook signatures |
| **Description** | Webhook HMAC secret hardcoded with silent fallback. If config missing, it silently uses a default value. |
| **Suggested Fix** | Move to environment variables, throw if missing at startup |

---

### F-BE-010 [HIGH] Auto-Migration on Startup

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Program.cs:174` |
| **Category** | Operations |
| **Impact** | Destructive migrations auto-apply in production |
| **Description** | `context.Database.MigrateAsync()` runs on every startup. In production, this means any migration runs automatically on deployment. |
| **Suggested Fix** | Manual migration in production, auto only in development |

---

### F-BE-011 [HIGH] AllowedHosts Wildcard

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\appsettings.json:38` |
| **Category** | Security |
| **Impact** | Host header injection, cache poisoning, SSRF |
| **Description** | `AllowedHosts = '*'` in production configuration. This accepts requests with any Host header. |
| **Suggested Fix** | Restrict to specific domains |

---

### F-BE-012 [MEDIUM] Empty Order Validation

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderService.cs:144-149` |
| **Category** | Validation |
| **Impact** | Orders with zero items consume IDs/slots |
| **Description** | No validation that the order items list is not empty. |
| **Suggested Fix** | Add items null/empty check |

---

### F-BE-013 [MEDIUM] Missing Transaction in ProcessCODOrder

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderService.cs:557-597` |
| **Category** | Data Integrity |
| **Impact** | Partial save possible on failure |
| **Description** | `ProcessCODOrder` calls `SaveChangesAsync` without an explicit transaction. |
| **Suggested Fix** | Wrap in explicit transaction |

---

### F-BE-014 [MEDIUM] Code Duplication — Cancellation

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderService.cs:428-553` |
| **Category** | Maintainability |
| **Impact** | Inconsistencies between cancellation paths |
| **Description** | 3 cancellation methods each implement their own stock restoration logic. |
| **Suggested Fix** | Unify to a single private method |

---

### F-BE-015 [MEDIUM] Date Inconsistency

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderService.cs:186-232` |
| **Category** | Correctness |
| **Impact** | Off-by-one-day errors around midnight |
| **Description** | Delivery date validation uses Vietnam time while `OrderDate` uses `UtcNow`. |
| **Suggested Fix** | Normalize all to UTC |

---

### F-BE-016 [MEDIUM] Rate Limit TOCTOU

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\AuthService.cs:220-232` |
| **Category** | Async/Concurrency |
| **Impact** | Rate limiting bypassable under concurrent requests |
| **Description** | `ConcurrentDictionary` check-and-set is not atomic. Two concurrent requests can both pass the rate limit check. |
| **Suggested Fix** | Use `GetOrAdd` with `Lazy<T>` pattern |

---

### F-BE-017 [MEDIUM] Order Delete Authorization

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Controllers\Api\OrdersController.cs:117-125` |
| **Category** | Authorization |
| **Impact** | Customers can delete others' orders |
| **Description** | Delete action requires only `[Authorize]`, no admin/staff role check. |
| **Suggested Fix** | `[Authorize(Policy = 'StaffOnly')]` |

---

### F-BE-018 [MEDIUM] Duplicate Cancellation Service

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderService.cs:599-637` |
| **Category** | Architecture |
| **Impact** | Double cancellation or missed windows |
| **Description** | `AutoCancelUnverifiedOrders` in `OrderService` duplicates the logic in `OrderExpiryBackgroundService`. |
| **Suggested Fix** | Remove from `OrderService`, keep only `BackgroundService` |

---

### F-BE-019 [MEDIUM] Missing Required DTO Attributes

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Models\DTOs\OrderInputDTO.cs:9-18` |
| **Category** | Validation |
| **Impact** | Incomplete order submissions pass validation |
| **Description** | `OrderInputDTO` is missing `[Required]` on `CustomerId`, `Items`, and `DeliveryAddress`. |
| **Suggested Fix** | Add `[Required]` attributes |

---

### F-BE-020 [MEDIUM] SessionValidation DB Query Per Request

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Middleware\SessionValidationMiddleware.cs:39-50` |
| **Category** | Performance |
| **Impact** | Performance degradation under load |
| **Description** | DB query on EVERY authenticated request for token validation. |
| **Suggested Fix** | Cache token validation or skip for JWT API requests |

---

### F-BE-021 [MEDIUM] Missing Logging — CancellationService

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderCancellationService.cs:27-68` |
| **Category** | Observability |
| **Impact** | No audit trail for cancellations |
| **Description** | Zero logging anywhere in the cancellation service. |
| **Suggested Fix** | Inject `ILogger` and log cancel events |

---

### F-BE-022 [MEDIUM] Role Name Inconsistency

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Program.cs:160-166` |
| **Category** | Configuration |
| **Impact** | Admin users locked out of some endpoints |
| **Description** | `'Admin'` vs `'Administrator'` roles used inconsistently across the codebase. |
| **Suggested Fix** | Standardize on one role name |

---

### F-BE-023 [MEDIUM] Multiple SaveChanges

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\PaymentService.cs:89-134` |
| **Category** | Data Integrity |
| **Impact** | Duplicate entity tracking errors |
| **Description** | `RecordPayment` calls `SaveChangesAsync` then calls another method that calls `SaveChangesAsync` again. |
| **Suggested Fix** | Consolidate to a single `SaveChangesAsync` |

---

### F-BE-024 [MEDIUM] No CSRF Protection

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Program.cs:99-105` |
| **Category** | Security |
| **Impact** | CSRF attacks on cookie-authenticated admin |
| **Description** | No anti-forgery token configured for MVC POST actions. |
| **Suggested Fix** | Add `[AutoValidateAntiforgeryToken]` globally |

---

### F-BE-025 [MEDIUM] No Rate Limiting

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Program.cs` |
| **Category** | Security |
| **Impact** | Brute-force Login/ForgotPassword attacks |
| **Description** | No rate limiting on any endpoint, including login and forgot-password. |
| **Suggested Fix** | Add built-in .NET rate limiting middleware |

---

### F-BE-026 [LOW] Fragile Scope Pattern

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderExpiryBackgroundService.cs:48-81` |
| **Category** | Architecture |
| **Impact** | Low risk, `ObjectDisposedException` if bug introduced |
| **Description** | Currently correct but fragile if any async operation is not awaited within the scope. |
| **Suggested Fix** | Document the pattern |

---

### F-BE-027 [LOW] StockLock TTL Mismatch

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\OrderService.cs:284` |
| **Category** | Data Integrity |
| **Impact** | Oversold if cache expires while order pending |
| **Description** | 15-minute TTL on stock lock — cache expiry silently releases stock. |
| **Suggested Fix** | Remove TTL, always use explicit release |

---

### F-BE-028 [LOW] ToDTO Null Pattern

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Models\DTOs\MappingExtensions.cs:12,38,...` |
| **Category** | Correctness |
| **Impact** | NRE if called without null check |
| **Description** | All `ToDTO` extension methods return `null` on null input — callers must null-check. |
| **Suggested Fix** | Document pattern or throw `ArgumentNullException` |

---

### F-BE-029 [LOW] Random Seed

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Utils\SlugHelper.cs:34` |
| **Category** | Concurrency |
| **Impact** | SKU collision under concurrent calls |
| **Description** | `new Random()` per call — on fast CPUs, seed reuse is possible. |
| **Suggested Fix** | Use static thread-safe `Random` or `Guid` |

---

### F-BE-030 [LOW] Unused Import

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\AuthService.cs:10` |
| **Category** | Code Quality |
| **Impact** | None |
| **Description** | `System.Threading.RateLimiting` imported but never used. |
| **Suggested Fix** | Remove unused import |

---

### F-BE-031 [LOW] REST Convention — Cancel Method

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Controllers\Api\OrdersController.cs:127-136` |
| **Category** | API Design |
| **Impact** | Minor REST convention deviation |
| **Description** | Cancel uses `HttpPut` — cancel is an action, not an update. |
| **Suggested Fix** | Change to `HttpPost` |

---

### F-BE-032 [LOW] No JWT Blacklist

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\AuthService.cs:171-184` |
| **Category** | Security |
| **Impact** | JWT valid for up to 60 min after revoke |
| **Description** | `RevokeUserTokensAsync` doesn't invalidate existing JWTs. They remain valid until expiry. |
| **Suggested Fix** | Add token blacklist or use short-lived JWTs |

---

### F-BE-033 [LOW] Email Parsing

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\EmailService.cs:425-452` |
| **Category** | Correctness |
| **Impact** | Notes with `|` break parsing |
| **Description** | `ParseNotes` splits by `|` which could appear in user content. |
| **Suggested Fix** | Use structured format (JSON) |

---

## Frontend Findings (38)

### F-FE-001 [CRITICAL] AuthContext No useMemo

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\context\AuthContext.tsx:88` |
| **Category** | Performance |
| **Impact** | ALL auth consumers re-render on any state change |
| **Description** | Context value is recreated on every render — no `useMemo` wrapping. Every context consumer re-renders even when unrelated state changes. |
| **Suggested Fix** | Wrap the context value object in `useMemo` |

---

### F-FE-002 [CRITICAL] Mobile Menu Broken

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\components\Header.tsx:158-160` |
| **Category** | Functionality |
| **Impact** | Mobile navigation completely non-functional |
| **Description** | Hamburger button has no `onClick` handler. Tapping it does nothing. |
| **Suggested Fix** | Implement mobile drawer/sheet |

---

### F-FE-003 [CRITICAL] Blog Pagination Hidden

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\blog\index.tsx:16-18,58` |
| **Category** | Functionality |
| **Impact** | Can't access blog posts beyond page 1 per category |
| **Description** | Pagination is hidden when a category filter is active. |
| **Suggested Fix** | Server-side category filtering or always show pagination |

---

### F-FE-004 [CRITICAL] Sort Not Working

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\shop\ShopHeader.tsx:19-24` |
| **Category** | Functionality |
| **Impact** | Sort feature non-functional |
| **Description** | Sort dropdown has no `onChange` handler. Selecting a sort option does nothing. |
| **Suggested Fix** | Add `onChange` handler + backend `sortBy` param |

---

### F-FE-005 [CRITICAL] JWT No Validation

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\context\AuthContext.tsx:15-17` |
| **Category** | Security |
| **Impact** | Fake/expired JWT accepted as valid |
| **Description** | JWT decoded client-side without signature verification and no `exp` claim check. |
| **Suggested Fix** | Verify signature + check `exp` claim on decode |

---

### F-FE-006 [HIGH] JWT in localStorage

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\services\tokenService.ts:1-8` |
| **Category** | Security |
| **Impact** | Token stolen via any XSS vulnerability |
| **Description** | JWT token stored in `localStorage`, which is accessible to any JavaScript on the page. |
| **Suggested Fix** | Use HttpOnly Secure cookies |

---

### F-FE-007 [HIGH] Homepage Fetches All Products

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\hooks\useProducts.ts:42-48` |
| **Category** | Performance |
| **Impact** | Fetches thousands of records for 3-4 items shown |
| **Description** | `useLatestProducts` fetches ALL products from the API, then slices client-side to show only the latest ones. |
| **Suggested Fix** | Dedicated API endpoint `/Products/latest?count=N` |

---

### F-FE-008 [HIGH] Blog Detail Fetches All Products

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\blog-detail\index.tsx:16,46` |
| **Category** | Performance |
| **Impact** | Wastes bandwidth, increases load time |
| **Description** | Blog detail page fetches the entire product catalog just to show 4 random recommendations. |
| **Suggested Fix** | Dedicated random products endpoint |

---

### F-FE-009 [HIGH] Type Erosion

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\hooks\useProducts.ts:19,57-62` |
| **Category** | TypeScript |
| **Impact** | 15+ components use `any`-typed data, no compile-time safety |
| **Description** | `useProductsPaged` returns `PagedResult<any>`, `useSearchProducts` returns `any[]`. |
| **Suggested Fix** | Type as `PagedResult<Product>` and `Product[]` |

---

### F-FE-010 [HIGH] HeroBanner Error Handling

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\home\HeroBanner.tsx:16-101` |
| **Category** | UX |
| **Impact** | User sees static fallback instead of error message |
| **Description** | Error state variable is set but never rendered in the UI. |
| **Suggested Fix** | Add error render block |

---

### F-FE-011 [MEDIUM] Duplicate CartItem Type

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\types\context.ts:25-27` |
| **Category** | Maintainability |
| **Impact** | Can diverge over time |
| **Description** | `CartItem` type defined in 2 separate locations. |
| **Suggested Fix** | Remove duplicate, import from single source |

---

### F-FE-012 [MEDIUM] OrderInput Type Mismatch

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\types\order.ts:42-46` |
| **Category** | TypeScript |
| **Impact** | No type safety for actual checkout payload |
| **Description** | `OrderInput` type doesn't include `paymentMethod`, `deliveryDate`, `recipientName`, etc. |
| **Suggested Fix** | Update `OrderInput` to match real payload |

---

### F-FE-013 [MEDIUM] PaymentMethod Validation

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\schemas\checkoutSchema.ts:23` |
| **Category** | Validation |
| **Impact** | Typos silently treated as COD |
| **Description** | `z.string()` instead of `z.enum()` for `paymentMethod`. Any string passes validation. |
| **Suggested Fix** | `z.enum(['COD', 'OnlinePayment'])` |

---

### F-FE-014 [MEDIUM] Filters Not in URL

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\shop\index.tsx:10-44` |
| **Category** | UX |
| **Impact** | Can't share filtered results, back button broken |
| **Description** | Category, price, and page filters are stored in React state only — not synced to URL search params. |
| **Suggested Fix** | Use `useSearchParams` to sync with URL |

---

### F-FE-015 [MEDIUM] Phone Blur API Spam

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\checkout\index.tsx:89-108` |
| **Category** | Performance |
| **Impact** | Multiple network requests on form interaction |
| **Description** | API call fires on EVERY blur event of the phone input. Tabbing through fields triggers unnecessary requests. |
| **Suggested Fix** | Debounce 300ms or only call on manual action |

---

### F-FE-016 [MEDIUM] SEO Missing OG Tags

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\components\SEO.tsx:1-16` |
| **Category** | SEO |
| **Impact** | Social shares show bare URL |
| **Description** | Only `<title>` and meta description are set — no Open Graph or Twitter Card tags. |
| **Suggested Fix** | Add `og:*`, `twitter:*` meta tags |

---

### F-FE-017 [MEDIUM] Status Duplication

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\components\OrderComponents.tsx:5-13` |
| **Category** | Maintainability |
| **Impact** | Inconsistent status display |
| **Description** | `statusConfig` object in component plus a separate inline mapping in `MyOrders`. |
| **Suggested Fix** | Export and reuse `statusConfig` |

---

### F-FE-018 [MEDIUM] Recipient Effect Runs Always

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\checkout\index.tsx:52-58` |
| **Category** | Performance |
| **Impact** | Unnecessary re-renders |
| **Description** | `useEffect` runs on every keystroke even when `recipientIsBuyer` is `false`. |
| **Suggested Fix** | Guard with `if (!recipientIsBuyer) return` |

---

### F-FE-019 [MEDIUM] GreetingCard Discarded

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\checkout\index.tsx:125-139` |
| **Category** | Functionality |
| **Impact** | Customer messages silently discarded |
| **Description** | `greetingCard` field is collected in the form but not included in `orderPayload`. |
| **Suggested Fix** | Add `greetingCard` to payload |

---

### F-FE-020 [MEDIUM] Form Autofill Overwrite

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\checkout\index.tsx:110-120` |
| **Category** | UX |
| **Impact** | User typing overwritten by effect |
| **Description** | `useEffect` fills form fields from user data, potentially overwriting what the user is currently typing. |
| **Suggested Fix** | Check `isDirty` before autofill |

---

### F-FE-021 [MEDIUM] WishlistCallback

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\context\WishlistContext.tsx:42-44` |
| **Category** | Performance |
| **Impact** | Minimal, but `useMemo` for Set would be better |
| **Description** | `useCallback` recreated on every `favorites` change. |
| **Suggested Fix** | Use `useMemo` + `Set` for O(1) lookup |

---

### F-FE-022 [MEDIUM] Sort by ID Not Date

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\home\LatestBlog.tsx:10` |
| **Category** | Correctness |
| **Impact** | 'Latest' may not be accurate |
| **Description** | Sorts by `b.id` descending instead of `createdDate`. |
| **Suggested Fix** | Sort by `createdDate` |

---

### F-FE-023 [MEDIUM] readOnly Conflict

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\checkout\index.tsx:290-294` |
| **Category** | UX |
| **Impact** | Recipient fields editable when they should be readonly |
| **Description** | `readOnly` prop may be overridden by `register()` from react-hook-form. |
| **Suggested Fix** | Verify or use `disabled` prop |

---

### F-FE-024 [MEDIUM] Carousel Loads All Images

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\home\HeroBanner.tsx:105-158` |
| **Category** | Performance |
| **Impact** | All images load on mount |
| **Description** | All slides rendered in DOM with CSS opacity — all images fetched regardless of visibility. |
| **Suggested Fix** | Lazy-load non-visible slides |

---

### F-FE-025 [MEDIUM] Orders Cache Not Invalidated

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\hooks\useOrders.ts:5-17` |
| **Category** | Data Freshness |
| **Impact** | Stale stock quantities after purchase |
| **Description** | After placing an order, the product cache is not invalidated. Users see stale stock counts. |
| **Suggested Fix** | `invalidateQueries(['products'])` in `onSuccess` |

---

### F-FE-026 [MEDIUM] MyOrders Typed as any

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\auth\MyOrders.tsx:116,118` |
| **Category** | TypeScript |
| **Impact** | No type safety for order data |
| **Description** | `orders.map((order: any) => ...)` — entire order list is `any`-typed. |
| **Suggested Fix** | Type as `Order[]` |

---

### F-FE-027 [LOW] ErrorFallback Bootstrap

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\components\ErrorFallback.tsx:7-16` |
| **Category** | Consistency |
| **Impact** | Inconsistent styling |
| **Description** | Uses Bootstrap classes while the rest of the app uses Tailwind CSS. |
| **Suggested Fix** | Refactor to match brand design system |

---

### F-FE-028 [LOW] Social Links href="#"

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\contact\index.tsx:50-61` |
| **Category** | Functionality |
| **Impact** | Non-functional social links |
| **Description** | Facebook, Instagram, TikTok, and Zalo links all have `href="#"`. |
| **Suggested Fix** | Add real URLs or remove |

---

### F-FE-029 [LOW] Blog Buttons No-op

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\blog-detail\index.tsx:99-109` |
| **Category** | Functionality |
| **Impact** | Non-functional buttons |
| **Description** | Share, bookmark, and print buttons have no `onClick` handler. |
| **Suggested Fix** | Implement or remove |

---

### F-FE-030 [LOW] Footer Policy Links

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\components\Footer.tsx:44-52` |
| **Category** | UX |
| **Impact** | Can't find policies |
| **Description** | Privacy Policy, Terms of Service links all go to `/shop`. |
| **Suggested Fix** | Create proper policy pages or remove links |

---

### F-FE-031 [LOW] Wrong Delivery Estimate

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\order-confirmation\index.tsx:54` |
| **Category** | Accuracy |
| **Impact** | Unrealistic for florist |
| **Description** | Shows "5-7 business days" delivery estimate for flowers. |
| **Suggested Fix** | Update to same-day/next-day estimates |

---

### F-FE-032 [LOW] DiscountPrice Not Shown

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\components\ProductCard.tsx:71,127` |
| **Category** | UX |
| **Impact** | Price discrepancy between card and cart |
| **Description** | Product card only shows `item.price`, never `discountPrice`. |
| **Suggested Fix** | Show `discountPrice` with strikethrough on original price |

---

### F-FE-033 [LOW] Hardcoded Video URL

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\product-detail\index.tsx:29,104-106` |
| **Category** | Robustness |
| **Impact** | Broken content if external resources change |
| **Description** | YouTube URL and Google image URLs hardcoded. |
| **Suggested Fix** | Add error handling / hosting fallback |

---

### F-FE-034 [LOW] No Max Date Constraint

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\checkout\index.tsx:356,371` |
| **Category** | UX |
| **Impact** | Users can schedule years ahead |
| **Description** | No `max` date constraint on delivery date picker. |
| **Suggested Fix** | Add `max=30 days` from today |

---

### F-FE-035 [LOW] getToken on Every Render

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\context\AuthContext.tsx:88` |
| **Category** | Performance |
| **Impact** | Minor performance overhead |
| **Description** | `tokenService.getToken()` called on every render. |
| **Suggested Fix** | Store token in `useState` |

---

### F-FE-036 [LOW] Dead Components

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\pages\home\LatestProducts.tsx:1-44` |
| **Category** | Code Quality |
| **Impact** | Dead code increases bundle size |
| **Description** | `LatestProducts` and `CategoryMenu` components are imported but never rendered anywhere. |
| **Suggested Fix** | Remove or document as unused |

---

### F-FE-037 [LOW] Keyboard Navigation

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\components\ProductCard.tsx:46,102` |
| **Category** | Accessibility |
| **Impact** | Keyboard-only users can't click cards |
| **Description** | Card `onClick` on a `<div>` has no `role` or `onKeyDown` handler. |
| **Suggested Fix** | `role="presentation"` or use a block-level `<a>` link |

---

### F-FE-038 [LOW] Currency Rounding

| Field | Value |
|-------|-------|
| **File** | `Flower-shop.frontend\src\utils\currency.ts:2` |
| **Category** | Accuracy |
| **Impact** | 49,900 VND shows as 50,000 |
| **Description** | Rounds to nearest 1000 VND. |
| **Suggested Fix** | Use `Intl.NumberFormat` without rounding |

---

## Database Findings (16)

### F-DB-001 [CRITICAL] PriceAdjustment Type Mismatch

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Migrations\20260705053142_AddRecipientAndProductSnapshot.cs:68` |
| **Category** | Schema |
| **Impact** | EF detects model diff, generates superfluous ALTER migration |
| **Description** | Entity annotation = `decimal(18,0)`, migration `Up()` = `decimal(18,2)`, Designer/Snapshot = `decimal(18,0)`. Three-way mismatch between entity, migration, and snapshot. |
| **Suggested Fix** | Sync all to `decimal(18,2)` |

---

### F-DB-002 [HIGH] UnitPrice decimal(18,0)

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\OrderDetail.cs:17-18` |
| **Category** | Schema |
| **Impact** | Fractional prices truncated in order history |
| **Description** | `OrderDetail.UnitPrice` has zero decimal places (`decimal(18,0)`). |
| **Suggested Fix** | Change to `decimal(18,2)` |

---

### F-DB-003 [HIGH] Payment.Amount decimal(18,0)

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\Payment.cs:14-15` |
| **Category** | Schema |
| **Impact** | Financial records lose precision |
| **Description** | `Payment.Amount` has zero decimal places (`decimal(18,0)`). |
| **Suggested Fix** | Change to `decimal(18,2)` |

---

### F-DB-004 [HIGH] No PhoneBlacklist Index

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\PhoneBlacklist.cs:12-13` |
| **Category** | Performance |
| **Impact** | Full table scan on hot path |
| **Description** | No index on `PhoneNumber`. `FraudDetectionService` queries by phone on every COD order. |
| **Suggested Fix** | Add composite index `(PhoneNumber, IsActive)` |

---

### F-DB-005 [HIGH] TotalOrders Counter Broken

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\Customer.cs:30-32` |
| **Category** | Correctness |
| **Impact** | Online payment customers appear as new (`TotalOrders == 0`), triggers unnecessary verification |
| **Description** | `TotalOrders` only incremented for COD orders, not OnlinePayment. |
| **Suggested Fix** | Always increment `TotalOrders` or remove the counter |

---

### F-DB-006 [MEDIUM] No Customer.Phone Index

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\Customer.cs:20` |
| **Category** | Performance |
| **Impact** | Full table scan |
| **Description** | `FraudDetectionService` queries customers by phone number — no index on `Phone`. |
| **Suggested Fix** | Add index on `Phone` |

---

### F-DB-007 [MEDIUM] No TokenHash Index

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\RefreshToken.cs:15-16` |
| **Category** | Performance |
| **Impact** | Full table scan on auth critical path |
| **Description** | `AuthService` queries `RefreshTokens` by `TokenHash` on every token refresh — no index. |
| **Suggested Fix** | Add unique index on `TokenHash` |

---

### F-DB-008 [MEDIUM] DeliverySlot Composite Index Needed

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\DeliverySlot.cs:12,14,17` |
| **Category** | Performance |
| **Impact** | Key lookups on every slot operation |
| **Description** | All queries filter by `(ProductId, DeliveryDate, TimeSlot)` — but indexes are single-column. |
| **Suggested Fix** | Replace single-column indexes with composite index |

---

### F-DB-009 [MEDIUM] No OrderDate Index

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\Order.cs:39` |
| **Category** | Performance |
| **Impact** | Full scan + sort on date queries |
| **Description** | Admin queries sort by `OrderDate` — no standalone descending index. |
| **Suggested Fix** | Add index on `OrderDate DESC` |

---

### F-DB-010 [MEDIUM] No ResetToken Index

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\Customer.cs:40-41` |
| **Category** | Performance |
| **Impact** | Full table scan on password reset |
| **Description** | `AuthService` queries by `ResetToken` on password reset — no filtered index. |
| **Suggested Fix** | Add filtered index `WHERE ResetToken IS NOT NULL` |

---

### F-DB-011 [MEDIUM] Missing ProductVariants Nav

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\Product.cs` |
| **Category** | Data Model |
| **Impact** | Can't `Include` variants from product query |
| **Description** | No `ICollection<ProductVariant>` navigation property on `Product` entity. |
| **Suggested Fix** | Add nav property |

---

### F-DB-012 [LOW] Missing Payments Nav

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\Order.cs` |
| **Category** | Data Model |
| **Impact** | Must query `Payments` DbSet directly |
| **Description** | No `ICollection<Payment>` navigation property on `Order`. |
| **Suggested Fix** | Add nav property (optional) |

---

### F-DB-013 [LOW] Non-Sargable Search

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\Product.cs:15-16` |
| **Category** | Performance |
| **Impact** | Full table scan on search |
| **Description** | `Name.ToLower().Contains(term)` translates to `LIKE '%term%'` — cannot use index. |
| **Suggested Fix** | Add SQL Server full-text index + `EF.Functions.Contains` |

---

### F-DB-014 [LOW] RefreshToken FK Not Explicit

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\ApplicationDbContext.cs:26,77` |
| **Category** | Maintainability |
| **Impact** | Relies on convention, unpredictable if changed |
| **Description** | `RefreshToken` → `User` foreign key not explicitly configured in `OnModelCreating`. |
| **Suggested Fix** | Add explicit configuration |

---

### F-DB-015 [LOW] Category.Posts Null Risk

| Field | Value |
|-------|-------|
| **File** | `Flower.Data\Entities\Category.cs:21` |
| **Category** | Correctness |
| **Impact** | NRE if accessed without loading |
| **Description** | `ICollection<Post> Posts` not initialized and not nullable. |
| **Suggested Fix** | Initialize to `new List<Post>()` or make nullable |

---

### F-DB-016 [LOW] DeliverySlot N+1

| Field | Value |
|-------|-------|
| **File** | `Flower.Backend\Services\DeliverySlotService.cs:128,134` |
| **Category** | Performance |
| **Impact** | 1001 queries for 1000 products |
| **Description** | Loads ALL products first, then calls a slot query per product. Classic N+1 pattern. |
| **Suggested Fix** | Single query with proper joins |

---

## Scoring Summary

| Category | Score | Reasoning |
|----------|-------|-----------|
| **Architecture** | 6/10 | Transaction nesting, code duplication, service coupling |
| **Backend** | 5/10 | Critical fire-and-forget, secrets in code, missing validation |
| **Frontend** | 5/10 | Mobile broken, JWT no validation, type erosion, sort not working |
| **Database** | 5/10 | Type mismatches, 7 missing indexes |
| **Performance** | 5/10 | N+1, full table scans, fetching all data |
| **Security** | 4/10 | OTP backdoor, JWT in localStorage, secrets in code, no CSRF, no rate limit |
| **Maintainability** | 5/10 | Dead code, duplicate types, missing logging, role confusion |
| **Scalability** | 4/10 | In-memory stock lock, auto-migration, no distributed cache |
| **Testing** | 4/10 | Only 37 tests |
| **Documentation** | 9/10 | Still strong |
| **Overall** | **5.2/10** | |

---

## Findings by Severity

| Severity | Count |
|----------|-------|
| 🔴 Critical | 12 |
| 🟠 High | 14 |
| 🟡 Medium | 34 |
| 🔵 Low | 23 |
| **Total** | **83** |

## Findings by Area

| Area | Count |
|------|-------|
| Backend | 29 |
| Frontend | 38 |
| Database | 16 |
| **Total** | **83** |
