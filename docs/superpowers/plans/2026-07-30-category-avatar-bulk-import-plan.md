# Category Image, Bulk Import & Avatar — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add image field to product categories, bulk import for categories (Excel+ZIP), and avatars for users/customers.

**Architecture:** Backend-first: add nullable `ImageUrl`/`Avatar` fields to entities + DTOs + migration, extend ImportService with category import, then update frontend components (CategoryDialog/Table, ImportPage tabs, UsersPage, AppHeader).

**Tech Stack:** .NET 8/C# (EF Core, CloudinaryDotNet, EPPlus), React 18/TypeScript (base-ui, Tailwind, react-dropzone, @tanstack/react-query)

## Global Constraints

- All new DB fields must be nullable (optional image/avatar)
- Cloudinary folder for category images: `{folder}/categories`
- Cloudinary folder for avatars: `{folder}/avatars`
- Existing `IPhotoService.UploadPhotoAsync` must accept optional subfolder parameter
- Excel bulk import must report per-row errors via `ImportResult`
- Frontend: shadcn `<Avatar>` + `<AvatarImage>` + `<AvatarFallback>` for avatars
- Frontend: 40×40 rounded-md thumbnail for categories, 36×36 rounded-full for users

---

### Task 1: Backend Entities, DTOs & Migration

**Files:**
- Modify: `Flower.Data/Entities/CategoryProduct.cs`
- Modify: `Flower.Data/Entities/User.cs`
- Modify: `Flower.Data/Entities/Customer.cs`
- Modify: `Flower.Backend/Models/DTOs/CategoryProductDTOs.cs`
- Modify: `Flower.Backend/Models/DTOs/UserDTOs.cs`
- Modify: `Flower.Backend/Models/DTOs/CustomerDTOs.cs`
- Modify: `Flower.Backend/Models/DTOs/MappingExtensions.cs`
- Create: EF Migration (auto-generated)

- [ ] **Step 1: Add ImageUrl to CategoryProduct entity**

Edit `Flower.Data/Entities/CategoryProduct.cs` — add after `Slug`:
```csharp
[MaxLength(2000)]
public string? ImageUrl { get; set; }
```

- [ ] **Step 2: Add Avatar to User entity**

Edit `Flower.Data/Entities/User.cs` — add after existing fields:
```csharp
[MaxLength(2000)]
public string? Avatar { get; set; }
```

- [ ] **Step 3: Add Avatar to Customer entity**

Edit `Flower.Data/Entities/Customer.cs` — add after existing fields:
```csharp
[MaxLength(2000)]
public string? Avatar { get; set; }
```

- [ ] **Step 4: Update CategoryProductDTOs**

Edit `Flower.Backend/Models/DTOs/CategoryProductDTOs.cs`:
- `CategoryProductDTO`: add `public string? ImageUrl { get; set; }`
- `CreateCategoryProductDTO`: add `[MaxLength(2000)] public string? ImageUrl { get; set; }`
- `UpdateCategoryProductDTO`: add `[MaxLength(2000)] public string? ImageUrl { get; set; }`

- [ ] **Step 5: Update UserDTOs**

Edit `Flower.Backend/Models/DTOs/UserDTOs.cs`:
- `UserDTO`: add `public string? Avatar { get; set; }`
- `CreateUserRequest`: add `public string? Avatar { get; set; }`
- `UpdateUserRequest`: add `public string? Avatar { get; set; }`

- [ ] **Step 6: Update CustomerDTO**

Edit `Flower.Backend/Models/DTOs/CustomerDTOs.cs`:
- `CustomerDTO`: add `public string? Avatar { get; set; }`

- [ ] **Step 7: Update MappingExtensions**

Edit `Flower.Backend/Models/DTOs/MappingExtensions.cs`:

In `CategoryProductDTO ToDTO(CategoryProduct)` — add `ImageUrl = categoryProduct.ImageUrl`

In `CategoryProduct ToEntity(CreateCategoryProductDTO)` — add `ImageUrl = dto.ImageUrl`

In `UpdateEntity(UpdateCategoryProductDTO, CategoryProduct)` — add `entity.ImageUrl = dto.ImageUrl;`

In `UserDTO ToDTO(User)` — add `Avatar = user.Avatar`

In `User ToEntity(CreateUserDTO)` — add `Avatar = dto.Avatar`

In `UpdateEntity(UpdateUserDTO, User)` — add `entity.Avatar = dto.Avatar;`

In `CustomerDTO ToDTO(Customer)` — add `Avatar = customer.Avatar`

- [ ] **Step 8: Generate EF migration**

```bash
cd Flower.Backend
dotnet ef migrations add AddCategoryImageAndAvatar
```

- [ ] **Step 9: Build and verify**

```bash
dotnet build
```
Expected: no errors

- [ ] **Step 10: Commit**

```bash
git add -A
git commit -m "feat: add ImageUrl to CategoryProduct, Avatar to User/Customer"
```

---

### Task 2: Backend PhotoService — Subfolder Support

**Files:**
- Modify: `Flower.Backend/Services/Interfaces/IPhotoService.cs`
- Modify: `Flower.Backend/Services/PhotoService.cs`

- [ ] **Step 1: Add folder parameter to IPhotoService interface**

Edit `Flower.Backend/Services/Interfaces/IPhotoService.cs`:

Change `UploadPhotoAsync` signature to accept optional folder parameter:
```csharp
Task<string?> UploadPhotoAsync(IFormFile file, string? subfolder = null);
```

- [ ] **Step 2: Update PhotoService implementation**

Edit `Flower.Backend/Services/PhotoService.cs`:

Change `UploadPhotoAsync` method signature:
```csharp
public async Task<string?> UploadPhotoAsync(IFormFile file, string? subfolder = null)
```

Modify the upload params to append subfolder:
```csharp
var folder = _settings!.Folder;
if (!string.IsNullOrEmpty(subfolder))
    folder = $"{folder}/{subfolder}";
var uploadParams = new ImageUploadParams
{
    File = new FileDescription(file.FileName, compressedStream),
    Folder = folder
};
```

Also update the `DeletePhotoAsync` to accept optional subfolder — for now it's fine since we pass the full URL, but for safety update the public ID construction:
```csharp
var publicId = folderName + "/" + System.IO.Path.GetFileNameWithoutExtension(lastSegment);
```
(No change needed — the URL contains the full path)

- [ ] **Step 3: Build and verify**

```bash
dotnet build
```

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "feat: add subfolder parameter to IPhotoService"
```

---

### Task 3: Backend Bulk Import for Categories

**Files:**
- Modify: `Flower.Backend/Services/Interfaces/IImportService.cs`
- Modify: `Flower.Backend/Services/ImportService.cs`
- Modify: `Flower.Backend/Controllers/Api/ImportsController.cs`
- Create: `Flower.Backend/wwwroot/templates/category_import_template.xlsx`

- [ ] **Step 1: Add method to IImportService**

Edit `Flower.Backend/Services/Interfaces/IImportService.cs` — add:
```csharp
Task<ImportResult> ImportCategoriesAsync(
    IFormFile excelFile,
    IFormFile? zipFile,
    string onDuplicate);
```

- [ ] **Step 2: Implement ImportCategoriesAsync in ImportService**

Edit `Flower.Backend/Services/ImportService.cs` — add this method after `ImportProductsAsync`:

The method follows the same pattern as `ImportProductsAsync`:
1. Validate .xlsx extension
2. Extract ZIP to temp dir, build `imageMap` (case-insensitive, allowed: .jpg/.jpeg/.png/.webp)
3. Load existing categories keyed by lowercase Name
4. Parse Excel rows:
   - Col B (2): Name (required)
   - Col C (3): Slug (auto-slugify if empty using existing SlugHelper)
   - Col D (4): Description
   - Col E (5): ImageFileName (case-insensitive match against ZIP)
5. Handle onDuplicate: "skip" or "update"
6. Upload matched images to Cloudinary using `_photoService.UploadPhotoAsync(file, "categories")`
7. Bulk insert/update, return `ImportResult`

```csharp
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
            var zipPath = Path.Combine(tempDir, "upload.zip");
            using (var stream = new FileStream(zipPath, FileMode.Create))
                await zipFile.CopyToAsync(stream);
            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, tempDir);

            foreach (var file in Directory.GetFiles(tempDir))
            {
                var ext = Path.GetExtension(file).ToLowerInvariant();
                if (AllowedImageExtensions.Contains(ext))
                {
                    var key = Path.GetFileNameWithoutExtension(file);
                    if (!imageMap.ContainsKey(key))
                        imageMap[key] = file;
                }
            }
        }

        var existingCategories = await _context.CategoriesProducts.ToListAsync();
        var categoryByName = new Dictionary<string, CategoryProduct>(StringComparer.OrdinalIgnoreCase);
        foreach (var c in existingCategories)
            categoryByName[c.Name] = c;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage(excelFile.OpenReadStream());
        var worksheet = package.Workbook.Worksheets[0];
        if (worksheet == null)
        {
            result.Errors.Add(new ImportError { ErrorMessage = "File Excel không có sheet nào" });
            return result;
        }

        var rowCount = worksheet.Dimension?.Rows ?? 0;
        if (rowCount < 2)
        {
            result.Errors.Add(new ImportError { ErrorMessage = "File Excel không có dữ liệu" });
            return result;
        }

        result.TotalRows = rowCount - 1;
        var toCreate = new List<CategoryProduct>();
        var createdCount = 0;

        for (int row = 2; row <= rowCount; row++)
        {
            var rowIndex = row - 1;
            var errors = new List<string>();

            var name = (worksheet.Cells[row, 2].Text ?? "").Trim();
            var slug = (worksheet.Cells[row, 3].Text ?? "").Trim();
            var description = (worksheet.Cells[row, 4].Text ?? "").Trim();
            var imageFileName = (worksheet.Cells[row, 5].Text ?? "").Trim();

            if (string.IsNullOrEmpty(name))
            {
                errors.Add("Tên danh mục không được để trống");
                result.FailureCount++;
                result.Errors.Add(new ImportError { RowIndex = rowIndex, ProductName = name, ErrorMessage = string.Join("; ", errors) });
                continue;
            }

            if (string.IsNullOrEmpty(slug))
                slug = Utils.SlugHelper.GenerateSlug(name);

            string? imageUrl = null;
            if (!string.IsNullOrEmpty(imageFileName))
            {
                var imageKey = Path.GetFileNameWithoutExtension(imageFileName);
                if (imageMap.TryGetValue(imageKey, out var imagePath))
                {
                    using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                    var formFile = new FormFile(fs, 0, fs.Length, "file", Path.GetFileName(imagePath))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/" + Path.GetExtension(imagePath).TrimStart('.')
                    };
                    imageUrl = await _photoService.UploadPhotoAsync(formFile, "categories");
                    if (imageUrl == null)
                        _logger.LogWarning("Failed to upload category image: {FileName}", imageFileName);
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
                if (imageUrl != null) existing.ImageUrl = imageUrl;
                existing.UpdatedAt = DateTime.UtcNow;
                createdCount++;
                continue;
            }

            toCreate.Add(new CategoryProduct
            {
                Name = name,
                Slug = slug,
                Description = description,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (toCreate.Count > 0)
        {
            _context.CategoriesProducts.AddRange(toCreate);
            await _context.SaveChangesAsync();
        }
        else if (createdCount > 0)
        {
            await _context.SaveChangesAsync();
        }

        result.SuccessCount = toCreate.Count + createdCount;

        _logger.LogInformation("ImportCategories: Created={Created}, Updated={Updated}, TotalRows={Total}",
            toCreate.Count, createdCount, result.TotalRows);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error importing categories");
        result.Errors.Add(new ImportError { ErrorMessage = $"Lỗi hệ thống: {ex.Message}" });
    }
    finally
    {
        if (!string.IsNullOrEmpty(tempDir) && Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);
    }

    return result;
}
```

- [ ] **Step 3: Add API endpoints to ImportsController**

Edit `Flower.Backend/Controllers/Api/ImportsController.cs` — add two endpoints:

```csharp
[HttpPost("categories/upload")]
public async Task<IActionResult> UploadCategories(
    IFormFile excelFile,
    IFormFile? zipFile,
    [FromForm] string onDuplicate = "skip")
{
    if (excelFile == null || excelFile.Length == 0)
        return BadRequest(new { message = "Vui lòng chọn file Excel" });

    var ext = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
    if (ext != ".xlsx")
        return BadRequest(new { message = "File Excel phải có định dạng .xlsx" });

    var result = await _importService.ImportCategoriesAsync(excelFile, zipFile, onDuplicate);

    var response = new ImportApiResponse
    {
        TotalRows = result.TotalRows,
        SuccessCount = result.SuccessCount,
        FailureCount = result.FailureCount,
        Errors = result.Errors,
        SkippedSkus = result.SkippedSkus
    };

    return Ok(response);
}

[HttpGet("categories/template")]
public IActionResult CategoriesTemplate()
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "category_import_template.xlsx");
    if (!System.IO.File.Exists(path))
        return NotFound(new { message = "File template không tồn tại" });

    var bytes = System.IO.File.ReadAllBytes(path);
    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "category_import_template.xlsx");
}
```

- [ ] **Step 4: Create Excel template for categories**

Create a method or startup script that generates `Flower.Backend/wwwroot/templates/category_import_template.xlsx`:

```csharp
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
using var package = new ExcelPackage();
var sheet = package.Workbook.Worksheets.Add("Danh mục sản phẩm");
sheet.Cells[1, 1].Value = "STT";
sheet.Cells[1, 2].Value = "Tên danh mục";
sheet.Cells[1, 3].Value = "Slug";
sheet.Cells[1, 4].Value = "Mô tả";
sheet.Cells[1, 5].Value = "File ảnh";
// Header styling
sheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
// Sample data
sheet.Cells[2, 1].Value = 1;
sheet.Cells[2, 2].Value = "Hoa sinh nhật";
sheet.Cells[2, 3].Value = "hoa-sinh-nhat";
sheet.Cells[2, 4].Value = "Danh mục hoa dành tặng sinh nhật";
sheet.Cells[2, 5].Value = "birthday.jpg";
sheet.Cells[3, 1].Value = 2;
sheet.Cells[3, 2].Value = "Hoa cưới";
sheet.Cells[3, 3].Value = "hoa-cuoi";
sheet.Cells[3, 4].Value = "";
sheet.Cells[3, 5].Value = "";
sheet.Cells.AutoFitColumns();
var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates");
Directory.CreateDirectory(dir);
var path = Path.Combine(dir, "category_import_template.xlsx");
File.WriteAllBytes(path, await package.GetAsByteArrayAsync());
```

Run this as a one-time script or add it to a startup task. Place the file at `Flower.Backend/wwwroot/templates/category_import_template.xlsx`.

- [ ] **Step 5: Build and verify**

```bash
dotnet build
```

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "feat: add bulk import for categories"
```

---

### Task 4: Frontend — Category Type & API Updates

**Files:**
- Modify: `flower-admin.frontend/src/types/category.ts`
- Modify: `flower-admin.frontend/src/api/categories.ts`
- Modify: `flower-admin.frontend/src/api/imports.ts`

- [ ] **Step 1: Update category types**

Edit `flower-admin.frontend/src/types/category.ts`:
```typescript
export interface CategoryProduct {
  id: number
  name: string
  description?: string
  slug?: string
  imageUrl?: string
}

export interface CreateCategoryRequest {
  name: string
  description?: string
  slug?: string
  imageUrl?: string
}

export interface UpdateCategoryRequest extends CreateCategoryRequest {
  id: number
}
```

- [ ] **Step 2: Add category import methods**

Edit `flower-admin.frontend/src/api/imports.ts` — add:
```typescript
export const importsApi = {
  upload(formData: FormData) {
    return apiClient.post<ImportApiResponse>('/api/imports/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },

  downloadTemplate() {
    return apiClient.get('/api/imports/template', { responseType: 'blob' })
  },

  uploadCategories(formData: FormData) {
    return apiClient.post<ImportApiResponse>('/api/imports/categories/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },

  downloadCategoryTemplate() {
    return apiClient.get('/api/imports/categories/template', { responseType: 'blob' })
  },
}
```

- [ ] **Step 3: Commit**

```bash
git add -A
git commit -m "feat: update category types and import API"
```

---

### Task 5: Frontend — CategoryDialog with Image Upload

**Files:**
- Modify: `flower-admin.frontend/src/pages/categories/components/CategoryDialog.tsx`

- [ ] **Step 1: Update CategoryDialog with image upload**

Edit `flower-admin.frontend/src/pages/categories/components/CategoryDialog.tsx` — add image upload dropzone:

New imports:
```typescript
import { useDropzone } from 'react-dropzone'
import { uploadApi } from '@/api/upload'
import { Image, X, Upload } from 'lucide-react'
```

Add state:
```typescript
const [imageUrl, setImageUrl] = useState('')
const [uploadingImage, setUploadingImage] = useState(false)
```

Update `useEffect` reset:
```typescript
if (category) {
  setName(category.name)
  setDescription(category.description || '')
  setSlug(category.slug || '')
  setImageUrl(category.imageUrl || '')
} else {
  setName('')
  setDescription('')
  setSlug('')
  setImageUrl('')
}
```

Update mutation payload:
```typescript
const payload = { name, description, slug: slug || generateSlug(name), imageUrl: imageUrl || undefined }
```

Add dropzone after the slug field and before description:
```typescript
<div className="space-y-2">
  <Label>Ảnh danh mục</Label>
  {imageUrl ? (
    <div className="relative">
      <img src={imageUrl} alt={name} className="h-32 w-full rounded-md object-cover" />
      <Button
        variant="destructive"
        size="icon"
        className="absolute right-2 top-2 size-6"
        onClick={() => setImageUrl('')}
      >
        <X className="size-4" />
      </Button>
    </div>
  ) : (
    <div
      {...getRootProps()}
      className="flex cursor-pointer flex-col items-center justify-center rounded-lg border-2 border-dashed p-6 transition-colors hover:border-primary hover:bg-muted/50"
    >
      <input {...getInputProps({ accept: 'image/*' })} />
      {uploadingImage ? (
        <Loader2 className="size-6 animate-spin text-muted-foreground" />
      ) : (
        <>
          <Upload className="mb-2 size-6 text-muted-foreground" />
          <p className="text-xs text-muted-foreground">Kéo thả hoặc chọn ảnh</p>
        </>
      )}
    </div>
  )}
</div>
```

Add dropzone handler in component:
```typescript
const { getRootProps, getInputProps } = useDropzone({
  accept: { 'image/*': [] },
  maxFiles: 1,
  onDrop: async (accepted) => {
    if (accepted.length === 0) return
    setUploadingImage(true)
    try {
      const res = await uploadApi.upload(accepted[0])
      setImageUrl(res.data.url)
    } catch {
      toast.error('Tải ảnh thất bại')
    } finally {
      setUploadingImage(false)
    }
  },
})
```

- [ ] **Step 2: Commit**

```bash
git add -A
git commit -m "feat: add image upload to category dialog"
```

---

### Task 6: Frontend — CategoryTable with Image Thumbnail

**Files:**
- Modify: `flower-admin.frontend/src/pages/categories/components/CategoryTable.tsx`

- [ ] **Step 1: Add image thumbnail column**

Edit `flower-admin.frontend/src/pages/categories/components/CategoryTable.tsx`:

New imports:
```typescript
import { Folder } from 'lucide-react'
```

Table header — add column after ID:
```typescript
<TableHead className="w-16">ID</TableHead>
<TableHead className="w-12">Ảnh</TableHead>
<TableHead>Tên danh mục</TableHead>
```

Table cell — add after ID cell:
```typescript
<TableCell className="text-muted-foreground">{cat.id}</TableCell>
<TableCell>
  {cat.imageUrl ? (
    <img src={cat.imageUrl} alt={cat.name} className="size-10 rounded-md object-cover" />
  ) : (
    <div className="flex size-10 items-center justify-center rounded-md bg-slate-100 dark:bg-slate-800">
      <Folder className="size-5 text-slate-400" />
    </div>
  )}
</TableCell>
<TableCell className="font-medium">{cat.name}</TableCell>
```

The full updated `TableRow`:
```tsx
<TableRow key={cat.id}>
  <TableCell className="text-muted-foreground">{cat.id}</TableCell>
  <TableCell>
    {cat.imageUrl ? (
      <img src={cat.imageUrl} alt={cat.name} className="size-10 rounded-md object-cover" />
    ) : (
      <div className="flex size-10 items-center justify-center rounded-md bg-slate-100 dark:bg-slate-800">
        <Folder className="size-5 text-slate-400" />
      </div>
    )}
  </TableCell>
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
```

- [ ] **Step 2: Commit**

```bash
git add -A
git commit -m "feat: add image thumbnail column to category table"
```

---

### Task 7: Frontend — ImportPage with Categories Tab

**Files:**
- Modify: `flower-admin.frontend/src/pages/imports/ImportPage.tsx`
- Modify: `flower-admin.frontend/src/pages/categories/CategoriesPage.tsx` (add "Nhập hàng loạt" nav button)

- [ ] **Step 1: Refactor ImportPage with tabs**

Edit `flower-admin.frontend/src/pages/imports/ImportPage.tsx` — add tab support:

Add imports:
```typescript
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
```

Add search params:
```typescript
const [searchParams, setSearchParams] = useSearchParams()
const activeTab = searchParams.get('tab') || 'products'
```

Wrap entire return content in Tabs:
```tsx
<Tabs value={activeTab} onValueChange={(v) => setSearchParams({ tab: v })}>
  <div className="flex items-center justify-between">
    <TabsList>
      <TabsTrigger value="products">Sản phẩm</TabsTrigger>
      <TabsTrigger value="categories">Danh mục</TabsTrigger>
    </TabsList>
  </div>

  <TabsContent value="products" className="mt-6">
    {/* existing import content - unchanged */}
  </TabsContent>

  <TabsContent value="categories" className="mt-6">
    {/* category import - same pattern as products but with category-specific handlers */}
  </TabsContent>
</Tabs>
```

The Category tab uses a separate `uploadCategoriesMutation`:
```typescript
const uploadCategoriesMutation = useMutation({
  mutationFn: (formData: FormData) => importsApi.uploadCategories(formData).then((r) => r.data),
  onSuccess: (data) => {
    setCategoryResult(data)
    if (data.successCount > 0) toast.success(`Import thành công ${data.successCount} danh mục`)
    if (data.failureCount > 0) toast.error(`${data.failureCount} dòng bị lỗi`)
  },
  onError: () => toast.error('Import danh mục thất bại. Vui lòng thử lại.'),
})
```

The category tab content is nearly identical to the products tab but:
- Title: "Import danh mục"
- Uses `importsApi.uploadCategories()` and `importsApi.downloadCategoryTemplate()`
- Result button says "Import danh mục"
- Error table headers: "Tên danh mục" instead of "Mã SP" / "Tên SP"

Since the ImportPage is a single file, extract common import UI into a shared pattern or use state prefixes. The cleanest approach: keep both tabs within ImportPage with separate state:

```typescript
// Product import state
const [excelFile, setExcelFile] = useState<File | null>(null)
const [zipFile, setZipFile] = useState<File | null>(null)
const [duplicateAction, setDuplicateAction] = useState<'skip' | 'update'>('skip')
const [result, setResult] = useState<ImportApiResponse | null>(null)

// Category import state
const [catExcelFile, setCatExcelFile] = useState<File | null>(null)
const [catZipFile, setCatZipFile] = useState<File | null>(null)
const [catDuplicateAction, setCatDuplicateAction] = useState<'skip' | 'update'>('skip')
const [catResult, setCatResult] = useState<ImportApiResponse | null>(null)
```

Each tab renders its own drop zone, file list, duplicate action radio, submit button, and result card. This avoids a complex abstraction for two similar but independent flows.

- [ ] **Step 2: Add navigation button to CategoriesPage**

Edit `flower-admin.frontend/src/pages/categories/CategoriesPage.tsx` — add import nav button:

Add import:
```typescript
import { useNavigate } from 'react-router-dom'
import { Upload } from 'lucide-react'
```

In component:
```typescript
const navigate = useNavigate()
```

Add button next to the "Thêm danh mục" trigger:
```tsx
<Button variant="outline" size="sm" onClick={() => navigate('/products/import?tab=categories')}>
  <Upload className="mr-1 size-4" />
  Nhập hàng loạt
</Button>
```

- [ ] **Step 3: Commit**

```bash
git add -A
git commit -m "feat: add categories tab to import page"
```

---

### Task 8: Frontend — User Avatar (Types, API, Dialog, Table, Header)

**Files:**
- Modify: `flower-admin.frontend/src/types/user.ts`
- Modify: `flower-admin.frontend/src/pages/users/UsersPage.tsx`
- Modify: `flower-admin.frontend/src/components/AppHeader.tsx`

- [ ] **Step 1: Update user types**

Edit `flower-admin.frontend/src/types/user.ts`:
```typescript
export interface User {
  id: number
  username: string
  fullName: string
  email?: string
  phone?: string
  address?: string
  role: string
  avatar?: string
}

export interface CreateUserRequest {
  username: string
  password: string
  fullName: string
  role: string
  avatar?: string
}

export interface UpdateUserRequest {
  id: number
  username: string
  password?: string
  fullName: string
  email?: string
  phone?: string
  address?: string
  role: string
  avatar?: string
}
```

- [ ] **Step 2: Add avatar to UsersPage dialog and table**

Edit `flower-admin.frontend/src/pages/users/UsersPage.tsx`:

New imports:
```typescript
import { Avatar, AvatarImage, AvatarFallback } from '@/components/ui/avatar'
import { useDropzone } from 'react-dropzone'
import { uploadApi } from '@/api/upload'
import { Upload, X } from 'lucide-react'
```

Add state:
```typescript
const [formAvatar, setFormAvatar] = useState('')
const [uploadingAvatar, setUploadingAvatar] = useState(false)
```

Update `openCreate` — add `setFormAvatar('')`
Update `openEdit` — add `setFormAvatar(item.avatar || '')`
Update mutation payload — add `avatar: formAvatar || undefined` to both create and update payloads

Add dropzone to the dialog form (after Họ tên, before Email):
```typescript
<div className="space-y-2">
  <Label>Ảnh đại diện</Label>
  {formAvatar ? (
    <div className="relative inline-block">
      <Avatar className="size-20">
        <AvatarImage src={formAvatar} />
        <AvatarFallback>{formFullName.charAt(0).toUpperCase() || '?'}</AvatarFallback>
      </Avatar>
      <Button variant="destructive" size="icon" className="absolute -right-2 -top-2 size-6 rounded-full" onClick={() => setFormAvatar('')}>
        <X className="size-3" />
      </Button>
    </div>
  ) : (
    <div
      {...getRootProps()}
      className="flex cursor-pointer flex-col items-center justify-center rounded-lg border-2 border-dashed p-4 transition-colors hover:border-primary"
    >
      <input {...getInputProps({ accept: 'image/*' })} />
      {uploadingAvatar ? (
        <Loader2 className="size-5 animate-spin text-muted-foreground" />
      ) : (
        <>
          <Upload className="mb-1 size-5 text-muted-foreground" />
          <p className="text-xs text-muted-foreground">Chọn ảnh đại diện</p>
        </>
      )}
    </div>
  )}
</div>
```

Add dropzone handler:
```typescript
const avatarDropzone = useDropzone({
  accept: { 'image/*': [] },
  maxFiles: 1,
  onDrop: async (accepted) => {
    if (accepted.length === 0) return
    setUploadingAvatar(true)
    try {
      const res = await uploadApi.upload(accepted[0])
      setFormAvatar(res.data.url)
    } catch {
      toast.error('Tải ảnh thất bại')
    } finally {
      setUploadingAvatar(false)
    }
  },
})
// Destructure avatarDropzone
const { getRootProps, getInputProps } = avatarDropzone
```

Note: The non-null assertion for `getRootProps` and `getInputProps` needs careful handling since they come from the hook. Set all state at the top level.

Add avatar column to table header:
```typescript
<TableHead>Ảnh</TableHead>
```

Add avatar cell to each row:
```typescript
<TableCell>
  <Avatar className="size-9">
    <AvatarImage src={user.avatar} />
    <AvatarFallback className="bg-primary/10 text-xs text-primary">
      {user.fullName.charAt(0).toUpperCase()}
    </AvatarFallback>
  </Avatar>
</TableCell>
```

Add it as the first cell in the row body.

- [ ] **Step 3: Update AppHeader to show avatar**

Edit `flower-admin.frontend/src/components/AppHeader.tsx`:

Add import:
```typescript
import { AvatarImage } from '@/components/ui/avatar'
```

Update the Avatar component:
```tsx
<Avatar className="size-8">
  {user?.avatar && <AvatarImage src={user.avatar} />}
  <AvatarFallback className="bg-primary text-xs text-on-primary">
    {initials}
  </AvatarFallback>
</Avatar>
```

- [ ] **Step 4: Build frontend and verify**

```bash
cd flower-admin.frontend
npm run build
```
Expected: no errors

- [ ] **Step 5: Commit**

```bash
git add -A
git commit -m "feat: add avatar to user management and header"
```
