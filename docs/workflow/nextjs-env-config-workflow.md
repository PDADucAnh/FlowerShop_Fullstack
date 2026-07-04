```markdown
# Workflow: Cấu hình Biến Môi trường (.env) trong Next.js chuẩn Doanh nghiệp

Tài liệu này đặc tả luồng thiết lập và sử dụng các hằng số môi trường (API Base URL, Image Base URL) trong Next.js nhằm loại bỏ hoàn toàn việc viết cứng (Hardcode) địa chỉ domain Backend, giúp hệ thống dễ dàng chuyển đổi giữa các môi trường Dev, Staging và Production.

---

## I. Phân tích Cơ chế Biến Môi trường trong Next.js

Trong Next.js, nếu một biến môi trường muốn được truy cập từ phía Trình duyệt (Client-side / React Component) thì bắt buộc phải được bắt đầu bằng tiền tố **`NEXT_PUBLIC_`**. 

*   Nếu viết dạng `REACT_APP_API_URL` (chuẩn của Create React App cũ), Next.js sẽ **không** nhận diện được ở phía Client và trả về giá trị `undefined`.
*   Khi thực hiện lệnh biên dịch dự án (`next build`), Next.js sẽ tự động tìm các biến có chữ `NEXT_PUBLIC_` và đóng băng (Hard-code) chúng vào các tệp Javascript tĩnh để gửi xuống cho trình duyệt khách hàng.

---

## II. Sơ đồ Luồng Hoạt động (Workflow Diagram)


```

[Khởi động ứng dụng Next.js]
│
▼ (Next.js tự động quét các tệp .env ở thư mục gốc)
[Hệ thống nạp các biến NEXT_PUBLIC_ vào Process]
│
├── Môi trường Dev (npm run dev) ───► Đọc file `.env.development`
└── Môi trường Prod (npm start) ──► Đọc file `.env.production`
│
▼
[Tạo tệp cấu hình tập trung config.js để map dữ liệu]
│
▼
[Các Component/Service gọi hằng số từ config.js (Không gọi trực tiếp process.env)]
│
├─► Axios/Fetch: Sử dụng API_BASE_URL để gọi API ngầm về ASP.NET
└─► Thẻ : Sử dụng IMAGE_BASE_URL để nối chuỗi đường dẫn ảnh

```

---

## III. Chi tiết Triển khai Kỹ thuật

### Bước 1: Khởi tạo các tệp `.env` tại Thư mục gốc (Root Project)

Doanh nghiệp luôn chia nhỏ các tệp môi trường để quản lý. Hãy tạo 2 tệp tin sau ở thư mục ngoài cùng của dự án Next.js:

1. Tệp **`.env.development`** (Dùng khi code ở máy cá nhân - Localhost):
```env
NEXT_PUBLIC_API_URL=https://localhost:5001/api/v1
NEXT_PUBLIC_IMAGE_BASE_URL=https://localhost:5001

```

2. Tệp **`.env.production`** (Dùng khi triển khai đưa web lên Internet cho khách dùng thật):

```env
NEXT_PUBLIC_API_URL=[https://api.tiemhoacuanam.com/api/v1](https://api.tiemhoacuanam.com/api/v1)
NEXT_PUBLIC_IMAGE_BASE_URL=[https://api.tiemhoacuanam.com](https://api.tiemhoacuanam.com)

```

> ⚠️ **Chốt chặn Bảo mật:** Thêm hai tệp `.env.development` và `.env.production` vào file `.gitignore` để tránh việc vô tình đẩy các thông tin cấu hình nhạy cảm lên GitHub công khai.

### Bước 2: Tạo tệp Cấu hình Tập trung (`src/config/index.js`)

Thay vì ở mọi file Component bạn đều gõ `process.env.NEXT_PUBLIC_API_URL`, hãy tạo một file trung gian để quản lý tập trung. Điều này giúp bạn dễ dàng bổ sung giá trị mặc định (Fallback) nếu file `.env` bị thiếu.

Mở hoặc tạo tệp **`src/config/index.js`**:

```javascript
const CONFIG = {
  API_BASE_URL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api/v1',
  IMAGE_BASE_URL: process.env.NEXT_PUBLIC_IMAGE_BASE_URL || 'http://localhost:5000',
  TIMEOUT: 10000, // Thời gian tối đa chờ API phản hồi (10 giây)
};

export default CONFIG;

```

### Bước 3: Áp dụng Hằng số vào Dự án thực tế

#### 1. Cấu hình cho Axios / Fetch Instance để gọi API ngầm về ASP.NET Core

Tạo một file quản lý gọi API tập trung (Ví dụ: `src/services/apiClient.js`):

```javascript
import axios from 'axios';
import CONFIG from '../config'; // Import file cấu hình tập trung

const apiClient = axios.create({
  baseURL: CONFIG.API_BASE_URL, // Sử dụng hằng số môi trường động
  timeout: CONFIG.TIMEOUT,
  headers: {
    'Content-Type': 'application/json',
  },
});

export default apiClient;

```

#### 2. Nối chuỗi hiển thị Hình ảnh từ Backend trên giao diện danh sách hoa

Khi Backend ASP.NET Core lưu ảnh, họ thường chỉ lưu tên file (Ví dụ: `hoa-hong-do.jpg`). Phía Next.js phải lấy `IMAGE_BASE_URL` nối với tên file để ra đường dẫn ảnh tuyệt đối.

Tại component hiển thị sản phẩm (Ví dụ: `ProductCard.jsx`):

```javascript
import CONFIG from '../config';

export default function ProductCard({ product }) {
  // Nối chuỗi đường dẫn ảnh: https://localhost:5001 + /uploads/ + hoa-hong-do.jpg
  const fullImageUrl = `${CONFIG.IMAGE_BASE_URL}/uploads/${product.imagePath}`;

  return (
    <div className="product-card">
      <img 
        src={product.imagePath ? fullImageUrl : '/images/default-flower.jpg'} 
        alt={product.name} 
        className="product-img"
      />
      <h3>{product.name}</h3>
      <p>{product.price.toLocaleString('vi-VN')} đ</p>
    </div>
  );
}

```

---

## IV. Tóm tắt Tiêu chuẩn Vận hành Doanh nghiệp

| Tiêu chuẩn | Cách thực hiện | Mục đích |
| --- | --- | --- |
| **Không Hardcode URL** | Chuyển toàn bộ domain sang file `.env` | Giúp dev nhảy môi trường (từ máy cá nhân lên server thật) chỉ bằng 1 lệnh mà không cần sửa code. |
| **Tiền tố `NEXT_PUBLIC_**` | Bắt buộc thêm vào đầu tên biến | Cho phép React Component chạy dưới trình duyệt đọc được dữ liệu. |
| **Cấu hình tập trung** | Luôn gọi thông qua file `config/index.js` | Tạo màng bọc an toàn, gán giá trị mặc định phòng hờ file `.env` bị xóa nhầm, dễ bảo trì hệ thống. |

```

```