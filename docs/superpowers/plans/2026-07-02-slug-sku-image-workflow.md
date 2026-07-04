# Auto Slug, SKU and Product Image Update Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Automatically generate slugs and SKUs on entity creation if left blank, and allow updating a product without uploading a new image (retaining the existing image).

**Architecture:**
- Create a utility class `SlugHelper` inside `CMS.Backend/Utils/SlugHelper.cs` that provides Vietnamese diacritics removal and robust slug and SKU generation.
- Modify `Create` methods in `ProductService.cs`, `CategoryService.cs`, `CategoryProductService.cs`, and `PostService.cs` to apply this helper when DTO inputs are empty.
- Modify the `Update` method in `ProductService.cs` to preserve the product's existing image if no new image URL is passed in the update DTO.

**Tech Stack:** ASP.NET Core 8, EF Core, C#

## Global Constraints
- Do not break existing functionality or database schemas.
- Ensure all Vietnamese diacritics are correctly normalized to ASCII equivalents in slugs and SKUs.
- Follow existing dependency injection and service patterns.

---

### Task 1: Create SlugHelper Utility

**Files:**
- Create: `CMS.Backend/Utils/SlugHelper.cs`

**Interfaces:**
- Produces: `string SlugHelper.GenerateSlug(string text)`
- Produces: `string SlugHelper.GenerateSku(string name)`

- [ ] **Step 1: Create SlugHelper class**
  Create the file `CMS.Backend/Utils/SlugHelper.cs` with the following content:
  ```csharp
  using System;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;

  namespace CMS.Backend.Utils
  {
      public static class SlugHelper
      {
          public static string GenerateSlug(string text)
          {
              if (string.IsNullOrWhiteSpace(text))
                  return string.Empty;

              string str = RemoveSign4VietnameseString(text.ToLowerInvariant());
              str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
              str = Regex.Replace(str, @"\s+", " ").Trim();
              str = Regex.Replace(str, @"\s", "-");
              str = Regex.Replace(str, @"-+", "-");
              return str.Trim('-');
          }

          public static string GenerateSku(string name)
          {
              if (string.IsNullOrWhiteSpace(name))
                  return "PROD-" + new Random().Next(1000, 9999);

              string unsignedName = RemoveSign4VietnameseString(name);
              var words = unsignedName.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
              var prefix = string.Join("", words.Select(w => char.ToUpper(w[0])));
              if (string.IsNullOrEmpty(prefix))
                  prefix = "PROD";

              var random = new Random();
              return $"{prefix}-{random.Next(1000, 9999)}";
          }

          private static readonly string[] VietnameseSigns = new string[]
          {
              "aAeEoOuUiIdDyYoOuUrRuU",
              "áàảãạăắằẳẵặâấầẩẫậ",
              "ÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬ",
              "éèẻẽẹêếềểễệ",
              "ÉÈẺẼẸÊẾỀỂỄỆ",
              "óòỏõọôốồổỗộơớờởỡợ",
              "ÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢ",
              "úùủũụưứừửữự",
              "ÚÙỦŨỤƯỨỪỬỮỰ",
              "íìỉĩị",
              "ÍÌỈĨỊ",
              "đ",
              "Đ",
              "ýỳỷỹỵ",
              "ÝỲỶỸỴ"
          };

          private static string RemoveSign4VietnameseString(string str)
          {
              for (int i = 1; i < VietnameseSigns.Length; i++)
              {
                  for (int j = 0; j < VietnameseSigns[i].Length; j++)
                  {
                      str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                  }
              }
              return str;
          }
      }
  }
  ```

- [ ] **Step 2: Compile to verify syntax**
  Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
  Expected: Build succeeded with 0 errors.

- [ ] **Step 3: Commit utility class**
  Run:
  ```bash
  git add CMS.Backend/Utils/SlugHelper.cs
  git commit -m "feat: add SlugHelper utility with Vietnamese accent removal"
  ```

---

### Task 2: Integrate Auto-Generation in Creation Services

**Files:**
- Modify: `CMS.Backend/Services/ProductService.cs`
- Modify: `CMS.Backend/Services/CategoryService.cs`
- Modify: `CMS.Backend/Services/CategoryProductService.cs`
- Modify: `CMS.Backend/Services/PostService.cs`

**Interfaces:**
- Consumes: `SlugHelper` functions from `CMS.Backend/Utils/SlugHelper.cs`

- [ ] **Step 1: Update ProductService.cs Create method**
  Open `CMS.Backend/Services/ProductService.cs` and replace the `Create` method (approx. lines 82-98) to auto-generate `Slug` and `Sku` if empty:
  ```csharp
          public async Task<ProductDTO> Create(CreateProductDTO dto)
          {
              if (string.IsNullOrEmpty(dto.Slug))
              {
                  dto.Slug = CMS.Backend.Utils.SlugHelper.GenerateSlug(dto.Name);
              }
              if (string.IsNullOrEmpty(dto.Sku))
              {
                  dto.Sku = CMS.Backend.Utils.SlugHelper.GenerateSku(dto.Name);
              }

              var product = dto.ToEntity();
              _context.Products.Add(product);
              await _context.SaveChangesAsync();

              await _context.Entry(product)
                  .Reference(p => p.CategoryProduct)
                  .LoadAsync();

              return product.ToDTO();
          }
  ```

- [ ] **Step 2: Update CategoryService.cs Create method**
  Open `CMS.Backend/Services/CategoryService.cs` and replace the `Create` method (approx. lines 54-60) to auto-generate `Slug` if empty:
  ```csharp
          public async Task<CategoryDTO> Create(CreateCategoryDTO dto)
          {
              if (string.IsNullOrEmpty(dto.Slug))
              {
                  dto.Slug = CMS.Backend.Utils.SlugHelper.GenerateSlug(dto.Name);
              }
              var category = dto.ToEntity();
              _context.Categories.Add(category);
              await _context.SaveChangesAsync();
              return category.ToDTO();
          }
  ```

- [ ] **Step 3: Update CategoryProductService.cs Create method**
  Open `CMS.Backend/Services/CategoryProductService.cs` and replace the `Create` method (approx. lines 53-59) to auto-generate `Slug` if empty:
  ```csharp
          public async Task<CategoryProductDTO> Create(CreateCategoryProductDTO dto)
          {
              if (string.IsNullOrEmpty(dto.Slug))
              {
                  dto.Slug = CMS.Backend.Utils.SlugHelper.GenerateSlug(dto.Name);
              }
              var category = dto.ToEntity();
              _context.CategoriesProducts.Add(category);
              await _context.SaveChangesAsync();
              return category.ToDTO();
          }
  ```

- [ ] **Step 4: Update PostService.cs Create method**
  Open `CMS.Backend/Services/PostService.cs` and replace the `Create` method (approx. lines 70-82) to auto-generate `Slug` if empty:
  ```csharp
          public async Task<PostDTO> Create(CreatePostDTO dto)
          {
              if (string.IsNullOrEmpty(dto.Slug))
              {
                  dto.Slug = CMS.Backend.Utils.SlugHelper.GenerateSlug(dto.Title);
              }
              var post = dto.ToEntity();
              post.CreatedDate = DateTime.Now;
              _context.Posts.Add(post);
              await _context.SaveChangesAsync();

              await _context.Entry(post)
                  .Reference(p => p.Category)
                  .LoadAsync();

              return post.ToDTO();
          }
  ```

- [ ] **Step 5: Compile to verify integrations**
  Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
  Expected: Build succeeded with 0 errors.

- [ ] **Step 6: Commit integrations**
  Run:
  ```bash
  git add CMS.Backend/Services/ProductService.cs CMS.Backend/Services/CategoryService.cs CMS.Backend/Services/CategoryProductService.cs CMS.Backend/Services/PostService.cs
  git commit -m "feat: integrate auto slug and SKU generation on entity creation"
  ```

---

### Task 3: Support Retaining Old Product Image on Edit

**Files:**
- Modify: `CMS.Backend/Services/ProductService.cs`

**Interfaces:**
- Consumes: `UpdateProductDTO` and updates existing database entity

- [ ] **Step 1: Update ProductService.cs Update method**
  Open `CMS.Backend/Services/ProductService.cs` and modify the `Update` method (approx. lines 100-111) to fall back to the existing entity ImageUrl if the DTO value is null or empty:
  ```csharp
          public async Task<bool> Update(int id, UpdateProductDTO dto)
          {
              if (id != dto.Id)
                  return false;

              var product = await _context.Products
                  .FirstOrDefaultAsync(p => p.Id == id);
              if (product == null)
                  return false;

              if (string.IsNullOrEmpty(dto.ImageUrl))
              {
                  dto.ImageUrl = product.ImageUrl;
              }

              dto.UpdateEntity(product);
  ```

- [ ] **Step 2: Compile to verify all changes**
  Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
  Expected: Build succeeded with 0 errors.

- [ ] **Step 3: Commit update fallback**
  Run:
  ```bash
  git add CMS.Backend/Services/ProductService.cs
  git commit -m "feat: preserve existing product image during update if new image is not provided"
  ```
