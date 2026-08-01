# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 2: Rename DTOs + MappingExtensions

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

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