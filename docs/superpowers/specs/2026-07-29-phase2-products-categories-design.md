# Phase 2 Spec: Products & Categories (Admin SPA)

**Date:** 2026-07-29
**Project:** FlowerShop Admin SPA
**Status:** Draft

---

## 1. Overview

Phase 2 builds the Products and Product Categories management UI for the admin SPA. It covers:

- Product list with DataTable (search, filter by category, stock badge, pagination)
- Product create/edit form with multi-image upload via Cloudinary
- Delete confirmation dialog with cascading checks
- Product Categories CRUD via inline dialogs
- Backend additions: `ProductImage` entity, image upload endpoint, product-image association endpoints

---

## 2. Backend Changes

### 2.1 New Entity: `ProductImage`

Add to `Flower.Data/Entities/ProductImage.cs`:

```csharp
public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Product Product { get; set; }
}
```

- Navigation property `ICollection<ProductImage>? Images` added to `Product` entity
- EF relationship via `ProductId` FK, cascade delete
- New migration required

### 2.2 DTO Changes

#### New DTOs

Add to `Flower.Backend/Models/DTOs/ProductDTOs.cs` (or new `ProductImageDTOs.cs`):

```csharp
public class ProductImageDTO
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public int SortOrder { get; set; }
}

public class UploadImageResponse
{
    public string Url { get; set; }
}
```

- `ProductDTO` gains `List<ProductImageDTO> Images` property
- Mapping extensions updated

#### Extended CreateProductDTO / UpdateProductDTO

Add these fields to both DTOs (in `ProductDTOs.cs`):

```csharp
public bool IsActive { get; set; } = true;
public string? FlowerMeaning { get; set; }
public string? Origin { get; set; }
public string? CareInstruction { get; set; }
public List<string>? NewImages { get; set; }  // URLs of uploaded images to associate
```

- `NewImages` is used to batch-associate images during create/update (avoids N+1 roundtrips)
- On create: product is created, then `ProductImage` records are created for each URL in `NewImages`
- On update: new URLs in `NewImages` are appended, existing images are untouched (removal handled via `DELETE` endpoint)

Mapping extensions `ToEntity()` and `UpdateEntity()` updated to set the new fields. `UpdateEntity` does NOT touch `NewImages` (that's handled in the service layer).

### 2.3 New API Endpoints

**`POST /api/Upload`** (single image upload to Cloudinary)

- Accepts `multipart/form-data` with field `file`
- Validates file is an image (via ImageSharp)
- Uploads via existing `IPhotoService.UploadPhotoAsync`
- Returns `{ url: "https://res.cloudinary.com/..." }`
- Requires authentication (`[Authorize(Policy = "StaffOnly")]`)

**`GET /api/Products/{id}/images`**

- Returns `List<ProductImageDTO>` sorted by `SortOrder`
- `[Authorize(Policy = "StaffOnly")]`

**`POST /api/Products/{id}/images`**

- Body: `{ imageUrl: string, sortOrder: int }`
- Adds image record, returns created `ProductImageDTO`
- `[Authorize(Policy = "StaffOnly")]`

**`DELETE /api/Products/{id}/images/{imageId}`**

- Removes image record (NOT the Cloudinary file)
- `[Authorize(Policy = "StaffOnly")]`

### 2.4 Updated ProductDTO

`ProductDTO` gains:
```csharp
public List<ProductImageDTO> Images { get; set; } = new();
```

Mapping extension `ToDTO()` loads `product.Images.OrderBy(i => i.SortOrder).Select(...)`.

`ProductService.BuildQuery()` adds `.Include(p => p.Images.OrderBy(i => i.SortOrder))`.
`ProductService.Create()` handles `dto.NewImages` by creating `ProductImage` records after saving the product.
`ProductService.Update()` handles `dto.NewImages` by appending new `ProductImage` records.

### 2.5 Image Upload Flow (Frontend → Backend)

1. User selects files in the form (drag & drop or click to select)
2. Each file is uploaded to `POST /api/Upload` (parallel upload with individual progress)
3. Backend uploads to Cloudinary via `PhotoService`, returns `{ url: "..." }`
4. Frontend collects returned URLs, displays preview thumbnails
5. On form submit, URLs are included in `CreateProductDTO.NewImages` / `UpdateProductDTO.NewImages`
6. Backend creates the product + associated `ProductImage` records in a single transaction
7. For existing images deleted in the edit form, frontend calls `DELETE /api/Products/{id}/images/{imageId}` immediately on remove

---

## 3. Frontend Pages

### 3.1 Products List (`/products`)

Replace the current placeholder `ProductsPage`.

**DataTable columns:**
| Column | Source | Notes |
|--------|--------|-------|
| Ảnh | `product.images[0]?.imageUrl` | 40×40 thumb, fallback placeholder |
| Tên sản phẩm | `product.name` | Linked to edit page |
| SKU | `product.sku` | — |
| Danh mục | `product.categoryProductName` | — |
| Giá | `product.price` | Formatted with commas, VNĐ suffix |
| Tồn kho | `product.stockQuantity` | Badge: ≥10 green, 1-9 amber, 0 red ("Hết hàng") |
| Trạng thái | `product.isActive` | Toggle badge (Đang bán / Ngừng bán) |
| Thao tác | — | Edit + Delete icon buttons |

**Features:**
- Search bar (text input, debounced 300ms, searches name + SKU via `/api/Products/search`)
- Category filter dropdown (fetches from `/api/CategoriesProducts`)
- Server-side pagination via `/api/Products/paged?page=X&pageSize=Y&categoryProductId=Z`
- Row count: 20 per page
- "Thêm sản phẩm" button → navigates to `/products/new`

**States:** Loading skeleton, empty state ("Chưa có sản phẩm nào"), error state with retry.

### 3.2 Product Create (`/products/new`)

**Form fields:**
| Field | Type | Validation | Notes |
|-------|------|-----------|-------|
| Tên sản phẩm | Text input | Required, max 200 | — |
| Slug | Text input | Max 300 | Auto-generated from name, editable |
| SKU | Text input | Max 50 | Auto-generated, editable |
| Danh mục | Select | Required | From `/api/CategoriesProducts` |
| Giá | Number input | Required, ≥ 0 | — |
| Số lượng tồn | Number input | ≥ 0 | — |
| Đang bán | Switch | — | Default true |
| Hình ảnh | Upload zone | — | Multi-file, drag & drop |
| Mô tả | Textarea | — | Rich text / plain |
| Ý nghĩa hoa | Text input | Max 500 | FlowerMeaning |
| Xuất xứ | Text input | Max 200 | Origin |
| Hướng dẫn chăm sóc | Textarea | — | CareInstruction |

**Image upload UX:**
- Drag & drop zone or click to select
- Preview thumbnails in a grid below the zone
- Each preview has a remove button (removes from list, does not call DELETE API until save)
- Upload progress indicator (per file)
- Max 10 images

**Behavior:**
- Unsaved changes guard: `beforeunload` + React Router `useBlocker`
- On submit: upload all new images (parallel) → get URLs → call `POST /api/Products` with `NewImages` containing URLs
- Backend creates product + `ProductImage` records in one transaction
- Success → navigate to `/products` with success toast
- Error → show error toast, stay on form

### 3.3 Product Edit (`/products/:id/edit`)

Same form as Create, pre-filled from `GET /api/Products/{id}`.

- Existing images shown as thumbnails with delete button
- Deleting an existing image immediately calls `DELETE /api/Products/{id}/images/{imageId}`
- New images uploaded same as create flow
- On submit: upload new images → get URLs → call `PUT /api/Products/{id}` with `NewImages`
- Backend updates product fields + appends new `ProductImage` records
- Success → navigate to `/products` with success toast

### 3.4 Product Delete (Dialog)

- Trigger: click delete icon in DataTable row
- Dialog shows: product name, thumbnail, warning ("Hành động này không thể hoàn tác")
- Confirm button: red "Xóa", Cancel: "Hủy"
- Loading state during deletion
- On success: remove row from table, success toast

### 3.5 Categories Management

**Location:** Accessible from `/products/categories` or a "Quản lý danh mục" button on the products page. Rendered as a standalone page with a DataTable + inline dialogs.

**Categories Table columns:**
| Column | Source |
|--------|--------|
| ID | `category.id` |
| Tên danh mục | `category.name` |
| Mô tả | `category.description` |
| Slug | `category.slug` |
| Số sản phẩm | calculated client-side or added to DTO |
| Thao tác | Edit + Delete |

**Create/Edit Dialog:**
- Fields: Name (required), Description (optional), Slug (auto-generated from name)
- Same dialog for create and edit (title changes)
- Validation: name required, max 200 chars

**Delete Dialog:**
- Shows category name
- Warning if products exist in this category ("Có X sản phẩm thuộc danh mục này")
- Confirm: "Xóa", Cancel: "Hủy"

---

## 4. Frontend Implementation Details

### 4.1 File Structure

```
src/
├── pages/
│   ├── products/
│   │   ├── ProductsPage.tsx       # DataTable list
│   │   ├── ProductFormPage.tsx    # Create/Edit (shared)
│   │   └── components/
│   │       ├── ProductTable.tsx
│   │       ├── ProductForm.tsx
│   │       ├── ImageUploader.tsx
│   │       └── DeleteProductDialog.tsx
│   └── categories/
│       ├── CategoriesPage.tsx     # Table + dialogs
│       └── components/
│           ├── CategoryTable.tsx
│           ├── CategoryDialog.tsx
│           └── DeleteCategoryDialog.tsx
├── api/
│   ├── products.ts                # Products API calls
│   └── categories.ts              # Categories API calls
├── types/
│   ├── product.ts                 # Product + ProductImage types
│   └── category.ts                # CategoryProduct type
```

### 4.2 Data Fetching

- Products list: use `@tanstack/react-query` `useQuery` with pagination/filter params
- Product detail: `useQuery(['product', id])`
- Categories list: `useQuery(['categories'])` (cache-bust on mutation)
- Mutations: `useMutation` with `onSuccess` invalidating relevant queries

### 4.3 Routing Updates

```tsx
<Route path="products" element={<ProductsPage />} />
<Route path="products/new" element={<ProductFormPage />} />
<Route path="products/:id/edit" element={<ProductFormPage />} />
<Route path="products/categories" element={<CategoriesPage />} />
```

### 4.4 Type Definitions

```typescript
// types/product.ts
export interface Product {
  id: number
  sku?: string
  name: string
  description?: string
  slug?: string
  price: number
  stockQuantity: number
  imageUrl?: string
  images: ProductImage[]
  categoryProductId: number
  categoryProductName?: string
  isActive: boolean
  flowerMeaning?: string
  origin?: string
  careInstruction?: string
  viewCount: number
  createdAt?: string
}

export interface ProductImage {
  id: number
  imageUrl: string
  sortOrder: number
}

export interface CreateProductRequest {
  name: string
  slug?: string
  sku?: string
  description?: string
  price: number
  stockQuantity: number
  categoryProductId: number
  isActive: boolean
  flowerMeaning?: string
  origin?: string
  careInstruction?: string
}

export interface UpdateProductRequest extends CreateProductRequest {
  id: number
}

// types/category.ts
export interface CategoryProduct {
  id: number
  name: string
  description?: string
  slug?: string
}

export interface CreateCategoryRequest {
  name: string
  description?: string
  slug?: string
}

export interface UpdateCategoryRequest extends CreateCategoryRequest {
  id: number
}
```

---

## 5. Migration Plan

1. **Backend Task 1:** Add `ProductImage` entity + migration + DbSet + relationship
2. **Backend Task 2:** Add `IPhotoService` API upload endpoint + product-image endpoints + update ProductDTO mapper
3. **Frontend Task 1:** Types + API functions for products and categories
4. **Frontend Task 2:** Products DataTable page with search/filter/pagination
5. **Frontend Task 3:** Product Create/Edit form with multi-image upload
6. **Frontend Task 4:** Delete product dialog
7. **Frontend Task 5:** Categories page with inline CRUD dialogs
8. **Frontend Task 6:** Routing updates + nav sidebar update + verify build

---

## 6. Out of Scope

- Product variants management (existing `ProductVariant` entity not used yet)
- Bulk product operations (import CSV, mass delete)
- Product reviews management
- Inventory history / stock adjustments log
