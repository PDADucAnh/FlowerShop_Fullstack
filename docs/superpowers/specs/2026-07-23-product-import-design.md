# Product Import — Design Spec

**Date:** 2026-07-23
**Project:** FlowerShop (ASP.NET Core 8 + React)

## Overview

Bulk import products via Excel (.xlsx) + ZIP (images) — industry-standard approach used by Shopee, Lazada, Tiki. Admin uploads a lightweight Excel file with product data and a ZIP archive of product images; the server processes them server-side and returns a detailed report.

---

## Backend: ImportService

**File:** `Services/ImportService.cs` + `Services/Interfaces/IImportService.cs`

### Processing pipeline (8 steps)

1. **Validate input** — File extensions must be `.xlsx` and `.zip`; reject otherwise.
2. **Extract ZIP** — `System.IO.Compression.ZipFile.ExtractToDirectory()` into a temp folder (`Path.GetTempPath()` + `Guid.NewGuid()`).
3. **Build image lookup** — Scan all extracted files with extensions `.jpg`, `.jpeg`, `.png`, `.webp` (case-insensitive). Build `Dictionary<string, string>` where key is lowercase filename and value is full physical path. O(1) lookup.
4. **Pre-load categories** — Load all `CategoryProduct` into `Dictionary<string, int>` (slug → Id) before iterating rows.
5. **Read Excel** — Use EPPlus. For each row:
   - Parse + validate fields (name required, price ≥ 0, stock ≥ 0)
   - Look up category slug → CategoryProductId
   - Look up image filename in ZIP dictionary → upload to Cloudinary → store URL
   - Auto-generate slug from name if not provided
   - Handle duplicate SKU: skip or update based on `onDuplicate` option
6. **Bulk save** — Add new products to DbContext / update existing ones; call `SaveChangesAsync()` once.
7. **Clean up** — `try/finally` to delete temp folder regardless of outcome.
8. **Return `ImportResult`** — JSON with total count, success count, failure count, per-row error list.

### Key implementation notes

- `ExcelPackage.LicenseContext = LicenseContext.NonCommercial;`
- Category lookup dictionary: pre-loaded BEFORE the row loop
- Image upload via existing `IPhotoService.UploadPhotoAsync()`
- Slug auto-generation: remove diacritics, lowercase, replace spaces with hyphens

### ImportResult model

```csharp
public class ImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<ImportError> Errors { get; set; } = new();
}

public class ImportError
{
    public int RowIndex { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string ErrorMessage { get; set; }
}
```

### Excel format (product_import_template.xlsx)

| STT | TenSanPham | MaSanPham | GiaBan | SoLuongKho | DanhMucSlug | TenFileAnh | MoTa |
|-----|-----------|-----------|--------|------------|-------------|------------|------|
| 1 | Bó Hoa Hồng Đỏ | HOA001 | 450000 | 20 | hoa-sinh-nhat | hoa-hong-do.jpg | Hoa hồng đỏ Đà Lạt... |

---

## Controller: ImportController

**File:** `Controllers/ImportController.cs`

- `[Authorize(Policy = "StaffOnly")]` at class level
- **GET /Import** — Render form with file inputs + duplicate-handling radio
- **POST /Import** — Accept `IFormFile excelFile`, `IFormFile zipFile`, `string onDuplicate` ("skip" | "update"). Call service, render same page with result.

No redirect on POST — result is rendered directly on the page so admin can see the error report immediately.

### View model

```csharp
public class ImportViewModel
{
    public ImportResult? Result { get; set; }
}
```

---

## View: Views/Import/Index.cshtml

- Form with two file inputs (Excel + ZIP), radio for duplicate handling
- "Tải file Excel mẫu (.xlsx)" download button next to form
- When `Result` is present: summary cards (thành công / thất bại) + scrollable error table (`max-h-60 overflow-y-auto`)
- Follows existing admin visual style (white card, shadows, border)

---

## Sidebar: _LayoutAdmin.cshtml

Add new item in "Sản phẩm" section, after "Danh mục sản phẩm":

```html
<a class="flex items-center gap-3 px-4 py-3 rounded-lg font-label-md text-label-md ..."
   asp-controller="Import" asp-action="Index">
    <span class="material-symbols-outlined">file_upload</span>
    Nhập hàng loạt
</a>
```

---

## Dependencies

Add NuGet package: `EPPlus` (latest stable for .NET 8)

---

## Files to create / modify

| File | Action |
|------|--------|
| `Flower.Backend/Services/Interfaces/IImportService.cs` | Create |
| `Flower.Backend/Services/ImportService.cs` | Create |
| `Flower.Backend/Controllers/ImportController.cs` | Create |
| `Flower.Backend/Views/Import/Index.cshtml` | Create |
| `Flower.Backend/Views/Shared/_LayoutAdmin.cshtml` | Modify (sidebar) |
| `Flower.Backend/Flower.Backend.csproj` | Modify (EPPlus package) |
| `Flower.Backend/wwwroot/templates/product_import_template.xlsx` | Create (template file) |
