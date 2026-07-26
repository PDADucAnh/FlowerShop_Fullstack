# Auth Loop Fix & Cart Selection Design

## Issue 1: Vòng lặp đăng nhập khi thanh toán

### Root cause

JWT token được tạo với key từ `appsettings.json` (`Jwt:SecretKey`) nhưng được validate với `JWT_SECRET_KEY` env var (nếu có). Trên môi trường có env var set (Render: `generateValue: true`), hai key khác nhau → 401 ngay lập tức.

Flow lỗi:
1. `POST /api/Auth/login` tạo JWT với key từ `_configuration["Jwt:SecretKey"]` (appsettings.json)
2. Request tiếp theo (`GET /api/Auth/profile`) validate JWT với key từ `Environment.GetEnvironmentVariable("JWT_SECRET_KEY")` (env var)
3. Key mismatch → 401
4. `axiosClient` interceptor nhận 401 → xoá token → emit `'unauthorized'`
5. `AuthRedirectHandler` navigate `/login`
6. User login lại → quay lại bước 1 → vòng lặp

Tham khảo: `Flower.Backend/Controllers/Api/AuthController.cs:56-57` vs `Flower.Backend/Program.cs:45-48`

### Fix 1.1 — JWT Secret Key resolution (AuthController)

Sửa `AuthController.Login` và `AuthController.UpdateProfile` để đọc key với cùng thứ tự ưu tiên như `Program.cs`:

```
env JWT_SECRET_KEY → config Jwt:SecretKey → throw
```

**Các file ảnh hưởng:**
- `Flower.Backend/Controllers/Api/AuthController.cs` (2 occurrences: Login dòng ~56, UpdateProfile dòng ~145)

### Fix 1.2 — SessionValidationMiddleware skip JWT API

Middleware được thiết kế cho Cookie auth (admin). Với JWT API request, nó kiểm tra không cần thiết (`IsActive`, `LoginTime`, `X-Refresh-Token`). Thêm guard:

```csharp
if (context.Request.Path.StartsWithSegments("/api"))
{
    // Skip SessionValidationMiddleware for JWT API calls from frontend
    await _next(context);
    return;
}
```

**File ảnh hưởng:**
- `Flower.Backend/Middleware/SessionValidationMiddleware.cs` (đầu `InvokeAsync`)

### Fix 1.3 — Frontend preserve navigation state

**`Flower-shop.frontend/src/api/axiosClient.ts`**: Trước emit 'unauthorized', lưu `window.location.pathname` + search vào `sessionStorage` key `redirectAfterLogin`.

**`Flower-shop.frontend/src/pages/login/index.tsx`**: Sau login thành công, kiểm tra `redirectAfterLogin` trong sessionStorage, nếu có thì navigate về đó thay vì `/`.

---

## Issue 2: Cart checkbox selection

### Yêu cầu

- Checkbox mỗi sản phẩm trong giỏ hàng
- "Select All" checkbox ở header
- Nút "Xoá đã chọn" → xoá item được check
- Nút "Thanh toán" → chỉ thanh toán item được check
- Nếu không chọn item nào → toast "Vui lòng chọn sản phẩm"

### Thiết kế

**`Flower-shop.frontend/src/pages/cart/CartTable.tsx`**:
- Thêm state `selectedIds: Set<number>`
- Checkbox cột đầu tiên trong grid header và mỗi dòng
- `handleSelectAll` / `handleSelectOne`
- Props mới: `selectedIds`, `onSelectionChange`, `onDeleteSelected`
- Thêm nút "Xoá đã chọn" (chỉ hiện khi có item được chọn)

**`Flower-shop.frontend/src/pages/cart/index.tsx`**:
- State `selectedIds` ở page level, pass xuống CartTable
- `handleCheckout`: chỉ navigate với selected items (gửi qua navigate state)
- Nếu chưa chọn item nào → toast warning
- Nút "TIẾN HÀNH THANH TOÁN" chỉ active khi có item được chọn

**`Flower-shop.frontend/src/pages/checkout/index.tsx`**:
- Nhận `selectedItems` từ navigate state (nếu có)
- Nếu không có (backward compatible), dùng `cartItems` như cũ

### Không thay đổi

- CartContext: không cần thêm selection state — nó chỉ là local UI state
- Backend: không thay đổi — order API vẫn nhận items array như cũ
