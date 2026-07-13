# Kiến Trúc Bộ Nhớ Đệm Cấu Hình (Settings Cache Architecture)

Tài liệu chi tiết về thiết kế, triển khai bộ nhớ đệm (Cache) cho hệ thống cấu hình động của FlowerShop nhằm tối ưu hóa hiệu năng, giảm tải truy vấn cơ sở dữ liệu.

---

## 1. Thiết Kế Bộ Nhớ Đệm (Cache Design)

Do các thông số cấu hình hệ thống (như thông tin cửa hàng, cổng thanh toán, SMTP) được truy xuất rất nhiều lần trên mỗi request của khách hàng và admin, việc truy vấn SQL Server liên tục sẽ gây nghẽn băng thông và suy giảm hiệu năng. 
Hệ thống sử dụng bộ nhớ đệm nội bộ **Memory Cache** (`IMemoryCache`) của ASP.NET Core để lưu trữ tạm thời các đối tượng cấu hình sau lần đọc đầu tiên.

```
                  +--------------------------------+
                  |  Client Request / Background   |
                  +--------------------------------+
                                  |
                                  v
                  +--------------------------------+
                  |  SystemSettingService          |
                  +--------------------------------+
                                  |
                   [Kiểm tra Cache theo Key]
                                  |
                  +---------------+---------------+
                  |                               |
          {Cache Hit}                     {Cache Miss}
                  |                               |
                  v                               v
         [Trả về đối tượng]               [Truy vấn SQL Server]
                                                  |
                                                  v
                                         [Giải mã JSON & Decrypt]
                                                  |
                                                  v
                                         [Lưu vào Cache 30 phút]
                                                  |
                                                  v
                                         [Trả về đối tượng]
```

---

## 2. Các Quy Tắc Cấu Hình Cache

### 2.1. Khóa Cache (Cache Keys)
Mỗi nhóm cấu hình được phân biệt bằng tiền tố rõ ràng để tránh xung đột dữ liệu:
- Nhóm thông tin cửa hàng: `setting_StoreInfo`
- Nhóm SMTP: `setting_Smtp`
- Nhóm VNPay: `setting_VNPay`
- Nhóm vận chuyển: `setting_Shipping`
- Nhóm đơn hàng: `setting_Order`

### 2.2. Thời Gian Sống (Expiration)
- **Absolute Expiration (Hạn dùng tuyệt đối)**: `30 phút`. Sau 30 phút kể từ lúc lưu cache, dữ liệu sẽ tự động hết hạn và được nạp mới từ database ở request tiếp theo.
- **Không dùng Singleton**: Dữ liệu cấu hình không lưu cứng trong bộ nhớ tĩnh để tránh lỗi rò rỉ bộ nhớ (Memory Leak) và đảm bảo tính nhất quán dữ liệu khi chạy multi-thread.

---

## 3. Cơ Chế Invalidation (Xóa Đệm Tức Thì)

Khi Quản trị viên (Admin) thay đổi cấu hình tại giao diện quản trị, dữ liệu mới sẽ được lưu trực tiếp vào CSDL. Để đảm bảo các client khác nhận được cấu hình mới ngay lập tức mà không cần chờ hết hạn 30 phút, hệ thống thực hiện **Invalidation (Xóa đệm)** chủ động:

```csharp
// Khi thực hiện lưu cấu hình mới thành công
await _context.SaveChangesAsync();

// Invalidate Cache ngay lập tức bằng cách xóa Key tương ứng khỏi MemoryCache
string cacheKey = $"setting_{key}";
_cache.Remove(cacheKey);
```

### Kết quả:
- Luồng gửi mail tiếp theo hoặc phiên thanh toán VNPay tiếp theo sẽ gặp trạng thái **Cache Miss**.
- Hệ thống tự động truy vấn CSDL để lấy thông số mới nhất, nạp lại vào Cache và áp dụng ngay lập tức mà **không cần restart Backend**.
- Đảm bảo tính thời gian thực (real-time) và tính nhất quán dữ liệu cực cao.
