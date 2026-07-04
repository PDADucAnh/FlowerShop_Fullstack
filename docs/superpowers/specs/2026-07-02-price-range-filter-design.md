# Tài liệu Đặc tả Thiết kế: Chức năng Bộ Lọc Giá (Price Range Filter)

Tài liệu này đặc tả kiến trúc tích hợp bộ lọc giá sản phẩm (Price Range Filter) đồng bộ từ giao diện người dùng tới cơ sở dữ liệu, tối ưu hóa phân trang và hiệu năng.

---

## 1. Thiết kế Cơ sở dữ liệu (Database Design)

Để tăng tốc độ truy vấn khoảng giá trên bảng `Products`, chúng ta tạo một Non-Clustered Index trên cột `BasePrice`:

```sql
CREATE NONCLUSTERED INDEX IX_Products_BasePrice 
ON Products (BasePrice) 
INCLUDE (Name, Slug, Sku, Status, ImageUrl);
```

---

## 2. API Backend & Nghiệp vụ (ASP.NET Core)

### A. Cập nhật Interface & Service Product

#### `IProductService.cs` (`CMS.Backend/Services/Interfaces/IProductService.cs`)
```csharp
Task<PagedResult<ProductDTO>> GetPaged(
    int page, 
    int pageSize, 
    decimal? minPrice = null, 
    decimal? maxPrice = null, 
    int? categoryProductId = null
);
```

#### `ProductService.cs` (`CMS.Backend/Services/ProductService.cs`)
Cập nhật logic `GetPaged` để lọc động trước khi phân trang:
```csharp
public async Task<PagedResult<ProductDTO>> GetPaged(
    int page, 
    int pageSize, 
    decimal? minPrice = null, 
    decimal? maxPrice = null, 
    int? categoryProductId = null)
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

### B. Controller API (`ProductsController.cs`)
Cập nhật Endpoint `GetPaged`:
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

---

## 3. Giao diện và Luồng Frontend (React SPA)

### A. Cấu hình API Client & React Hooks

#### `productService.ts` (`cms.frontend/src/services/productService.ts`)
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
    }
```

#### `useProducts.ts` (`cms.frontend/src/hooks/useProducts.ts`)
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

### B. Tích hợp Trang Cửa hàng (`cms.frontend/src/pages/shop/index.tsx`)
*   Sử dụng cơ chế Debounce 500ms đối với `minPrice` và `maxPrice`.
*   Nối `debouncedMinPrice`, `debouncedMaxPrice` và `selectedCategoryId` vào hook `useProductsPaged`.

### C. Sidebar Bộ lọc (`cms.frontend/src/pages/shop/ShopSidebar.tsx`)
*   Hiển thị danh sách các mốc khoảng giá VND:
    *   Tất cả giá
    *   Dưới 200.000đ
    *   200.000đ - 500.000đ
    *   500.000đ - 1.000.000đ
    *   Trên 1.000.000đ
*   Bổ sung 2 ô nhập Min / Max thủ công kèm theo nút bấm áp dụng bộ lọc trực tiếp.
