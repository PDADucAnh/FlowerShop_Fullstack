# Báo Cáo Tối Ưu Hiệu Năng Tái Tính Toán Giỏ Hàng (Cart Recalculate Performance Fix Report)

Báo cáo chi tiết về nguyên nhân, giải pháp khắc phục triệt để hiện tượng gọi API lặp, giật lag giao diện và tối ưu hóa quản lý state trong nghiệp vụ giỏ hàng & thanh toán FlowerShop.

---

## 1. Nguyên nhân gây chớp giao diện (UI Jitter)
- Do hàm `recalculateCartPrices` có dependency là `cartItems`. Mỗi lần giỏ hàng thay đổi (ví dụ: người dùng tăng/giảm số lượng hoặc xóa sản phẩm), hàm này lại thay đổi tham chiếu (reference).
- Các trang `cart/index.tsx` và `checkout/index.tsx` có `useEffect` lắng nghe theo `recalculateCartPrices`. Khi tham chiếu này thay đổi, `useEffect` bị kích hoạt lại, tạo ra vòng lặp render nhảy giá trị từ cũ sang mới liên tục, gây chớp nháy giật màn hình.

---

## 2. Nguyên nhân gây gọi API lặp (Infinite API Loops)
- Vòng lặp vô hạn xảy ra do dòng thác dependency:
  `cartItems thay đổi` -> `recalculateCartPrices thay đổi tham chiếu` -> `useEffect ở trang Cart/Checkout kích hoạt` -> `Gửi API recalculate-cart` -> `Nhận giá mới và gọi setCartItems` -> `cartItems thay đổi` (vòng lặp lặp lại vô hạn).

---

## 3. Nguyên nhân gây Backend log liên tục
- Do vòng lặp vô hạn ở Frontend liên tục gửi yêu cầu `POST /api/Products/recalculate-cart` lên máy chủ, làm tràn ngập log yêu cầu của Backend và chiếm dụng kết nối database không cần thiết.

---

## 4. Các useEffect đã sửa (Modified useEffect Hooks)
- **Trong `CartContext.tsx`**:
  - Loại bỏ hoàn toàn hook `useEffect` tự động chạy recalculate khi `cartItems` thay đổi.
  - Sử dụng một `cartItemsRef` để lưu giữ giá trị giỏ hàng mới nhất một cách đồng bộ mà không tạo ra các kích hoạt dependency lên `useCallback`.
- **Trong `cart/index.tsx` & `checkout/index.tsx`**:
  - Kích hoạt gọi `recalculateCartPrices` một lần duy nhất khi Component mount nhờ vào hàm `recalculateCartPrices` đã trở nên hoàn toàn ổn định (stable reference) với dependency array `[setCartItems]`.

---

## 5. Các state đã tối ưu (Optimized States)
- Trạng thái giỏ hàng chỉ thay đổi cục bộ và lập tức đồng bộ khi thực hiện tăng, giảm hoặc xóa sản phẩm (Instant Render).
- Không cập nhật đè state cũ khi có phản hồi chậm từ API nhờ loại bỏ hoàn toàn các cuộc gọi API nền (background API requests) khi chỉnh sửa số lượng hay xóa món hàng.

---

## 6. Các API đã giảm số lần gọi (API Call Reductions)
- **API `POST /api/Products/recalculate-cart`**:
  - Số lần gọi khi chỉnh sửa số lượng / xóa sản phẩm: **Giảm từ N lần về 0 lần**.
  - Số lần gọi khi mở Cart hoặc mở Checkout: **Giảm từ vô hạn lần về đúng 1 lần**.

---

## 7. Các thay đổi về debounce/throttle (nếu có)
- Không cần áp dụng debounce/throttle cho API khi thay đổi số lượng, do hệ thống đã được tối ưu không thực hiện cuộc gọi API nào trong lúc tăng/giảm số lượng hay xóa sản phẩm. Mọi phép tính toán đều diễn ra nhanh chóng tại local state và chỉ đồng bộ giá trị cuối qua API Recalculate duy nhất khi vào trang Thanh toán.

---

## 8. Kết quả kiểm thử (Test Cases Results)
- **Xóa nhiều sản phẩm liên tiếp**: Hoạt động mượt mà, không bị hiện tượng item xuất hiện lại (race condition) do không có API recalculate chạy ngầm ghi đè state.
- **Tăng/giảm số lượng**: Phản hồi tức thì, không chớp giật giao diện.
- **Refresh Cart**: Gọi API recalculate đúng 1 lần duy nhất để lấy giá khuyến mãi mới nhất từ Backend.
- **Checkout**: Nhận đúng giá trị sau cùng và kiểm tra chéo an toàn ở Backend.

---

## 9. Kết quả build Backend
- `dotnet build Flower.Backend/Flower.Backend.csproj` hoàn thành thành công: **0 lỗi (0 Errors)**.

---

## 10. Kết quả build Frontend
- `npm run build` hoàn thành thành công: **0 lỗi (0 Errors)**.
