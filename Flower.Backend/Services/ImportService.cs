using Flower.Backend.Helpers;
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

            var categoryMap = await _context.ProductCategories
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

            // Track pending image uploads: local path + file name per valid product.
            // OldImageUrl holds the existing Cloudinary image to delete after a successful
            // upload of a replacement image (update mode only).
            var pendingImages = new Dictionary<Flower.Data.Entities.Product, (string LocalPath, string FileName, string? OldImageUrl)>();

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

                    // Check image file exists in ZIP (if specified), but DON'T upload yet
                    string? pendingImagePath = null;
                    if (!string.IsNullOrWhiteSpace(imageFileName))
                    {
                        if (imageMap.TryGetValue(imageFileName, out var imgPath))
                        {
                            pendingImagePath = imgPath;
                        }
                        else
                        {
                            _logger.LogWarning("Image file not found in ZIP: {FileName}", imageFileName);
                            result.Errors.Add(new ImportError
                            {
                                RowIndex = rowIndex,
                                ProductCode = sku,
                                ProductName = name,
                                ErrorMessage = $"Không tìm thấy file ảnh '{imageFileName}' trong file ZIP"
                            });
                        }
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
                                existingProduct.ProductCategoryId = resolvedCategoryId!.Value;
                                existingProduct.Slug = GenerateSlug(name);
                                existingProduct.UpdatedAt = DateTime.UtcNow;
                                if (pendingImagePath != null)
                                {
                                    var existingImageBase = GetImageFileBaseName(existingProduct.ImageUrl);
                                    var incomingImageBase = Path.GetFileNameWithoutExtension(imageFileName!);
                                    if (existingImageBase != null &&
                                        string.Equals(existingImageBase, incomingImageBase, StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Same image name as the one already on Cloudinary -> skip upload.
                                        _logger.LogInformation(
                                            "Import update: image '{FileName}' unchanged for SKU '{Sku}', skipping upload",
                                            imageFileName, sku);
                                    }
                                    else
                                    {
                                        // New/replacement image -> upload then delete the old Cloudinary image.
                                        pendingImages[existingProduct] = (pendingImagePath, imageFileName!, existingProduct.ImageUrl);
                                    }
                                }
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
                        ProductCategoryId = resolvedCategoryId!.Value,
                        Slug = GenerateSlug(name),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        ViewCount = 0,
                        AddToCartCount = 0
                    };

                    if (pendingImagePath != null)
                        pendingImages[product] = (pendingImagePath, imageFileName!, null);

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

            // Upload images ONLY for validated products
            foreach (var (product, (localPath, fileName, oldImageUrl)) in pendingImages)
            {
                await using var fs = new FileStream(localPath, FileMode.Open, FileAccess.Read);
                var formFile = new FormFile(fs, 0, fs.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary()
                };
                var uploadedUrl = await _photoService.UploadPhotoAsync(formFile, CloudinaryFolders.Products);
                if (uploadedUrl == null)
                {
                    _logger.LogWarning("Import update: upload failed for image '{FileName}' of SKU '{Sku}', keeping existing image",
                        fileName, product.Sku);
                    continue;
                }

                product.ImageUrl = uploadedUrl;
                if (!string.IsNullOrEmpty(oldImageUrl) &&
                    !string.Equals(oldImageUrl, uploadedUrl, StringComparison.OrdinalIgnoreCase))
                {
                    await _photoService.DeletePhotoAsync(oldImageUrl);
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

    public async Task<ImportResult> ImportCategoriesAsync(
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
                    result.Errors.Add(new ImportError { ErrorMessage = "File ảnh phải có định dạng .zip" });
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

            var existingCategories = await _context.ProductCategories.ToListAsync();
            var categoryByName = new Dictionary<string, Flower.Data.Entities.ProductCategory>(StringComparer.OrdinalIgnoreCase);
            foreach (var c in existingCategories)
                categoryByName[c.Name] = c;

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

            var itemsToAdd = new List<Flower.Data.Entities.ProductCategory>();
            var pendingCatImages = new Dictionary<Flower.Data.Entities.ProductCategory, (string LocalPath, string FileName, string? OldImageUrl)>();

            for (int row = 2; row <= rowCount; row++)
            {
                var errors = new List<string>();
                var rowIndex = row - 1;

                try
                {
                    var name = worksheet.GetValue<string>(row, 2)?.Trim();
                    var slug = worksheet.GetValue<string>(row, 3)?.Trim();
                    var description = worksheet.GetValue<string>(row, 4)?.Trim();
                    var imageFileName = worksheet.GetValue<string>(row, 5)?.Trim();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        errors.Add("Tên danh mục không được để trống");
                        result.Errors.Add(new ImportError
                        {
                            RowIndex = rowIndex,
                            ProductName = name,
                            ErrorMessage = string.Join("; ", errors)
                        });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(slug))
                        slug = GenerateSlug(name);

                    // Check image exists but DON'T upload yet
                    string? pendingImagePath = null;
                    string? resolvedImageKey = null;
                    if (!string.IsNullOrWhiteSpace(imageFileName))
                    {
                        resolvedImageKey = Path.GetFileName(imageFileName);
                        if (imageMap.TryGetValue(resolvedImageKey, out var imgPath))
                        {
                            pendingImagePath = imgPath;
                        }
                        else
                        {
                            _logger.LogWarning("Image file not found in ZIP: {FileName}", imageFileName);
                        }
                    }

                    if (categoryByName.TryGetValue(name, out var existing))
                    {
                        if (onDuplicate.Equals("skip", StringComparison.OrdinalIgnoreCase))
                        {
                            result.SkippedSkus.Add(name);
                            continue;
                        }

                        existing.Name = name;
                        existing.Slug = slug;
                        existing.Description = description;
                        existing.UpdatedAt = DateTime.UtcNow;
                        if (pendingImagePath != null)
                        {
                            var existingImageBase = GetImageFileBaseName(existing.ImageUrl);
                            var incomingImageBase = Path.GetFileNameWithoutExtension(resolvedImageKey!);
                            if (existingImageBase != null &&
                                string.Equals(existingImageBase, incomingImageBase, StringComparison.OrdinalIgnoreCase))
                            {
                                // Same image name as the one already on Cloudinary -> skip upload.
                                _logger.LogInformation(
                                    "Import update: image '{FileName}' unchanged for category '{Name}', skipping upload",
                                    resolvedImageKey, name);
                            }
                            else
                            {
                                // New/replacement image -> upload then delete the old Cloudinary image.
                                pendingCatImages[existing] = (pendingImagePath, resolvedImageKey!, existing.ImageUrl);
                            }
                        }
                        result.SuccessCount++;
                        continue;
                    }

                    var category = new Flower.Data.Entities.ProductCategory
                    {
                        Name = name,
                        Slug = slug,
                        Description = description,
                        CreatedAt = DateTime.UtcNow
                    };

                    if (pendingImagePath != null)
                        pendingCatImages[category] = (pendingImagePath, resolvedImageKey!, null);

                    itemsToAdd.Add(category);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing category import row {Row}", row);
                    result.Errors.Add(new ImportError
                    {
                        RowIndex = rowIndex,
                        ErrorMessage = $"Lỗi xử lý dòng: {ex.Message}"
                    });
                }
            }

            // Upload images ONLY for validated categories
            foreach (var (cat, (localPath, fileName, oldImageUrl)) in pendingCatImages)
            {
                await using var fs = new FileStream(localPath, FileMode.Open, FileAccess.Read);
                var formFile = new FormFile(fs, 0, fs.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary()
                };
                var uploadedUrl = await _photoService.UploadPhotoAsync(formFile, CloudinaryFolders.Categories);
                if (uploadedUrl == null)
                {
                    _logger.LogWarning("Import update: upload failed for image '{FileName}' of category '{Name}', keeping existing image",
                        fileName, cat.Name);
                    continue;
                }

                cat.ImageUrl = uploadedUrl;
                if (!string.IsNullOrEmpty(oldImageUrl) &&
                    !string.Equals(oldImageUrl, uploadedUrl, StringComparison.OrdinalIgnoreCase))
                {
                    await _photoService.DeletePhotoAsync(oldImageUrl);
                }
            }

            if (itemsToAdd.Count > 0)
            {
                _context.ProductCategories.AddRange(itemsToAdd);
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
            _logger.LogError(ex, "Category import failed");
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

    /// <summary>
    /// Extracts the base file name (no extension) of a Cloudinary image URL.
    /// e.g. "https://res.cloudinary.com/x/image/upload/v1234/flower-shop/products/product1.jpg" -> "product1".
    /// Returns null when the URL is empty, local, or not a Cloudinary /upload/ URL.
    /// </summary>
    private string? GetImageFileBaseName(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl) || !imageUrl.StartsWith("http"))
            return null;

        try
        {
            var path = new Uri(imageUrl).AbsolutePath;
            var uploadIndex = path.IndexOf("/upload/", StringComparison.OrdinalIgnoreCase);
            if (uploadIndex < 0)
                return null;

            var afterUpload = path[(uploadIndex + 8)..];

            if (afterUpload.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                var slashIndex = afterUpload.IndexOf('/');
                if (slashIndex > 0)
                    afterUpload = afterUpload[(slashIndex + 1)..];
            }

            return Path.GetFileNameWithoutExtension(Path.GetFileName(afterUpload));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetImageFileBaseName failed for URL: {Url}", imageUrl);
            return null;
        }
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
