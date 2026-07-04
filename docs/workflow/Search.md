```markdown
# Workflow: Chức năng Tìm kiếm trên Header và Điều hướng Kết quả

Tài liệu này đặc tả luồng xử lý dữ liệu và trải nghiệm người dùng (UX) cho tính năng tìm kiếm sản phẩm/bài viết từ ô tìm kiếm tập trung trên Header, thực hiện gọi API ngầm và điều hướng trang hiển thị kết quả.

---

## I. Phân tích Hành vi Hệ thống (UX/UI & Architecture)

Đối với các website thương mại điện tử hiện đại, thanh tìm kiếm trên Header thường hoạt động theo 2 mô hình phối hợp:
1. **Tìm kiếm nhanh gợi ý (Instant Search Suggestion):** Khi người dùng gõ phím, một menu nhỏ thả xuống (dropdown) ngay dưới ô tìm kiếm để hiển thị nhanh 3-5 sản phẩm khớp từ khóa mà chưa cần chuyển trang.
2. **Tìm kiếm toàn trang (Full Search Page):** Khi người dùng bấm phím `Enter` hoặc click vào icon kính lúp, hệ thống sẽ điều hướng (Redirect) họ sang một trang riêng (Ví dụ: `/search?query=từ-khóa`) để hiển thị toàn bộ danh sách bộ lọc nâng cao.

Phương án dưới đây triển khai mô hình **Tìm kiếm toàn trang** kết hợp kỹ thuật xử lý bất đồng bộ, đồng thời tận dụng sức mạnh truy vấn văn bản của **SQL Server** mà không làm tăng độ phức tạp công nghệ.

---

## II. Sơ đồ Luồng Hoạt động (Workflow Diagram)


```

[Người dùng gõ ký tự vào ô Tìm kiếm trên Header]
│
├── Trường hợp 1: Nhập từ khóa + Bấm ENTER hoặc click Kính lúp
│     │
│     ▼ (Frontend xử lý URL Encode từ khóa)
│   [Điều hướng sang trang kết quả: /search?query=hoa+hong]
│     │
│     ▼ (Trang /search tự động bóc tách từ khóa từ URL Query)
│   [Next.js thực hiện gọi API ngầm về Backend]
│
└── Trường hợp 2: Gõ ký tự hiển thị gợi ý nhanh (Dropdown)
│
▼ (Áp dụng Debounce 400ms để tránh spam request)
[Gọi API lấy nhanh gợi ý: GET /api/v1/search/suggest?q=hoa]
│
▼ (Hiển thị Dropdown danh sách kết quả nhanh dưới Header)

```
       │
       ▼ (API Backend tiếp nhận: GET /api/v1/search?query=...)

```

[Backend ASP.NET Core nhận tham số và làm sạch chuỗi tìm kiếm]
│
▼ (Truy vấn SQL Server sử dụng cơ chế Like hoặc Full-Text Search)
[SQL Server trả kết quả ──► ASP.NET phản hồi JSON ──► Frontend render lưới sản phẩm]

```

---

## III. Chi tiết Triển khai Kỹ thuật trên Stack Công nghệ

### 1. Xử lý tại Frontend (Next.js - Thành phần Header chung)

Thành phần Header cần quản lý trạng thái của ô nhập liệu và kích hoạt lệnh điều hướng trang bằng hook `useRouter` của Next.js.

* **Logic Code tại `Header.jsx`:**
  ```javascript
  import { useState } from 'react';
  import { useRouter } from 'next/router'; // Hoặc 'next/navigation' nếu dùng App Router

  export default function Header() {
    const [searchTerm, setSearchTerm] = useState('');
    const router = useRouter();

    const handleSearchSubmit = (e) => {
      e.preventDefault(); // Chặn hành vi reload trang mặc định của Form
      
      if (!searchTerm.trim()) return; // Nếu ô tìm kiếm rỗng thì không xử lý

      // Tiến hành điều hướng sang trang search kèm tham số query đã được mã hóa URL mã hóa an toàn
      router.push(`/search?query=${encodeURIComponent(searchTerm.trim())}`);
    };

    return (
      <header>
        <form onSubmit={handleSearchSubmit} className="search-box">
          <input 
            type="text" 
            placeholder="Tìm kiếm sản phẩm..." 
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          <button type="submit">🔍</button>
        </form>
      </header>
    );
  }

```

* **Logic Code tại trang hiển thị kết quả (`pages/search.jsx`):**
Trang này sẽ chịu trách nhiệm bóc tách từ khóa từ thanh địa chỉ và gọi API ngầm về Backend để lấy dữ liệu.
```javascript
import { useRouter } from 'next/router';
import { useEffect, useState } from 'react';

export default function SearchPage() {
  const router = useRouter();
  const { query } = router.query; // Lấy tham số ?query= từ URL
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!query) return;

    async function startSearch() {
      setLoading(true);
      const response = await fetch(`/api/v1/search?query=${encodeURIComponent(query)}`);
      const data = await response.json();
      setProducts(data);
      setLoading(false);
    }

    startSearch();
  }, [query]); // Khởi chạy lại hàm mỗi khi từ khóa trên URL thay đổi

  return (
    <div className="search-result-container">
      <h2>Kết quả tìm kiếm cho: "{query}"</h2>
      {loading ? <p>Đang tìm kiếm...</p> : <ProductGrid items="{products}"/>}
    </div>
  );
}

```



### 2. Xử lý tại Backend (ASP.NET Core API)

Backend xử lý làm sạch chuỗi đầu vào (tránh lỗi SQL Injection) và thực hiện tìm kiếm không phân biệt chữ hoa, chữ thường, có dấu hay không dấu.

* **Cấu trúc API Endpoint:** `GET /api/v1/search?query=hoa%20hong`
* **Logic Controller xử lý:**
```csharp
[HttpGet("api/v1/search")]
public async Task<IActionResult> SearchProducts([FromQuery] string query)
{
    if (string.IsNullOrEmpty(query)) {
        return Ok(new List<Product>());
    }

    string cleanQuery = query.Trim().ToLower();

    // Sử dụng LINQ / Entity Framework Core truy vấn cơ bản dạng LIKE
    var results = await _context.Products
        .Where(p => p.Status == "active" && 
                   (p.Name.ToLower().Contains(cleanQuery) || p.Sku.ToLower().Contains(cleanQuery)))
        .ToListAsync();

    return Ok(results);
}

```



### 3. Tối ưu hóa Cơ sở Dữ liệu (SQL Server) - Chống thắt nút cổ chai

Mặc định, câu lệnh `Contains` trong Entity Framework khi biên dịch ra SQL sẽ có dạng `WHERE Name LIKE '%từ-khóa%'`. Dấu `%` ở phía trước từ khóa khiến SQL Server **không thể sử dụng chỉ mục thông thường (Index Scan)** mà phải quét toàn bộ bảng dữ liệu, dẫn đến IO cao và chậm hệ thống nếu bảng có hàng vạn bản ghi.

* **Giải pháp 1 (Quy mô nhỏ - Dưới 10.000 sản phẩm):**
Đảm bảo cột `Name` và `Sku` trong SQL Server được cấu hình hệ mã **Collation kiểu Insensitive (`SQL_Latin1_General_CP1_CI_AI`)** để hệ thống tự động nhận diện không phân biệt chữ hoa/thường (`CI`) và không phân biệt dấu tiếng Việt (`AI`).
* **Giải pháp 2 (Quy mô lớn - Khuyên dùng cho Đồ án tốt nghiệp):**
Kích hoạt tính năng **Full-Text Search (FTS)** tích hợp sẵn trong SQL Server. Đây là công nghệ chuyên dụng tạo ra các chỉ mục từ vựng (Token) giống như Elasticsearch, giúp tìm kiếm theo từ khóa có dấu/không dấu cực kỳ nhanh.
```sql
-- Câu lệnh SQL tạo Full-Text Catalog và Index cho bảng Products
CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;
CREATE FULLTEXT INDEX ON Products(Name, Description)
KEY INDEX PK_Products_Id; -- Khóa chính của bảng

```


Khi đã bật FTS, trong code ASP.NET Core bạn có thể chuyển sang sử dụng hàm `EF.Functions.Contains(p.Name, cleanQuery)` hoặc `FreeText` để SQL Server thực hiện tìm kiếm toàn văn với tốc độ chỉ vài mili-giây.

---

## IV. Tóm tắt Tiêu chuẩn Vận hành (Best Practices)

| Thành phần | Kỹ thuật áp dụng | Mục đích |
| --- | --- | --- |
| **Frontend** | `encodeURIComponent()` | Mã hóa các ký tự đặc biệt (khoảng trắng, dấu tiếng Việt) trên URL để không làm gãy đường link. |
| **Frontend** | URL Query State (`?query=`) | Giúp người dùng có thể copy đường link kết quả tìm kiếm gửi cho người khác, hoặc bấm nút Back quay lại trang trước một cách tự nhiên. |
| **Database** | Full-Text Search (SQL Server) | Đảm bảo tốc độ tìm kiếm văn bản tối ưu, hỗ trợ tìm kiếm không dấu, có dấu và chịu tải tốt. |

```

```