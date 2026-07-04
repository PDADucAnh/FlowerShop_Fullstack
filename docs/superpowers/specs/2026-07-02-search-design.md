# Tài liệu Đặc tả Thiết kế: Chức năng Tìm kiếm Sản phẩm (Search)

Tài liệu này đặc tả kiến trúc tích hợp hệ thống tìm kiếm sản phẩm từ Header, điều hướng trang hiển thị kết quả `/search?query=...` và xử lý truy vấn an toàn ở Backend.

---

## 1. API Backend & Nghiệp vụ (ASP.NET Core)

### A. Cập nhật Interface & Service Product

#### `IProductService.cs` (`CMS.Backend/Services/Interfaces/IProductService.cs`)
Bổ sung định nghĩa phương thức tìm kiếm:
```csharp
Task<IEnumerable<ProductDTO>> Search(string query);
```

#### `ProductService.cs` (`CMS.Backend/Services/ProductService.cs`)
Triển khai tìm kiếm không phân biệt chữ hoa thường (Case-Insensitive) trên tên và SKU:
```csharp
public async Task<IEnumerable<ProductDTO>> Search(string query)
{
    if (string.IsNullOrEmpty(query))
    {
        return new List<ProductDTO>();
    }

    var cleanQuery = query.Trim().ToLower();
    var products = await BuildQuery()
        .Where(p => p.Name.ToLower().Contains(cleanQuery) || 
                   (p.Sku != null && p.Sku.ToLower().Contains(cleanQuery)))
        .OrderByDescending(p => p.Id)
        .ToListAsync();

    return products.Select(p => p.ToDTO());
}
```

### B. Controller API (`ProductsController.cs`)
Expose endpoint tìm kiếm `GET /api/Products/search?query=...`:
```csharp
[AllowAnonymous]
[HttpGet("search")]
public async Task<IActionResult> Search([FromQuery] string query)
{
    var results = await _productService.Search(query);
    return Ok(results);
}
```

---

## 2. Giao diện và Luồng Frontend (React SPA)

### A. Cấu hình API Client & React Hooks

#### `productService.ts` (`cms.frontend/src/services/productService.ts`)
```typescript
    searchProducts: async (query: string) => {
        try {
            const response = await axiosClient.get(`/Products/search?query=${encodeURIComponent(query)}`);
            return response.data || response;
        } catch (error) {
            console.error('API searchProducts error:', error);
            throw error;
        }
    }
```

#### `useProducts.ts` (`cms.frontend/src/hooks/useProducts.ts`)
```typescript
export const useSearchProducts = (query: string) => {
  return useQuery<any[]>({
    queryKey: ['products', 'search', query],
    queryFn: () => productService.searchProducts(query),
    enabled: !!query,
  });
};
```

### B. Cập nhật thanh tìm kiếm trên Header (`cms.frontend/src/components/Header.tsx`)
Thay đổi cơ chế điều hướng khi người dùng nhấn `Enter` hoặc click vào nút Tìm kiếm:
```typescript
navigate(`/search?query=${encodeURIComponent(searchQuery.trim())}`);
```

### C. Tạo trang hiển thị kết quả (`cms.frontend/src/pages/search/index.tsx`)
*   Sử dụng `useSearchParams()` để lấy `query` từ URL.
*   Gọi hook `useSearchProducts(query)` để fetch dữ liệu.
*   Hiển thị danh sách kết quả dạng lưới bằng component `ProductCard.tsx`.
