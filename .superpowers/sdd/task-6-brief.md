# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 6: ProductVariant CRUD (STEP 2)

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

---

## Task 6: ProductVariant CRUD (STEP 2)

**Files:**
- Create: `Flower.Backend/Models/DTOs/ProductVariantDTOs.cs`
- Modify: `Flower.Backend/Models/DTOs/ProductDTOs.cs` (add `Variants` to `ProductDTO`)
- Modify: `Flower.Backend/Models/DTOs/MappingExtensions.cs` (add `ProductVariant.ToDTO()`; extend `Product.ToDTO()` to map `Variants`)
- Modify: `Flower.Backend/Services/Interfaces/IProductService.cs` (3 new methods)
- Modify: `Flower.Backend/Services/ProductService.cs` (implement; include `ProductVariants` in `BuildQuery`)
- Modify: `Flower.Backend/Controllers/Api/ProductsController.cs` (3 new endpoints)

**Interfaces:**
- Consumes: `ProductVariant.Price`/`Sku` (Task 1), `ProductVariantDTO` (this task).
- Produces: `IProductService.AddVariantAsync(int, CreateProductVariantDTO) → ProductVariantDTO?`, `UpdateVariantAsync(int, UpdateProductVariantDTO) → bool`, `DeleteVariantAsync(int) → bool`; `ProductDTO.Variants`.

- [ ] **Step 1: Create `ProductVariantDTOs.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class ProductVariantDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Sku { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CreateProductVariantDTO
    {
        [Required(ErrorMessage = "Tên size không được để trống")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string? Sku { get; set; }

        public bool IsDefault { get; set; }
    }

    public class UpdateProductVariantDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên size không được để trống")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string? Sku { get; set; }

        public bool IsDefault { get; set; }
    }
}
```

- [ ] **Step 2: Add `Variants` to `ProductDTO`**

In `ProductDTOs.cs`, add after the `Images` property (line 27):

```csharp
public List<ProductVariantDTO> Variants { get; set; } = new();
```

- [ ] **Step 3: Add mapping in `MappingExtensions.cs`**

Add the `ProductVariant.ToDTO()` extension method (anywhere in the static class):

```csharp
public static ProductVariantDTO ToDTO(this ProductVariant v)
{
    return new ProductVariantDTO
    {
        Id = v.Id,
        ProductId = v.ProductId,
        Name = v.Name,
        Price = v.Price,
        Sku = v.Sku,
        IsDefault = v.IsDefault
    };
}
```

Then in `Product.ToDTO()`, add:

```csharp
Variants = product.ProductVariants?.Select(v => v.ToDTO()).ToList() ?? new List<ProductVariantDTO>()
```

- [ ] **Step 4: Add interface methods (`IProductService.cs`)**

```csharp
Task<ProductVariantDTO?> AddVariantAsync(int productId, CreateProductVariantDTO dto);
Task<bool> UpdateVariantAsync(int variantId, UpdateProductVariantDTO dto);
Task<bool> DeleteVariantAsync(int variantId);
```

- [ ] **Step 5: Implement in `ProductService.cs`**

Add `using System.Collections.Generic;` if missing, and implement:

```csharp
public async Task<ProductVariantDTO?> AddVariantAsync(int productId, CreateProductVariantDTO dto)
{
    var product = await _context.Products.FindAsync(productId);
    if (product == null) return null;

    if (dto.IsDefault)
    {
        var others = _context.ProductVariants.Where(v => v.ProductId == productId && v.IsDefault);
        foreach (var v in others) v.IsDefault = false;
    }

    var variant = new ProductVariant
    {
        ProductId = productId,
        Name = dto.Name,
        Price = dto.Price,
        Sku = dto.Sku,
        IsDefault = dto.IsDefault
    };

    _context.ProductVariants.Add(variant);
    await _context.SaveChangesAsync();
    return variant.ToDTO();
}

public async Task<bool> UpdateVariantAsync(int variantId, UpdateProductVariantDTO dto)
{
    var variant = await _context.ProductVariants.FindAsync(variantId);
    if (variant == null) return false;

    if (dto.IsDefault)
    {
        var others = _context.ProductVariants
            .Where(v => v.ProductId == variant.ProductId && v.IsDefault && v.Id != variantId);
        foreach (var v in others) v.IsDefault = false;
    }

    variant.Name = dto.Name;
    variant.Price = dto.Price;
    variant.Sku = dto.Sku;
    variant.IsDefault = dto.IsDefault;

    await _context.SaveChangesAsync();
    return true;
}

public async Task<bool> DeleteVariantAsync(int variantId)
{
    var variant = await _context.ProductVariants.FindAsync(variantId);
    if (variant == null) return false;

    _context.ProductVariants.Remove(variant);
    await _context.SaveChangesAsync();
    return true;
}
```

Also extend `BuildQuery` (line 54-58) to include variants:

```csharp
IQueryable<Product> query = _context.Products
    .Include(p => p.ProductCategory)
    .Include(p => p.Images)
    .Include(p => p.ProductVariants);
```

> After Task 1, `p.CategoryProduct` is now `p.ProductCategory`. `ProductVariants` navigation is `Product.ProductVariants` (exists at `Product.cs:44`).

- [ ] **Step 6: Add controller endpoints (`ProductsController.cs`)**

Add after `Delete` (line 149), before the bulk endpoints:

```csharp
[HttpPost("{id}/variants")]
public async Task<IActionResult> AddVariant(int id, [FromBody] CreateProductVariantDTO dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var variant = await _productService.AddVariantAsync(id, dto);
    if (variant == null)
        return NotFound(new { message = "Không tìm thấy sản phẩm này" });

    await _notificationService.NotifyEntityChanged("Product");
    return CreatedAtAction(nameof(GetDetail), new { id }, variant);
}

[HttpPut("{id}/variants/{variantId}")]
public async Task<IActionResult> UpdateVariant(int id, int variantId, [FromBody] UpdateProductVariantDTO dto)
{
    if (variantId != dto.Id)
        return BadRequest();

    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var updated = await _productService.UpdateVariantAsync(variantId, dto);
    if (!updated)
        return NotFound();

    await _notificationService.NotifyEntityChanged("Product");
    return NoContent();
}

[HttpDelete("{id}/variants/{variantId}")]
public async Task<IActionResult> DeleteVariant(int id, int variantId)
{
    var deleted = await _productService.DeleteVariantAsync(variantId);
    if (!deleted)
        return NotFound();

    await _notificationService.NotifyEntityChanged("Product");
    return NoContent();
}
```

- [ ] **Step 7: Build + test + smoke-test**

```powershell
dotnet build
dotnet test Flower.Tests
```

Expected: build succeeds, `37` tests pass. Manual smoke: `GET /api/Products/{id}` returns `variants: []`; `POST /api/Products/{id}/variants` with `{ "name": "Nhỏ", "price": 100000, "sku": "R-001", "isDefault": true }` returns the created variant and subsequent `GET` shows it.

- [ ] **Step 8: Commit**

```bash
git add Flower.Backend
git commit -m "feat: add product variant CRUD API"
```

---