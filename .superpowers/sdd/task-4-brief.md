# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 4: Rename controllers + views + SignalR entity strings

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

---

## Task 4: Rename controllers + views + SignalR entity strings

**Files:**
- Rename: `Controllers/Api/CategoriesController.cs` → `PostCategoriesController.cs` (class + route `api/PostCategories`; `ICategoryService` → `IPostCategoryService`; DTO types → `PostCategoryDTO`/`CreatePostCategoryDTO`/`UpdatePostCategoryDTO`)
- Rename: `Controllers/Api/CategoriesApiController.cs` → `PostCategoriesApiController.cs` (class; explicit route stays `api/postcategories`; same type renames)
- Rename: `Controllers/Api/CategoriesProductsController.cs` → `ProductCategoriesController.cs` (class + route `api/ProductCategories`; `ICategoryProductService` → `IProductCategoryService`; `INotificationService` → `ICustomerNotificationService`; DTOs → `ProductCategoryDTO`/`CreateProductCategoryDTO`/`UpdateProductCategoryDTO`; `NotifyEntityChanged("CategoryProduct")` → `NotifyEntityChanged("ProductCategory")`)
- Rename: `Controllers/Api/NotificationsController.cs` → `CustomerNotificationsController.cs` (class + route `api/CustomerNotifications`; `INotificationService` → `ICustomerNotificationService`)
- Rename: `Controllers/CategoryController.cs` → `PostCategoryController.cs` (MVC; service refs)
- Rename: `Controllers/CategoryProductController.cs` → `ProductCategoryController.cs` (MVC; `NotifyEntityChanged("CategoryProduct")` → `NotifyEntityChanged("ProductCategory")`; `INotificationService` → `ICustomerNotificationService`)
- Rename view folders: `Views/Category/*` → `Views/PostCategory/*`; `Views/CategoryProduct/*` → `Views/ProductCategory/*` (files keep names; update `@model` types inside to the renamed DTOs)
- Modify: `Controllers/Api/ProductsController.cs` (`categoryProductId` param at :46,50; route `categoryproduct/{categoryProductId}` at :71; `GetByCategoryProduct` at :72,74 → `GetByProductCategory` + `productcategory/{productCategoryId}`; `INotificationService` → `ICustomerNotificationService`)
- Modify: `Controllers/Api/PostsController.cs` (any `categoryId`/`Category` refs → `postCategoryId`/`PostCategory`)
- Modify: `Controllers/PostController.cs` (`ICategoryService` → `IPostCategoryService`, DTO types)
- Modify: `Views/Shared/_LayoutAdmin.cshtml` (asp-controller/asp-action links to `Category`/`CategoryProduct` → `PostCategory`/`ProductCategory`)
- Grep targets: `INotificationService`, `ICategoryService`, `ICategoryProductService`, `CategoryProductDTO`, `CategoryDTO`, `categoryProductId`, `CategoryProduct`, `_context.Categories`, `CategoriesProducts` across `Flower.Backend`

**Interfaces:**
- Consumes: Task 1-3 renames.
- Produces: API routes `api/PostCategories`, `api/postcategories`, `api/ProductCategories`, `api/CustomerNotifications`; SignalR entity string `"ProductCategory"`; `api/Products/productcategory/{productCategoryId}`.

- [ ] **Step 1: Rename the 4 API controllers**

Apply the file renames and in-file type renames exactly as listed. Do not change action signatures or routes except where listed. The two `api/PostCategories` controllers (`PostCategoriesController` + `PostCategoriesApiController`) remain duplicates exactly as today (pre-existing, out of scope).

- [ ] **Step 2: Rename the 2 MVC controllers + view folders**

Rename files, classes, and `Views/` folders. Update `@model` directives in the `.cshtml` files to the renamed DTO types. Update `_LayoutAdmin.cshtml` menu links.

- [ ] **Step 3: Rename the SignalR entity string**

Every `NotifyEntityChanged("CategoryProduct")` → `NotifyEntityChanged("ProductCategory")`. Grep `NotifyEntityChanged("Category")` — if any exists, change to `"PostCategory"`.

- [ ] **Step 4: Update `ProductsController.cs` and `PostsController.cs`**

- ProductsController: param/route/action renames listed above; controller ctor type `INotificationService` → `ICustomerNotificationService`.
- PostsController: rename `categoryId` params to `postCategoryId` where they filter blog posts by category.

- [ ] **Step 5: Grep-sweep remaining stale identifiers**

```powershell
rg -n "INotificationService|ICategoryService|ICategoryProductService|CategoryProduct|CategoriesProducts|Categories\b|categoryProductId|CategoryDTO" Flower.Backend --type cs
```

Fix every hit. Leave **only** these intact: `AdminNotificationService`/`IAdminNotificationService`/`AdminNotification`, `Views/Notification` (MVC admin), `Order.PaymentMethod` enum.

- [ ] **Step 6: Build + test (solution must compile again)**

```powershell
dotnet build
dotnet test Flower.Tests
```

Expected: build succeeds; `37` tests pass (all renames are compile-time).

- [ ] **Step 7: Commit**

```bash
git add Flower.Backend
git commit -m "refactor: rename category and notification controllers and views"
```

---