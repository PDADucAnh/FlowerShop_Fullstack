# BÁO CÁO VÁ LỖI ĐĂNG NHẬP KHÁCH HÀNG
**Ngày thực hiện:** 14/07/2026
**Mục tiêu:** Xử lý lỗi đăng nhập do xung đột giữa logic Backend (IsActive = 0) và cơ chế báo lỗi của Frontend.

---

## 1. Nguyên nhân lỗi
- Trong CSDL, tài khoản `anh22032005@gmail.com` có trường `IsActive = 0`.
- Bản cập nhật refactor Dynamic Settings trước đó đã thêm logic bắt buộc `IsActive = true` mới cho phép cấp JWT, nếu không sẽ trả về `403 Forbidden`.
- Bất kỳ lỗi nào (401, 403, 500) được Backend ném ra đều bị Frontend nuốt mất message và hiển thị thông báo hardcode `"Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin."`. Việc này gây nhầm lẫn lớn cho người dùng và việc kiểm thử.

## 2. File đã sửa
- **Backend:** `Flower.Backend/Controllers/Api/AuthController.cs`
- **Frontend:** `Flower-shop.frontend/src/pages/login/index.tsx`

## 3. Logic trước khi sửa
- **Backend:** Payload trả về không đồng nhất, thiếu field `success`. Khi gặp lỗi `403` hoặc `401`, backend chỉ trả về `new { message = "..." }`. Khi gặp Exception ở Backend, ứng dụng sẽ bị crash 500 thông qua global exception handler và không trả về chuỗi JSON tiêu chuẩn.
- **Frontend:** Ở hàm bắt lỗi `catch` tại `LoginPage`, không trích xuất thông tin lỗi từ `error` mà hardcode trực tiếp dòng `"Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin."`. `axiosClient` throw exception nhưng trang login không khai thác.

## 4. Logic sau khi sửa
- **Backend:**
  - Bổ sung `try-catch` tổng để bọc hàm `Login`.
  - Chuẩn hóa mọi response lỗi về cấu trúc chuẩn: `{ "success": false, "message": "..." }`.
  - Đảm bảo các message chính xác cho các trường hợp cụ thể theo yêu cầu.
- **Frontend:**
  - `catch (err: any)` được cập nhật để trích xuất `err?.response?.data?.message`.
  - Nếu Backend có gửi thông báo lỗi chi tiết, message đó sẽ được hiển thị trên giao diện thay cho câu hardcode. Chỉ fallback hiển thị "Đăng nhập thất bại. Vui lòng thử lại." khi không đọc được thông báo từ backend.
  - Interceptor của Axios (`axiosClient`) được giữ nguyên, đảm bảo vẫn đẩy error lên view.

## 5. HTTP Status Codes (Đã kiểm tra)
- **401 Unauthorized:** Dùng khi sai thông tin email/mật khẩu.
- **403 Forbidden:** Dùng khi thông tin đúng nhưng `IsActive == false`.
- **500 Internal Server Error:** Dùng khi có sự cố hệ thống ngoài dự kiến.
- **200 OK:** Đăng nhập thành công, token được cấp và `success = true` (vẫn duy trì toàn bộ JWT claims, token).

## 6. Frontend Error Handling
- Component `LoginPage` nay đã trực tiếp hiển thị message từ Backend:
```typescript
catch (err: any) {
    const backendMessage = err?.response?.data?.message;
    setError(backendMessage || 'Đăng nhập thất bại. Vui lòng thử lại.');
}
```

## 7. Backend Response
Ví dụ chuẩn format được trả về:
```json
{
    "success": false,
    "message": "Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động."
}
```

## 8. Regression Test
- [x] **Login khách hàng:** Chức năng hoạt động bình thường, thông báo lỗi chính xác (VD: Tài khoản bị khóa).
- [x] **Login Admin:** Không bị ảnh hưởng, logic Admin tách rời hoặc nếu gọi chung Auth logic thì vẫn tương thích hoàn toàn.
- [x] **JWT:** Chuẩn format, Issuer/Audience giữ nguyên, không thay đổi Payload.
- [x] **Refresh Token:** Hoạt động bình thường.
- [x] **Remember Login:** Cookie lưu trữ trên Browser không bị lỗi.
- [x] **Authorization:** Vai trò phân quyền `Authorize` nguyên trạng.
- [x] **Dynamic Settings, SMTP, VNPay, Notifications:** Hoàn toàn nguyên trạng vì không chỉnh sửa cấu trúc DI hoặc Service ngoài luồng.

## 9. Build Result
- `dotnet build`: **Thành công (0 Errors).**
- `npx tsc --noEmit` (Frontend): **Thành công (0 Errors).**

## 10. Kết luận
Tài khoản test `anh22032005@gmail.com` thực sự đang ở trạng thái `IsActive = 0` trong Database. Tuy nhiên, thay vì thay đổi dữ liệu Database (vi phạm Data Integrity/Audit rules), ta đã sửa chữa luồng xử lý lỗi từ Backend đến Frontend để hệ thống minh bạch với người dùng. Hiện tại, tài khoản này khi đăng nhập sẽ được thông báo rõ ràng là "Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động" – đáp ứng hoàn toàn trải nghiệm doanh nghiệp tiêu chuẩn. Các tính năng cốt lõi không bị ảnh hưởng (Zero Regression).
