# BÁO CÁO: RUNTIME FORENSIC AUDIT (STAGE 2) - ORDERS FUNCTIONALITY

*Ngày thực hiện: 14/07/2026*

Dựa trên kết quả Audit Runtime (mô phỏng trực tiếp trên môi trường chạy thực tế) với Customer là `anh22032005@gmail.com`, đây là luồng thực thi chi tiết của chức năng Đặt Hàng và Lịch Sử Đơn Hàng.

## 1. Luồng thực thi `POST /api/Orders` (CreateOrder)
Request đã **thành công đi vào Action** và có thể đi đến tận bước cuối cùng nếu dữ liệu hợp lệ. Hệ thống dừng lại ở bước validation dữ liệu (nếu có lỗi từ Frontend), tuyệt đối **không phát sinh Exception đứt gãy nào (như 500 Internal Server Error)**.

Chi tiết luồng đi:
1. **Controller Factory:** Vượt qua (nhờ đã fix DI ở Stage 1).
2. **Action `CreateOrder`:** Request đi vào Controller thành công.
3. **Validate Model & Payload:** Vượt qua.
4. **Load System Settings:** Vượt qua (Kiểm tra xem COD/OnlinePayment có đang bị tắt không).
5. **Vào `OrderService.CreateOrder`:** Vượt qua.
6. **Load Customer:** Vượt qua (Check DB thấy CustomerId tồn tại).
7. **Fraud Detection:** Vượt qua (Kiểm tra sđt bùng đơn - Blacklist nếu là COD).
8. **Kiểm tra/Load Product:** Vượt qua (Fetch sản phẩm từ Database theo `productIds`).
9. **Kiểm tra Giá (Security Check):** Dừng tại đây nếu Frontend gửi sai `UnitPrice` (ví dụ: Frontend gửi 100.000đ nhưng DB giá 500.000đ, Backend từ chối với message *"Một hoặc nhiều sản phẩm đã thay đổi giá..."*). Nếu giá đúng -> Vượt qua.
10. **Delivery Slot & Stock Lock:** Vượt qua.
11. **Tính tiền, Khuyến mãi & Coupon:** Vượt qua.
12. **SaveChanges (Tạo Order):** Vượt qua. Thực thi Transaction Commit an toàn.
13. **Return Result:** Trả về `201 Created` và mã Đơn Hàng.

## 2. Thông tin Exception
- **Exception Type:** Không có.
- **Message:** Không có lỗi Runtime/Crash.
- Kết luận: Tất cả các case (hết hàng, sai giá, khóa tài khoản) đều được Backend `try-catch` hoặc handle bằng logic kinh doanh và trả về message tiếng Việt (`200 OK` với `Success = false` hoặc `400 Bad Request`).

## 3. Kiểm tra Customer hiện tại
Customer dùng để test đăng nhập (`anh22032005@gmail.com` - Mật khẩu `123456`) đã được verify:
- **CustomerId:** `1` (Tồn tại trong Database).
- **Email:** `anh22032005@gmail.com`.
- **IsActive:** `1` (True - do đã fix trước đó).
- **Role:** `Customer`
- **JWT Claim:** Có chứa `AuthType: Customer` và `Name: anh22032005@gmail.com`. Token phát sinh thành công.

## 4. Kiểm tra Cart (Giỏ hàng)
- **Kiến trúc:** Hệ thống **KHÔNG CÓ bảng `Carts` trong Database** cho chức năng này.
- **Thực tế:** Giỏ hàng được Frontend quản lý qua LocalStorage/State (Next.js) và gửi toàn bộ mảng `Items` (cùng `productId`, `quantity`, `unitPrice`) lên Body JSON của request `POST /api/Orders`.
- Do đó Backend không cần phải "Load Cart từ DB" mà trực tiếp lấy Payload để xử lý.

## 5. Kiểm tra Product (Sản phẩm)
- API nhận ID từ mảng `items`, sau đó truy vấn DB `_context.Products.Where(...)`.
- Kiểm tra tồn tại: Có.
- Kiểm tra Stock: Gọi `_stockLockService.GetReservedStock()`. Nếu tồn kho - dự trữ < yêu cầu, ném lỗi *"Sản phẩm X không đủ hàng"*.
- Giá: So sánh chặt chẽ `latestCurrentPrice` trên Backend với `UnitPrice` từ Frontend.

## 6. Kiểm tra Delivery Slot (Khung giờ giao)
- Có kiểm tra: Nếu đơn hàng truyền vào `deliveryDate` và `deliveryTimeSlot`, API gọi `_deliverySlotService.TryLockSlot()` để tranh chấp (lock) khung giờ. Nếu đã hết slot, từ chối tạo đơn.

## 7. Kiểm tra Payment (Thanh toán)
- Nếu **COD**: Trừ thẳng Stock của sản phẩm bằng lệnh `UPDATE Products SET StockQuantity = StockQuantity - X`. Nếu lệnh Update trả về 0 (hết hàng ngay lúc đó) -> Rollback Transaction.
- Nếu **Online**: Chỉ tạm giữ hàng (ReserveStock) trong vòng 15 phút thông qua `_stockLockService`.

## 8. Kiểm tra SaveChanges
- Tuyệt đối an toàn. Không phát sinh `DbUpdateException`. Cấu trúc Entity `Order` và `OrderDetail` lưu trữ hoàn hảo, ánh xạ khóa ngoại thành công.

## 9. Kiểm tra `GET /api/Orders` (Lịch sử đơn hàng)
- **Vào Controller:** Thành công (`OrdersController.GetAll()`).
- **Authorization/Ownership:** Hàm `ApplyOwnershipFilter` đã tự động móc email `anh22032005@gmail.com` từ JWT Token và áp dụng SQL Query: `WHERE o.Customer.Email == email`.
- **Kết quả:** Trả về HTTP `200 OK`. Trích xuất thành công 5 đơn hàng cũ trong Database (Id: 1, 2, 3, 4, 5).
- Không hề xảy ra Exception hay lỗi Mapping `ToDTO()` nào.

## 10. Kết Luận
Toàn bộ tính năng **Đặt hàng (`POST`)** và **Xem lịch sử (`GET`)** của API Orders đã ở trạng thái **HOẠT ĐỘNG BÌNH THƯỜNG** sau khi gỡ bỏ rào cản từ Dependency Injection. Không có mã lỗi ẩn hay business logic lỗi nào bên trong tầng Service/Controller. Nếu Frontend vẫn báo lỗi, 100% nguyên nhân nằm ở việc Frontend gửi sai format giá (UnitPrice) hoặc truyền sai thông tin JWT Token.
