# Báo cáo Audit & Sửa Lỗi Frontend: Success Page & Localization

## 1. Vấn đề 1: Lỗi Render tĩnh trang Order Success (BUG 1)

### 1.1 Root Cause
- Frontend component `src/pages/order-confirmation/index.tsx` sử dụng **State Client-Side tạm thời** qua Query String (`?payment=success`) để quyết định giao diện.
- Không fetch thông tin Order thực tế từ Backend dựa vào `orderId`.
- Hậu quả: Dù là đơn VNPay đang chờ thanh toán hay đơn COD (vốn dĩ trạng thái Payment là "Chưa thanh toán" và Order là "Chờ xác nhận"), trang vẫn luôn hardcode dòng chữ: "Giao dịch hoàn tất", "Đơn hàng của bạn đã được ghi nhận và đang được chuẩn bị giao." gây hiểu lầm nghiêm trọng cho khách hàng.

### 1.2 Before & After

**Before:**
```tsx
const isSuccess = searchParams.get('payment') === 'success';

// Render Hardcoded
<h2 className="font-display-lg...">Giao dịch hoàn tất</h2>
<p>Đơn hàng của bạn đã được ghi nhận và đang được chuẩn bị giao.</p>
```

**After:**
- Tích hợp hook `useOrderDetail(orderId)` để fetch thông tin Order Realtime.
- Dùng `useMemo` phân tích logic kinh doanh:
    - Nếu VNPay & Đã thanh toán -> Giao dịch thành công.
    - Nếu VNPay & Chưa thanh toán -> Đơn hàng đã được tạo. Đang chờ thanh toán.
    - Nếu VNPay & Lỗi -> Thanh toán thất bại.
    - Nếu COD -> Đặt hàng thành công. Đơn hàng đang chờ cửa hàng xác nhận.

## 2. Vấn đề 2: Hardcode trạng thái tiếng Anh (BUG 2)

### 2.1 Root Cause
- Các component như `MyOrders.tsx`, `OrderDetail.tsx`, `OrderComponents.tsx` đang sử dụng text tiếng Anh trực tiếp trong code như `Pending`, `Completed` hoặc gán vào Object `statusConfig` không thống nhất.
- Khi thêm mới trạng thái (VD: `PendingPayment`), UI phải update lẻ tẻ ở nhiều nơi.

### 2.2 Before & After

**Before:**
- Khai báo gộp: `statusConfig: { Pending: { label: 'Chờ xử lý', dot: '...' }}` 
- Check IF/ELSE chay: `order.paymentStatus === 1 ? 'Đã thanh toán' : 'Thất bại'...` 

**After:**
- Tách riêng UI (colors, dots) và Text rendering.
- Bổệ sung `src/utils/statusMappers.ts`:
    - `getOrderStatusText()`
    - `getPaymentStatusText()`
    - `getPaymentMethodText()`
- Tại `MyOrders.tsx` & `OrderDetail.tsx`, import mappers để hiển thị nhất quán trên mọi component.

## 3. Regression Test
- ✅ Không chạm vào Code Backend C# (.NET Core). Các fix Backend Payment/COD trước đó vẫn an toàn 100%.
- ✅ Compile Frontend thành công (Zero Warning/Errors trong bước `npm run build`).
- ✅ Page Success hiển thị Dynamic content, không phá vỡ UI UX hiện tại (hiệu ứng Check/Hourglass/Error hoạt động trơn tru dựa trên condition).
- ✅ Centralized Localization áp dụng gọn nhẹ, dễ bảo trì cho các Frontend Devs về sau.
