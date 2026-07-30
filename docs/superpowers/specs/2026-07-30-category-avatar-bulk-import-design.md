# Category Image, Bulk Import & Avatar Design

**Date:** 2026-07-30
**Status:** Approved

## Overview

Add image support to product categories, bulk import for categories (Excel+ZIP), and avatar for users/customers across the flower shop admin panel.

---

## Part 1: Backend Entities & DTOs

### CategoryProduct Entity (`Flower.Data/Entities/CategoryProduct.cs`)
- Add `public string? ImageUrl { get; set; }` — `[MaxLength(2000)]`, nullable

### User Entity (`Flower.Data/Entities/User.cs`)
- Add `public string? Avatar { get; set; }` — `[MaxLength(2000)]`, nullable

### Customer Entity (`Flower.Data/Entities/Customer.cs`)
- Add `public string? Avatar { get; set; }` — `[MaxLength(2000)]`, nullable

### DTO Changes

**CategoryProductDTOs.cs:**
- `CategoryProductDTO`: add `ImageUrl`
- `CreateCategoryProductDTO`: add `ImageUrl`
- `UpdateCategoryProductDTO`: add `ImageUrl`

**UserDTOs.cs:**
- `UserDTO`: add `Avatar`
- `CreateUserRequest`: add `Avatar`
- `UpdateUserRequest`: add `Avatar`

**CustomerDTOs.cs:**
- `CustomerDTO`: add `Avatar`

**MappingExtensions.cs:**
- Update `ToDTO()`, `ToEntity()`, `UpdateEntity()` for all 3 entities

### Migration
- `dotnet ef migrations add AddCategoryImageAndAvatar`
- Update `ApplicationDbContextModelSnapshot`

### Cloudinary Folder
- Category images: `{CloudinarySettings.Folder}/categories` (e.g., `flowershop_products/categories`)
- Avatars: `{CloudinarySettings.Folder}/avatars` (e.g., `flowershop_products/avatars`)

---

## Part 2: Bulk Import for Categories

### API Endpoints (in `ImportsController.cs`)

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/imports/categories/upload` | Upload Excel + optional ZIP |
| GET | `/api/imports/categories/template` | Download Excel template |

### ImportService — New Method: `ImportCategoriesAsync`

**Parameters:**
- `Stream excelStream` — .xlsx file stream
- `string? zipPath` — extracted temp dir path (or null)
- `string onDuplicate` — "skip" | "update"

**Excel Template Columns:**

| Col | Field | Required | Notes |
|-----|-------|----------|-------|
| A | STT | No | Row number |
| B | Tên danh mục | Yes | Category name |
| C | Slug | No | Auto-slugify from name if empty |
| D | Mô tả | No | |
| E | File ảnh | No | Filename in ZIP (case-insensitive match) |

**Flow:**
1. Validate file extension (.xlsx)
2. If ZIP: extract to temp dir, index files by lowercase filename (allowed: .jpg, .jpeg, .png, .webp)
3. Load existing categories keyed by lowercase Name
4. Parse Excel rows (row 2+):
   - Trim whitespace from all cells
   - Name required → error if empty
   - Slug: if empty → auto-generate via slugify helper
   - Duplicate Name check: onDuplicate=="skip" → skip, "update" → update in-place
   - ImageFileName: trim + case-insensitive match against ZIP index → upload to Cloudinary `{folder}/categories`
5. Bulk insert/update via `AddRangeAsync` + `SaveChangesAsync`
6. Return `ImportResult` with errors list

**Error Handling:**
- Row-level error reporting: RowIndex, ProductName (category name), ErrorMessage
- Example errors: "Tên danh mục không được để trống", "Không tìm thấy file ảnh 'ABC.jpg' trong ZIP"

### Template File
- `wwwroot/templates/category_import_template.xlsx`
- Include 1-2 rows of sample data

---

## Part 3: Frontend — Category UI

### CategoryDialog (`pages/categories/components/CategoryDialog.tsx`)
- Add image dropzone below form fields
- Use `react-dropzone` + `uploadApi.upload(file)` → `POST /api/Upload`
- Preview: `<img>` with `rounded-md object-cover h-32 w-full`
- If `imageUrl` exists on edit: show current image + delete button
- Accept: `image/*`
- Not required — can leave empty

### CategoryTable (`pages/categories/components/CategoryTable.tsx`)
- Add new first column "Ảnh"
- Thumbnail: `40×40`, `rounded-md`, `object-cover`
- Fallback: `<Folder className="size-5 text-slate-400" />` on `bg-slate-100 rounded-md p-2`
- Mobile: add `overflow-x-auto` to table container

### CategoriesPage (`pages/categories/CategoriesPage.tsx`)
- Add "Nhập hàng loạt" button → navigates to `/products/imports?tab=categories`

### Type Updates (`types/category.ts`)
- `CategoryProduct`: add `imageUrl?: string`
- `CreateCategoryRequest`: add `imageUrl?: string`
- `UpdateCategoryRequest`: add `imageUrl?: string`

---

## Part 4: Frontend — User & Customer Avatar

### Type Updates
- `types/user.ts` — `User`, `CreateUserRequest`, `UpdateUserRequest`: add `avatar?: string`

### UsersPage (`pages/users/UsersPage.tsx`)
- **Table**: add column "Ảnh đại diện"
  - Thumbnail `36×36`, `rounded-full`, `object-cover`
  - Fallback: `<AvatarFallback>` with initials (first char of fullName)
  - Use shadcn `<Avatar>` + `<AvatarImage>` + `<AvatarFallback>`
- **Dialog**: add avatar dropzone
  - Preview: `rounded-full` (circular preview)
  - Same upload pattern: `react-dropzone` + `uploadApi.upload`

### AppHeader (`components/AppHeader.tsx`)
- Update existing `<Avatar>` to use `user.avatar` as `<AvatarImage src>`
- Keep `<AvatarFallback>` for initials when no avatar or load error

### Customer
- Backend entity + DTO only (admin panel has no customer management page yet)

---

## Implementation Order
1. Backend entities + DTOs + migration
2. Backend bulk import service + controller + template
3. Frontend category types + dialog + table
4. Frontend import page tab for categories
5. Frontend user types + avatar in UsersPage
6. Frontend AppHeader avatar
