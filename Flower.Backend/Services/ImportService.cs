using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text.RegularExpressions;

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
            var excelExt = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
            if (excelExt != ".xlsx")
            {
                result.Errors.Add(new ImportError { ErrorMessage = "File Excel phải có định dạng .xlsx" });
                return result;
            }

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

            var categoryMap = await _context.CategoriesProducts
                .Where(c => c.Slug != null)
                .ToDictionaryAsync(c => c.Slug!, c => c.Id, StringComparer.OrdinalIgnoreCase);

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
            result.TotalRows = Math.Max(0, rowCount - 1);

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

                    if (string.IsNullOrWhiteSpace(name))
                        errors.Add("Tên sản phẩm không được để trống");

                    if (!decimal.TryParse(priceText, out var price) || price < 0)
                        errors.Add("Giá bán không hợp lệ");

                    if (!int.TryParse(stockText, out var stock) || stock < 0)
                        errors.Add("Số lượng kho không hợp lệ");

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
                    }

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
                                existingProduct.CategoryProductId = resolvedCategoryId!.Value;
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
                        CategoryProductId = resolvedCategoryId!.Value,
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

            if (productsToAdd.Count > 0)
            {
                await _context.Products.AddRangeAsync(productsToAdd);
                await _context.SaveChangesAsync();
            }
            else if (onDuplicate == "update")
            {
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
        slug = Regex.Replace(slug, @"[áàảãạâấầẩẫậăắằẳẵặ]", "a");
        slug = Regex.Replace(slug, @"[éèẻẽẹêếềểễệ]", "e");
        slug = Regex.Replace(slug, @"[íìỉĩị]", "i");
        slug = Regex.Replace(slug, @"[óòỏõọôốồổỗộơớờởỡợ]", "o");
        slug = Regex.Replace(slug, @"[úùủũụưứừửữự]", "u");
        slug = Regex.Replace(slug, @"[ýỳỷỹỵ]", "y");
        slug = Regex.Replace(slug, @"[đ]", "d");
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }
}
