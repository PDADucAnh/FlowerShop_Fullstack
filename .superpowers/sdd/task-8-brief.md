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
### Task 8: Frontend — Routing + Sidebar + Build Verify

**Files:**
- Modify: `flower-admin.frontend/src/App.tsx`
- Modify: `flower-admin.frontend/src/components/AppSidebar.tsx`
- Possibly delete: `flower-admin.frontend/src/pages/PlaceholderPages.tsx` (remove ProductsPage, keep others)

**Interfaces:**
- Consumes: all page components from Tasks 4-7
- Produces: complete routing tree and sidebar with active state

- [ ] **Step 1: Update `App.tsx` routes**

Add imports and routes for products and categories. Replace the existing Imports section and routes:

```typescript
// imports — add these:
import { ProductsPage } from '@/pages/products/ProductsPage'
import { ProductFormPage } from '@/pages/products/ProductFormPage'
import { CategoriesPage } from '@/pages/categories/CategoriesPage'

// routes — replace the existing products and add categories:
<Route path="products" element={<ProductsPage />} />
<Route path="products/new" element={<ProductFormPage />} />
<Route path="products/:id/edit" element={<ProductFormPage />} />
<Route path="products/categories" element={<CategoriesPage />} />
```

Remove `ProductsPage` import from PlaceholderPages if it was there (it was a placeholder, now replaced by real page).

- [ ] **Step 2: Update `AppSidebar.tsx` — add Categories sub-link under Products**

In the navItems array, change the Products nav item to show a sub-menu, OR add a separate nav item for Categories below Products:

```typescript
// Option: Add a separate nav item
{ label: 'Danh mục', href: '/products/categories', icon: FolderTree },
```

Import `FolderTree` from lucide-react:
```typescript
import { ..., FolderTree } from 'lucide-react'
```

Actually, a simpler approach: keep the Products item as-is, and let users navigate to categories from the products page via the "Quản lý danh mục" button. The sidebar stays clean.

Let's just add a small "Danh mục" entry below Products since the spec mentions it:

```typescript
const navItems: NavItem[] = [
  { label: 'Tổng quan', href: '/', icon: LayoutDashboard },
  { label: 'Sản phẩm', href: '/products', icon: Package },
  { label: 'Danh mục', href: '/products/categories', icon: FolderTree },
  { label: 'Đơn hàng', href: '/orders', icon: ShoppingBag },
  { label: 'Nội dung', href: '/content', icon: FileText },
  { label: 'Marketing', href: '/marketing', icon: Megaphone },
  { label: 'Hệ thống', href: '/system', icon: Settings },
]
```

Add `FolderTree` to the lucide-react import.

- [ ] **Step 3: Clean up old placeholder**

Remove the old `ProductsPage` export from `PlaceholderPages.tsx` (it now has a real implementation).

- [ ] **Step 4: Full build verification**

```bash
npm run build
```
Expected: 0 errors

- [ ] **Step 5: Commit**

```bash
git add flower-admin.frontend/src/App.tsx
git add flower-admin.frontend/src/components/AppSidebar.tsx
git add flower-admin.frontend/src/pages/PlaceholderPages.tsx  # if modified
git commit -m "feat: wire up product/category routes and sidebar"
```

---

## Self-Review Checklist

After writing the plan, verify against the spec:

1. **ProductImage entity** — Task 1, Step 1 ✓
2. **Product.Images navigation** — Task 1, Step 2 ✓
3. **EF migration** — Task 1, Step 6 ✓
4. **ProductImageDTO + UploadImageResponse** — Task 2, Step 1 ✓
5. **Extended CreateProductDTO / UpdateProductDTO** (IsActive, FlowerMeaning, Origin, CareInstruction, NewImages) — Task 2, Steps 2-3 ✓
6. **Updated ProductDTO with Images** — Task 2, Step 4 ✓
7. **Updated mapping extensions** — Task 2, Steps 5-8 ✓
8. **UploadController** — Task 2, Step 12 ✓
9. **Image CRUD endpoints** — Task 2, Step 13 ✓
10. **ProductService.BuildQuery includes Images** — Task 2, Step 9 ✓
11. **ProductService.Create/Update handles NewImages** — Task 2, Steps 10-11 ✓
12. **Frontend types** — Task 3, Steps 1-2 ✓
13. **Frontend API functions** — Task 3, Steps 3-5 ✓
14. **Products DataTable** — Task 4 ✓
15. **Product Create/Edit form + ImageUploader** — Task 5 ✓
16. **Delete product dialog** — Task 6 ✓
17. **Categories CRUD page** — Task 7 ✓
18. **Routing + sidebar** — Task 8 ✓
