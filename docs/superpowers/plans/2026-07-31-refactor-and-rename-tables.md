# RefactorAndRenameTables Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Rename 3 confusingly-named entity/table/DTO/controller sets (`Categories→PostCategories`, `CategoriesProducts→ProductCategories`, `Notifications→CustomerNotifications`), unify the product-image flow (already done — verify only), and ship new APIs for ProductVariants CRUD, CustomerAddresses CRUD, and a PaymentMethods `active` endpoint — via a single data-preserving migration named `RefactorAndRenameTables`.

**Architecture:** A data-preserving EF Core migration renames tables/columns/FKs/indexes (no drop/create, so existing data survives). Backend C# classes, DTOs, services, and controllers are renamed to match, then the two React frontends (`flower-admin`, `Flower-shop`) are updated to the new routes/types. Step 2 adds layered service + controller APIs for the 3 feature sets, following the existing Service→Controller→DTO pattern in the codebase.

**Tech Stack:** ASP.NET Core 8, EF Core 8 + Npgsql (PostgreSQL), SignalR, React 18 + TypeScript + TanStack Query, Vite, Tailwind.

## Global Constraints

- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

---

## File Map (rename targets — read this first)

### Backend — renamed entity files
| Old | New |
|---|---|
| `Flower.Data/Entities/Category.cs` | `Flower.Data/Entities/PostCategory.cs` (class `PostCategory`) |
| `Flower.Data/Entities/CategoryProduct.cs` | `Flower.Data/Entities/ProductCategory.cs` (class `ProductCategory`) |
| `Flower.Data/Entities/Notification.cs` | `Flower.Data/Entities/CustomerNotification.cs` (class `CustomerNotification`) |

### Backend — renamed DTO files
| Old | New |
|---|---|
| `Models/DTOs/CategoryDTOs.cs` | `Models/DTOs/PostCategoryDTOs.cs` (`PostCategoryDTO`, `CreatePostCategoryDTO`, `UpdatePostCategoryDTO`) |
| `Models/DTOs/CategoryProductDTOs.cs` | `Models/DTOs/ProductCategoryDTOs.cs` (`ProductCategoryDTO`, `CreateProductCategoryDTO`, `UpdateProductCategoryDTO`) |
| `Models/DTOs/ProductDTOs.cs` | modify: `CategoryProductId`/`CategoryProductName` → `ProductCategoryId`/`ProductCategoryName` |
| `Models/DTOs/PostDTOs.cs` | modify: `CategoryId`/`CategoryName` → `PostCategoryId`/`PostCategoryName` |
| — | create `Models/DTOs/ProductVariantDTOs.cs` |
| — | create `Models/DTOs/CustomerAddressDTOs.cs` |
| — | create `Models/DTOs/PaymentMethodDTOs.cs` |

### Backend — renamed services
| Old | New |
|---|---|
| `Services/Interfaces/ICategoryService.cs` | `Services/Interfaces/IPostCategoryService.cs` |
| `Services/CategoryService.cs` | `Services/PostCategoryService.cs` |
| `Services/Interfaces/ICategoryProductService.cs` | `Services/Interfaces/IProductCategoryService.cs` |
| `Services/CategoryProductService.cs` | `Services/ProductCategoryService.cs` |
| `Services/Interfaces/INotificationService.cs` | `Services/Interfaces/ICustomerNotificationService.cs` |
| `Services/NotificationService.cs` | `Services/CustomerNotificationService.cs` |
| `Services/ProductService.cs` | modify: + variant CRUD methods |
| — | create `Services/Interfaces/ICustomerAddressService.cs` |
| — | create `Services/CustomerAddressService.cs` |

### Backend — renamed controllers
| Old | New (route) |
|---|---|
| `Controllers/Api/CategoriesController.cs` | `Controllers/Api/PostCategoriesController.cs` (`api/PostCategories`) |
| `Controllers/Api/CategoriesApiController.cs` | `Controllers/Api/PostCategoriesApiController.cs` (`api/postcategories`) |
| `Controllers/Api/CategoriesProductsController.cs` | `Controllers/Api/ProductCategoriesController.cs` (`api/ProductCategories`) |
| `Controllers/Api/NotificationsController.cs` | `Controllers/Api/CustomerNotificationsController.cs` (`api/CustomerNotifications`) |
| `Controllers/CategoryController.cs` | `Controllers/PostCategoryController.cs` (MVC) |
| `Controllers/CategoryProductController.cs` | `Controllers/ProductCategoryController.cs` (MVC) |
| `Views/Category/*` (4 files) | `Views/PostCategory/*` |
| `Views/CategoryProduct/*` (3 files) | `Views/ProductCategory/*` |
| `Controllers/Api/ProductsController.cs` | modify: `categoryproduct` route + params |
| `Controllers/Api/PostsController.cs` | modify: `categoryId` param rename |
| `Controllers/PostController.cs` | modify: `ICategoryService` → `IPostCategoryService` |
| — | create `Controllers/Api/CustomerAddressesController.cs` |
| — | create `Controllers/Api/PaymentMethodsController.cs` |

### Key lines to remember
- `Flower.Data/ApplicationDbContext.cs:14,17,34` → DbSet renames; `IApplicationDbContext.cs:13,16,33`.
- `Flower.Backend/Program.cs:188,189,196` → DI renames (services already wired; only interface/class names change).
- `Flower.Backend/Services/DashboardService.cs:314` and `ImportService.cs` → `_context.CategoriesProducts` → `_context.ProductCategories`.
- `Flower.Tests/PaymentServiceTests.cs:52` → `Mock.Of<ICustomerNotificationService>()`.
- `NotifyEntityChanged("CategoryProduct")` occurs in old `Api/CategoriesProductsController.cs:49,67,79` and old `Controllers/CategoryProductController.cs:49,58,88` → all become `"ProductCategory"`.
- Existing FK/Index names (from migration `20260722230554_AddPagesAndContacts.cs`) used by the rename migration:
  - `FK_Posts_Categories_CategoryId`, `IX_Posts_CategoryId`
  - `FK_Products_CategoriesProducts_CategoryProductId`, `IX_Products_CategoryProductId`
  - `FK_Notifications_Customers_CustomerId`, `IX_Notifications_CustomerId`, `IX_Notifications_CustomerId_IsRead`
- Frontend admin: `api/categories.ts` → `api/productCategories.ts` (export `productCategoriesApi`, URLs `/api/ProductCategories`); `types/category.ts` → `types/productCategory.ts` (`ProductCategory`, `CreateProductCategoryRequest`, `UpdateProductCategoryRequest`); imports in `App.tsx:11`, `pages/categories/**` (4 files), `pages/products/ProductsPage.tsx:5,48`, `pages/products/components/ProductForm.tsx:5,123`.
- Frontend shop: `services/categoryService.ts`, `services/categoryProductService.ts`, `hooks/useCategories.ts`, `hooks/useRealtimeUpdates.ts:6`, `hooks/useProducts.ts`, `services/productService.ts`, `hooks/useNotifications.ts`, `types/product.ts`, `types/post.ts`, `types/category.ts` (split into `postCategory.ts` + `productCategory.ts`), `pages/blog/BlogSidebar.tsx:5`, `hooks/usePosts.ts:21`, `services/postService.ts:53`.

---

## Task 0: Baseline check + branch

**Files:** none (verification only)

- [ ] **Step 1: Verify current state builds and tests pass**

```powershell
dotnet build
dotnet test Flower.Tests
```

Expected: build succeeds; `37` tests pass.

- [ ] **Step 2: Create a feature branch**

```powershell
git checkout -b refactor/rename-tables
```

- [ ] **Step 3: Record pre-rename DB state (sanity anchor)**

Run the app once (or connect to Postgres) and confirm tables `Categories`, `CategoriesProducts`, `Notifications` exist and contain data (blog categories, product categories, customer notifications).

---

## Task 1: Rename entities + DbContext (+ ProductVariant columns)

**Files:**
- Rename: `Flower.Data/Entities/Category.cs` → `PostCategory.cs`
- Rename: `Flower.Data/Entities/CategoryProduct.cs` → `ProductCategory.cs`
- Rename: `Flower.Data/Entities/Notification.cs` → `CustomerNotification.cs`
- Modify: `Flower.Data/ApplicationDbContext.cs:14,17,34`
- Modify: `Flower.Data/IApplicationDbContext.cs:13,16,33`
- Modify: `Flower.Data/Entities/ProductVariant.cs:17-18` (add `Sku`, rename `PriceAdjustment`→`Price`)
- Modify: `Flower.Data/Entities/Product.cs:35-38` (nav + FK property rename), `Flower.Data/Entities/Post.cs` (nav + FK property rename)

**Interfaces:**
- Consumes: existing entity classes (contents preserved verbatim except names).
- Produces: entity CLR types `PostCategory`, `ProductCategory`, `CustomerNotification`; DbSets `PostCategories`, `ProductCategories`, `CustomerNotifications`; `ProductVariant` with `Price` + `Sku`; nav property names `Post.PostCategory`/`Post.PostCategoryId`, `Product.ProductCategory`/`Product.ProductCategoryId`.

- [ ] **Step 1: Rename `Category.cs` → `PostCategory.cs`**

Create `Flower.Data/Entities/PostCategory.cs` with **exactly** this content (identical to old `Category.cs`, only class name changed):

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class PostCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
```

Then delete `Flower.Data/Entities/Category.cs`.

- [ ] **Step 2: Rename `CategoryProduct.cs` → `ProductCategory.cs`**

Create `Flower.Data/Entities/ProductCategory.cs` with **exactly** this content (identical to old `CategoryProduct.cs`, only class name changed):

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        [MaxLength(2000)]
        public string? ImageUrl { get; set; }

        public virtual ICollection<Product>? Products { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
```

Then delete `Flower.Data/Entities/CategoryProduct.cs`.

- [ ] **Step 3: Rename `Notification.cs` → `CustomerNotification.cs`**

Create `Flower.Data/Entities/CustomerNotification.cs` with **exactly** this content (identical to old `Notification.cs`, only class name changed):

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class CustomerNotification
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int? OrderId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Content { get; set; }

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ReferenceType { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(20)]
        public string? Priority { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? NavigationUrl { get; set; }

        public string? Metadata { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}
```

Then delete `Flower.Data/Entities/Notification.cs`.

- [ ] **Step 4: Update `Post.cs` navigation + FK properties**

Replace the `CategoryId`/`Category` members in `Flower.Data/Entities/Post.cs` (old lines 30-33):

```csharp
        public int PostCategoryId { get; set; }

        [ForeignKey("PostCategoryId")]
        public virtual PostCategory PostCategory { get; set; }
```

- [ ] **Step 5: Update `Product.cs` navigation + FK properties**

In `Flower.Data/Entities/Product.cs:35-38`:
- `public int CategoryProductId { get; set; }` → `public int ProductCategoryId { get; set; }`
- `public virtual CategoryProduct? CategoryProduct { get; set; }` → `public virtual ProductCategory? ProductCategory { get; set; }`

- [ ] **Step 6: Update `ProductVariant.cs`**

```csharp
[Column(TypeName = "decimal(18,2)")]
public decimal Price { get; set; }

[MaxLength(50)]
public string? Sku { get; set; }
```

(Replace `PriceAdjustment` with `Price`; add `Sku` directly after it. The `[ForeignKey("ProductId")]` on the nav stays as-is.)

- [ ] **Step 7: Update `ApplicationDbContext.cs`**

```csharp
public DbSet<PostCategory> PostCategories { get; set; }   // was Categories (line 14)
public DbSet<ProductCategory> ProductCategories { get; set; } // was CategoriesProducts (line 17)
public DbSet<CustomerNotification> CustomerNotifications { get; set; } // was Notifications (line 34)
```

- [ ] **Step 8: Update `IApplicationDbContext.cs`**

Same three renames at lines 13, 16, 33.

- [ ] **Step 9: Commit**

```bash
git add Flower.Data
git commit -m "refactor: rename Category/CategoryProduct/Notification entities"
```

> Note: the solution will NOT compile until Task 4 finishes. Do not stop here — continue through Task 4 before building.

---

## Task 2: Rename DTOs + MappingExtensions

**Files:**
- Rename: `Flower.Backend/Models/DTOs/CategoryDTOs.cs` → `PostCategoryDTOs.cs`
- Rename: `Flower.Backend/Models/DTOs/CategoryProductDTOs.cs` → `ProductCategoryDTOs.cs`
- Modify: `Flower.Backend/Models/DTOs/ProductDTOs.cs:17-18,81,117` (`CategoryProductId`/`CategoryProductName`)
- Modify: `Flower.Backend/Models/DTOs/PostDTOs.cs:15-16` (`CategoryId`/`CategoryName`)
- Modify: `Flower.Backend/Models/DTOs/MappingExtensions.cs` (`~50-74`, `~82-107`, `~141-186`, `~531`)

**Interfaces:**
- Consumes: entity types from Task 1.
- Produces: DTO names `PostCategoryDTO`, `CreatePostCategoryDTO`, `UpdatePostCategoryDTO`, `ProductCategoryDTO`, `CreateProductCategoryDTO`, `UpdateProductCategoryDTO`; renamed props `ProductDTO.ProductCategoryId`/`ProductCategoryName`; `PostDTO.PostCategoryId`/`PostCategoryName`; mapping extensions `ToDTO()`/`ToEntity()`/`UpdateEntity()` for the renamed DTOs.

- [ ] **Step 1: Rename DTO files + classes**

`PostCategoryDTOs.cs`: `CategoryDTO` → `PostCategoryDTO`, `CreateCategoryDTO` → `CreatePostCategoryDTO`, `UpdateCategoryDTO` → `UpdatePostCategoryDTO` (class names and all internal references).

`ProductCategoryDTOs.cs`: `CategoryProductDTO` → `ProductCategoryDTO`, `CreateCategoryProductDTO` → `CreateProductCategoryDTO`, `UpdateCategoryProductDTO` → `UpdateProductCategoryDTO`.

- [ ] **Step 2: Rename properties in `ProductDTOs.cs`**

Replace every `CategoryProductId` → `ProductCategoryId` and `CategoryProductName` → `ProductCategoryName` across the whole file (lines 17-18 in `ProductDTO`, 81 in `CreateProductDTO`, 117 in `UpdateProductDTO`).

- [ ] **Step 3: Rename properties in `PostDTOs.cs`**

Replace `CategoryId` → `PostCategoryId` and `CategoryName` → `PostCategoryName` (lines 15-16).

- [ ] **Step 4: Update `MappingExtensions.cs`**

Update all references so the file compiles with new entity + DTO names:
- `Category.ToDTO()` → `PostCategory.ToDTO()` returning `PostCategoryDTO`
- `PostCategoryDTO.ToEntity()`/`UpdateEntity()` equivalents
- `CategoryProduct` → `ProductCategory` equivalents (keep `ImageUrl` mapping)
- `Product.ToDTO()`: map `ProductCategoryId = p.ProductCategoryId`, `ProductCategoryName = p.ProductCategory?.Name`
- `Post.ToDTO()`: map `PostCategoryId`/`PostCategoryName`

> The new mappings `ProductVariant.ToDTO()` and `PaymentMethodDefinition.ToDTO()` are added in Tasks 6 and 8 (with the DTO types that task defines).

- [ ] **Step 5: Commit**

```bash
git add Flower.Backend/Models
git commit -m "refactor: rename category DTOs and mapping extensions"
```

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

## Task 5: Data-preserving migration `RefactorAndRenameTables`

**Files:**
- Generate: `Flower.Data/Migrations/<timestamp>_RefactorAndRenameTables.cs` (+ `.Designer.cs` + snapshot update)

**Interfaces:**
- Consumes: the Task 1 model (new entity names, `ProductVariant.Price`/`Sku`).
- Produces: a database where `PostCategories`, `ProductCategories`, `CustomerNotifications` exist with all rows preserved, and `ProductVariants` has `Price` + `Sku`.

- [ ] **Step 1: Generate the migration**

```powershell
dotnet ef migrations add RefactorAndRenameTables --project Flower.Data --startup-project Flower.Backend
```

> The generated `Up()` will (wrongly) `DropTable("Categories")` etc. because EF treats renamed CLR types as new entities. Do NOT run it yet.

- [ ] **Step 2: Replace the generated `Up()` body**

Open the generated `<timestamp>_RefactorAndRenameTables.cs` and replace the entire `Up()` body with:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameTable(
        name: "Categories",
        newName: "PostCategories");

    migrationBuilder.RenameColumn(
        name: "CategoryId",
        table: "Posts",
        newName: "PostCategoryId");

    migrationBuilder.RenameForeignKey(
        name: "FK_Posts_Categories_CategoryId",
        table: "Posts",
        newName: "FK_Posts_PostCategories_PostCategoryId");

    migrationBuilder.RenameIndex(
        name: "IX_Posts_CategoryId",
        table: "Posts",
        newName: "IX_Posts_PostCategoryId");

    migrationBuilder.RenameTable(
        name: "CategoriesProducts",
        newName: "ProductCategories");

    migrationBuilder.RenameColumn(
        name: "CategoryProductId",
        table: "Products",
        newName: "ProductCategoryId");

    migrationBuilder.RenameForeignKey(
        name: "FK_Products_CategoriesProducts_CategoryProductId",
        table: "Products",
        newName: "FK_Products_ProductCategories_ProductCategoryId");

    migrationBuilder.RenameIndex(
        name: "IX_Products_CategoryProductId",
        table: "Products",
        newName: "IX_Products_ProductCategoryId");

    migrationBuilder.RenameTable(
        name: "Notifications",
        newName: "CustomerNotifications");

    migrationBuilder.RenameForeignKey(
        name: "FK_Notifications_Customers_CustomerId",
        table: "CustomerNotifications",
        newName: "FK_CustomerNotifications_Customers_CustomerId");

    migrationBuilder.RenameIndex(
        name: "IX_Notifications_CustomerId",
        table: "CustomerNotifications",
        newName: "IX_CustomerNotifications_CustomerId");

    migrationBuilder.RenameIndex(
        name: "IX_Notifications_CustomerId_IsRead",
        table: "CustomerNotifications",
        newName: "IX_CustomerNotifications_CustomerId_IsRead");

    migrationBuilder.RenameColumn(
        name: "PriceAdjustment",
        table: "ProductVariants",
        newName: "Price");

    migrationBuilder.AddColumn<string>(
        name: "Sku",
        table: "ProductVariants",
        type: "character varying(50)",
        maxLength: 50,
        nullable: true);
}
```

> `RenameForeignKey`/`RenameIndex` take the table name as it exists at the time the operation runs — after the preceding `RenameTable`, the table is `CustomerNotifications`, so the FK/index renames above must pass `table: "CustomerNotifications"` (do NOT revert the note to the old name).

- [ ] **Step 3: Replace the generated `Down()` body**

```csharp
protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropColumn(
        name: "Sku",
        table: "ProductVariants");

    migrationBuilder.RenameColumn(
        name: "Price",
        table: "ProductVariants",
        newName: "PriceAdjustment");

    migrationBuilder.RenameIndex(
        name: "IX_CustomerNotifications_CustomerId_IsRead",
        table: "CustomerNotifications",
        newName: "IX_Notifications_CustomerId_IsRead");

    migrationBuilder.RenameIndex(
        name: "IX_CustomerNotifications_CustomerId",
        table: "CustomerNotifications",
        newName: "IX_Notifications_CustomerId");

    migrationBuilder.RenameForeignKey(
        name: "FK_CustomerNotifications_Customers_CustomerId",
        table: "CustomerNotifications",
        newName: "FK_Notifications_Customers_CustomerId");

    migrationBuilder.RenameTable(
        name: "CustomerNotifications",
        newName: "Notifications");

    migrationBuilder.RenameIndex(
        name: "IX_Products_ProductCategoryId",
        table: "Products",
        newName: "IX_Products_CategoryProductId");

    migrationBuilder.RenameForeignKey(
        name: "FK_Products_ProductCategories_ProductCategoryId",
        table: "Products",
        newName: "FK_Products_CategoriesProducts_CategoryProductId");

    migrationBuilder.RenameColumn(
        name: "ProductCategoryId",
        table: "Products",
        newName: "CategoryProductId");

    migrationBuilder.RenameTable(
        name: "ProductCategories",
        newName: "CategoriesProducts");

    migrationBuilder.RenameIndex(
        name: "IX_Posts_PostCategoryId",
        table: "Posts",
        newName: "IX_Posts_CategoryId");

    migrationBuilder.RenameForeignKey(
        name: "FK_Posts_PostCategories_PostCategoryId",
        table: "Posts",
        newName: "FK_Posts_Categories_CategoryId");

    migrationBuilder.RenameColumn(
        name: "PostCategoryId",
        table: "Posts",
        newName: "CategoryId");

    migrationBuilder.RenameTable(
        name: "PostCategories",
        newName: "Categories");
}
```

> Do NOT include the generated DropTable/CreateTable ops. Do NOT touch `ProductVariants.ProductId1` (a pre-existing harmless shadow FK — leave it).

- [ ] **Step 4: Review the generated `.Designer.cs`**

Confirm the designer's target model contains `Flower.Data.Entities.PostCategory`, `Flower.Data.Entities.ProductCategory`, `Flower.Data.Entities.CustomerNotification`, and `ProductVariant` with `Price` + `Sku`. If the designer still references old types, regenerate (`dotnet ef migrations remove` + re-add) rather than hand-editing.

- [ ] **Step 5: Back up DB, then apply**

```powershell
dotnet ef database update --project Flower.Data --startup-project Flower.Backend
```

- [ ] **Step 6: Verify data + schema**

Query Postgres and confirm: tables `PostCategories`, `ProductCategories`, `CustomerNotifications` exist; row counts match pre-rename; `Posts.PostCategoryId`/`Products.ProductCategoryId` populated; `ProductVariants` has `Price` (old values) and new nullable `Sku`.

- [ ] **Step 7: Commit**

```bash
git add Flower.Data/Migrations
git commit -m "feat: add RefactorAndRenameTables migration"
```

---

## Task 6: ProductVariant CRUD (STEP 2)

**Files:**
- Create: `Flower.Backend/Models/DTOs/ProductVariantDTOs.cs`
- Modify: `Flower.Backend/Models/DTOs/ProductDTOs.cs` (add `Variants` to `ProductDTO`)
- Modify: `Flower.Backend/Models/DTOs/MappingExtensions.cs` (add `ProductVariant.ToDTO()`; extend `Product.ToDTO()` to map `Variants`)
- Modify: `Flower.Backend/Services/Interfaces/IProductService.cs` (3 new methods)
- Modify: `Flower.Backend/Services/ProductService.cs` (implement; include `ProductVariants` in `BuildQuery`)
- Modify: `Flower.Backend/Controllers/Api/ProductsController.cs` (3 new endpoints)

**Interfaces:**
- Consumes: `ProductVariant.Price`/`Sku` (Task 1), `ProductVariantDTO` (this task).
- Produces: `IProductService.AddVariantAsync(int, CreateProductVariantDTO) → ProductVariantDTO?`, `UpdateVariantAsync(int, UpdateProductVariantDTO) → bool`, `DeleteVariantAsync(int) → bool`; `ProductDTO.Variants`.

- [ ] **Step 1: Create `ProductVariantDTOs.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class ProductVariantDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Sku { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CreateProductVariantDTO
    {
        [Required(ErrorMessage = "Tên size không được để trống")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string? Sku { get; set; }

        public bool IsDefault { get; set; }
    }

    public class UpdateProductVariantDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên size không được để trống")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string? Sku { get; set; }

        public bool IsDefault { get; set; }
    }
}
```

- [ ] **Step 2: Add `Variants` to `ProductDTO`**

In `ProductDTOs.cs`, add after the `Images` property (line 27):

```csharp
public List<ProductVariantDTO> Variants { get; set; } = new();
```

- [ ] **Step 3: Add mapping in `MappingExtensions.cs`**

Add the `ProductVariant.ToDTO()` extension method (anywhere in the static class):

```csharp
public static ProductVariantDTO ToDTO(this ProductVariant v)
{
    return new ProductVariantDTO
    {
        Id = v.Id,
        ProductId = v.ProductId,
        Name = v.Name,
        Price = v.Price,
        Sku = v.Sku,
        IsDefault = v.IsDefault
    };
}
```

Then in `Product.ToDTO()`, add:

```csharp
Variants = product.ProductVariants?.Select(v => v.ToDTO()).ToList() ?? new List<ProductVariantDTO>()
```

- [ ] **Step 4: Add interface methods (`IProductService.cs`)**

```csharp
Task<ProductVariantDTO?> AddVariantAsync(int productId, CreateProductVariantDTO dto);
Task<bool> UpdateVariantAsync(int variantId, UpdateProductVariantDTO dto);
Task<bool> DeleteVariantAsync(int variantId);
```

- [ ] **Step 5: Implement in `ProductService.cs`**

Add `using System.Collections.Generic;` if missing, and implement:

```csharp
public async Task<ProductVariantDTO?> AddVariantAsync(int productId, CreateProductVariantDTO dto)
{
    var product = await _context.Products.FindAsync(productId);
    if (product == null) return null;

    if (dto.IsDefault)
    {
        var others = _context.ProductVariants.Where(v => v.ProductId == productId && v.IsDefault);
        foreach (var v in others) v.IsDefault = false;
    }

    var variant = new ProductVariant
    {
        ProductId = productId,
        Name = dto.Name,
        Price = dto.Price,
        Sku = dto.Sku,
        IsDefault = dto.IsDefault
    };

    _context.ProductVariants.Add(variant);
    await _context.SaveChangesAsync();
    return variant.ToDTO();
}

public async Task<bool> UpdateVariantAsync(int variantId, UpdateProductVariantDTO dto)
{
    var variant = await _context.ProductVariants.FindAsync(variantId);
    if (variant == null) return false;

    if (dto.IsDefault)
    {
        var others = _context.ProductVariants
            .Where(v => v.ProductId == variant.ProductId && v.IsDefault && v.Id != variantId);
        foreach (var v in others) v.IsDefault = false;
    }

    variant.Name = dto.Name;
    variant.Price = dto.Price;
    variant.Sku = dto.Sku;
    variant.IsDefault = dto.IsDefault;

    await _context.SaveChangesAsync();
    return true;
}

public async Task<bool> DeleteVariantAsync(int variantId)
{
    var variant = await _context.ProductVariants.FindAsync(variantId);
    if (variant == null) return false;

    _context.ProductVariants.Remove(variant);
    await _context.SaveChangesAsync();
    return true;
}
```

Also extend `BuildQuery` (line 54-58) to include variants:

```csharp
IQueryable<Product> query = _context.Products
    .Include(p => p.ProductCategory)
    .Include(p => p.Images)
    .Include(p => p.ProductVariants);
```

> After Task 1, `p.CategoryProduct` is now `p.ProductCategory`. `ProductVariants` navigation is `Product.ProductVariants` (exists at `Product.cs:44`).

- [ ] **Step 6: Add controller endpoints (`ProductsController.cs`)**

Add after `Delete` (line 149), before the bulk endpoints:

```csharp
[HttpPost("{id}/variants")]
public async Task<IActionResult> AddVariant(int id, [FromBody] CreateProductVariantDTO dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var variant = await _productService.AddVariantAsync(id, dto);
    if (variant == null)
        return NotFound(new { message = "Không tìm thấy sản phẩm này" });

    await _notificationService.NotifyEntityChanged("Product");
    return CreatedAtAction(nameof(GetDetail), new { id }, variant);
}

[HttpPut("{id}/variants/{variantId}")]
public async Task<IActionResult> UpdateVariant(int id, int variantId, [FromBody] UpdateProductVariantDTO dto)
{
    if (variantId != dto.Id)
        return BadRequest();

    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var updated = await _productService.UpdateVariantAsync(variantId, dto);
    if (!updated)
        return NotFound();

    await _notificationService.NotifyEntityChanged("Product");
    return NoContent();
}

[HttpDelete("{id}/variants/{variantId}")]
public async Task<IActionResult> DeleteVariant(int id, int variantId)
{
    var deleted = await _productService.DeleteVariantAsync(variantId);
    if (!deleted)
        return NotFound();

    await _notificationService.NotifyEntityChanged("Product");
    return NoContent();
}
```

- [ ] **Step 7: Build + test + smoke-test**

```powershell
dotnet build
dotnet test Flower.Tests
```

Expected: build succeeds, `37` tests pass. Manual smoke: `GET /api/Products/{id}` returns `variants: []`; `POST /api/Products/{id}/variants` with `{ "name": "Nhỏ", "price": 100000, "sku": "R-001", "isDefault": true }` returns the created variant and subsequent `GET` shows it.

- [ ] **Step 8: Commit**

```bash
git add Flower.Backend
git commit -m "feat: add product variant CRUD API"
```

---

## Task 7: CustomerAddress service + controller (STEP 2)

**Files:**
- Create: `Flower.Backend/Models/DTOs/CustomerAddressDTOs.cs`
- Create: `Flower.Backend/Services/Interfaces/ICustomerAddressService.cs`
- Create: `Flower.Backend/Services/CustomerAddressService.cs`
- Create: `Flower.Backend/Controllers/Api/CustomerAddressesController.cs`
- Modify: `Flower.Backend/Program.cs` (DI registration)

**Interfaces:**
- Consumes: `CustomerAddress` entity (unchanged).
- Produces: `ICustomerAddressService` with `GetByCustomerId`, `GetById`, `Create`, `Update`, `Delete`, `SetDefault`; routes under `api/CustomerAddresses`.

- [ ] **Step 1: Create `CustomerAddressDTOs.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class CustomerAddressDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? AddressLine { get; set; }
        public string? PostalCode { get; set; }
        public string? Note { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCustomerAddressDTO
    {
        public int CustomerId { get; set; }

        [MaxLength(200)]
        public string? ReceiverName { get; set; }

        [MaxLength(20)]
        public string? ReceiverPhone { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? Ward { get; set; }

        [MaxLength(500)]
        public string? AddressLine { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
    }

    public class UpdateCustomerAddressDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(200)]
        public string? ReceiverName { get; set; }

        [MaxLength(20)]
        public string? ReceiverPhone { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? Ward { get; set; }

        [MaxLength(500)]
        public string? AddressLine { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
    }
}
```

- [ ] **Step 2: Create `ICustomerAddressService.cs`**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface ICustomerAddressService
    {
        Task<IEnumerable<CustomerAddressDTO>> GetByCustomerId(int customerId);
        Task<CustomerAddressDTO?> GetById(int id);
        Task<CustomerAddressDTO> Create(CreateCustomerAddressDTO dto);
        Task<bool> Update(int id, UpdateCustomerAddressDTO dto);
        Task<bool> Delete(int id);
        Task<bool> SetDefault(int id, int customerId);
    }
}
```

- [ ] **Step 3: Create `CustomerAddressService.cs`**

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Flower.Backend.Services
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly IApplicationDbContext _context;

        public CustomerAddressService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerAddressDTO>> GetByCustomerId(int customerId)
        {
            var list = await _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && a.IsActive)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.Id)
                .ToListAsync();
            return list.Select(ToDTO);
        }

        public async Task<CustomerAddressDTO?> GetById(int id)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            return address == null ? null : ToDTO(address);
        }

        public async Task<CustomerAddressDTO> Create(CreateCustomerAddressDTO dto)
        {
            if (dto.IsDefault)
            {
                var others = _context.CustomerAddresses
                    .Where(a => a.CustomerId == dto.CustomerId && a.IsDefault);
                foreach (var a in others) a.IsDefault = false;
            }

            var address = new CustomerAddress
            {
                CustomerId = dto.CustomerId,
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,
                Province = dto.Province,
                District = dto.District,
                Ward = dto.Ward,
                AddressLine = dto.AddressLine,
                PostalCode = dto.PostalCode,
                Note = dto.Note,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsDefault = dto.IsDefault
            };

            if (!await _context.CustomerAddresses.AnyAsync(a => a.CustomerId == dto.CustomerId))
            {
                address.IsDefault = true;
            }

            _context.CustomerAddresses.Add(address);
            await _context.SaveChangesAsync();
            return ToDTO(address);
        }

        public async Task<bool> Update(int id, UpdateCustomerAddressDTO dto)
        {
            if (id != dto.Id) return false;

            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null) return false;

            if (dto.IsDefault)
            {
                var others = _context.CustomerAddresses
                    .Where(a => a.CustomerId == dto.CustomerId && a.IsDefault && a.Id != id);
                foreach (var a in others) a.IsDefault = false;
            }

            address.CustomerId = dto.CustomerId;
            address.ReceiverName = dto.ReceiverName;
            address.ReceiverPhone = dto.ReceiverPhone;
            address.Province = dto.Province;
            address.District = dto.District;
            address.Ward = dto.Ward;
            address.AddressLine = dto.AddressLine;
            address.PostalCode = dto.PostalCode;
            address.Note = dto.Note;
            address.Latitude = dto.Latitude;
            address.Longitude = dto.Longitude;
            address.IsDefault = dto.IsDefault;
            address.UpdatedAt = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null) return false;

            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefault(int id, int customerId)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null || address.CustomerId != customerId) return false;

            var others = _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && a.IsDefault && a.Id != id);
            foreach (var a in others) a.IsDefault = false;

            address.IsDefault = true;
            address.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private static CustomerAddressDTO ToDTO(CustomerAddress a)
        {
            return new CustomerAddressDTO
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                ReceiverName = a.ReceiverName,
                ReceiverPhone = a.ReceiverPhone,
                Province = a.Province,
                District = a.District,
                Ward = a.Ward,
                AddressLine = a.AddressLine,
                PostalCode = a.PostalCode,
                Note = a.Note,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                IsDefault = a.IsDefault,
                IsActive = a.IsActive
            };
        }
    }
}
```

- [ ] **Step 4: Create `CustomerAddressesController.cs`**

```csharp
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly ICustomerAddressService _addressService;

        public CustomerAddressesController(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("{customerId:int}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            return Ok(await _addressService.GetByCustomerId(customerId));
        }

        [HttpGet("by-id/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var address = await _addressService.GetById(id);
            if (address == null) return NotFound();
            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerAddressDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _addressService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerAddressDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _addressService.Update(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _addressService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPut("{id:int}/set-default")]
        public async Task<IActionResult> SetDefault(int id, [FromQuery] int customerId)
        {
            var updated = await _addressService.SetDefault(id, customerId);
            if (!updated) return NotFound();
            return NoContent();
        }
    }
}
```

- [ ] **Step 5: Register DI (`Program.cs`)**

```csharp
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.ICustomerAddressService, Flower.Backend.Services.CustomerAddressService>();
```

- [ ] **Step 6: Build + test + smoke-test**

```powershell
dotnet build
dotnet test Flower.Tests
```

Expected: build succeeds, `37` tests pass. Manual smoke (with a customer token): `POST /api/CustomerAddresses`, `GET /api/CustomerAddresses/{customerId}`, `PUT /api/CustomerAddresses/{id}/set-default?customerId=1`, `DELETE /api/CustomerAddresses/{id}`.

- [ ] **Step 7: Commit**

```bash
git add Flower.Backend
git commit -m "feat: add customer address API"
```

---

## Task 8: PaymentMethods `active` endpoint (STEP 2)

**Files:**
- Create: `Flower.Backend/Models/DTOs/PaymentMethodDTOs.cs`
- Modify: `Flower.Backend/Models/DTOs/MappingExtensions.cs` (add `PaymentMethodDefinition.ToDTO()`)
- Create: `Flower.Backend/Controllers/Api/PaymentMethodsController.cs`

**Interfaces:**
- Consumes: `PaymentMethodDefinition` entity + `IApplicationDbContext`, `PaymentMethodDTO` (this task).
- Produces: `GET api/PaymentMethods/active` → `IEnumerable<PaymentMethodDTO>` where `IsActive == true`.

- [ ] **Step 1: Create `PaymentMethodDTOs.cs`**

```csharp
namespace Flower.Backend.Models.DTOs
{
    public class PaymentMethodDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsOnline { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
```

> Add `PaymentMethodDefinition.ToDTO()` to `MappingExtensions.cs` (Step 2).

- [ ] **Step 2: Add `PaymentMethodDefinition.ToDTO()` to `MappingExtensions.cs`**

```csharp
public static PaymentMethodDTO ToDTO(this PaymentMethodDefinition m)
{
    return new PaymentMethodDTO
    {
        Id = m.Id,
        Code = m.Code,
        Name = m.Name,
        Description = m.Description,
        IsOnline = m.IsOnline,
        IsActive = m.IsActive,
        DisplayOrder = m.DisplayOrder
    };
}
```

- [ ] **Step 3: Create `PaymentMethodsController.cs`**

```csharp
using Flower.Backend.Models.DTOs;
using Flower.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public PaymentMethodsController(IApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var methods = await _context.PaymentMethods
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.Id)
                .ToListAsync();

            return Ok(methods.Select(m => m.ToDTO()));
        }
    }
}
```

- [ ] **Step 4: Build + test + smoke-test**

```powershell
dotnet build
dotnet test Flower.Tests
```

Expected: build succeeds, `37` tests pass. Manual smoke: `GET /api/PaymentMethods/active` returns only methods with `isActive: true`, ordered by `displayOrder`.

- [ ] **Step 5: Commit**

```bash
git add Flower.Backend
git commit -m "feat: add payment methods active endpoint"
```

---

## Task 9: Frontend — flower-admin rename

**Files:**
- Rename: `flower-admin.frontend/src/api/categories.ts` → `api/productCategories.ts` (export `productCategoriesApi`; URLs `/api/CategoriesProducts` → `/api/ProductCategories`)
- Rename: `flower-admin.frontend/src/types/category.ts` → `types/productCategory.ts` (`CategoryProduct` → `ProductCategory`, `CreateCategoryRequest` → `CreateProductCategoryRequest`, `UpdateCategoryRequest` → `UpdateProductCategoryRequest`)
- Modify: `flower-admin.frontend/src/App.tsx:11` (import `productCategoriesApi`? no — App imports `CategoriesPage`; if page import paths change, update; otherwise leave)
- Modify: `flower-admin.frontend/src/pages/categories/CategoriesPage.tsx:3,11,21`, `components/DeleteCategoryDialog.tsx:2,14,30`, `components/CategoryTable.tsx:11`, `components/CategoryDialog.tsx:3,18,71-72` (imports + type + `categoriesApi` → `productCategoriesApi`)
- Modify: `flower-admin.frontend/src/pages/products/ProductsPage.tsx:5,48`, `components/ProductForm.tsx:5,123` (import + `categoriesApi` → `productCategoriesApi`)
- Modify: `flower-admin.frontend/src/types/product.ts:11-12,34` (`categoryProductId` → `productCategoryId`, `categoryProductName` → `productCategoryName`)
- Modify: `flower-admin.frontend/src/api/products.ts:7` (`categoryProductId` → `productCategoryId`)
- Modify: `flower-admin.frontend/src/pages/products/ProductsPage.tsx:57`, `components/ProductForm.tsx:76,93,208,226,318,320`, `components/ProductTable.tsx:90` (`categoryProductId`/`categoryProductName` → `productCategoryId`/`productCategoryName`)

**Interfaces:**
- Consumes: backend routes `/api/ProductCategories`, `/api/Products` (renamed query param).
- Produces: admin app talking to the new routes/types.

- [ ] **Step 1: Rename API file + object + URLs**

`api/productCategories.ts`:

```ts
import { apiClient } from './client'
import type { ProductCategory, CreateProductCategoryRequest, UpdateProductCategoryRequest } from '@/types/productCategory'

export const productCategoriesApi = {
  getAll() {
    return apiClient.get<ProductCategory[]>('/api/ProductCategories')
  },
  getById(id: number) {
    return apiClient.get<ProductCategory>(`/api/ProductCategories/${id}`)
  },
  create(data: CreateProductCategoryRequest) {
    return apiClient.post<ProductCategory>('/api/ProductCategories', data)
  },
  update(id: number, data: UpdateProductCategoryRequest) {
    return apiClient.put(`/api/ProductCategories/${id}`, data)
  },
  delete(id: number) {
    return apiClient.delete(`/api/ProductCategories/${id}`)
  },
}
```

- [ ] **Step 2: Rename type file**

`types/productCategory.ts` with `ProductCategory`, `CreateProductCategoryRequest`, `UpdateProductCategoryRequest` (same field shapes as today's `types/category.ts`).

- [ ] **Step 3: Update imports in consumers**

In the 5 files listed, change `@/api/categories` → `@/api/productCategories` and `categoriesApi` → `productCategoriesApi`; change `@/types/category` → `@/types/productCategory` and `CategoryProduct` → `ProductCategory`, `CreateCategoryRequest` → `CreateProductCategoryRequest`, `UpdateCategoryRequest` → `UpdateProductCategoryRequest`.

- [ ] **Step 4: Rename product-category props**

In `types/product.ts`, `api/products.ts`, `ProductsPage.tsx`, `ProductForm.tsx`, `ProductTable.tsx`: replace `categoryProductId` → `productCategoryId` and `categoryProductName` → `productCategoryName`.

- [ ] **Step 5: Grep-sweep**

```powershell
rg -n "CategoriesProducts|CategoryProduct|categoryProductId|categoryProductName|@/types/category|@/api/categories" flower-admin.frontend/src
```

Fix all hits (type names can stay `CategoryDialog`, `CategoriesPage`, route `products/categories` — those are UI names, unchanged).

- [ ] **Step 6: Typecheck**

```powershell
npm run build
```

Expected: TypeScript compiles clean.

- [ ] **Step 7: Commit**

```bash
git add flower-admin.frontend
git commit -m "refactor: update admin frontend to renamed category API"
```

---

## Task 10: Frontend — Flower-shop rename

**Files:**
- Modify: `Flower-shop.frontend/src/services/categoryService.ts` (`/CategoriesProducts` → `/ProductCategories`; `/Categories` → `/PostCategories`)
- Modify: `Flower-shop.frontend/src/services/categoryProductService.ts` (`/CategoriesProducts` → `/ProductCategories`; method `getAllCategoryProducts` → `getAllProductCategories`)
- Modify: `Flower-shop.frontend/src/hooks/useCategories.ts` (use renamed method; queryKeys `['categories','products']` → `['product-categories']`, `['categories','blog']` → `['post-categories']`)
- Modify: `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts:6` (`CategoryProduct: ['categories', 'products']` → `ProductCategory: ['product-categories', 'products']`)
- Rename: `Flower-shop.frontend/src/types/category.ts` → `types/postCategory.ts` (`Category` → `PostCategory`, `CategoryInput` → `PostCategoryInput`)
- Create: `Flower-shop.frontend/src/types/productCategory.ts` (`ProductCategory` interface)
- Modify: `Flower-shop.frontend/src/pages/blog/BlogSidebar.tsx:5` (import `PostCategory` from `postCategory`)
- Modify: `Flower-shop.frontend/src/types/product.ts:21-22,44,55` (`categoryProductName` → `productCategoryName`, `categoryProductId` → `productCategoryId`)
- Modify: `Flower-shop.frontend/src/types/post.ts:8-9` (`categoryName` → `postCategoryName`, `categoryId` → `postCategoryId`)
- Modify: `Flower-shop.frontend/src/hooks/usePosts.ts:21` (`categoryId` → `postCategoryId`)
- Modify: `Flower-shop.frontend/src/services/postService.ts:53-59` (`getPostsByCategory(categoryId)` → `(postCategoryId)`)
- Modify: `Flower-shop.frontend/src/hooks/useProducts.ts:18-24` (param `categoryProductId` → `productCategoryId`)
- Modify: `Flower-shop.frontend/src/services/productService.ts:9,19,51-56` (param rename; `/Products/categoryproduct/${...}` → `/Products/productcategory/${...}`)
- Modify: `Flower-shop.frontend/src/hooks/useNotifications.ts:21,26,110,123` (`/api/notifications` → `/api/customer-notifications`)
- Grep targets: `categoryId`, `categoryName`, `categoryProduct`, `categories`, `notifications`, `Category` in `Flower-shop.frontend/src`

**Interfaces:**
- Consumes: backend routes `/api/ProductCategories`, `/api/PostCategories`, `/api/CustomerNotifications`, `/api/Products/productcategory/{id}`, `/Posts/category/{id}`.
- Produces: shop app talking to new routes/types; realtime map key `ProductCategory`.

- [ ] **Step 1: Update category services + hooks**

`categoryService.ts`: `getProductCategories` → `/ProductCategories`, `getBlogCategories` → `/PostCategories`.
`categoryProductService.ts`: `getAllCategoryProducts` → `getAllProductCategories`, URL `/ProductCategories`.
`useCategories.ts`:

```ts
export const useProductCategories = () =>
  useQuery({ queryKey: ['product-categories'], queryFn: () => categoryProductService.getAllProductCategories() });

export const useBlogCategories = () =>
  useQuery({ queryKey: ['post-categories'], queryFn: () => categoryService.getBlogCategories() });
```

`useRealtimeUpdates.ts` map:

```ts
const entityQueryMap: Record<string, string[]> = {
  ProductCategory: ['product-categories', 'products'],
  Product: ['products'],
  Post: ['posts'],
  ...
};
```

- [ ] **Step 2: Split the category types file**

`types/postCategory.ts`:

```ts
export interface PostCategory {
  id: number;
  name: string;
  description?: string;
}

export interface PostCategoryInput {
  name: string;
  description?: string;
}
```

`types/productCategory.ts`:

```ts
export interface ProductCategory {
  id: number;
  name: string;
  description?: string;
  slug?: string;
  imageUrl?: string;
}
```

Update `BlogSidebar.tsx` to import `PostCategory` from `'../../types/postCategory'`.

- [ ] **Step 3: Rename product/post category props**

- `types/product.ts`, `services/productService.ts`, `hooks/useProducts.ts`: `categoryProductId` → `productCategoryId`; `types/product.ts` `categoryProductName` → `productCategoryName`; URL `/Products/categoryproduct/` → `/Products/productcategory/`.
- `types/post.ts`: `categoryName` → `postCategoryName`, `categoryId` → `postCategoryId`. `hooks/usePosts.ts:21` and `services/postService.ts:53-59`: rename `categoryId` param → `postCategoryId` (URL `/Posts/category/${postCategoryId}` unchanged).
- Grep blog pages for `.categoryId`/`.categoryName` usages (e.g. `pages/blog/*`) and rename to the `postCategory*` field names.

- [ ] **Step 4: Update notification URLs**

`useNotifications.ts`: `/api/notifications` → `/api/customer-notifications`, `/api/notifications/unread-count` → `/api/customer-notifications/unread-count`, `/api/notifications/${id}/read` → `/api/customer-notifications/${id}/read`, `/api/notifications/read-all` → `/api/customer-notifications/read-all`.

- [ ] **Step 5: Grep-sweep**

```powershell
rg -n "CategoriesProducts|/Categories'|/Categories\`|categoryProduct|categoryName|categoryId|/api/notifications" Flower-shop.frontend/src
```

Fix hits; leave the SignalR hub string `/hubs/notifications` and the `'categories'` query keys only where they still match the map (they should now be `'product-categories'`/`'post-categories'`).

- [ ] **Step 6: Typecheck**

```powershell
npm run build
```

Expected: TypeScript compiles clean.

- [ ] **Step 7: Commit**

```bash
git add Flower-shop.frontend
git commit -m "refactor: update shop frontend to renamed category routes"
```

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
