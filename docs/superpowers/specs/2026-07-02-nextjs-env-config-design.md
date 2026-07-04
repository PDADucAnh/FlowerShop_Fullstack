# Tài liệu Đặc tả Thiết kế: Cấu hình Biến Môi trường Tập trung cho Frontend

Tài liệu này đặc tả cơ chế tích hợp quản lý cấu hình biến môi trường tập trung cho dự án Frontend (sử dụng Create React App / `react-scripts`), ánh xạ theo tiêu chuẩn từ tài liệu workflow `nextjs-env-config-workflow.md`.

---

## 1. Phân tích sự tương thích với dự án

Dự án hiện tại là một SPA chạy React 19 (`react-scripts`) thay vì Next.js. Do đó:
- Tiền tố biến môi trường phía trình duyệt là **`REACT_APP_`** thay vì `NEXT_PUBLIC_`.
- Các cơ chế quản lý môi trường bằng tệp `.env`, phân chia tệp theo môi trường (`.env.development`, `.env.production`), quản lý tập trung qua `src/config/index.ts` và gán giá trị fallback sẽ được giữ nguyên 100% đúng chuẩn doanh nghiệp.

---

## 2. Các thành phần thiết kế

### 2.1. Thiết lập các tệp môi trường ở root Frontend (`cms.frontend/`)
1. Tệp **`.env.development`** (Môi trường Dev/Local):
   ```env
   REACT_APP_API_URL=https://localhost:7224
   REACT_APP_IMAGE_BASE_URL=https://localhost:7224
   ```
2. Tệp **`.env.production`** (Môi trường Production/Deploy thực tế):
   ```env
   REACT_APP_API_URL=https://api.tiemhoacuanam.com/api/v1
   REACT_APP_IMAGE_BASE_URL=https://api.tiemhoacuanam.com
   ```
3. Cập nhật `.gitignore` để tránh đẩy các tệp cấu hình này lên Git:
   ```
   .env
   .env.development
   .env.production
   ```

### 2.2. Khởi tạo cấu hình tập trung (`cms.frontend/src/config/index.ts`)
Tạo một tệp cấu hình tập trung để quản lý các hằng số môi trường an toàn kèm giá trị mặc định phòng ngừa thiếu tệp `.env`:
```typescript
const CONFIG = {
  API_BASE_URL: process.env.REACT_APP_API_URL || 'https://localhost:7224',
  IMAGE_BASE_URL: process.env.REACT_APP_IMAGE_BASE_URL || 'https://localhost:7224',
  TIMEOUT: 10000,
};

export default CONFIG;
```

### 2.3. Ánh xạ các Utils hiện tại sang dùng Config mới (`cms.frontend/src/utils/apiUtils.ts`)
Cấu hình lại [apiUtils.ts](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/utils/apiUtils.ts) để kế thừa trực tiếp từ đối tượng `CONFIG`:
```typescript
import CONFIG from '../config';

export const API_BASE_URL: string = CONFIG.API_BASE_URL;

export const getImageUrl = (path?: string): string => {
  if (!path) return 'https://via.placeholder.com/400x400?text=No+Image';
  if (path.startsWith('http')) return path;
  return `${CONFIG.IMAGE_BASE_URL}${path.startsWith('/') ? '' : '/'}${path}`;
};
```
*Lưu ý:* Việc kế thừa này giúp toàn bộ các component và hook đang gọi `API_BASE_URL` hoặc `getImageUrl` từ `apiUtils.ts` hoạt động bình thường mà không cần sửa đổi hàng loạt file khác, giảm thiểu rủi ro lỗi cú pháp.
