# Phase 2: Products & Categories Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build Products & Categories management UI (list, create, edit, delete with multi-image upload) in the admin SPA.

**Architecture:** Backend adds `ProductImage` entity + image upload/association endpoints; frontend adds DataTable listing, create/edit forms with multi-image upload, and inline category CRUD dialogs.

**Tech Stack:** ASP.NET Core 8, EF Core + PostgreSQL, Cloudinary (via `PhotoService`), React 19 + Vite + Tailwind v4 + shadcn/ui (Base UI) + React Query + React Router v7.

## Global Constraints

- All UI text in Vietnamese
- API responses are raw data (not wrapped in `ApiResponse<T>`) — frontend reads `response.data` directly from axios
- Image upload via `POST /api/Upload` (returns `{ url: string }`), not direct browser-to-Cloudinary
- `CreateProductDTO` / `UpdateProductDTO` extended with: `IsActive`, `FlowerMeaning`, `Origin`, `CareInstruction`, `NewImages`
- `ProductDTO` gains `List<ProductImageDTO> Images`
- Backend creates product + `ProductImage` records in a single transaction (`NewImages` batch)
- Existing MVC controllers (`ProductController`, `CategoryProductController`) NOT modified — only API controllers changed
- Follow existing patterns: `ICategoryProductService` / `CategoryProductService`, `IProductService` / `ProductService`


---
### Task 3: Frontend — Types + API Functions

**Files:**
- Create: `flower-admin.frontend/src/types/product.ts`
- Create: `flower-admin.frontend/src/types/category.ts`
- Create: `flower-admin.frontend/src/api/products.ts`
- Create: `flower-admin.frontend/src/api/categories.ts`
- Create: `flower-admin.frontend/src/api/upload.ts`

**Interfaces:**
- Consumes: existing `src/api/client.ts` (axios instance)
- Produces: typed API functions consumed by all page components

- [ ] **Step 1: Create `src/types/product.ts`**

```typescript
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
  isActive?: boolean
  flowerMeaning?: string
  origin?: string
  careInstruction?: string
  newImages?: string[]
}

export interface UpdateProductRequest extends CreateProductRequest {
  id: number
}

export interface PagedResponse<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}
```

- [ ] **Step 2: Create `src/types/category.ts`**

```typescript
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

- [ ] **Step 3: Create `src/api/products.ts`**

```typescript
import { apiClient } from './client'
import type { Product, CreateProductRequest, UpdateProductRequest, PagedResponse } from '@/types/product'

export interface ProductListParams {
  page?: number
  pageSize?: number
  categoryProductId?: number | null
  minPrice?: number | null
  maxPrice?: number | null
}

export const productsApi = {
  getPaged(params: ProductListParams = {}) {
    return apiClient.get<PagedResponse<Product>>('/api/Products/paged', { params })
  },

  getById(id: number) {
    return apiClient.get<Product>(`/api/Products/${id}`)
  },

  search(query: string) {
    return apiClient.get<Product[]>('/api/Products/search', { params: { query } })
  },

  create(data: CreateProductRequest) {
    return apiClient.post<Product>('/api/Products', data)
  },

  update(id: number, data: UpdateProductRequest) {
    return apiClient.put(`/api/Products/${id}`, data)
  },

  delete(id: number) {
    return apiClient.delete(`/api/Products/${id}`)
  },

  getImages(productId: number) {
    return apiClient.get(`/api/Products/${productId}/images`)
  },

  addImage(productId: number, imageUrl: string) {
    return apiClient.post(`/api/Products/${productId}/images`, { imageUrl })
  },

  deleteImage(productId: number, imageId: number) {
    return apiClient.delete(`/api/Products/${productId}/images/${imageId}`)
  },
}
```

- [ ] **Step 4: Create `src/api/categories.ts`**

```typescript
import { apiClient } from './client'
import type { CategoryProduct, CreateCategoryRequest, UpdateCategoryRequest } from '@/types/category'

export const categoriesApi = {
  getAll() {
    return apiClient.get<CategoryProduct[]>('/api/CategoriesProducts')
  },

  getById(id: number) {
    return apiClient.get<CategoryProduct>(`/api/CategoriesProducts/${id}`)
  },

  create(data: CreateCategoryRequest) {
    return apiClient.post<CategoryProduct>('/api/CategoriesProducts', data)
  },

  update(id: number, data: UpdateCategoryRequest) {
    return apiClient.put(`/api/CategoriesProducts/${id}`, data)
  },

  delete(id: number) {
    return apiClient.delete(`/api/CategoriesProducts/${id}`)
  },
}
```

- [ ] **Step 5: Create `src/api/upload.ts`**

```typescript
import { apiClient } from './client'

export const uploadApi = {
  upload(file: File) {
    const formData = new FormData()
    formData.append('file', file)
    return apiClient.post<{ url: string }>('/api/Upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },
}
```

- [ ] **Step 6: Verify build**

```bash
npm run build
```
Expected: 0 errors

- [ ] **Step 7: Commit**

```bash
git add flower-admin.frontend/src/types/product.ts
git add flower-admin.frontend/src/types/category.ts
git add flower-admin.frontend/src/api/products.ts
git add flower-admin.frontend/src/api/categories.ts
git add flower-admin.frontend/src/api/upload.ts
git commit -m "feat: add product/category types and API functions"
```

---

