### Task 3: Implement ImportService

**Files:**
- Create: `Flower.Backend/Services/ImportService.cs`
- Modify: `Flower.Backend/Program.cs` (register service)

**Interfaces:**
- Consumes: `IImportService` (interface from Task 2), `IApplicationDbContext` (existing), `IPhotoService` (existing), `ICategoryProductService` (existing)
- Produces: `ImportService` (implementation)

- [ ] **Step 1: Create ImportService**

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

- [ ] **Step 2: Register service in Program.cs**

Add after existing service registrations (~line 180):
```csharp
builder.Services.AddScoped<IImportService, ImportService>();
```

- [ ] **Step 3: Build to verify compilation**

```bash
dotnet build D:\TrenLop\ThucTapTaiTruong\FlowerShop\Flower.Backend\Flower.Backend.csproj
```

Expected: Build succeeded with 0 errors

- [ ] **Step 4: Commit**

```bash
git add Flower.Backend/Services/ImportService.cs Flower.Backend/Program.cs
git commit -m "feat: implement ImportService with Excel+ZIP product import pipeline"
```

---


