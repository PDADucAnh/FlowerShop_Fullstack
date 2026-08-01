# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 10: Frontend — Flower-shop rename

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

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