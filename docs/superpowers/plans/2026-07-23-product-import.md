# Product Import Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [x]`) syntax for tracking.

**Goal:** Allow Admin to bulk import products via Excel (.xlsx) + product images via ZIP archive.

**Architecture:** ImportController (MVC) → ImportService (pipeline: unzip → build image map → read Excel → upload to Cloudinary → bulk save). Reuses existing IPhotoService for Cloudinary uploads and existing DI patterns.

**Tech Stack:** ASP.NET Core 8 (Backend), EPPlus (Excel), System.IO.Compression (ZIP), CloudinaryDotNet (image hosting), SixLabors.ImageSharp (image processing).

## Global Constraints

- All admin controllers use `[Authorize(Policy = "StaffOnly")]` at class level
- All services registered as `AddScoped` in `Program.cs`
- `ExcelPackage.LicenseContext = LicenseContext.NonCommercial;` set before any Excel read
- Category lookup: pre-load `Dictionary<string, int>` (slug → Id) before row loop
- Image lookup: `Dictionary<string, string>` (lowercase filename → physical path), O(1)
- Temp folder cleanup in `try/finally` block
- Duplicate SKU behavior controlled by `onDuplicate` parameter ("skip" | "update")
- Slug auto-generated from product name if not provided
- Follow existing file naming conventions

---

### Task 1: Add EPPlus NuGet Package + Create Excel Template

**Files:**
- Modify: `Flower.Backend/Flower.Backend.csproj`
- Create: `Flower.Backend/wwwroot/templates/product_import_template.xlsx`

- [x] **Step 1: Add EPPlus package**

```bash
cd D:\TrenLop\ThucTapTaiTruong\FlowerShop
dotnet add Flower.Backend/Flower.Backend.csproj package EPPlus
```

Expected output: `PackageReference for 'EPPlus' added to file 'Flower.Backend/Flower.Backend.csproj'`

- [x] **Step 2: Create template directory**

```bash
New-Item -ItemType Directory -Path "D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Backend\wwwroot\templates" -Force
```

- [x] **Step 3: Create the Excel template programmatically via a small script**

Create `Flower.Backend/wwwroot/templates/product_import_template.xlsx` using EPPlus with headers:
`STT`, `TenSanPham`, `MaSanPham`, `GiaBan`, `SoLuongKho`, `DanhMucSlug`, `TenFileAnh`, `MoTa`

- In the controller, add a `DownloadTemplate` action that serves this file.

- [x] **Step 4: Commit**

```bash
git add Flower.Backend/Flower.Backend.csproj Flower.Backend/wwwroot/templates/product_import_template.xlsx
git commit -m "chore: add EPPlus package and product import Excel template"
```

---

### Task 2: Create IImportService + ImportResult Models

**Files:**
- Create: `Flower.Backend/Services/Interfaces/IImportService.cs`
- Create: `Flower.Backend/Models/DTOs/ImportDTOs.cs`

**Interfaces:**
- Produces: `IImportService` (interface), `ImportResult`, `ImportError` (models)

- [x] **Step 1: Create ImportDTOs**

`Flower.Backend/Models/DTOs/ImportDTOs.cs`:

```csharp
namespace Flower.Backend.Models.DTOs;

public class ImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public List<string> SkippedSkus { get; set; } = new();
}

public class ImportError
{
    public int RowIndex { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class ImportViewModel
{
    public ImportResult? Result { get; set; }
}
```

- [x] **Step 2: Create IImportService interface**

`Flower.Backend/Services/Interfaces/IImportService.cs`:

```csharp
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace Flower.Backend.Services.Interfaces;

public interface IImportService
{
    Task<ImportResult> ImportProductsAsync(
        IFormFile excelFile,
        IFormFile? zipFile,
        string onDuplicate);
}
```

- [x] **Step 3: Commit**

```bash
git add Flower.Backend/Services/Interfaces/IImportService.cs Flower.Backend/Models/DTOs/ImportDTOs.cs
git commit -m "feat: add IImportService interface and ImportResult models"
```

---

### Task 3: Implement ImportService

**Files:**
- Create: `Flower.Backend/Services/ImportService.cs`
- Modify: `Flower.Backend/Program.cs` (register service)

**Interfaces:**
- Consumes: `IImportService` (interface from Task 2), `IApplicationDbContext` (existing), `IPhotoService` (existing), `ICategoryProductService` (existing)
- Produces: `ImportService` (implementation)

- [x] **Step 1: Create ImportService**

`Flower.Backend/Services/ImportService.cs`:

```csharp
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text.Json;

namespace Flower.Backend.Services;

public class ImportService : IImportService
{
    private readonly IApplicationDbContext _context;
    private readonly IPhotoService _photoService;
    private readonly ILogger<ImportService> _logger;

    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    public ImportService(
        IApplicationDbContext context,
        IPhotoService photoService,
        ILogger<ImportService> logger)
    {
        _context = context;
        _photoService = photoService;
        _logger = logger;
    }

    public async Task<ImportResult> ImportProductsAsync(
        IFormFile excelFile,
        IFormFile? zipFile,
        string onDuplicate)
    {
        var result = new ImportResult();
        var tempDir = string.Empty;

        try
        {
            // 1. Validate extensions
            var excelExt = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
            if (excelExt != ".xlsx")
            {
                result.Errors.Add(new ImportError { ErrorMessage = "File Excel phải có định dạng .xlsx" });
                return result;
            }

            // 2. Extract ZIP if provided
            Dictionary<string, string> imageMap = new(StringComparer.OrdinalIgnoreCase);
            if (zipFile != null && zipFile.Length > 0)
            {
                var zipExt = Path.GetExtension(zipFile.FileName).ToLowerInvariant();
                if (zipExt != ".zip")
                {
                    result.Errors.Add(new ImportError { ErrorMessage = "File ảnh phải là định dạng .zip" });
                    return result;
                }

                tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                using var zipStream = zipFile.OpenReadStream();
                System.IO.Compression.ZipFile.ExtractToDirectory(zipStream, tempDir, overwriteFiles: true);

                // Build image lookup map
                foreach (var filePath in Directory.EnumerateFiles(tempDir, "*.*", SearchOption.AllDirectories))
                {
                    var ext = Path.GetExtension(filePath);
                    if (AllowedImageExtensions.Contains(ext))
                    {
                        var fileName = Path.GetFileName(filePath);
                        imageMap[fileName] = filePath;
                    }
                }
            }

            // 3. Pre-load category slug -> Id map
            var categoryMap = await _context.CategoryProducts
                .Where(c => c.Slug != null)
                .ToDictionaryAsync(c => c.Slug!, c => c.Id, StringComparer.OrdinalIgnoreCase);

            // 4. Read Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var excelStream = new MemoryStream();
            await excelFile.CopyToAsync(excelStream);
            excelStream.Position = 0;

            using var package = new ExcelPackage(excelStream);
            var worksheet = package.Workbook.Worksheets[0];
            if (worksheet == null)
            {
                result.Errors.Add(new ImportError { ErrorMessage = "File Excel không có worksheet nào" });
                return result;
            }

            var rowCount = worksheet.Dimension?.Rows ?? 0;
            result.TotalRows = Math.Max(0, rowCount - 1); // minus header

            // 5. Process each data row (starting from row 2)
            var productsToAdd = new List<Flower.Data.Entities.Product>();
            var skuSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var existingProducts = await _context.Products
                .Where(p => p.Sku != null)
                .ToDictionaryAsync(p => p.Sku!, p => p, StringComparer.OrdinalIgnoreCase);

            for (int row = 2; row <= rowCount; row++)
            {
                var errors = new List<string>();
                var rowIndex = row - 1;

                try
                {
                    var name = worksheet.GetValue<string>(row, 2)?.Trim();
                    var sku = worksheet.GetValue<string>(row, 3)?.Trim();
                    var priceText = worksheet.GetValue<string>(row, 4)?.Trim();
                    var stockText = worksheet.GetValue<string>(row, 5)?.Trim();
                    var categorySlug = worksheet.GetValue<string>(row, 6)?.Trim();
                    var imageFileName = worksheet.GetValue<string>(row, 7)?.Trim();
                    var description = worksheet.GetValue<string>(row, 8)?.Trim();

                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(name))
                        errors.Add("Tên sản phẩm không được để trống");

                    if (!decimal.TryParse(priceText, out var price) || price < 0)
                        errors.Add("Giá bán không hợp lệ");

                    if (!int.TryParse(stockText, out var stock) || stock < 0)
                        errors.Add("Số lượng kho không hợp lệ");

                    if (errors.Count > 0)
                    {
                        result.Errors.Add(new ImportError
                        {
                            RowIndex = rowIndex,
                            ProductCode = sku,
                            ProductName = name,
                            ErrorMessage = string.Join("; ", errors)
                        });
                        continue;
                    }

                    var resolvedCatId = resolvedCategoryId!.Value;

                    // Resolve category
                    int? resolvedCategoryId = null;
                    if (!string.IsNullOrWhiteSpace(categorySlug))
                    {
                        if (categoryMap.TryGetValue(categorySlug, out var catId))
                            resolvedCategoryId = catId;
                        else
                            errors.Add($"Không tìm thấy danh mục với slug '{categorySlug}'");
                    }
                    else
                    {
                        errors.Add("Danh mục sản phẩm (cột DanhMucSlug) không được để trống");
                    }

                    // Upload image if available
                    string? imageUrl = null;
                    if (!string.IsNullOrWhiteSpace(imageFileName) && imageMap.TryGetValue(imageFileName, out var imagePath))
                    {
                        await using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                        var formFile = new FormFile(fs, 0, fs.Length, "file", imageFileName)
                        {
                            Headers = new HeaderDictionary()
                        };
                        imageUrl = await _photoService.UploadPhotoAsync(formFile);
                    }
                    else if (!string.IsNullOrWhiteSpace(imageFileName))
                    {
                        result.Errors.Add(new ImportError
                        {
                            RowIndex = rowIndex,
                            ProductCode = sku,
                            ProductName = name,
                            ErrorMessage = $"Không tìm thấy file ảnh '{imageFileName}' trong file ZIP"
                        });
                        // Continue processing without image
                    }

                    // Handle duplicate SKU
                    if (!string.IsNullOrWhiteSpace(sku))
                    {
                        if (existingProducts.TryGetValue(sku, out var existingProduct))
                        {
                            if (onDuplicate == "update")
                            {
                                existingProduct.Name = name;
                                existingProduct.Price = price;
                                existingProduct.StockQuantity = stock;
                                existingProduct.Description = description;
                                if (imageUrl != null) existingProduct.ImageUrl = imageUrl;
                                existingProduct.CategoryProductId = resolvedCatId;
                                existingProduct.Slug = GenerateSlug(name);
                                existingProduct.UpdatedAt = DateTime.UtcNow;
                                result.SuccessCount++;
                                continue;
                            }
                            else
                            {
                                result.Errors.Add(new ImportError
                                {
                                    RowIndex = rowIndex,
                                    ProductCode = sku,
                                    ProductName = name,
                                    ErrorMessage = $"SKU '{sku}' đã tồn tại"
                                });
                                continue;
                            }
                        }

                        if (!skuSet.Add(sku))
                        {
                            result.Errors.Add(new ImportError
                            {
                                RowIndex = rowIndex,
                                ProductCode = sku,
                                ProductName = name,
                                ErrorMessage = $"SKU '{sku}' bị trùng trong file Excel"
                            });
                            continue;
                        }
                    }

                    var product = new Flower.Data.Entities.Product
                    {
                        Name = name,
                        Sku = sku,
                        Price = price,
                        StockQuantity = stock,
                        Description = description,
                        ImageUrl = imageUrl,
                        CategoryProductId = resolvedCatId,
                        Slug = GenerateSlug(name),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        ViewCount = 0,
                        AddToCartCount = 0
                    };

                    productsToAdd.Add(product);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Excel row {Row}", row);
                    result.Errors.Add(new ImportError
                    {
                        RowIndex = rowIndex,
                        ErrorMessage = $"Lỗi xử lý dòng: {ex.Message}"
                    });
                }
            }

            // 6. Bulk insert
            if (productsToAdd.Count > 0)
            {
                await _context.Products.AddRangeAsync(productsToAdd);
                await _context.SaveChangesAsync();
            }
            else if (onDuplicate != "update")
            {
                // Only save if we had updates
                await _context.SaveChangesAsync();
            }

            result.FailureCount = result.Errors.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import failed");
            result.Errors.Add(new ImportError
            {
                RowIndex = 0,
                ErrorMessage = $"Lỗi hệ thống: {ex.Message}"
            });
        }
        finally
        {
            // 7. Cleanup temp folder
            if (!string.IsNullOrEmpty(tempDir) && Directory.Exists(tempDir))
            {
                try { Directory.Delete(tempDir, recursive: true); }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to delete temp dir {Path}", tempDir); }
            }
        }

        result.FailureCount = result.Errors.Count;
        return result;
    }

    private static string GenerateSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        var slug = name.ToLowerInvariant().Trim();
        // Basic diacritics removal
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[áàảãạâấầẩẫậăắằẳẵặ]", "a");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[éèẻẽẹêếềểễệ]", "e");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[íìỉĩị]", "i");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[óòỏõọôốồổỗộơớờởỡợ]", "o");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[úùủũụưứừửữự]", "u");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[ýỳỷỹỵ]", "y");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[đ]", "d");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }
}
```

- [x] **Step 2: Register service in Program.cs**

Add after existing service registrations (~line 180):
```csharp
builder.Services.AddScoped<IImportService, ImportService>();
```

- [x] **Step 3: Build to verify compilation**

```bash
dotnet build D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Backend\Flower.Backend.csproj
```

Expected: Build succeeded with 0 errors

- [x] **Step 4: Commit**

```bash
git add Flower.Backend/Services/ImportService.cs Flower.Backend/Program.cs
git commit -m "feat: implement ImportService with Excel+ZIP product import pipeline"
```

---

### Task 4: Create ImportController

**Files:**
- Create: `Flower.Backend/Controllers/ImportController.cs`

**Interfaces:**
- Consumes: `IImportService` (from Task 3)

- [x] **Step 1: Create ImportController**

`Flower.Backend/Controllers/ImportController.cs`:

```csharp
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flower.Backend.Controllers;

[Authorize(Policy = "StaffOnly")]
public class ImportController : Controller
{
    private readonly IImportService _importService;

    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ImportViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        IFormFile excelFile,
        IFormFile? zipFile,
        string onDuplicate)
    {
        if (excelFile == null || excelFile.Length == 0)
        {
            TempData["Error"] = "Vui lòng chọn file Excel";
            return View(new ImportViewModel());
        }

        var result = await _importService.ImportProductsAsync(excelFile, zipFile, onDuplicate ?? "skip");
        return View(new ImportViewModel { Result = result });
    }

    [HttpGet]
    public IActionResult DownloadTemplate()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "product_import_template.xlsx");
        if (!System.IO.File.Exists(path))
            return NotFound("File template không tồn tại");
        return PhysicalFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "product_import_template.xlsx");
    }
}
```

- [x] **Step 2: Build to verify**

```bash
dotnet build D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Backend\Flower.Backend.csproj
```

Expected: Build succeeded with 0 errors

- [x] **Step 3: Commit**

```bash
git add Flower.Backend/Controllers/ImportController.cs
git commit -m "feat: add ImportController with GET/POST and template download"
```

---

### Task 5: Create Import View (Index.cshtml)

**Files:**
- Create: `Flower.Backend/Views/Import/Index.cshtml`
- Create: `Flower.Backend/Views/Import/Index.cshtml` directory

- [x] **Step 1: Create directory**

```bash
New-Item -ItemType Directory -Path "D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Backend\Views\Import" -Force
```

- [x] **Step 2: Create the view**

`Flower.Backend/Views/Import/Index.cshtml`:

```html
@model ImportViewModel
@{
    ViewData["Title"] = "Nhập hàng loạt";
    Layout = "_LayoutAdmin";
}

<div class="max-w-4xl mx-auto">
    <div class="mb-6">
        <h3 class="text-label-sm uppercase tracking-[0.3em] text-secondary">Sản phẩm</h3>
        <p class="serif text-3xl font-bold">Nhập hàng loạt</p>
    </div>

    <div class="bg-surface rounded-xl shadow-sm border border-outline-variant/20 p-lg mb-stack-md">
        <form method="post" enctype="multipart/form-data" asp-action="Index">
            @Html.AntiForgeryToken()

            <div class="space-y-5">
                <!-- Excel file -->
                <div>
                    <label class="block font-label-md text-sm mb-1">File Excel (.xlsx)</label>
                    <input type="file" name="excelFile" accept=".xlsx" required
                           class="w-full px-3 py-2 border border-outline-variant/30 rounded-lg bg-surface" />
                </div>

                <!-- ZIP file -->
                <div>
                    <label class="block font-label-md text-sm mb-1">File ảnh (.zip)</label>
                    <input type="file" name="zipFile" accept=".zip"
                           class="w-full px-3 py-2 border border-outline-variant/30 rounded-lg bg-surface" />
                    <p class="text-xs text-on-surface-variant mt-1">Không bắt buộc. Ảnh sẽ được upload lên Cloudinary.</p>
                </div>

                <!-- Duplicate handling -->
                <div>
                    <label class="block font-label-md text-sm mb-2">Khi trùng mã sản phẩm (SKU)</label>
                    <div class="flex gap-6">
                        <label class="flex items-center gap-2">
                            <input type="radio" name="onDuplicate" value="skip" checked class="w-4 h-4" />
                            <span>Bỏ qua</span>
                        </label>
                        <label class="flex items-center gap-2">
                            <input type="radio" name="onDuplicate" value="update" class="w-4 h-4" />
                            <span>Cập nhật</span>
                        </label>
                    </div>
                </div>
            </div>

            <div class="flex justify-end gap-3 mt-6">
                <a asp-action="DownloadTemplate"
                   class="px-5 py-2.5 border border-outline rounded-lg text-primary bg-transparent text-sm no-underline inline-flex items-center gap-2">
                    <span class="material-symbols-outlined text-sm">download</span>
                    Tải file Excel mẫu (.xlsx)
                </a>
                <button type="submit" class="px-5 py-2.5 bg-primary text-on-primary border-0 rounded-lg">
                    Import sản phẩm
                </button>
            </div>
        </form>
    </div>

    @if (Model?.Result != null)
    {
        var r = Model.Result;
        <div class="bg-surface rounded-xl shadow-sm border border-outline-variant/20 p-lg">
            <h4 class="text-lg font-bold mb-4">Kết quả import</h4>

            <div class="grid grid-cols-3 gap-4 mb-4">
                <div class="bg-primary-container/30 rounded-lg p-4 text-center">
                    <p class="text-2xl font-bold text-primary">@r.TotalRows</p>
                    <p class="text-sm text-on-surface-variant">Tổng số dòng</p>
                </div>
                <div class="bg-green-50 rounded-lg p-4 text-center">
                    <p class="text-2xl font-bold text-green-600">@r.SuccessCount</p>
                    <p class="text-sm text-green-700">Thành công</p>
                </div>
                <div class="bg-red-50 rounded-lg p-4 text-center">
                    <p class="text-2xl font-bold text-red-600">@r.FailureCount</p>
                    <p class="text-sm text-red-700">Thất bại</p>
                </div>
            </div>

            @if (r.Errors.Count > 0)
            {
                <div>
                    <h5 class="font-semibold mb-2 text-red-600">Chi tiết lỗi</h5>
                    <div class="max-h-60 overflow-y-auto border border-outline-variant/30 rounded-lg">
                        <table class="w-full text-sm">
                            <thead class="bg-surface-variant/50 sticky top-0">
                                <tr>
                                    <th class="px-3 py-2 text-left">Dòng</th>
                                    <th class="px-3 py-2 text-left">Mã SP</th>
                                    <th class="px-3 py-2 text-left">Tên SP</th>
                                    <th class="px-3 py-2 text-left">Lỗi</th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var err in r.Errors)
                                {
                                    <tr class="border-t border-outline-variant/20">
                                        <td class="px-3 py-2">@err.RowIndex</td>
                                        <td class="px-3 py-2">@err.ProductCode</td>
                                        <td class="px-3 py-2">@err.ProductName</td>
                                        <td class="px-3 py-2 text-red-600">@err.ErrorMessage</td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                </div>
            }
        </div>
    }
</div>
```

- [x] **Step 3: Build to verify**

```bash
dotnet build D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Backend\Flower.Backend.csproj
```

Expected: Build succeeded with 0 errors

- [x] **Step 4: Commit**

```bash
git add Flower.Backend/Views/Import/Index.cshtml
git commit -m "feat: add Import view with form and result display"
```

---

### Task 6: Update Sidebar

**Files:**
- Modify: `Flower.Backend/Views/Shared/_LayoutAdmin.cshtml` (after line 177)

- [x] **Step 1: Add sidebar link**

After line 177 (`Danh mục sản phẩm` link, before the `Bán hàng` section header at line 179), insert:

```html
            <a class="flex items-center gap-3 px-4 py-3 rounded-lg font-label-md text-label-md @(controller == "Import" ? "text-primary bg-secondary-container" : "text-on-surface-variant hover:text-primary hover:bg-primary-container/20") transition-all duration-200 no-underline"
               asp-controller="Import" asp-action="Index">
                <span class="material-symbols-outlined @(controller == "Import" ? "text-primary" : "")">file_upload</span>
                Nhập hàng loạt
            </a>
```

- [x] **Step 2: Commit**

```bash
git add Flower.Backend/Views/Shared/_LayoutAdmin.cshtml
git commit -m "feat: add 'Nhập hàng loạt' link to admin sidebar"
```

---

### Task 7: Final Build and Verify

- [x] **Step 1: Full build**

```bash
dotnet build D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Backend\Flower.Backend.csproj 2>&1
```

Expected: `Build succeeded` with 0 errors

- [x] **Step 2: Run existing tests (if any)**

```bash
# Check if tests exist
if (Test-Path "D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Tests") {
    dotnet test D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Tests\Flower.Tests.csproj
}
```

- [x] **Step 3: Final commit**

```bash
git add -A
git status
```

If all clean, no need for additional commit.

- [x] **Step 4: Push**

```bash
git push
```
