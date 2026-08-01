# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 9: Frontend — flower-admin rename

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

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