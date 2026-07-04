Markdown
# Workflow: Chức năng Hủy Đơn Hàng (Order Cancellation)

Tài liệu này đặc tả luồng xử lý nghiệp vụ và dữ liệu khi khách hàng thực hiện yêu cầu "Hủy đơn" đối với các đơn hàng đang trong trạng thái chờ xử lý tại FlowerShop.

---

## I. Điều kiện Nghiệp vụ (Business Rules)

Đối với cửa hàng hoa tươi, việc hủy đơn cần được Backend kiểm soát nghiêm ngặt dựa trên thời gian và trạng thái để tránh thiệt hại chi phí nguyên liệu (hoa tươi đã cắt cắm):
1. **Trạng thái hợp lệ:** Khách hàng chỉ có quyền tự bấm nút "Hủy đơn" từ giao diện khi đơn hàng ở trạng thái **"Chờ xác minh" (Pending/Awaiting Verification)** như đơn hàng `#16`. Nếu đơn đã chuyển sang *Đang chuẩn bị (Processing)* hoặc *Đang giao (Shipping)*, nút Hủy sẽ bị ẩn.
2. **Trạng thái Thanh toán:** Đơn hàng `#16` hiện có trạng thái *Chưa thanh toán* với phương thức *Chuyển khoản / Online*, do đó Backend không cần xử lý hoàn tiền (Refund Log), chỉ cần hoàn trả số lượng hoa vào kho (`StockQuantity`).

---

## II. Sơ đồ Luồng Hoạt động (Workflow Diagram)

[Khách bấm nút "Hủy đơn" tại chi tiết đơn #16]
│
▼ (Hiển thị Pop-up xác nhận)
[Khách chọn Lý do hủy + Bấm "Xác nhận hủy"]
│
▼ (Next.js gửi Request: PUT /api/v1/orders/16/cancel)
[Backend ASP.NET Core tiếp nhận và mở Transaction]
│
├── Chốt chặn 1: Xác thực JWT Token (Đúng chủ đơn Phạm đức anh?)
├── Chốt chặn 2: Kiểm tra trạng thái đơn trong DB còn là "Chờ xác minh"?
│
▼ (Vượt qua các chốt chặn an toàn)
[Thực hiện cập nhật dữ liệu ngầm trong SQL Server]
│
├── 1. UPDATE Orders SET Status = 'Cancelled', CancelledAt = GETDATE()
├── 2. Đọc bảng OrderDetails -> Lấy ProductId và Quantity (B0148 - PILLOW, SL: 2)
└── 3. UPDATE Products SET StockQuantity = StockQuantity + 2 WHERE Id = ProductId
│
▼ (Giao dịch hoàn tất - Commit)
[Backend tự động gửi Email thông báo hủy đơn thành công đến anh22032005@gmail.com]
│
▼ (Trả phản hồi 200 OK về Next.js)
[Frontend cập nhật lại giao diện đơn #16 thành "Đã hủy" ──► Vô hiệu hóa nút Hủy]
Tóm tắt Tiêu chuẩn Vận hành An toàn (Best Practices)Thành phầnKỹ thuật áp dụngMục đíchFrontendKhóa điều kiện giao diệnTránh việc người dùng gửi yêu cầu hủy đơn nhiều lần liên tiếp khi mạng chậm (Double-submit).BackendDatabase TransactionĐảm bảo tính nhất quán dữ liệu: Đơn hủy thì kho phải tăng, không bao giờ xảy ra lỗi đơn hủy nhưng kho không đổi.BackendKhóa logic quyền sở hữuKiểm tra UserId từ Token để ngăn chặn tình trạng User A gọi API để xóa đơn hàng của User B.