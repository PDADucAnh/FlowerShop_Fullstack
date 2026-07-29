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

