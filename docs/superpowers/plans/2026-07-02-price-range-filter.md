# Price Range Filter Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement a high-performance price range filter integrated from React Frontend to EF Core / SQL Server database, optimizing pagination and database lookup efficiency.

**Architecture:** Extended C# GetPaged methods with optional minPrice, maxPrice, and categoryProductId filters, non-clustered database index on Product BasePrice, and React page state with 500ms debounce.

**Tech Stack:** ASP.NET Core 8, EF Core, SQL Server, React, React Query, Tailwind CSS.

## Global Constraints
- **Timezone**: Asia/Ho_Chi_Minh
- **Debounce Delay**: 500ms

---

### Task 1: Create Database Index for Product BasePrice

**Files:**
- Modify: `CMS.Data/Migrations` (create new migration)

**Interfaces:**
- Produces: `IX_Products_BasePrice` index on SQL Server database.

- [ ] **Step 1: Create EF Core Migration**

Run: `dotnet ef migrations add AddProductBasePriceIndex --project CMS.Data --startup-project CMS.Backend`
Expected: Success. A migration skeleton is created.

- [ ] **Step 2: Update Migration Code**

Modify the generated migration file in [CMS.Data/Migrations](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Data/Migrations) to add the index SQL:
```csharp
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE NONCLUSTERED INDEX IX_Products_BasePrice ON Products (BasePrice) INCLUDE (Name, Slug, Sku, Status, ImageUrl);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IX_Products_BasePrice ON Products;");
        }
```

- [ ] **Step 3: Update Database**

Run: `dotnet ef database update --project CMS.Data --startup-project CMS.Backend`
Expected: Success. SQL Server applies migration.

- [ ] **Step 4: Commit**

```bash
git add CMS.Data/Migrations/
git commit -m "feat: add non-clustered index on Product BasePrice"
```

---

### Task 2: Update Product Service with Filter Parameters

**Files:**
- Modify: `CMS.Backend/Services/Interfaces/IProductService.cs`
- Modify: `CMS.Backend/Services/ProductService.cs`

**Interfaces:**
- Produces: `Task<PagedResult<ProductDTO>> IProductService.GetPaged(int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null, int? categoryProductId = null)`

- [ ] **Step 1: Update IProductService Interface**

Modify [IProductService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/Interfaces/IProductService.cs):
```csharp
        Task<PagedResult<ProductDTO>> GetPaged(int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null, int? categoryProductId = null);
```

- [ ] **Step 2: Update ProductService Implementation**

Modify [ProductService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/ProductService.cs):
```csharp
        public async Task<PagedResult<ProductDTO>> GetPaged(int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null, int? categoryProductId = null)
        {
            var query = BuildQuery();

            if (categoryProductId.HasValue)
            {
                query = query.Where(p => p.CategoryProductId == categoryProductId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.BasePrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.BasePrice <= maxPrice.Value);
            }

            query = query.OrderByDescending(p => p.Id);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDTO>
            {
                Items = items.Select(p => p.ToDTO()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
```

- [ ] **Step 3: Compile & Verify**

Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
Expected: Build successfully.

- [ ] **Step 4: Commit**

```bash
git add CMS.Backend/Services/Interfaces/IProductService.cs CMS.Backend/Services/ProductService.cs
git commit -m "feat: add minPrice, maxPrice, and categoryProductId filters to ProductService.GetPaged"
```

---

### Task 3: Expose Filter Parameters in ProductsController

**Files:**
- Modify: `CMS.Backend/Controllers/Api/ProductsController.cs`

**Interfaces:**
- Produces: `GET /api/Products/paged?minPrice=X&maxPrice=Y&categoryProductId=Z` endpoint.

- [ ] **Step 1: Update Controller GetPaged Endpoint**

Modify [ProductsController.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Controllers/Api/ProductsController.cs):
```csharp
        [AllowAnonymous]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 8,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? categoryProductId = null)
        {
            var result = await _productService.GetPaged(page, pageSize, minPrice, maxPrice, categoryProductId);
            return Ok(result);
        }
```

- [ ] **Step 2: Compile & Verify**

Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
Expected: Build successfully.

- [ ] **Step 3: Commit**

```bash
git add CMS.Backend/Controllers/Api/ProductsController.cs
git commit -m "feat: update ProductsController.GetPaged API to accept query filters"
```

---

### Task 4: Update Frontend API Client and Query Hooks

**Files:**
- Modify: `cms.frontend/src/services/productService.ts`
- Modify: `cms.frontend/src/hooks/useProducts.ts`

**Interfaces:**
- Produces: updated TS client signatures and Query Keys matching React Query requirements.

- [ ] **Step 1: Update productService TS client**

Modify [productService.ts](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/services/productService.ts):
```typescript
    getProductsPaged: async (
        page: number, 
        pageSize: number, 
        minPrice?: number | null, 
        maxPrice?: number | null, 
        categoryProductId?: number | null
    ) => {
        try {
            const searchParams = new URLSearchParams();
            searchParams.set('page', page.toString());
            searchParams.set('pageSize', pageSize.toString());
            if (minPrice !== undefined && minPrice !== null) searchParams.set('minPrice', minPrice.toString());
            if (maxPrice !== undefined && maxPrice !== null) searchParams.set('maxPrice', maxPrice.toString());
            if (categoryProductId !== undefined && categoryProductId !== null) searchParams.set('categoryProductId', categoryProductId.toString());
            
            const response = await axiosClient.get(`/Products/paged?${searchParams.toString()}`);
            return response.data || response;
        } catch (error) {
            console.error('API getProductsPaged error:', error);
            throw error;
        }
    },
```

- [ ] **Step 2: Update useProductsPaged hook**

Modify [useProducts.ts](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/hooks/useProducts.ts):
```typescript
export const useProductsPaged = (
  page: number, 
  pageSize: number, 
  minPrice?: number | null, 
  maxPrice?: number | null, 
  categoryProductId?: number | null
) => {
  return useQuery<PagedResult<any>>({
    queryKey: ['products', 'paged', page, pageSize, minPrice, maxPrice, categoryProductId],
    queryFn: () => productService.getProductsPaged(page, pageSize, minPrice, maxPrice, categoryProductId),
    placeholderData: (prev) => prev,
  });
};
```

- [ ] **Step 3: Verify TS type safety**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile successfully.

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/services/productService.ts cms.frontend/src/hooks/useProducts.ts
git commit -m "feat: update frontend product service and hook to pass filters"
```

---

### Task 5: Implement Filter UI and Debounced Query State on Shop Page

**Files:**
- Modify: `cms.frontend/src/pages/shop/ShopSidebar.tsx`
- Modify: `cms.frontend/src/pages/shop/index.tsx`

**Interfaces:**
- Produces: updated Shop page interface with interactive price range filter controls (presets & inputs) and debounced updates.

- [ ] **Step 1: Update ShopSidebar component**

Modify [ShopSidebar.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/shop/ShopSidebar.tsx) to render presets and custom inputs:
```tsx
import React, { useState } from 'react';
import { useProductCategories } from '../../hooks/useCategories';

interface ShopSidebarProps {
  onCategoryChange: (id: number | null) => void;
  activeCategoryId: number | null;
  onPriceChange: (min: number | null, max: number | null) => void;
  activePricePreset: string | null;
  setActivePricePreset: (preset: string | null) => void;
}

const ShopSidebar = ({ 
  onCategoryChange, 
  activeCategoryId, 
  onPriceChange,
  activePricePreset,
  setActivePricePreset
}: ShopSidebarProps) => {
  const { data: categories = [] } = useProductCategories();
  const [minInput, setMinInput] = useState('');
  const [maxInput, setMaxInput] = useState('');

  const handlePresetSelect = (preset: string | null, min: number | null, max: number | null) => {
    setActivePricePreset(preset);
    setMinInput('');
    setMaxInput('');
    onPriceChange(min, max);
  };

  const handleCustomApply = (e: React.FormEvent) => {
    e.preventDefault();
    setActivePricePreset('custom');
    const minVal = minInput ? Number(minInput) : null;
    const maxVal = maxInput ? Number(maxInput) : null;
    onPriceChange(minVal, maxVal);
  };

  return (
    <div className="flex flex-col gap-stack-md bg-surface-container-lowest border border-outline-variant/20 rounded-2xl p-6 shadow-sm">
      <div className="mb-stack-sm border-b border-outline-variant/20 pb-4 flex justify-between items-center">
        <h2 className="font-headline-sm text-headline-sm text-on-surface">Lọc theo</h2>
        <button
          className="font-label-sm text-label-sm text-primary hover:underline bg-transparent border-0 p-0 cursor-pointer"
          onClick={() => {
            onCategoryChange(null);
            handlePresetSelect(null, null, null);
          }}
        >
          Xóa tất cả
        </button>
      </div>

      <div className="flex flex-col gap-stack-sm border-b border-outline-variant/20 pb-4">
        <h3 className="font-label-md text-label-md text-on-surface uppercase tracking-widest">Danh mục</h3>
        <div className="flex flex-col gap-2">
          <button
            className={`text-left bg-transparent border-0 p-0 font-body-md text-body-md transition-colors cursor-pointer ${
              activeCategoryId === null ? 'text-primary font-semibold' : 'text-on-surface-variant hover:text-primary'
            }`}
            onClick={() => onCategoryChange(null)}
          >
            Tất cả
          </button>
          {(categories as any[]).map((cat: any) => (
            <button
              key={cat.id}
              className={`text-left bg-transparent border-0 p-0 font-body-md text-body-md transition-colors cursor-pointer ${
                activeCategoryId === cat.id ? 'text-primary font-semibold' : 'text-on-surface-variant hover:text-primary'
              }`}
              onClick={() => onCategoryChange(cat.id)}
            >
              {cat.name}
            </button>
          ))}
        </div>
      </div>

      <div className="flex flex-col gap-stack-sm border-b border-outline-variant/20 pb-4">
        <h3 className="font-label-md text-label-md text-on-surface uppercase tracking-widest">Khoảng giá</h3>
        <div className="flex flex-col gap-2">
          {[
            { id: 'all', label: 'Tất cả giá', min: null, max: null },
            { id: 'under200', label: 'Dưới 200.000đ', min: null, max: 200000 },
            { id: '200to500', label: '200.000đ - 500.000đ', min: 200000, max: 500000 },
            { id: '500to1000', label: '500.000đ - 1.000.000đ', min: 500000, max: 1000000 },
            { id: 'above1000', label: 'Trên 1.000.000đ', min: 1000000, max: null }
          ].map((preset) => (
            <label key={preset.id} className="flex items-center gap-3 cursor-pointer group">
              <input 
                className="border-outline-variant text-primary focus:ring-primary/20 w-4 h-4 transition-colors" 
                name="pricePreset" 
                type="radio"
                checked={activePricePreset === preset.id || (preset.id === 'all' && activePricePreset === null)}
                onChange={() => handlePresetSelect(preset.id === 'all' ? null : preset.id, preset.min, preset.max)}
              />
              <span className={`font-body-md text-body-md text-on-surface-variant group-hover:text-primary transition-colors ${
                (activePricePreset === preset.id || (preset.id === 'all' && activePricePreset === null)) ? 'text-primary font-semibold' : ''
              }`}>
                {preset.label}
              </span>
            </label>
          ))}
        </div>

        {/* Custom Price Range Input */}
        <form onSubmit={handleCustomApply} className="mt-4 space-y-3">
          <p className="font-label-sm text-label-sm text-on-surface-variant font-medium">Khoảng giá tự chọn (VND)</p>
          <div className="flex items-center gap-2">
            <input 
              type="number"
              placeholder="Từ"
              value={minInput}
              onChange={(e) => setMinInput(e.target.value)}
              className="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg px-3 py-1.5 text-sm placeholder:text-outline/50 focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary/20"
            />
            <span className="text-outline">-</span>
            <input 
              type="number"
              placeholder="Đến"
              value={maxInput}
              onChange={(e) => setMaxInput(e.target.value)}
              className="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg px-3 py-1.5 text-sm placeholder:text-outline/50 focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary/20"
            />
          </div>
          <button 
            type="submit"
            className="w-full bg-primary text-on-primary py-2 rounded-lg font-label-sm text-label-sm hover:bg-primary/90 active:scale-[0.98] transition-all border-0 cursor-pointer"
          >
            Áp dụng
          </button>
        </form>
      </div>
    </div>
  );
};

export default ShopSidebar;
```

- [ ] **Step 2: Update Shop index page**

Modify [index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/shop/index.tsx) to integrate price filters and 500ms debounce:
```tsx
import React, { useState, useEffect } from 'react';
import ShopSidebar from './ShopSidebar';
import ShopHeader from './ShopHeader';
import ProductList from './ProductList';
import Pagination from '../../components/Pagination';
import { useProductsPaged } from '../../hooks/useProducts';

const ShopPage: React.FC = () => {
  const [page, setPage] = useState(1);
  const pageSize = 9;

  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);
  
  // Realtime UI state
  const [priceRange, setPriceRange] = useState<{ min: number | null, max: number | null }>({ min: null, max: null });
  const [activePricePreset, setActivePricePreset] = useState<string | null>(null);

  // Debounced API state
  const [debouncedMin, setDebouncedMin] = useState<number | null>(null);
  const [debouncedMax, setDebouncedMax] = useState<number | null>(null);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedMin(priceRange.min);
      setDebouncedMax(priceRange.max);
      setPage(1);
    }, 500);

    return () => clearTimeout(handler);
  }, [priceRange]);

  const { data: paged, isLoading, error } = useProductsPaged(page, pageSize, debouncedMin, debouncedMax, selectedCategoryId);

  const products = paged?.items ?? [];

  const handleCategoryChange = (id: number | null) => {
    setSelectedCategoryId(id);
    setPage(1);
  };

  const handlePriceChange = (min: number | null, max: number | null) => {
    setPriceRange({ min, max });
  };

  return (
    <div className="flex-grow w-full max-w-container-max mx-auto px-margin-desktop py-stack-lg flex flex-col md:flex-row gap-gutter">
      <aside className="w-full md:w-64 flex-shrink-0">
        <ShopSidebar 
          onCategoryChange={handleCategoryChange} 
          activeCategoryId={selectedCategoryId}
          onPriceChange={handlePriceChange}
          activePricePreset={activePricePreset}
          setActivePricePreset={setActivePricePreset}
        />
      </aside>
      <section className="flex-grow">
        <ShopHeader count={paged?.totalCount ?? 0} page={paged?.page} pageSize={paged?.pageSize} />
        <ProductList products={products} isLoading={isLoading} error={error ? "Không thể tải bộ sưu tập vào lúc này." : null} />
        {paged && paged.totalPages > 1 && (
          <Pagination
            page={paged.page}
            totalPages={paged.totalPages}
            onPageChange={setPage}
          />
        )}
      </section>
    </div>
  );
};

export default ShopPage;
```

- [ ] **Step 3: Verify TypeScript compilation**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile cleanly.

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/pages/shop/
git commit -m "feat: implement shop sidebar price filtering with custom inputs and presets"
```
