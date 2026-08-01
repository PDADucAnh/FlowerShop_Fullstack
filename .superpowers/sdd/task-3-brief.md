# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 3: Rename services + interfaces + DI + consumers

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

---

## Task 3: Rename services + interfaces + DI + consumers

**Files:**
- Rename: `Flower.Backend/Services/Interfaces/ICategoryService.cs` → `IPostCategoryService.cs` (type → `IPostCategoryService`)
- Rename: `Flower.Backend/Services/CategoryService.cs` → `PostCategoryService.cs` (class → `PostCategoryService : IPostCategoryService`; internal `_context.Categories` → `_context.PostCategories`; `Category` → `PostCategory`)
- Rename: `Flower.Backend/Services/Interfaces/ICategoryProductService.cs` → `IProductCategoryService.cs` (type → `IProductCategoryService`)
- Rename: `Flower.Backend/Services/CategoryProductService.cs` → `ProductCategoryService.cs` (class → `ProductCategoryService : IProductCategoryService`; `_context.CategoriesProducts` → `_context.ProductCategories`; `CategoryProductDTO` → `ProductCategoryDTO`, etc.)
- Rename: `Flower.Backend/Services/Interfaces/INotificationService.cs` → `ICustomerNotificationService.cs` (type → `ICustomerNotificationService`)
- Rename: `Flower.Backend/Services/NotificationService.cs` → `CustomerNotificationService.cs` (class → `CustomerNotificationService : ICustomerNotificationService`; `Notification` → `CustomerNotification`, `_context.Notifications` → `_context.CustomerNotifications`)
- Modify: `Flower.Backend/Services/Interfaces/IProductService.cs:10-11` (`categoryProductId` param → `productCategoryId`; `GetByCategoryProduct` → `GetByProductCategory`)
- Modify: `Flower.Backend/Services/ProductService.cs` (same renames in the implementations + `p.CategoryProduct` → `p.ProductCategory` in `BuildQuery:57`)
- Modify: `Flower.Backend/Program.cs:188,189,196`
- Modify: `Flower.Backend/Services/DashboardService.cs:314` (`_context.CategoriesProducts` → `_context.ProductCategories`)
- Modify: `Flower.Backend/Services/ImportService.cs` (same rename; also `CategoryProduct` entity refs → `ProductCategory`)
- Modify: `Flower.Tests/PaymentServiceTests.cs:52` (`Mock.Of<INotificationService>()` → `Mock.Of<ICustomerNotificationService>()`)

**Interfaces:**
- Consumes: Task 1 entities, Task 2 DTOs.
- Produces: `IPostCategoryService`, `IProductCategoryService`, `ICustomerNotificationService` (method signatures unchanged — only type names).

- [ ] **Step 1: Rename the 6 service/interface files and their type names**

Apply each file rename + class/interface rename + member-type rename (fields, ctor params, DTO types, entity types, DbSet references) listed above. Method signatures and bodies stay logically identical — only type/name identifiers change.

- [ ] **Step 2: Update `Program.cs` DI (lines 188, 189, 196)**

```csharp
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IPostCategoryService, Flower.Backend.Services.PostCategoryService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IProductCategoryService, Flower.Backend.Services.ProductCategoryService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.ICustomerNotificationService, Flower.Backend.Services.CustomerNotificationService>();
```

- [ ] **Step 3: Update consumers of the renamed DbSets**

- `DashboardService.cs:314` and `ImportService.cs`: `_context.CategoriesProducts` → `_context.ProductCategories`; any `CategoryProduct` type references → `ProductCategory`.
- Grep for any remaining `_context.Categories\b` / `CategoriesProducts` / `_context.Notifications` in `Flower.Backend/Services` and fix to the new DbSet names.

- [ ] **Step 4: Rename `IProductService`/`ProductService` method + params**

`Flower.Backend/Services/Interfaces/IProductService.cs`:
```csharp
Task<PagedResult<ProductDTO>> GetPaged(int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null, int? productCategoryId = null, bool includeInactive = false, bool? isActive = null);
Task<IEnumerable<ProductDTO>> GetByProductCategory(int productCategoryId, bool includeInactive = false);
```

`Flower.Backend/Services/ProductService.cs`: rename the `GetPaged` parameter `categoryProductId` → `productCategoryId` and the member access `p.CategoryProductId` → `p.ProductCategoryId`; rename `GetByCategoryProduct` → `GetByProductCategory` with matching param.

- [ ] **Step 5: Update the test mock**

`Flower.Tests/PaymentServiceTests.cs:52`: `Mock.Of<INotificationService>()` → `Mock.Of<ICustomerNotificationService>()`.

- [ ] **Step 6: Commit**

```bash
git add Flower.Backend/Services Flower.Backend/Program.cs Flower.Tests/PaymentServiceTests.cs
git commit -m "refactor: rename category and notification services"
```

---