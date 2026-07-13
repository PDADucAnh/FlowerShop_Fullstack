# Tài Liệu Sử Dụng API Cấu Hình (Settings API Usage Document)

Tài liệu hướng dẫn gọi API cấu hình công khai phục vụ đồng bộ dữ liệu cửa hàng lên Frontend Next.js.

---

## 1. API Lấy Thông Tin Cửa Hàng (Store Info API)

API công khai hỗ trợ Frontend Next.js tải các thiết lập thông tin liên hệ của cửa hàng thay vì hardcode tĩnh trong các component React.

### 1.1. Chi tiết Endpoint
- **URL**: `/api/settings/store-info`
- **Phương thức**: `GET`
- **Quyền truy cập**: Public (Không yêu cầu Token xác thực / Header Authorization)
- **Định dạng dữ liệu trả về**: JSON

### 1.2. Phản hồi thành công mẫu (Response JSON - 200 OK)
```json
{
  "storeName": "FlowerShop - Hoa Tươi Quận 1",
  "logo": "/images/logo.png",
  "hotline": "0908765432",
  "email": "contact@flowershop.vn",
  "address": "123 Đường Hoa, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh",
  "facebook": "https://facebook.com/flowershop.quan1",
  "zalo": "0908765432",
  "openHours": "Thứ 2 - Thứ 7: 7:00 - 21:00 | CN: 8:00 - 18:00"
}
```

---

## 2. Hướng Dẫn Tích Hợp Frontend Next.js (Khuyên dùng)

Mặc dù giao diện Frontend Next.js hiện đang sử dụng dữ liệu tĩnh, trong tương lai khi có nhu cầu đồng bộ hóa, các kỹ thuật viên có thể sử dụng hook React hoặc Fetch API để gọi endpoint này:

### Ví dụ tích hợp với React useEffect:
```typescript
import { useState, useEffect } from 'react';

interface StoreInfo {
  storeName: string;
  logo: string;
  hotline: string;
  email: string;
  address: string;
  facebook: string;
  zalo: string;
  openHours: string;
}

export function useStoreInfo() {
  const [storeInfo, setStoreInfo] = useState<StoreInfo | null>(null);

  useEffect(() => {
    fetch('/api/settings/store-info')
      .then(res => res.json())
      .then(data => setStoreInfo(data))
      .catch(err => console.error('Failed to load store settings:', err));
  }, []);

  return storeInfo;
}
```
Sau đó, thay thế các văn bản cứng như `"FlowerShop"` hoặc `"support@flowershop.retail"` ở `Header.tsx` và `Footer.tsx` bằng `storeInfo?.storeName` và `storeInfo?.email`.
- Không cần sửa giao diện, chỉ sửa nơi lấy dữ liệu giúp hệ thống đồng nhất tuyệt đối từ CSDL SQL Server (Single Source of Truth).
