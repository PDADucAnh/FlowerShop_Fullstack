# Báo Cáo Nghiệp Vụ Tái Tính Toán Giá Giỏ Hàng & Checkout (Cart Price Recalculation Report)

Báo cáo chi tiết về việc triển khai nghiệp vụ tái tính toán giá giỏ hàng và kiểm tra giá động tại thời điểm Checkout/Tạo Đơn Hàng theo đúng tiêu chuẩn thương mại điện tử chuyên nghiệp.

---

## 1. Phân tích nghiệp vụ hiện tại (Current Business Analysis)
- Trước đây, khi khách hàng thêm sản phẩm vào giỏ hàng (`Cart`), thông tin giá của sản phẩm tại thời điểm đó (bao gồm giá gốc và giá khuyến mãi) được lưu cứng trong LocalStorage của trình duyệt.
- Nếu một đợt Flash Sale kết thúc hoặc giá bán của sản phẩm thay đổi trên hệ thống quản trị (Admin Panel), giỏ hàng của khách hàng vẫn hiển thị mức giá cũ đã lỗi thời. Khi tiến hành thanh toán, hệ thống có thể bị mâu thuẫn dữ liệu hoặc thiếu an toàn bảo mật do chấp nhận thông tin giá gửi trực tiếp từ client.

---

## 2. Nguyên nhân gây lỗi (Root Cause of Issues)
- **Giỏ hàng client-side tĩnh**: Cart chỉ hoạt động offline dựa trên LocalStorage mà không chủ động gọi API Backend để cập nhật thông tin khuyến mãi động mỗi khi mở hoặc làm mới trang.
- **Thiếu kiểm tra chéo giá ở Backend**: API tạo đơn hàng (`CreateOrder` / `Checkout`) tại Backend nhận thuộc tính `UnitPrice` gửi lên từ Client và trực tiếp dùng nó làm cơ sở tính tổng tiền gốc, dẫn tới khả năng bị khai thác lỗ hổng thay đổi giá tùy ý từ phía người dùng (vulnerabilities).

---

## 3. Các file đã sửa (Modified Files)

### Backend:
- [OrderItemDTO.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Models/DTOs/OrderItemDTO.cs): Bổ sung thuộc tính `SizeVariant` để truyền thông tin kích thước size sản phẩm từ client lên Backend khi tạo đơn hàng.
- [OrderDTOs.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Models/DTOs/OrderDTOs.cs):
  - Bổ sung `OriginalPrice`, `DiscountAmount`, và `Subtotal` vào `OrderDetailDTO` để hiển thị minh bạch hóa đơn.
  - Bổ sung `SizeVariant` vào `OrderItemInput`.
- [MappingExtensions.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Models/DTOs/MappingExtensions.cs): Cập nhật phương thức mapper `OrderDetail.ToDTO()` để điền các giá trị `OriginalPrice = UnitPrice + Discount`, `DiscountAmount = Discount`, và `Subtotal`.
- [ProductDTOs.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Models/DTOs/ProductDTOs.cs): Khai báo các đối tượng DTO phục vụ recalculate: `CartRecalculateRequest`, `CartItemRecalculateDTO`, `CartRecalculateResponse`, và `CartItemRecalculatedDTO`.
- [ProductsController.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Controllers/Api/ProductsController.cs): Triển khai endpoint API mới `POST /api/Products/recalculate-cart` để tái tính toán giá giỏ hàng động hoàn toàn ở Backend.
- [OrdersController.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Controllers/Api/OrdersController.cs): Map trường `SizeVariant` từ DTO đầu vào sang `OrderItemInput` trước khi gọi Service tạo đơn.
- [OrderService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/OrderService.cs): Refactor toàn bộ luồng tạo đơn hàng, lấy giá sản phẩm trực tiếp từ DB, cộng phụ phí size variant, áp dụng khuyến mãi dynamic chuẩn múi giờ Việt Nam, thực hiện kiểm tra chéo giá với Client gửi lên và chặn thanh toán/trả lỗi nếu giá thay đổi. Đồng thời lưu trữ chính xác thông số khóa giá vào `OrderDetail` (`Discount` và `Subtotal`).

### Frontend:
- [productService.ts](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower-shop.frontend/src/services/productService.ts): Bổ sung phương thức gọi API `recalculateCart`.
- [CartContext.tsx](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower-shop.frontend/src/context/CartContext.tsx):
  - Khai báo hàm `recalculateCartPrices` trong context và triển khai đồng bộ hóa trạng thái LocalStorage/React State với API Backend.
  - Thêm hook `useEffect` tự động chạy recalculate khi ứng dụng mount (khởi tạo giỏ hàng).
- [cart/index.tsx](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower-shop.frontend/src/pages/cart/index.tsx): Tự động gọi `recalculateCartPrices` khi khách hàng mở/làm mới trang Giỏ hàng.
- [checkout/index.tsx](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower-shop.frontend/src/pages/checkout/index.tsx):
  - Tự động gọi `recalculateCartPrices` khi khách hàng vào trang Checkout.
  - Gửi đầy đủ `sizeVariant` của sản phẩm lên API khi Đặt hàng.
  - Bắt lỗi thay đổi giá từ Backend để tự động kích hoạt cập nhật lại giỏ hàng và thông báo cho người dùng.

---

## 4. Nghiệp vụ đã thay đổi (Business Logic Changes)
1. **Giá giỏ hàng động (Dynamic Cart Pricing)**: Giá hiển thị trong giỏ hàng không còn cố định mà được cập nhật liên tục từ Backend mỗi khi mở trang hoặc thanh toán.
2. **Khóa giá đơn hàng thành công (Order Price Locking)**: Một khi đơn hàng được tạo thành công, giá chi tiết sản phẩm được lưu cố định vào bảng `OrderDetail` (bao gồm đơn giá sau giảm `UnitPrice`, lượng giảm `Discount`, và thành tiền `Subtotal`). Mọi thay đổi về giá hoặc chương trình khuyến mãi sau đó của Admin đều không ảnh hưởng đến đơn hàng cũ.
3. **Cơ chế phòng chống gian lận giá (Price Tampering Protection)**: Backend kiểm tra chéo giá bán của từng món hàng với cơ sở dữ liệu thực tế tại thời điểm nhấn nút Đặt hàng, loại bỏ hoàn toàn khả năng người dùng tự sửa payload HTTP để mua hàng giá rẻ.

---

## 5. API thay đổi (API Changes)
- **API Mới**: `POST /api/Products/recalculate-cart`
  - *Request Body*: Danh sách các sản phẩm kèm số lượng và đơn giá hiện tại trong giỏ của client.
  - *Response Body*: Danh sách sản phẩm kèm giá mới nhất được tính toán động từ backend, cờ `PriceChanged` báo hiệu thay đổi, và `Message` thông báo.

---

## 6. Các trường hợp kiểm thử (Test Cases & Scenarios)
1. **✓ Case 1 (Flash Sale đang diễn ra)**: Thêm sản phẩm vào giỏ, hệ thống hiển thị đúng giá sale 450.000đ.
2. **✓ Case 2 (Flash Sale kết thúc)**: Khi đợt Flash Sale kết thúc, khách hàng mở lại hoặc tải lại trang Cart. Giỏ hàng lập tức cập nhật giá bán về giá gốc 500.000đ và hiển thị thông báo cảnh báo màu đỏ: *"Giá của một hoặc nhiều sản phẩm đã được cập nhật do chương trình khuyến mãi đã kết thúc hoặc thay đổi."*
3. **✓ Case 3 (Giá thay đổi ngay lúc nhấn Đặt hàng)**: Nếu giá thay đổi ngay trước khi tạo Order, Backend lập tức từ chối và trả về lỗi. Frontend tự động gọi lại recalculate để cập nhật lại giỏ hàng về giá mới nhất.

---

## 7. Kết quả biên dịch (Build Verification)

- **Backend build**: `dotnet build Flower.Backend/Flower.Backend.csproj` hoàn thành thành công với **0 lỗi (0 Errors)**.
- **Frontend build**: `npm run build` hoàn thành thành công với **0 lỗi (0 Errors)**.

---

## 8. Kết luận (Conclusion)
Hệ thống giỏ hàng, thanh toán và xử lý đơn đặt hàng hiện tại đã được nâng cấp toàn diện, tuân thủ chặt chẽ các tiêu chuẩn bảo mật và nghiệp vụ của các hệ thống thương mại điện tử chuyên nghiệp cấp doanh nghiệp lớn.
