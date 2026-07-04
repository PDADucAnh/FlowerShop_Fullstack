```markdown
# Workflow: Chức năng Quên Mật khẩu (Forgot Password)

Tài liệu này đặc tả luồng hoạt động chi tiết cho chức năng Quên mật khẩu dựa trên kiến trúc hệ thống: **ASP.NET Core (Backend API), Next.js (Frontend), SQL Server (Database)** và sử dụng **Gmail SMTP (App Password)** để gửi mã xác thực.

---

## I. Thiết kế Cơ sở Dữ liệu (SQL Server)

Bổ sung 2 trường dữ liệu vào bảng `Users` để quản lý và xác thực mã đặt lại mật khẩu sử dụng cơ chế **One-Time Reset Token**:

```sql
ALTER TABLE Users 
ADD ResetToken NVARCHAR(MAX) NULL,          -- Chuỗi token ngẫu nhiên dùng 1 lần
    ResetTokenExpiry DATETIME NULL;         -- Thời gian hết hạn của token (TTL: 15 phút)

```

---

## II. Sơ đồ Luồng Hoạt động (Workflow Diagram)

```
[Khách bấm Quên mật khẩu]
           │
           ▼ (Frontend Next.js gửi Email)
[Backend ASP.NET check Email trong SQL Server]
           │
           ├── Không tồn tại ──► Trả phản hồi giả: "Đã gửi mail" (Chống dò quét tài khoản)
           └── Tồn tại ────────► 1. Tạo ngẫu nhiên Reset Token (Guid.NewGuid)
                                 2. Tính Expiry = DateTime.Now.AddMinutes(15)
                                 3. Lưu ngầm Token & Expiry vào SQL Server
                                 4. Gửi Link đặt lại mật khẩu qua Gmail thật
                                 5. Trả thông báo "Vui lòng check hộp thư"
           │
           ▼ (Khách click vào Link trong Email gửi về)
[Giao diện Next.js mở Form đặt lại Mật khẩu mới]
           │
           ▼ (Frontend gửi Token + Mật khẩu mới lên API Backend)
[Backend Xác thực Token Chốt chặn cuối]
           │
           ├── Token sai / Không tồn tại ──────► Báo lỗi "Liên kết không hợp lệ"
           ├── Token đã quá 15 phút (Hết hạn) ──► Báo lỗi "Liên kết đã hết hạn"
           └── Token hợp lệ & Còn hạn ─────────► 1. Mã hóa (Hash) mật khẩu mới
                                                 2. Cập nhật mật khẩu mới vào DB
                                                 3. Xóa Token ngầm (Set ResetToken = NULL)
                                                 4. Trả về: "Đổi mật khẩu thành công!"

```

---

## III. Chi tiết các Bước Xử lý Kỹ thuật

### Bước 1: Giai đoạn Yêu cầu đặt lại mật khẩu (Request Phase)

1. **Frontend (Next.js):**
* Khách hàng truy cập đường dẫn `/forgot-password`, nhập Email vào Form và bấm "Gửi yêu cầu".
* Frontend gửi một request `POST /api/v1/auth/forgot-password` kèm theo Body chứa `email`.


2. **Backend (ASP.NET Core):**
* Kiểm tra bản ghi trong bảng `Users` dựa theo `email` nhận được.
* **Bảo mật chống dò quét (Email Enumeration):** Dù tìm thấy hay không tìm thấy email, hệ thống luôn trả về Http Status Code `200 OK` cùng thông báo: *"Nếu email tồn tại trên hệ thống, một liên kết đặt lại mật khẩu đã được gửi đi. Vui lòng kiểm tra hộp thư"*.
* **Nếu Tìm thấy User:**
* Tạo mã ngẫu nhiên: `string token = Guid.NewGuid().ToString();`
* Thiết lập thời gian hết hạn: `DateTime expiry = DateTime.Now.AddMinutes(15);`
* Lưu `token` và `expiry` vào dòng dữ liệu của User đó trong SQL Server.
* Tạo đường dẫn liên kết chứa mã: `string resetLink = $"https://yourdomain.com/reset-password?token={token}";`
* Kích hoạt dịch vụ gửi mail gửi `resetLink` dưới định dạng HTML sang Gmail của khách hàng.





### Bước 2: Giai đoạn Xác thực và Đặt lại mật khẩu (Reset Phase)

1. **Frontend (Next.js):**
* Khách hàng mở hộp thư, bấm vào `resetLink`. Trình duyệt mở trang `/reset-password?token=...` trên giao diện Next.js.
* Next.js bóc tách giá trị `token` từ URL Query.
* Hiển thị Form gồm 2 trường nhập: **Mật khẩu mới** và **Xác nhận mật khẩu mới**.
* Khi khách nhập xong và bấm "Lưu thay đổi", Frontend gửi request `POST /api/v1/auth/reset-password` với Payload:
```json
{
  "token": "chuoi-token-lay-tu-url",
  "newPassword": "mat-khau-moi-cua-khach"
}

```




2. **Backend (ASP.NET Core):**
* Truy vấn SQL Server: `SELECT * FROM Users WHERE ResetToken = @token`.
* **Chốt chặn 1 (Kiểm tra tồn tại):** Nếu thực thể `User` trả về bị rỗng (`null`), lập tức hủy luồng và báo lỗi `400 Bad Request` (*"Mã xác thực không hợp lệ"*).
* **Chốt chặn 2 (Kiểm tra thời gian TTL):** Kiểm tra điều kiện `if (user.ResetTokenExpiry < DateTime.Now)`. Nếu đúng, lập tức hủy luồng và báo lỗi `400 Bad Request` (*"Liên kết đã hết hạn, vui lòng yêu cầu lại"*).
* **Xử lý cập nhật dữ liệu:**
* Tiến hành Mã hóa (Hash) chuỗi `newPassword` bằng thuật toán bảo mật (BCrypt/PBKDF2).
* Ghi đè mật khẩu đã mã hóa vào trường mật khẩu cũ của User.
* **Xóa dấu vết (Vô hiệu hóa Token):** Gán trường `ResetToken = null` và `ResetTokenExpiry = null`. Việc này đảm bảo Token chỉ sử dụng được **Duy nhất 1 lần**.
* Thực hiện `SaveChanges()` để cập nhật vào Database.
* Phản hồi trạng thái thành công `200 OK`.




3. **Frontend Kết thúc:**
* Next.js nhận tín hiệu thành công, hiển thị Alert thông báo đổi mật khẩu hoàn tất và tự động điều hướng khách hàng quay trở lại trang `/login` sau 3 giây.



---

## IV. Các Tham số Cấu hình Hệ thống (System Env / AppSettings)

| Tham số | Giá trị | Ý nghĩa |
| --- | --- | --- |
| `TOKEN_TTL_MINUTES` | `15` | Thời gian sống tối đa của mã Token dùng một lần |
| `TIMEZONE` | `Asia/Ho_Chi_Minh` | Múi giờ chuẩn để tính toán thời gian hết hạn của token |

```

```