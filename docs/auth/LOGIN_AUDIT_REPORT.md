# BÁO CÁO KIỂM TOÁN LỖI ĐĂNG NHẬP KHÁCH HÀNG
**Ngày thực hiện:** 14/07/2026
**Mục tiêu:** Xác định nguyên nhân tài khoản khách hàng `anh22032005@gmail.com` không thể đăng nhập sau khi triển khai Dynamic Settings.

---

## 1. URL Login của Frontend (Endpoint)
`POST /api/Auth/login`
Gọi qua `axiosClient.post('/api/Auth/login', ...)`

## 2. Body Frontend gửi lên (Payload)
```json
{
  "username": "anh22032005@gmail.com",
  "password": "123456"
}
```

## 3. Headers
- `Content-Type: application/json`

## 4. File Xử Lý Frontend
- **Tên file:** `Flower-shop.frontend/src/pages/login/index.tsx`
- **Dòng lỗi hardcode:** Dòng số 26 có đoạn code sau:
  ```typescript
  } catch {
      setError('Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
  }
  ```
- **File liên quan:** `Flower-shop.frontend/src/services/authService.ts` (Hàm `login`)

## 5. Endpoint Backend nhận (Route)
`[HttpPost("login")]` tại route `api/Auth/login`

## 6. File Controller Backend
- **Tên file:** `Flower.Backend/Controllers/Api/AuthController.cs`
- **Hàm xử lý:** `public async Task<IActionResult> Login([FromBody] LoginRequest login)`

## 7. File Service Backend
- **Tên file:** `Flower.Backend/Services/AuthService.cs`
- **Hàm xử lý:** `public async Task<LoginResult?> Login(string identifier, string password)`

## 8. Truy vấn Database
- **Table:** `Customers`
- **Cột liên quan:** `Email`, `Password` (nơi lưu PasswordHash), `IsActive`

## 9. Trạng thái Database hiện tại của user
- **Email:** `anh22032005@gmail.com`
- **IsActive:** `0` (False)
- *(Mật khẩu đã được xác minh bằng PasswordHasher là chính xác cho chuỗi "123456")*

## 10. Commit refactor gây lỗi
- **Tên file thay đổi:** `Flower.Backend/Controllers/Api/AuthController.cs`
- **Code được thêm vào:**
  ```csharp
  if (!result.IsActive)
  {
      return StatusCode(403, new { message = "Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động!" });
  }
  ```

## 11. Logic gây lỗi ở Backend
Trước khi refactor, hàm `Login` không kiểm tra thuộc tính `IsActive` của `LoginResult`, do đó tài khoản dù có `IsActive = 0` trong DB vẫn đăng nhập thành công. Trong quá trình refactor gần đây (Dynamic Settings + EmailService), developer đã bổ sung điều kiện từ chối truy cập nếu `IsActive` là `false`. Do đó, người dùng này bị chặn.

## 12. Status Code Backend trả về
`403 Forbidden`

## 13. Message Backend trả về
`{"message": "Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động!"}`

## 14. Cơ chế Frontend bắt lỗi (Axios Interceptor)
- `axiosClient` trong `Flower-shop.frontend/src/api/axiosClient.ts` bắt lỗi HTTP 403 ở interceptor và trả về bằng `Promise.reject(error);`
- Ở block `try/catch` của component `LoginPage`, Frontend chỉ thực hiện `catch` chung chung, hoàn toàn bỏ qua thông báo lỗi chi tiết từ backend (`error.response.data.message`), và thay bằng lỗi hardcode `"Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin."`. Điều này khiến người dùng lầm tưởng rằng họ đã nhập sai email hoặc mật khẩu.

## 15. Kết luận nguyên nhân gốc rễ
Nguyên nhân gốc rễ là sự cộng hưởng của **3 yếu tố**:
1. **Dữ liệu Database:** Tài khoản khách hàng `anh22032005@gmail.com` đang có trạng thái `IsActive = 0`.
2. **Thay đổi Logic Backend:** Code refactor đã thêm rule mới chặt chẽ hơn (kiểm tra `IsActive`) khiến tài khoản này bị backend từ chối với status 403. 
3. **Lỗi UX Frontend:** Cơ chế bắt lỗi ở Frontend không tái sử dụng message của Backend mà hardcode dòng chữ báo lỗi sai thông tin. Việc "nuốt" mất lỗi thực sự khiến việc xác định nguyên nhân từ góc nhìn người dùng (và tester) bị sai lệch hoàn toàn.
