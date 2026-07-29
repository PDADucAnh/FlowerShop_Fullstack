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

## File Structure

### Backend — New/Modified Files

| File | Action | Purpose |
|------|--------|---------|
| `Flower.Data/Entities/ProductImage.cs` | Create | Entity for multi-image storage |
| `Flower.Data/Entities/Product.cs` | Modify | Add `Images` navigation property |
| `Flower.Data/IApplicationDbContext.cs` | Modify | Add `DbSet<ProductImage>` |
| `Flower.Data/ApplicationDbContext.cs` | Modify | Add `DbSet<ProductImage>` + EF config |
| (migration) | Auto | EF migration for ProductImage table |
| `Flower.Backend/Models/DTOs/ProductDTOs.cs` | Modify | Add `ProductImageDTO`, `UploadImageResponse`; extend Create/Update DTOs |
| `Flower.Backend/Models/DTOs/MappingExtensions.cs` | Modify | Add ProductImage mapping; update product mapping for Images + new fields |
| `Flower.Backend/Services/ProductService.cs` | Modify | Update BuildQuery (include Images), Create/Update (handle NewImages) |
| `Flower.Backend/Controllers/Api/ProductsController.cs` | Modify | Extend Create/Update for NewImages; add image CRUD endpoints |
| `Flower.Backend/Controllers/Api/UploadController.cs` | Create | Image upload endpoint |
| `Flower.Backend/Program.cs` | Possibly modify | Register `UploadController` route if needed (usually auto from `[Route("api/[controller]")]`) |

### Frontend — New/Modified Files

| File | Action | Purpose |
|------|--------|---------|
| `src/types/product.ts` | Create | Product, ProductImage, CreateProductRequest, UpdateProductRequest |
| `src/types/category.ts` | Create | CategoryProduct, CreateCategoryRequest, UpdateCategoryRequest |
| `src/api/products.ts` | Create | Product API functions (list, detail, create, update, delete, upload, image CRUD) |
| `src/api/categories.ts` | Create | Category API functions |
| `src/pages/products/ProductsPage.tsx` | Create | Products list with DataTable |
| `src/pages/products/ProductFormPage.tsx` | Create | Create/Edit form wrapper |
| `src/pages/products/components/ProductTable.tsx` | Create | Reusable DataTable |
| `src/pages/products/components/ProductForm.tsx` | Create | Product form (shared create/edit) |
| `src/pages/products/components/ImageUploader.tsx` | Create | Multi-image upload zone with previews |
| `src/pages/products/components/DeleteProductDialog.tsx` | Create | Delete confirmation |
| `src/pages/categories/CategoriesPage.tsx` | Create | Categories list + inline CRUD |
| `src/pages/categories/components/CategoryTable.tsx` | Create | Categories table |
| `src/pages/categories/components/CategoryDialog.tsx` | Create | Create/Edit dialog |
| `src/pages/categories/components/DeleteCategoryDialog.tsx` | Create | Delete confirmation |
| `src/App.tsx` | Modify | Add product and category routes |
| `src/components/AppSidebar.tsx` | Modify | Add Categories link under Products |

---

### Task 1: Backend — ProductImage Entity + Migration + DbSet

**Files:**
- Create: `Flower.Data/Entities/ProductImage.cs`
- Modify: `Flower.Data/Entities/Product.cs` (add `Images` nav property)
- Modify: `Flower.Data/IApplicationDbContext.cs` (add DbSet)
- Modify: `Flower.Data/ApplicationDbContext.cs` (add DbSet property + entity config)
- Auto: EF migration

**Interfaces:**
- Consumes: `ApplicationDbContext`, `Product` entity (existing)
- Produces: `ProductImage` entity ready for DTO mapping and DbSet injection

- [ ] **Step 1: Create ProductImage entity**

```csharp
// Flower.Data/Entities/ProductImage.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string ImageUrl { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
```

- [ ] **Step 2: Add Images navigation property to Product**

```csharp
// In Flower.Data/Entities/Product.cs, after the ProductVariants line:
public virtual ICollection<ProductImage>? Images { get; set; }
```

Insert it right after:
```csharp
public virtual ICollection<ProductVariant>? ProductVariants { get; set; }

public virtual ICollection<ProductImage>? Images { get; set; }
```

- [ ] **Step 3: Add DbSet<ProductImage> to IApplicationDbContext**

```csharp
// In Flower.Data/IApplicationDbContext.cs, add:
DbSet<ProductImage> ProductImages { get; set; }
```

- [ ] **Step 4: Add DbSet<ProductImage> to ApplicationDbContext**

```csharp
// In Flower.Data/ApplicationDbContext.cs, add property:
public DbSet<ProductImage> ProductImages { get; set; }
```

- [ ] **Step 5: Configure ProductImage in ApplicationDbContext.OnModelCreating**

Add inside `OnModelCreating`:
```csharp
modelBuilder.Entity<ProductImage>(entity =>
{
    entity.ToTable("ProductImages");

    entity.HasKey(e => e.Id);

    entity.Property(e => e.ImageUrl)
        .IsRequired()
        .HasMaxLength(2000);

    entity.Property(e => e.SortOrder)
        .HasDefaultValue(0);

    entity.HasOne(e => e.Product)
        .WithMany(p => p.Images)
        .HasForeignKey(e => e.ProductId)
        .OnDelete(DeleteBehavior.Cascade);
});
```

- [ ] **Step 6: Create EF migration**

Run:
```bash
dotnet ef migrations add AddProductImages
```

- [ ] **Step 7: Verify build**

```bash
dotnet build
```
Expected: 0 errors, warning count unchanged

- [ ] **Step 8: Commit**

```bash
git add Flower.Data/Entities/ProductImage.cs
git add Flower.Data/Entities/Product.cs
git add Flower.Data/IApplicationDbContext.cs
git add Flower.Data/ApplicationDbContext.cs
git add Flower.Data/Migrations/<new-migration-files>
git commit -m "feat: add ProductImage entity + migration"
```

---

### Task 2: Backend — DTOs + Mappers + Service + Endpoints

**Files:**
- Modify: `Flower.Backend/Models/DTOs/ProductDTOs.cs`
- Modify: `Flower.Backend/Models/DTOs/MappingExtensions.cs`
- Modify: `Flower.Backend/Services/ProductService.cs`
- Create: `Flower.Backend/Controllers/Api/UploadController.cs`
- Modify: `Flower.Backend/Controllers/Api/ProductsController.cs`

**Interfaces:**
- Consumes: `ProductImage` entity (Task 1), `IProductService`, `IPhotoService`
- Produces: extended product DTOs, upload endpoint, product-image association endpoints, updated product service

- [ ] **Step 1: Add new DTOs to ProductDTOs.cs**

Add these classes at the end of `ProductDTOs.cs`:

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

- [ ] **Step 2: Extend CreateProductDTO**

Add these properties to the existing `CreateProductDTO` class:
```csharp
public bool IsActive { get; set; } = true;
[MaxLength(500)]
public string? FlowerMeaning { get; set; }
[MaxLength(200)]
public string? Origin { get; set; }
public string? CareInstruction { get; set; }
public List<string>? NewImages { get; set; }
```

- [ ] **Step 3: Extend UpdateProductDTO**

Add these properties to the existing `UpdateProductDTO` class:
```csharp
public bool IsActive { get; set; } = true;
[MaxLength(500)]
public string? FlowerMeaning { get; set; }
[MaxLength(200)]
public string? Origin { get; set; }
public string? CareInstruction { get; set; }
public List<string>? NewImages { get; set; }
```

- [ ] **Step 4: Add ProductDTO Images property**

Add to existing `ProductDTO` class:
```csharp
public List<ProductImageDTO> Images { get; set; } = new();
public bool IsActive { get; set; }
public string? FlowerMeaning { get; set; }
public string? Origin { get; set; }
public string? CareInstruction { get; set; }
```

- [ ] **Step 5: Update MappingExtensions — ProductImage mapping**

Add these methods to `MappingExtensions`:
```csharp
public static ProductImageDTO ToDTO(this ProductImage image)
{
    if (image == null) return null;
    return new ProductImageDTO
    {
        Id = image.Id,
        ImageUrl = image.ImageUrl,
        SortOrder = image.SortOrder
    };
}
```

- [ ] **Step 6: Update ProductDTO mapping to include Images + new fields**

Replace the existing `ToDTO(this Product product)` method:
```csharp
public static ProductDTO ToDTO(this Product product)
{
    if (product == null) return null;
    return new ProductDTO
    {
        Id = product.Id,
        Sku = product.Sku,
        Name = product.Name ?? "",
        Description = product.Description,
        Slug = product.Slug,
        Price = product.Price,
        DiscountPrice = product.DiscountPrice,
        StockQuantity = product.StockQuantity,
        ImageUrl = product.ImageUrl,
        CategoryProductId = product.CategoryProductId,
        CategoryProductName = product.CategoryProduct?.Name,
        ViewCount = product.ViewCount,
        AddToCartCount = product.AddToCartCount,
        OriginalPrice = product.Price,
        CurrentPrice = product.Price,
        IsFlashSale = false,
        Images = product.Images?.OrderBy(i => i.SortOrder).Select(i => i.ToDTO()).ToList() ?? new(),
        IsActive = product.IsActive,
        FlowerMeaning = product.FlowerMeaning,
        Origin = product.Origin,
        CareInstruction = product.CareInstruction
    };
}
```

- [ ] **Step 7: Update CreateProduct ToEntity mapping**

Replace the existing `ToEntity(this CreateProductDTO dto)`:
```csharp
public static Product ToEntity(this CreateProductDTO dto)
{
    if (dto == null) return null;
    return new Product
    {
        Sku = dto.Sku,
        Name = dto.Name,
        Description = dto.Description,
        Slug = dto.Slug,
        Price = dto.Price,
        StockQuantity = dto.StockQuantity,
        ImageUrl = dto.ImageUrl,
        CategoryProductId = dto.CategoryProductId,
        IsActive = dto.IsActive,
        FlowerMeaning = dto.FlowerMeaning,
        Origin = dto.Origin,
        CareInstruction = dto.CareInstruction
    };
}
```

- [ ] **Step 8: Update UpdateProductDTO UpdateEntity mapping**

Replace the existing `UpdateEntity(this UpdateProductDTO dto, Product entity)`:
```csharp
public static void UpdateEntity(this UpdateProductDTO dto, Product entity)
{
    if (dto == null || entity == null) return;
    entity.Sku = dto.Sku;
    entity.Name = dto.Name;
    entity.Description = dto.Description;
    entity.Slug = dto.Slug;
    entity.Price = dto.Price;
    entity.StockQuantity = dto.StockQuantity;
    entity.ImageUrl = dto.ImageUrl;
    entity.CategoryProductId = dto.CategoryProductId;
    entity.IsActive = dto.IsActive;
    entity.FlowerMeaning = dto.FlowerMeaning;
    entity.Origin = dto.Origin;
    entity.CareInstruction = dto.CareInstruction;
}
```

- [ ] **Step 9: Update ProductService.BuildQuery to include Images**

Replace the existing `BuildQuery()` method:
```csharp
private IQueryable<Product> BuildQuery()
{
    return _context.Products
        .Include(p => p.CategoryProduct)
        .Include(p => p.Images.OrderBy(i => i.SortOrder));
}
```

- [ ] **Step 10: Update ProductService.Create to handle NewImages**

Replace the existing `Create` method:
```csharp
public async Task<ProductDTO> Create(CreateProductDTO dto)
{
    if (string.IsNullOrEmpty(dto.Slug))
    {
        dto.Slug = Flower.Backend.Utils.SlugHelper.GenerateSlug(dto.Name);
    }
    if (string.IsNullOrEmpty(dto.Sku))
    {
        dto.Sku = Flower.Backend.Utils.SlugHelper.GenerateSku(dto.Name);
    }

    var product = dto.ToEntity();
    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    // Associate uploaded images
    if (dto.NewImages != null && dto.NewImages.Count > 0)
    {
        for (int i = 0; i < dto.NewImages.Count; i++)
        {
            _context.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                ImageUrl = dto.NewImages[i],
                SortOrder = i
            });
        }
        await _context.SaveChangesAsync();
    }

    await _context.Entry(product)
        .Reference(p => p.CategoryProduct)
        .LoadAsync();

    await _context.Entry(product)
        .Collection(p => p.Images)
        .LoadAsync();

    return product.ToDTO();
}
```

- [ ] **Step 11: Update ProductService.Update to handle NewImages**

Replace the existing `Update` method:
```csharp
public async Task<bool> Update(int id, UpdateProductDTO dto)
{
    if (id != dto.Id)
        return false;

    var product = await _context.Products
        .Include(p => p.Images)
        .FirstOrDefaultAsync(p => p.Id == id);
    if (product == null)
        return false;

    if (string.IsNullOrEmpty(dto.ImageUrl))
    {
        dto.ImageUrl = product.ImageUrl;
    }

    dto.UpdateEntity(product);

    // Append new images
    if (dto.NewImages != null && dto.NewImages.Count > 0)
    {
        var maxSort = product.Images?.Any() == true
            ? product.Images.Max(i => i.SortOrder)
            : 0;
        for (int i = 0; i < dto.NewImages.Count; i++)
        {
            _context.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                ImageUrl = dto.NewImages[i],
                SortOrder = maxSort + 1 + i
            });
        }
    }

    try
    {
        await _context.SaveChangesAsync();
        return true;
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!await _context.Products.AnyAsync(e => e.Id == id))
            return false;
        throw;
    }
}
```

- [ ] **Step 12: Create UploadController**

```csharp
// Flower.Backend/Controllers/Api/UploadController.cs
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize(Policy = "StaffOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IPhotoService photoService, ILogger<UploadController> logger)
        {
            _photoService = photoService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn file ảnh" });

            try
            {
                var validateStream = file.OpenReadStream();
                using var _ = Image.Load(validateStream);
            }
            catch
            {
                return BadRequest(new { message = "File không hợp lệ. Chỉ chấp nhận file ảnh." });
            }

            var url = await _photoService.UploadPhotoAsync(file);
            if (string.IsNullOrEmpty(url))
            {
                return StatusCode(500, new { message = "Upload ảnh thất bại. Vui lòng thử lại." });
            }

            return Ok(new UploadImageResponse { Url = url });
        }
    }
}
```

- [ ] **Step 13: Add image CRUD endpoints to ProductsController**

Add these methods to the existing `ProductsController`:

```csharp
[HttpGet("{id}/images")]
public async Task<IActionResult> GetImages(int id)
{
    var product = await _context.Products
        .Include(p => p.Images.OrderBy(i => i.SortOrder))
        .FirstOrDefaultAsync(p => p.Id == id);

    if (product == null)
        return NotFound();

    return Ok(product.Images.Select(i => i.ToDTO()).ToList());
}

[HttpPost("{id}/images")]
public async Task<IActionResult> AddImage(int id, [FromBody] AddProductImageRequest request)
{
    var product = await _context.Products.FindAsync(id);
    if (product == null)
        return NotFound();

    var maxSort = await _context.ProductImages
        .Where(i => i.ProductId == id)
        .MaxAsync(i => (int?)i.SortOrder) ?? -1;

    var image = new ProductImage
    {
        ProductId = id,
        ImageUrl = request.ImageUrl,
        SortOrder = maxSort + 1
    };

    _context.ProductImages.Add(image);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetDetail), new { id }, image.ToDTO());
}

[HttpDelete("{id}/images/{imageId}")]
public async Task<IActionResult> DeleteImage(int id, int imageId)
{
    var image = await _context.ProductImages
        .FirstOrDefaultAsync(i => i.Id == imageId && i.ProductId == id);

    if (image == null)
        return NotFound();

    _context.ProductImages.Remove(image);
    await _context.SaveChangesAsync();

    return NoContent();
}
```

Also add the `AddProductImageRequest` DTO to `ProductDTOs.cs`:
```csharp
public class AddProductImageRequest
{
    [Required]
    public string ImageUrl { get; set; }
}
```

- [ ] **Step 14: Verify build**

```bash
dotnet build
```
Expected: 0 errors

- [ ] **Step 15: Commit**

```bash
git add Flower.Backend/Models/DTOs/ProductDTOs.cs
git add Flower.Backend/Models/DTOs/MappingExtensions.cs
git add Flower.Backend/Services/ProductService.cs
git add Flower.Backend/Services/Interfaces/IProductService.cs  # if modified
git add Flower.Backend/Controllers/Api/UploadController.cs
git add Flower.Backend/Controllers/Api/ProductsController.cs
git commit -m "feat: extend product DTOs, add image upload/association endpoints"
```

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

### Task 4: Frontend — Products DataTable Page

**Files:**
- Create: `flower-admin.frontend/src/pages/products/ProductsPage.tsx`
- Create: `flower-admin.frontend/src/pages/products/components/ProductTable.tsx`

**Interfaces:**
- Consumes: `productsApi`, `categoriesApi`, `Product`, `CategoryProduct`, `PagedResponse`
- Produces: `/products` route content (replace placeholder)

- [ ] **Step 1: Create `ProductTable.tsx`**

```typescript
import { useNavigate } from 'react-router-dom'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Pencil, Trash2 } from 'lucide-react'
import type { Product } from '@/types/product'

interface ProductTableProps {
  products: Product[]
  onDelete: (product: Product) => void
}

export function ProductTable({ products, onDelete }: ProductTableProps) {
  const navigate = useNavigate()

  const stockBadge = (qty: number) => {
    if (qty === 0) return <Badge variant="destructive">Hết hàng</Badge>
    if (qty <= 5) return <Badge className="bg-amber-100 text-amber-800 hover:bg-amber-100">{qty}</Badge>
    return <Badge className="bg-green-100 text-green-800 hover:bg-green-100">{qty}</Badge>
  }

  const formatPrice = (price: number) =>
    new Intl.NumberFormat('vi-VN').format(price) + '₫'

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-12">Ảnh</TableHead>
          <TableHead>Tên sản phẩm</TableHead>
          <TableHead>SKU</TableHead>
          <TableHead>Danh mục</TableHead>
          <TableHead className="text-right">Giá</TableHead>
          <TableHead className="text-center">Tồn kho</TableHead>
          <TableHead className="text-center">Trạng thái</TableHead>
          <TableHead className="w-24">Thao tác</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {products.map((product) => (
          <TableRow key={product.id}>
            <TableCell>
              <img
                src={product.images?.[0]?.imageUrl || product.imageUrl || '/placeholder.svg'}
                alt={product.name}
                className="size-10 rounded-md object-cover"
              />
            </TableCell>
            <TableCell className="font-medium">{product.name}</TableCell>
            <TableCell className="text-muted-foreground">{product.sku || '—'}</TableCell>
            <TableCell>{product.categoryProductName || '—'}</TableCell>
            <TableCell className="text-right font-mono">{formatPrice(product.price)}</TableCell>
            <TableCell className="text-center">{stockBadge(product.stockQuantity)}</TableCell>
            <TableCell className="text-center">
              <Badge variant={product.isActive ? 'default' : 'outline'}>
                {product.isActive ? 'Đang bán' : 'Ngừng bán'}
              </Badge>
            </TableCell>
            <TableCell>
              <div className="flex items-center gap-1">
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => navigate(`/products/${product.id}/edit`)}
                >
                  <Pencil className="size-4" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => onDelete(product)}
                >
                  <Trash2 className="size-4 text-destructive" />
                </Button>
              </div>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
```

- [ ] **Step 2: Create `ProductsPage.tsx`**

```typescript
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { productsApi } from '@/api/products'
import { categoriesApi } from '@/api/categories'
import { ProductTable } from './components/ProductTable'
import { DeleteProductDialog } from './components/DeleteProductDialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Plus, Search, Loader2, AlertCircle } from 'lucide-react'
import type { Product } from '@/types/product'

export function ProductsPage() {
  const navigate = useNavigate()
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [categoryFilter, setCategoryFilter] = useState<string>('all')
  const [deleteTarget, setDeleteTarget] = useState<Product | null>(null)
  const pageSize = 20

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoriesApi.getAll().then((r) => r.data),
  })

  const { data, isLoading, error } = useQuery({
    queryKey: ['products', page, categoryFilter],
    queryFn: () =>
      productsApi.getPaged({
        page,
        pageSize,
        categoryProductId: categoryFilter === 'all' ? null : Number(categoryFilter),
      }).then((r) => r.data),
  })

  const handleSearch = () => {
    if (!search.trim()) return
    navigate(`/products?search=${encodeURIComponent(search)}`)
  }

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không thể tải danh sách sản phẩm</p>
        <Button variant="outline" onClick={() => window.location.reload()}>
          Thử lại
        </Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Sản phẩm</h1>
        <Button onClick={() => navigate('/products/new')}>
          <Plus className="mr-2 size-4" />
          Thêm sản phẩm
        </Button>
      </div>

      <div className="flex items-center gap-3">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Tìm kiếm sản phẩm…"
            className="pl-9"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
          />
        </div>
        <Select value={categoryFilter} onValueChange={(v) => { setCategoryFilter(v); setPage(1) }}>
          <SelectTrigger className="w-48">
            <SelectValue placeholder="Tất cả danh mục" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">Tất cả danh mục</SelectItem>
            {categories?.map((cat) => (
              <SelectItem key={cat.id} value={String(cat.id)}>
                {cat.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">
            {data ? `${data.totalCount} sản phẩm` : ''}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {data && data.items.length > 0 ? (
            <div>
              <ProductTable
                products={data.items}
                onDelete={setDeleteTarget}
              />
              {/* Pagination */}
              {(data.totalPages ?? 0) > 1 && (
                <div className="flex items-center justify-between border-t px-4 py-3">
                  <p className="text-sm text-muted-foreground">
                    Trang {data.page} / {data.totalPages}
                  </p>
                  <div className="flex gap-2">
                    <Button
                      variant="outline"
                      size="sm"
                      disabled={page <= 1}
                      onClick={() => setPage((p) => Math.max(1, p - 1))}
                    >
                      Trước
                    </Button>
                    <Button
                      variant="outline"
                      size="sm"
                      disabled={page >= (data.totalPages ?? 1)}
                      onClick={() => setPage((p) => p + 1)}
                    >
                      Sau
                    </Button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="flex h-48 flex-col items-center justify-center text-muted-foreground">
              <p>Chưa có sản phẩm nào</p>
              <Button variant="link" onClick={() => navigate('/products/new')}>
                Thêm sản phẩm đầu tiên
              </Button>
            </div>
          )}
        </CardContent>
      </Card>

      <DeleteProductDialog
        product={deleteTarget}
        open={!!deleteTarget}
        onOpenChange={(open) => !open && setDeleteTarget(null)}
        onDeleted={() => {
          setDeleteTarget(null)
          // Refetch will happen automatically via query invalidation in the dialog
        }}
      />
    </div>
  )
}
```

- [ ] **Step 3: Create the Select shadcn component** (needed by ProductsPage)

```bash
npx shadcn@canary add select -y
```

- [ ] **Step 4: Verify build**

```bash
npm run build
```
Expected: 0 errors (the DeleteProductDialog import will error since it doesn't exist yet — we'll handle this in Task 6; temporarily comment out the import or use a minimal placeholder)

- [ ] **Step 5: Commit**

```bash
git add flower-admin.frontend/src/pages/products/
git add flower-admin.frontend/src/components/ui/select.tsx
git commit -m "feat: add products list page with DataTable"
```

---

### Task 5: Frontend — Product Create/Edit Form + ImageUploader

**Files:**
- Create: `flower-admin.frontend/src/pages/products/ProductFormPage.tsx`
- Create: `flower-admin.frontend/src/pages/products/components/ProductForm.tsx`
- Create: `flower-admin.frontend/src/pages/products/components/ImageUploader.tsx`

**Interfaces:**
- Consumes: `productsApi`, `categoriesApi`, `uploadApi`, `Product`, `CategoryProduct`
- Produces: `/products/new` and `/products/:id/edit` pages

- [ ] **Step 1: Create `ImageUploader.tsx`**

```typescript
import { useState, useCallback } from 'react'
import { useDropzone } from 'react-dropzone'
import { uploadApi } from '@/api/upload'
import { Button } from '@/components/ui/button'
import { X, Upload, Loader2 } from 'lucide-react'
import { toast } from 'sonner'
import type { ProductImage } from '@/types/product'

interface ImageItem {
  id: string
  url: string
  isExisting: boolean
  existingId?: number
  uploading?: boolean
}

interface ImageUploaderProps {
  existingImages?: ProductImage[]
  onImagesChange: (urls: string[]) => void
  onDeleteExisting?: (imageId: number) => void
}

export function ImageUploader({ existingImages = [], onImagesChange, onDeleteExisting }: ImageUploaderProps) {
  const [images, setImages] = useState<ImageItem[]>(
    existingImages.map((img) => ({
      id: `existing-${img.id}`,
      url: img.imageUrl,
      isExisting: true,
      existingId: img.id,
    }))
  )

  const onDrop = useCallback(async (acceptedFiles: File[]) => {
    const newUrls: string[] = []

    for (const file of acceptedFiles) {
      const tempId = `uploading-${Date.now()}-${Math.random()}`
      setImages((prev) => [
        ...prev,
        { id: tempId, url: '', isExisting: false, uploading: true },
      ])

      try {
        const { data } = await uploadApi.upload(file)
        newUrls.push(data.url)
        setImages((prev) =>
          prev.map((img) =>
            img.id === tempId
              ? { ...img, url: data.url, uploading: false }
              : img
          )
        )
      } catch {
        toast.error(`Tải ảnh thất bại: ${file.name}`)
        setImages((prev) => prev.filter((img) => img.id !== tempId))
      }
    }

    onImagesChange(newUrls)
  }, [onImagesChange])

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: { 'image/*': ['.png', '.jpg', '.jpeg', '.gif', '.webp'] },
    maxSize: 5 * 1024 * 1024, // 5MB
  })

  const removeImage = (item: ImageItem) => {
    if (item.isExisting && item.existingId && onDeleteExisting) {
      onDeleteExisting(item.existingId)
    }
    setImages((prev) => prev.filter((img) => img.id !== item.id))
  }

  return (
    <div className="space-y-3">
      <div
        {...getRootProps()}
        className={`flex cursor-pointer flex-col items-center justify-center rounded-lg border-2 border-dashed p-6 transition-colors ${
          isDragActive
            ? 'border-primary bg-primary/5'
            : 'border-muted-foreground/25 hover:border-primary/50'
        }`}
      >
        <input {...getInputProps()} />
        <Upload className="mb-2 size-8 text-muted-foreground" />
        <p className="text-sm text-muted-foreground">
          {isDragActive
            ? 'Thả ảnh vào đây…'
            : 'Kéo thả ảnh vào đây hoặc nhấn để chọn'}
        </p>
        <p className="mt-1 text-xs text-muted-foreground">
          PNG, JPG, WebP tối đa 5MB
        </p>
      </div>

      {images.length > 0 && (
        <div className="grid grid-cols-4 gap-3 sm:grid-cols-6 md:grid-cols-8">
          {images.map((item) => (
            <div key={item.id} className="group relative aspect-square">
              {item.uploading ? (
                <div className="flex h-full items-center justify-center rounded-lg border bg-muted">
                  <Loader2 className="size-5 animate-spin text-muted-foreground" />
                </div>
              ) : (
                <img
                  src={item.url}
                  alt=""
                  className="h-full w-full rounded-lg border object-cover"
                />
              )}
              {!item.uploading && (
                <button
                  type="button"
                  onClick={() => removeImage(item)}
                  className="absolute -right-1.5 -top-1.5 flex size-5 items-center justify-center rounded-full bg-destructive text-destructive-foreground shadow transition-opacity opacity-0 group-hover:opacity-100"
                >
                  <X className="size-3" />
                </button>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
```

- [ ] **Step 2: Install react-dropzone**

```bash
npm install react-dropzone
```

- [ ] **Step 3: Add @types/react-dropzone if needed**

```bash
npm install -D @types/react-dropzone
```
(May not be needed with newer TypeScript — skip if `npm run build` passes without it)

- [ ] **Step 4: Create `ProductForm.tsx`**

```typescript
import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { productsApi } from '@/api/products'
import { categoriesApi } from '@/api/categories'
import { ImageUploader } from './ImageUploader'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { Switch } from '@/components/ui/switch'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { toast } from 'sonner'
import { Loader2, ArrowLeft } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import type { Product, CreateProductRequest } from '@/types/product'

interface ProductFormProps {
  product?: Product | null
}

function generateSlug(name: string): string {
  return name
    .toLowerCase()
    .replace(/đ/g, 'd')
    .replace(/[^a-z0-9\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
    .trim()
}

function generateSku(name: string): string {
  const prefix = name
    .split(' ')
    .map((w) => w[0])
    .join('')
    .toUpperCase()
    .slice(0, 5)
  const timestamp = Date.now().toString().slice(-6)
  return `SP-${prefix}-${timestamp}`
}

export function ProductForm({ product }: ProductFormProps) {
  const navigate = useNavigate()
  const isEditing = !!product
  const [saving, setSaving] = useState(false)
  const [newImageUrls, setNewImageUrls] = useState<string[]>([])

  const [form, setForm] = useState({
    name: '',
    slug: '',
    sku: '',
    price: 0,
    stockQuantity: 0,
    categoryProductId: 0,
    isActive: true,
    description: '',
    flowerMeaning: '',
    origin: '',
    careInstruction: '',
  })

  useEffect(() => {
    if (product) {
      setForm({
        name: product.name,
        slug: product.slug || '',
        sku: product.sku || '',
        price: product.price,
        stockQuantity: product.stockQuantity,
        categoryProductId: product.categoryProductId,
        isActive: product.isActive,
        description: product.description || '',
        flowerMeaning: product.flowerMeaning || '',
        origin: product.origin || '',
        careInstruction: product.careInstruction || '',
      })
    }
  }, [product])

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoriesApi.getAll().then((r) => r.data),
  })

  const handleNameChange = (name: string) => {
    setForm((prev) => ({
      ...prev,
      name,
      slug: isEditing ? prev.slug : generateSlug(name),
      sku: isEditing ? prev.sku : prev.sku || generateSku(name),
    }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!form.name || !form.categoryProductId || form.price <= 0) {
      toast.error('Vui lòng điền đầy đủ thông tin bắt buộc')
      return
    }

    setSaving(true)
    try {
      const payload: CreateProductRequest = {
        name: form.name,
        slug: form.slug || undefined,
        sku: form.sku || undefined,
        price: form.price,
        stockQuantity: form.stockQuantity,
        categoryProductId: form.categoryProductId,
        isActive: form.isActive,
        description: form.description || undefined,
        flowerMeaning: form.flowerMeaning || undefined,
        origin: form.origin || undefined,
        careInstruction: form.careInstruction || undefined,
        newImages: newImageUrls.length > 0 ? newImageUrls : undefined,
      }

      if (isEditing && product) {
        await productsApi.update(product.id, { ...payload, id: product.id })
        toast.success('Cập nhật sản phẩm thành công')
      } else {
        await productsApi.create(payload)
        toast.success('Thêm sản phẩm thành công')
      }

      navigate('/products')
    } catch {
      toast.error(isEditing ? 'Cập nhật thất bại' : 'Thêm sản phẩm thất bại')
    } finally {
      setSaving(false)
    }
  }

  const handleDeleteExistingImage = async (imageId: number) => {
    if (!product) return
    try {
      await productsApi.deleteImage(product.id, imageId)
    } catch {
      toast.error('Xóa ảnh thất bại')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="icon" onClick={() => navigate('/products')}>
          <ArrowLeft className="size-5" />
        </Button>
        <h1 className="text-2xl font-semibold">
          {isEditing ? 'Chỉnh sửa sản phẩm' : 'Thêm sản phẩm'}
        </h1>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        <Card>
          <CardHeader>
            <CardTitle className="text-base">Thông tin cơ bản</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="name">Tên sản phẩm *</Label>
                <Input
                  id="name"
                  value={form.name}
                  onChange={(e) => handleNameChange(e.target.value)}
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="category">Danh mục *</Label>
                <Select
                  value={String(form.categoryProductId)}
                  onValueChange={(v) =>
                    setForm((prev) => ({ ...prev, categoryProductId: Number(v) }))
                  }
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Chọn danh mục" />
                  </SelectTrigger>
                  <SelectContent>
                    {categories?.map((cat) => (
                      <SelectItem key={cat.id} value={String(cat.id)}>
                        {cat.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>

            <div className="grid gap-4 md:grid-cols-3">
              <div className="space-y-2">
                <Label htmlFor="slug">Slug</Label>
                <Input
                  id="slug"
                  value={form.slug}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, slug: e.target.value }))
                  }
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="sku">SKU</Label>
                <Input
                  id="sku"
                  value={form.sku}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, sku: e.target.value }))
                  }
                />
              </div>
              <div className="flex items-end gap-2">
                <div className="flex-1 space-y-2">
                  <Label htmlFor="isActive">Trạng thái</Label>
                  <div className="flex items-center gap-2 rounded-lg border px-3 py-2">
                    <Switch
                      id="isActive"
                      checked={form.isActive}
                      onCheckedChange={(v) =>
                        setForm((prev) => ({ ...prev, isActive: v }))
                      }
                    />
                    <Label htmlFor="isActive" className="cursor-pointer">
                      {form.isActive ? 'Đang bán' : 'Ngừng bán'}
                    </Label>
                  </div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-base">Giá & Tồn kho</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="price">Giá (VNĐ) *</Label>
              <Input
                id="price"
                type="number"
                min={0}
                value={form.price}
                onChange={(e) =>
                  setForm((prev) => ({ ...prev, price: Number(e.target.value) }))
                }
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="stockQuantity">Số lượng tồn</Label>
              <Input
                id="stockQuantity"
                type="number"
                min={0}
                value={form.stockQuantity}
                onChange={(e) =>
                  setForm((prev) => ({
                    ...prev,
                    stockQuantity: Number(e.target.value),
                  }))
                }
              />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-base">Hình ảnh</CardTitle>
          </CardHeader>
          <CardContent>
            <ImageUploader
              existingImages={product?.images || []}
              onImagesChange={setNewImageUrls}
              onDeleteExisting={handleDeleteExistingImage}
            />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-base">Mô tả & Thông tin thêm</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="description">Mô tả</Label>
              <Textarea
                id="description"
                rows={4}
                value={form.description}
                onChange={(e) =>
                  setForm((prev) => ({ ...prev, description: e.target.value }))
                }
              />
            </div>
            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="flowerMeaning">Ý nghĩa hoa</Label>
                <Input
                  id="flowerMeaning"
                  value={form.flowerMeaning}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, flowerMeaning: e.target.value }))
                  }
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="origin">Xuất xứ</Label>
                <Input
                  id="origin"
                  value={form.origin}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, origin: e.target.value }))
                  }
                />
              </div>
            </div>
            <div className="space-y-2">
              <Label htmlFor="careInstruction">Hướng dẫn chăm sóc</Label>
              <Textarea
                id="careInstruction"
                rows={3}
                value={form.careInstruction}
                onChange={(e) =>
                  setForm((prev) => ({ ...prev, careInstruction: e.target.value }))
                }
              />
            </div>
          </CardContent>
        </Card>

        <div className="flex items-center justify-end gap-3">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate('/products')}
          >
            Hủy
          </Button>
          <Button type="submit" disabled={saving}>
            {saving && <Loader2 className="mr-2 size-4 animate-spin" />}
            {isEditing ? 'Cập nhật' : 'Thêm sản phẩm'}
          </Button>
        </div>
      </form>
    </div>
  )
}
```

- [ ] **Step 5: Add missing shadcn components**

```bash
npx shadcn@canary add textarea switch label -y
```

- [ ] **Step 6: Create `ProductFormPage.tsx`**

```typescript
import { useParams } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { productsApi } from '@/api/products'
import { ProductForm } from './components/ProductForm'
import { Loader2 } from 'lucide-react'

export function ProductFormPage() {
  const { id } = useParams()
  const isEditing = !!id

  const { data: product, isLoading } = useQuery({
    queryKey: ['product', id],
    queryFn: () => productsApi.getById(Number(id)).then((r) => r.data),
    enabled: isEditing,
  })

  if (isEditing && isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  return <ProductForm product={product ?? null} />
}
```

- [ ] **Step 7: Add `useBlocker` for unsaved changes guard (minimal)**

Add a simple `useEffect` with `beforeunload` in `ProductForm.tsx`:
```typescript
// At the top of ProductForm component, add:
useEffect(() => {
  const handler = (e: BeforeUnloadEvent) => {
    e.preventDefault()
    e.returnValue = ''
  }
  window.addEventListener('beforeunload', handler)
  return () => window.removeEventListener('beforeunload', handler)
}, [])
```

- [ ] **Step 8: Verify build**

```bash
npm run build
```
Expected: 0 errors

- [ ] **Step 9: Commit**

```bash
git add flower-admin.frontend/src/pages/products/ProductFormPage.tsx
git add flower-admin.frontend/src/pages/products/components/ProductForm.tsx
git add flower-admin.frontend/src/pages/products/components/ImageUploader.tsx
git commit -m "feat: add product create/edit form with multi-image upload"
```

---

### Task 6: Frontend — Delete Product Dialog

**Files:**
- Create: `flower-admin.frontend/src/pages/products/components/DeleteProductDialog.tsx`

**Interfaces:**
- Consumes: `productsApi`, `Product`
- Produces: delete confirmation dialog used by ProductsPage

- [ ] **Step 1: Create `DeleteProductDialog.tsx`**

```typescript
import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { productsApi } from '@/api/products'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { toast } from 'sonner'
import { Loader2, AlertTriangle } from 'lucide-react'
import type { Product } from '@/types/product'

interface DeleteProductDialogProps {
  product: Product | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onDeleted?: () => void
}

export function DeleteProductDialog({
  product,
  open,
  onOpenChange,
  onDeleted,
}: DeleteProductDialogProps) {
  const queryClient = useQueryClient()

  const deleteMutation = useMutation({
    mutationFn: (id: number) => productsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] })
      toast.success('Đã xóa sản phẩm')
      onOpenChange(false)
      onDeleted?.()
    },
    onError: () => {
      toast.error('Xóa sản phẩm thất bại')
    },
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <div className="flex items-center gap-2">
            <AlertTriangle className="size-5 text-destructive" />
            <DialogTitle>Xóa sản phẩm</DialogTitle>
          </div>
          <DialogDescription>
            Bạn có chắc chắn muốn xóa sản phẩm này? Hành động này không thể hoàn tác.
          </DialogDescription>
        </DialogHeader>

        {product && (
          <div className="flex items-center gap-3 rounded-lg border bg-muted/50 p-3">
            <img
              src={product.images?.[0]?.imageUrl || product.imageUrl || '/placeholder.svg'}
              alt={product.name}
              className="size-12 rounded-md object-cover"
            />
            <div>
              <p className="font-medium">{product.name}</p>
              <p className="text-sm text-muted-foreground">SKU: {product.sku || '—'}</p>
            </div>
          </div>
        )}

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={deleteMutation.isPending}
          >
            Hủy
          </Button>
          <Button
            variant="destructive"
            onClick={() => product && deleteMutation.mutate(product.id)}
            disabled={deleteMutation.isPending}
          >
            {deleteMutation.isPending && (
              <Loader2 className="mr-2 size-4 animate-spin" />
            )}
            Xóa
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
```

- [ ] **Step 2: Verify build**

```bash
npm run build
```
Expected: 0 errors

- [ ] **Step 3: Commit**

```bash
git add flower-admin.frontend/src/pages/products/components/DeleteProductDialog.tsx
git commit -m "feat: add delete product confirmation dialog"
```

---

### Task 7: Frontend — Categories CRUD Page

**Files:**
- Create: `flower-admin.frontend/src/pages/categories/CategoriesPage.tsx`
- Create: `flower-admin.frontend/src/pages/categories/components/CategoryTable.tsx`
- Create: `flower-admin.frontend/src/pages/categories/components/CategoryDialog.tsx`
- Create: `flower-admin.frontend/src/pages/categories/components/DeleteCategoryDialog.tsx`

**Interfaces:**
- Consumes: `categoriesApi`, `CategoryProduct`
- Produces: `/products/categories` page with inline CRUD

- [ ] **Step 1: Create `CategoryTable.tsx`**

```typescript
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Pencil, Trash2 } from 'lucide-react'
import type { CategoryProduct } from '@/types/category'

interface CategoryTableProps {
  categories: CategoryProduct[]
  onEdit: (category: CategoryProduct) => void
  onDelete: (category: CategoryProduct) => void
}

export function CategoryTable({ categories, onEdit, onDelete }: CategoryTableProps) {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-16">ID</TableHead>
          <TableHead>Tên danh mục</TableHead>
          <TableHead>Mô tả</TableHead>
          <TableHead>Slug</TableHead>
          <TableHead className="w-24">Thao tác</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {categories.map((cat) => (
          <TableRow key={cat.id}>
            <TableCell className="text-muted-foreground">{cat.id}</TableCell>
            <TableCell className="font-medium">{cat.name}</TableCell>
            <TableCell className="text-muted-foreground max-w-xs truncate">
              {cat.description || '—'}
            </TableCell>
            <TableCell className="text-muted-foreground">{cat.slug || '—'}</TableCell>
            <TableCell>
              <div className="flex items-center gap-1">
                <Button variant="ghost" size="icon" onClick={() => onEdit(cat)}>
                  <Pencil className="size-4" />
                </Button>
                <Button variant="ghost" size="icon" onClick={() => onDelete(cat)}>
                  <Trash2 className="size-4 text-destructive" />
                </Button>
              </div>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
```

- [ ] **Step 2: Create `CategoryDialog.tsx`**

```typescript
import { useState, useEffect } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { categoriesApi } from '@/api/categories'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { toast } from 'sonner'
import { Loader2 } from 'lucide-react'
import type { CategoryProduct } from '@/types/category'

interface CategoryDialogProps {
  category: CategoryProduct | null
  open: boolean
  onOpenChange: (open: boolean) => void
}

function generateSlug(name: string): string {
  return name
    .toLowerCase()
    .replace(/đ/g, 'd')
    .replace(/[^a-z0-9\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
    .trim()
}

export function CategoryDialog({ category, open, onOpenChange }: CategoryDialogProps) {
  const queryClient = useQueryClient()
  const isEditing = !!category
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [slug, setSlug] = useState('')

  useEffect(() => {
    if (category) {
      setName(category.name)
      setDescription(category.description || '')
      setSlug(category.slug || '')
    } else {
      setName('')
      setDescription('')
      setSlug('')
    }
  }, [category, open])

  const mutation = useMutation({
    mutationFn: () => {
      const payload = { name, description, slug }
      return isEditing
        ? categoriesApi.update(category!.id, { ...payload, id: category!.id })
        : categoriesApi.create(payload)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
      toast.success(isEditing ? 'Cập nhật danh mục thành công' : 'Thêm danh mục thành công')
      onOpenChange(false)
    },
    onError: () => {
      toast.error(isEditing ? 'Cập nhật thất bại' : 'Thêm danh mục thất bại')
    },
  })

  const handleNameChange = (value: string) => {
    setName(value)
    if (!isEditing && !slug) {
      setSlug(generateSlug(value))
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>
            {isEditing ? 'Chỉnh sửa danh mục' : 'Thêm danh mục'}
          </DialogTitle>
        </DialogHeader>
        <div className="space-y-4 py-2">
          <div className="space-y-2">
            <Label htmlFor="cat-name">Tên danh mục *</Label>
            <Input
              id="cat-name"
              value={name}
              onChange={(e) => handleNameChange(e.target.value)}
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="cat-slug">Slug</Label>
            <Input
              id="cat-slug"
              value={slug}
              onChange={(e) => setSlug(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="cat-desc">Mô tả</Label>
            <Textarea
              id="cat-desc"
              rows={3}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Hủy
          </Button>
          <Button
            onClick={() => mutation.mutate()}
            disabled={!name || mutation.isPending}
          >
            {mutation.isPending && <Loader2 className="mr-2 size-4 animate-spin" />}
            {isEditing ? 'Cập nhật' : 'Thêm'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
```

- [ ] **Step 3: Create `DeleteCategoryDialog.tsx`**

```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { categoriesApi } from '@/api/categories'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { toast } from 'sonner'
import { Loader2, AlertTriangle } from 'lucide-react'
import type { CategoryProduct } from '@/types/category'

interface DeleteCategoryDialogProps {
  category: CategoryProduct | null
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function DeleteCategoryDialog({
  category,
  open,
  onOpenChange,
}: DeleteCategoryDialogProps) {
  const queryClient = useQueryClient()

  const deleteMutation = useMutation({
    mutationFn: (id: number) => categoriesApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
      toast.success('Đã xóa danh mục')
      onOpenChange(false)
    },
    onError: () => {
      toast.error('Xóa danh mục thất bại. Có thể danh mục đang chứa sản phẩm.')
    },
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <div className="flex items-center gap-2">
            <AlertTriangle className="size-5 text-destructive" />
            <DialogTitle>Xóa danh mục</DialogTitle>
          </div>
          <DialogDescription>
            Bạn có chắc chắn muốn xóa danh mục "{category?.name}"?
            {category && (
              <span className="mt-2 block text-destructive">
                Lưu ý: Các sản phẩm thuộc danh mục này sẽ bị ảnh hưởng nếu danh mục đang được sử dụng.
              </span>
            )}
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={deleteMutation.isPending}
          >
            Hủy
          </Button>
          <Button
            variant="destructive"
            onClick={() => category && deleteMutation.mutate(category.id)}
            disabled={deleteMutation.isPending}
          >
            {deleteMutation.isPending && (
              <Loader2 className="mr-2 size-4 animate-spin" />
            )}
            Xóa
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
```

- [ ] **Step 4: Create `CategoriesPage.tsx`**

```typescript
import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { categoriesApi } from '@/api/categories'
import { CategoryTable } from './components/CategoryTable'
import { CategoryDialog } from './components/CategoryDialog'
import { DeleteCategoryDialog } from './components/DeleteCategoryDialog'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Plus, Loader2, ArrowLeft } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import type { CategoryProduct } from '@/types/category'

export function CategoriesPage() {
  const navigate = useNavigate()
  const [editTarget, setEditTarget] = useState<CategoryProduct | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<CategoryProduct | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)

  const { data: categories, isLoading } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoriesApi.getAll().then((r) => r.data),
  })

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="icon" onClick={() => navigate('/products')}>
            <ArrowLeft className="size-5" />
          </Button>
          <h1 className="text-2xl font-semibold">Danh mục sản phẩm</h1>
        </div>
        <Button onClick={() => { setEditTarget(null); setDialogOpen(true) }}>
          <Plus className="mr-2 size-4" />
          Thêm danh mục
        </Button>
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">
            {categories ? `${categories.length} danh mục` : ''}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {isLoading ? (
            <div className="flex h-48 items-center justify-center">
              <Loader2 className="size-8 animate-spin text-muted-foreground" />
            </div>
          ) : categories && categories.length > 0 ? (
            <CategoryTable
              categories={categories}
              onEdit={(cat) => { setEditTarget(cat); setDialogOpen(true) }}
              onDelete={setDeleteTarget}
            />
          ) : (
            <div className="flex h-48 items-center justify-center text-muted-foreground">
              Chưa có danh mục nào
            </div>
          )}
        </CardContent>
      </Card>

      <CategoryDialog
        category={editTarget}
        open={dialogOpen}
        onOpenChange={(open) => { setDialogOpen(open); if (!open) setEditTarget(null) }}
      />

      <DeleteCategoryDialog
        category={deleteTarget}
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null) }}
      />
    </div>
  )
}
```

- [ ] **Step 5: Verify build**

```bash
npm run build
```
Expected: 0 errors

- [ ] **Step 6: Commit**

```bash
git add flower-admin.frontend/src/pages/categories/
git commit -m "feat: add categories CRUD page with inline dialogs"
```

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
