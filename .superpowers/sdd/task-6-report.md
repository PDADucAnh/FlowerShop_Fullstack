# Task 6 Report: ProductVariant CRUD (STEP 2)

## What I implemented

Added ProductVariant CRUD to the existing `ProductService` per the brief, with size + price + SKU:

- **`Flower.Backend/Models/DTOs/ProductVariantDTOs.cs`** (new): `ProductVariantDTO`, `CreateProductVariantDTO`, `UpdateProductVariantDTO` — exact code from the brief (validation via DataAnnotations: required Name ≤50, Price range, Sku ≤50, IsDefault).
- **`Flower.Backend/Models/DTOs/ProductDTOs.cs`**: added `List<ProductVariantDTO> Variants` to `ProductDTO` after `Images`.
- **`Flower.Backend/Models/DTOs/MappingExtensions.cs`**: added `ProductVariant.ToDTO()`; extended `Product.ToDTO()` to map `Variants` (null-safe with `?? new List<ProductVariantDTO>()`).
- **`Flower.Backend/Services/Interfaces/IProductService.cs`**: added `AddVariantAsync(int, CreateProductVariantDTO) → Task<ProductVariantDTO?>`, `UpdateVariantAsync(int, UpdateProductVariantDTO) → Task<bool>`, `DeleteVariantAsync(int) → Task<bool>`.
- **`Flower.Backend/Services/ProductService.cs`**: implemented the three methods (is-default exclusivity logic clears `IsDefault` on other variants of the same product on create/update); extended `BuildQuery` with `.Include(p => p.ProductVariants)`.
- **`Flower.Backend/Controllers/Api/ProductsController.cs`**: added `POST {id}/variants`, `PUT {id}/variants/{variantId}`, `DELETE {id}/variants/{variantId}` after `Delete` and before the bulk endpoints, each notifying `"Product"` via `_notificationService.NotifyEntityChanged`.

No migration created (columns exist from Task 5). No controllers/views/frontend/other services touched beyond the files listed in the brief.

## Verification

- `dotnet build Flower.Backend` → **0 errors** (131 pre-existing warnings — codebase-wide nullability warnings + SixLabors ImageSharp NU1902/NU1903 advisories; none introduced by this task).
- `dotnet test Flower.Tests` → **37 passed, 0 failed, 0 skipped** (baseline unchanged; brief added no tests).

Smoke test not run (would require a running server + auth); route shapes are identical to existing endpoints and match the brief.

## Files changed

- `Flower.Backend/Models/DTOs/ProductVariantDTOs.cs` (new)
- `Flower.Backend/Models/DTOs/ProductDTOs.cs`
- `Flower.Backend/Models/DTOs/MappingExtensions.cs`
- `Flower.Backend/Services/Interfaces/IProductService.cs`
- `Flower.Backend/Services/ProductService.cs`
- `Flower.Backend/Controllers/Api/ProductsController.cs`

## Self-review findings / concerns

- All brief checklist items (Steps 1–8) implemented; code matches the brief verbatim.
- Grepped touched files: no leftover TODOs/FIXME/NotImplementedException.
- **Concern (low, inherited from brief, not this task):** `[Range(0, (double)decimal.MaxValue, ...)]` on `decimal Price` allows `0` despite the "phải lớn hơn 0" message — same pattern as the existing `CreateProductDTO.Price`. Kept verbatim per brief to stay consistent.
- `UpdateVariantAsync` uses `FindAsync(variantId)` and trusts the client `ProductId` is unaffected (it isn't mutable). Route keeps `{id}` for REST symmetry; the variant's owning product is not re-validated against route `{id}` — matches the brief exactly.
- No new tests were added (brief didn't specify any).
