# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 11: Final verification

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

---

## Task 11: Final verification

**Files:** none (verification only)

- [ ] **Step 1: Backend build + tests**

```powershell
dotnet build
dotnet test Flower.Tests
```

Expected: build clean; `37` tests pass.

- [ ] **Step 2: Both frontends typecheck**

```powershell
npm run build   # in flower-admin.frontend
npm run build   # in Flower-shop.frontend
```

- [ ] **Step 3: End-to-end smoke**

Run backend. Verify with HTTP calls (or Swagger):
- `GET /api/PostCategories` and `GET /api/ProductCategories` return data (old `/api/Categories`, `/api/CategoriesProducts` now return 404/route match error — acceptable).
- `GET /api/CustomerNotifications` (with customer token) returns the same notifications as before.
- `GET /api/Products/paged?productCategoryId=1` filters correctly.
- `GET /api/PaymentMethods/active` returns active methods.
- Product detail shows `variants`; variant create/update/delete works.
- Customer address create/list/set-default/delete works.
- Shop UI loads categories + blog categories; admin UI lists/manages product categories.
- **Image flow (unify — verify only, no code change):** `GET /api/Products/{id}` returns `imageUrl` (main thumbnail) AND `images[]` (gallery); admin `ProductForm` still writes main image → `ImageUrl` and gallery → `ProductImages`. Confirm no duplicates were introduced by the import flow (regression check on `ImportService`).

- [ ] **Step 4: Commit any leftover**

```bash
git status
git add -A
git commit -m "chore: final refactor cleanup"
```

---

## What NOT to do

- Do NOT merge `CategoriesController` + `CategoriesApiController` (duplicate blog-category API controllers with routes `api/PostCategories` + `api/postcategories`). Pre-existing duplication; out of scope.
- Do NOT remove `ProductVariants.ProductId1` / `FK_ProductVariants_Products_ProductId1` / `IX_ProductVariants_ProductId1` — leftover shadow FK; out of scope.
- Do NOT rename `Order.PaymentMethod` enum, the `/hubs/notifications` SignalR hub, `AdminNotification`/MVC `NotificationController`, `ProductImage`, `Product.ImageUrl`, or the Cloudinary folder constant.
- Do NOT restructure `CategoryService`/`CategoryProductService`/`NotificationService` internals beyond the renames.
- Do NOT rename frontend UI component names (`CategoriesPage`, `CategoryDialog`, `CategoryTable`, `DeleteCategoryDialog`, route `products/categories`) — UI labels stay.
- Do NOT add a second migration for the variant columns — they belong in `RefactorAndRenameTables` (Task 5).

## Testing strategy

- Backend: `dotnet test Flower.Tests` must stay green (`37` tests) after every task from Task 4 onward. Task 0-3 are mechanical renames — build gate only.
- Migration: verify against a **backup** of Postgres before `database update`; confirm row counts and column values survive.
- Frontend: `npm run build` (tsc) is the type gate. Manual smoke covers the SignalR realtime category refresh on the shop.
- No new unit tests are required for the 3 new APIs (matching repo convention — existing services have no dedicated tests); correctness is verified by the smoke tests in each task.

## Exit criteria

- [ ] Tables renamed (data intact): `PostCategories`, `ProductCategories`, `CustomerNotifications`.
- [ ] `ProductVariants` has `Price` (old values) + `Sku`.
- [ ] No stale identifiers: `CategoriesProducts`, `CategoryProduct`, `INotificationService`, `ICategoryService`, `ICategoryProductService`, `categoryProductId` (backend), `/api/CategoriesProducts`, `/api/Categories`, `/api/notifications` (frontend) all gone.
- [ ] `dotnet build` + `dotnet test` pass (`37` tests).
- [ ] Both frontends `npm run build` clean.
- [ ] New endpoints work: product variant CRUD, customer address CRUD + set-default, `GET /api/PaymentMethods/active`.