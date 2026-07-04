```markdown
# Workflow: Chức năng Bộ Lọc Giá Sản Phẩm (Price Range Filter)

Tài liệu này đặc tả luồng xử lý dữ liệu từ giao diện người dùng (Frontend ReactJS/Next.js) đến hệ thống xử lý ngầm (Backend API ASP.NET Core) và truy vấn dữ liệu tối ưu (SQL Server) cho tính năng lọc sản phẩm theo khoảng giá.

---

## I. Các website lớn họ có xử lý như vậy không?

**CÓ, nhưng họ thêm các kỹ thuật tối ưu hóa để chống sập hệ thống.**

Về mặt trải nghiệm người dùng (UX), các trang web như Shopee, Lazada, Tiki hay Amazon đều tự động cập nhật sản phẩm khi người dùng thay đổi giá mà không cần bấm nút "Tìm kiếm". Tuy nhiên, ở phía Backend và hạ tầng, họ xử lý rất nghiêm ngặt vì 2 lý do:
1. **Lượng yêu cầu khổng lồ (High Traffic Spam):** Khi người dùng kéo thanh trượt (Slider), sự kiện thay đổi giá trị (`onChange`) kích hoạt liên tục (hàng chục lần mỗi giây). Nếu cứ mỗi mili-giây kéo slider lại bắn 1 API về Backend, hệ thống sẽ lập tức bị nghẽn mạch (DDoS vô ý).
2. **Hạ tầng tìm kiếm chuyên dụng:** Các trang lớn không tìm kiếm giá trực tiếp bằng lệnh `WHERE Price >= Min AND Price <= Max` trong SQL Server. Họ đồng bộ dữ liệu sang các bộ máy tìm kiếm chuyên dụng như **Elasticsearch** hoặc **Algolia** để trả kết quả trong vòng vài mili-giây.

Với dự án hiện tại của bạn, chúng ta hoàn toàn có thể đạt hiệu năng mượt mà tương tự bằng cách sử dụng kỹ thuật **Debounce** ở Frontend và **Tạo chỉ mục (Index)** ở SQL Server.

---

## II. Sơ đồ Luồng Hoạt động (Workflow Diagram)


```

[Người dùng kéo Slider / Nhập ô Min-Max]
│
▼ (Sự kiện onChange kích hoạt liên tục)
[Lớp Chốt Chặn: Debounce (Chờ 500ms)]
│
├── Người dùng vẫn đang kéo ──► Hủy kích hoạt API, tiếp tục đợi
└── Người dùng dừng kéo 500ms ─► Kích hoạt gọi API ngầm (Fetch/Axios)
│
▼ (Next.js gọi API: GET /api/v1/products?minPrice=X&maxPrice=Y)
[Backend ASP.NET Core nhận tham số]
│
▼ (Chuyển đổi kiểu dữ liệu, gán giá trị mặc định nếu rỗng)
[SQL Server truy vấn dữ liệu tối ưu]
│
▼ (SELECT ... WHERE BasePrice >= @Min AND BasePrice <= @Max)
[Backend nhận kết quả ──► Trả JSON về cho Next.js ──► Cập nhật State ──► Re-render lưới sản phẩm]

```

---

## III. Chi tiết Triển khai Kỹ thuật

### 1. Xử lý tại Frontend (Next.js / ReactJS) - Kỹ thuật Debounce

Để tránh việc gửi API liên tục khi người dùng đang kéo thanh trượt, chúng ta dùng kỹ thuật **Debounce**. Thuật toán này hiểu đơn giản là: *"Chờ người dùng dừng tay hoàn toàn trong vòng 500ms thì mới chính thức gọi API"*.

* **Logic Code tại `Shop.jsx`:**
  ```javascript
  import { useState, useEffect } from 'react';

  export default function Shop() {
    const [products, setProducts] = useState([]);
    const [priceRange, setPriceRange] = useState({ min: 0, max: 2000000 }); // Giá trị hiển thị trên UI
    const [debouncedPrice, setDebouncedPrice] = useState({ min: 0, max: 2000000 }); // Giá trị thực tế để gọi API

    // Luồng xử lý Debounce: Lắng nghe sự thay đổi của priceRange
    useEffect(() => {
      const handler = setTimeout(() => {
        setDebouncedPrice(priceRange);
      }, 500); // 500 mili-giây

      return () => clearTimeout(handler); // Hủy lệnh chờ nếu người dùng lại tiếp tục kéo slider
    }, [priceRange]);

    // Luồng gọi API ngầm khi giá debounced đã chốt
    useEffect(() => {
      async function fetchFilteredProducts() {
        const response = await fetch(`/api/v1/products?minPrice=${debouncedPrice.min}&maxPrice=${debouncedPrice.max}`);
        const data = await response.json();
        setProducts(data); // Cập nhật lại lưới sản phẩm
      }
      fetchFilteredProducts();
    }, [debouncedPrice]);

    return (
      <div>
        {/* Render Thanh trượt Range Slider và 2 ô nhập ở đây */}
        {/* Khi OnChange chỉ cập nhật setPriceRange, không gọi API trực tiếp */}
      </div>
    );
  }

```

### 2. Xử lý tại Backend (ASP.NET Core API)

Backend đóng vai trò nhận tham số một cách an toàn, bóc tách chuỗi Query String từ URL và truyền xuống tầng dữ liệu.

* **Cấu trúc API Endpoint:** `GET /api/v1/products?minPrice=100000&maxPrice=500000`
* **Logic Controller xử lý đầu vào:**
```csharp
[HttpGet]
public async Task<IActionResult> GetProducts([FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
{
    // Phòng hờ trường hợp khách không nhập, gán giá trị mặc định
    decimal filterMin = minPrice ?? 0;
    decimal filterMax = maxPrice ?? 999999999; // Giá tối đa mặc định khổng lồ

    var filteredProducts = await _context.Products
        .Where(p => p.BasePrice >= filterMin && p.BasePrice <= filterMax && p.Status == "active")
        .ToListAsync();

    return Ok(filteredProducts); // Trả về JSON kết quả
}

```



### 3. Tối ưu hóa Cơ sở Dữ liệu (SQL Server) - Chốt chặn Hiệu năng

Khi số lượng sản phẩm lên tới hàng nghìn, việc chạy lệnh `WHERE BasePrice >= ...` sẽ bắt SQL Server phải quét qua từng dòng của bảng (Table Scan), gây chậm hệ thống.

* **Giải pháp áp dụng:** Tạo một **Non-Clustered Index (Chỉ mục phụ)** cho cột giá tiền trong SQL Server để Database tra cứu nhanh theo thuật toán cây nhị phân mà không cần quét toàn bảng.
* **Câu lệnh SQL cần chạy:**
```sql
CREATE NONCLUSTERED INDEX IX_Products_BasePrice 
ON Products (BasePrice) 
INCLUDE (Name, Sku, Status); -- Gom thêm các cột hiển thị để tăng tốc độ truy vấn

```



---

## IV. Tóm tắt Tham số Vận hành (Best Practices)

| Thành phần | Kỹ thuật áp dụng | Mục đích |
| --- | --- | --- |
| **Frontend** | Debounce (Thời gian chờ 500ms) | Giảm số lượng request API vô hạn khi kéo Slider. |
| **Backend** | Nullable Parameters (`decimal?`) | Linh hoạt xử lý khi khách chỉ nhập ô Min mà bỏ trống ô Max hoặc ngược lại. |
| **Database** | Non-Clustered Index trên cột giá | Tăng tốc độ truy vấn SQL từ hàng giây xuống vài mili-giây. |

```

```