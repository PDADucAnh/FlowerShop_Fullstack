# Task 2 Report: Rename DTOs + MappingExtensions

Date: 2026-07-31
Branch: `refactor/rename-tables`

## What I implemented

Following `docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md` Task 2:

1. **Renamed DTO files + classes** (via `git mv`):
   - `Flower.Backend/Models/DTOs/CategoryDTOs.cs` → `PostCategoryDTOs.cs` with classes `CategoryDTO`→`PostCategoryDTO`, `CreateCategoryDTO`→`CreatePostCategoryDTO`, `UpdateCategoryDTO`→`UpdatePostCategoryDTO`.
   - `Flower.Backend/Models/DTOs/CategoryProductDTOs.cs` → `ProductCategoryDTOs.cs` with classes `CategoryProductDTO`→`ProductCategoryDTO`, `CreateCategoryProductDTO`→`CreateProductCategoryDTO`, `UpdateCategoryProductDTO`→`UpdateProductCategoryDTO`.

2. **Renamed `ProductDTOs.cs` properties**: every `CategoryProductId`→`ProductCategoryId` and `CategoryProductName`→`ProductCategoryName` across the file (ProductDTO, CreateProductDTO, UpdateProductDTO).

3. **Renamed `PostDTOs.cs` properties**: every `CategoryId`→`PostCategoryId` and `CategoryName`→`PostCategoryName` across the file (PostDTO, CreatePostDTO, UpdatePostDTO).

4. **Updated `MappingExtensions.cs`** so the file compiles against the Task 1 entities + renamed DTOs:
   - `Category.ToDTO()` → `PostCategory.ToDTO()` returning `PostCategoryDTO`; `CreatePostCategoryDTO.ToEntity()` → `PostCategory`; `UpdatePostCategoryDTO.UpdateEntity(PostCategory)`.
   - `CategoryProduct` → `ProductCategory` equivalents (`ProductCategoryDTO`, `CreateProductCategoryDTO`, `UpdateProductCategoryDTO`), keeping the `ImageUrl` mapping.
   - `Product.ToDTO()`: `ProductCategoryId = product.ProductCategoryId`, `ProductCategoryName = product.ProductCategory?.Name`; `Product.ToEntity()`/`UpdateEntity()` use `dto.ProductCategoryId`.
   - `Post.ToDTO()`: `PostCategoryId = post.PostCategoryId`, `PostCategoryName = post.PostCategory?.Name`; `Post.ToEntity()`/`UpdateEntity()` use `dto.PostCategoryId`/`entity.PostCategoryId`.

5. Committed as `9b02ed2`.

## Verification

**Build (`dotnet build Flower.Backend`):** FAILED with 50 errors — but **all 50 are the expected downstream errors** in files this task must NOT touch:
- `Controllers/…` (CategoriesController, CategoriesApiController, CategoriesProductsController, CategoryController, CategoryProductController) — Task 4
- `Services/…` (CategoryService, CategoryProductService, ICategoryService, ICategoryProductService, INotificationService, NotificationService) — Task 3
- `Views/Category/*`, `Views/CategoryProduct/*` (.cshtml + Razor generator output) — Task 4

All errors are `CS0246: type … could not be found` for the **old** DTO/entity names. **Zero errors** reference any of the files I touched (`PostCategoryDTOs.cs`, `ProductCategoryDTOs.cs`, `ProductDTOs.cs`, `PostDTOs.cs`, `MappingExtensions.cs`) — the DTO + mapping layer is internally consistent against the Task 1 entities. This matches the plan's expected compile state ("will NOT compile until Task 4").

**Grep (stale identifiers in touched scope):** `CategoryDTO`, `CategoryProduct`, `CategoryProductId`, `CategoryProductName`, `CreateCategoryDTO`, `UpdateCategoryDTO`, `\bCategory\b`, `CategoryId`, `CategoryName` — **no matches** in `Flower.Backend/Models/DTOs` other than the new `PostCategory*`/`ProductCategory*` names.

## Files changed

- `Flower.Backend/Models/DTOs/CategoryDTOs.cs` → `PostCategoryDTOs.cs` (renamed + class renames)
- `Flower.Backend/Models/DTOs/CategoryProductDTOs.cs` → `ProductCategoryDTOs.cs` (renamed + class renames)
- `Flower.Backend/Models/DTOs/ProductDTOs.cs` (modified)
- `Flower.Backend/Models/DTOs/PostDTOs.cs` (modified)
- `Flower.Backend/Models/DTOs/MappingExtensions.cs` (modified)

## Self-review findings / concerns

1. **`PostDTOs.cs` scope extension (flagging):** the brief's Step 3 named lines 15-16 (the `PostDTO` class) only, but I also renamed `CategoryId`→`PostCategoryId` in `CreatePostDTO`/`UpdatePostDTO` (lines 35, 56) and the matching mappings. This was **required** for the DTO layer to compile: after Task 1, `Post` entity has `PostCategoryId`, so `Post.ToEntity()`/`UpdateEntity()` (`CategoryId = dto.CategoryId`, `entity.CategoryId = dto.CategoryId`) would not compile without it. The plan's File Map line ("`PostDTOs.cs` — modify: `CategoryId`/`CategoryName` → `PostCategoryId`/`PostCategoryName`") is consistent with this reading. `CreatePostDTO`/`UpdatePostDTO` are consumed by controllers/posts in later tasks, which will reference `PostCategoryId` — consistent.
2. **Project location:** the dispatch context mentioned "Flower.Shared", but no such project exists; the plan/brief list all DTO paths under `Flower.Backend/Models/DTOs`, which is where the files actually live. Followed the plan.
3. **`DashboardDTOs.cs:147` (`CategoryName` property):** a dashboard-stat DTO not listed in the brief's file map. Left untouched (out of scope). If it maps to `ProductCategory` it may need attention in a later task — not this one.
4. **Unused-table constraint:** `ProductVariants`, `CustomerAddresses`, etc. untouched. No mapping changes for `Notification` DTO (none existed; notification DTO handling is in later tasks).
