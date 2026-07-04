# Kế hoạch triển khai Luồng xử lý đơn hàng tối ưu (ASP.NET & React Course Project)

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Triển khai luồng đặt hàng hoa tươi tinh gọn (1 shop hoa duy nhất, thanh toán MoMo giả lập, xác nhận COD thủ công bằng Admin, giữ hàng tạm thời bằng MemoryCache), xác thực thợ cắm hoa quá tải (max 5 đơn/khung giờ/sản phẩm), tự động chặn COD nếu SĐT thuộc Blacklist, gửi Email khi Xác nhận đơn và khi Giao hàng thành công.

**Architecture:**
- **Frontend (React)**: 
  - Giao diện chọn Quận/Huyện TP.HCM để lọc kho cơ bản (chỉ giao khu vực hỗ trợ và kiểm tra tồn kho tổng).
  - Form Checkout chia làm 3 phần: Người mua (Họ tên, SĐT, Email), Người nhận (Họ tên, SĐT, Lời chúc cắm thiệp), và Ngày + Khung giờ giao hàng.
  - Tích hợp API kiểm tra Blacklist SĐT để ẩn/disable phương thức thanh toán COD ngay trên UI.
  - Trang giả lập MoMo `/momo-mock` mô phỏng thanh toán thành công/thất bại.
- **Backend (ASP.NET Core API)**:
  - Cấu hình `IMemoryCache` quản lý giữ chỗ tạm thời (TTL 15 phút) trong quá trình thanh toán MoMo Mock.
  - Kiểm tra điều kiện tạo đơn: Blacklist SĐT khách hàng, và quá tải thợ cắm hoa (`DeliverySlot` có `CurrentBooked >= 5`).
  - Tích hợp gửi Gmail tự động ở 2 thời điểm: Khi đơn hàng được **Xác nhận (Confirmed)** và khi đơn hàng **Hoàn thành (Completed)**.
  - Background Service (`IHostedService`) tự động quét hủy đơn quá hạn 30 phút (COD chưa duyệt) hoặc 15 phút (Online chưa thanh toán).

**Tech Stack:** ASP.NET Core, EF Core, SQL Server, IMemoryCache, React, TypeScript, MailKit / System.Net.Mail.

## Global Constraints
- Sử dụng trực tiếp tồn kho thực tế `StockQuantity` của thực thể `Product` tại cửa hàng chính.
- Email gửi đi sử dụng SMTP cấu hình trong `appsettings.json`.
- Ngôn ngữ giao diện: Tiếng Việt.

---

### Task 1: Cấu hình giữ hàng tạm thời bằng IMemoryCache (Memory Lock Manager)

**Files:**
- Create: `CMS.Backend/Services/StockLockService.cs`
- Modify: `CMS.Backend/Program.cs`
- Modify: `CMS.Backend/Services/OrderService.cs`

**Interfaces:**
- Consumes: `IMemoryCache` từ hệ thống
- Produces: Service quản lý giữ chỗ tạm thời `StockLockService`

- [ ] **Step 1: Viết StockLockService quản lý giữ kho tạm**
  Tạo file [StockLockService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/StockLockService.cs):
  ```csharp
  using Microsoft.Extensions.Caching.Memory;
  using System;

  namespace CMS.Backend.Services
  {
      public class StockLockService
      {
          private readonly IMemoryCache _cache;
          private static readonly object LockObj = new();

          public StockLockService(IMemoryCache cache)
          {
              _cache = cache;
          }

          // Giữ kho tạm thời trong 15 phút
          public bool ReserveStock(int productId, int quantity, TimeSpan ttl)
          {
              lock (LockObj)
              {
                  string key = $"stock_reserved:{productId}";
                  int currentReserved = _cache.Get<int?>(key) ?? 0;
                  _cache.Set(key, currentReserved + quantity, ttl);
                  return true;
              }
          }

          public int GetReservedStock(int productId)
          {
              lock (LockObj)
              {
                  return _cache.Get<int?>($"stock_reserved:{productId}") ?? 0;
              }
          }

          public void ReleaseReservedStock(int productId, int quantity)
          {
              lock (LockObj)
              {
                  string key = $"stock_reserved:{productId}";
                  int currentReserved = _cache.Get<int?>(key) ?? 0;
                  int newReserved = Math.Max(0, currentReserved - quantity);
                  
                  if (newReserved == 0)
                      _cache.Remove(key);
                  else
                      _cache.Set(key, newReserved);
              }
          }
      }
  }
  ```

- [ ] **Step 2: Đăng ký MemoryCache và StockLockService**
  Sửa file [Program.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Program.cs#L136-L139):
  ```csharp
  builder.Services.AddMemoryCache();
  builder.Services.AddScoped<CMS.Backend.Services.StockLockService>();
  ```

---

### Task 2: Backend API Lọc Kho theo Vị trí & API Kiểm tra Blacklist SĐT

**Files:**
- Create: `CMS.Backend/Controllers/Api/LocationGatingController.cs`
- Modify: `CMS.Backend/Controllers/Api/OrdersController.cs`

**Interfaces:**
- Consumes: `IApplicationDbContext`, `StockLockService`, `IFraudDetectionService`
- Produces: 
  - API Endpoint `POST /api/LocationGating/check-availability`
  - API Endpoint `GET /api/Orders/check-blacklist`

- [ ] **Step 1: Viết API kiểm tra khả dụng Quận/Huyện và tồn kho khả dụng**
  Tạo file [LocationGatingController.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Controllers/Api/LocationGatingController.cs):
  ```csharp
  using CMS.Backend.Models.DTOs;
  using CMS.Backend.Services;
  using CMS.Data;
  using Microsoft.AspNetCore.Mvc;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  namespace CMS.Backend.Controllers.Api
  {
      [Route("api/[controller]")]
      [ApiController]
      public class LocationGatingController : ControllerBase
      {
          private readonly IApplicationDbContext _context;
          private readonly StockLockService _stockLockService;

          private static readonly HashSet<string> SupportedDistricts = new()
          {
              "Quận 1", "Quận 3", "Quận 4", "Quận 5", "Quận 6", "Quận 7", "Quận 8", "Quận 10", "Quận 11", "Quận 12",
              "Quận Bình Thạnh", "Quận Gò Vấp", "Quận Phú Nhuận", "Quận Tân Bình", "Quận Tân Phú", "Quận Bình Tân",
              "Thành phố Thủ Đức"
          };

          public LocationGatingController(IApplicationDbContext context, StockLockService stockLockService)
          {
              _context = context;
              _stockLockService = stockLockService;
          }

          [HttpPost("check-availability")]
          public async Task<IActionResult> CheckAvailability([FromBody] CheckAvailabilityRequest request)
          {
              if (!ModelState.IsValid) return BadRequest(ModelState);

              if (!SupportedDistricts.Contains(request.District))
              {
                  return Ok(new AvailabilityResponse
                  {
                      Available = false,
                      Message = "Cửa hàng hiện chưa hỗ trợ giao hoa tới Quận/Huyện này. Vui lòng chọn khu vực khác."
                  });
              }

              foreach (var item in request.Items)
              {
                  var product = await _context.Products.FindAsync(item.ProductId);
                  if (product == null) return NotFound(new { message = "Không tìm thấy sản phẩm" });

                  int reserved = _stockLockService.GetReservedStock(item.ProductId);
                  int availableStock = product.StockQuantity - reserved;

                  if (availableStock < item.Quantity)
                  {
                      return Ok(new AvailabilityResponse
                      {
                          Available = false,
                          Message = $"Sản phẩm '{product.Name}' hiện không đủ số lượng (còn {availableStock} sản phẩm khả dụng)."
                      });
                  }
              }

              return Ok(new AvailabilityResponse { Available = true, Message = "Sẵn sàng giao hoa!" });
          }
      }
  }
  ```

- [ ] **Step 2: Viết API kiểm tra số điện thoại có bị Blacklist hay không**
  Sửa file [OrdersController.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Controllers/Api/OrdersController.cs) bổ sung endpoint:
  ```csharp
  [HttpGet("check-blacklist")]
  [AllowAnonymous]
  public async Task<IActionResult> CheckBlacklist([FromQuery] string phone)
  {
      if (string.IsNullOrEmpty(phone)) return BadRequest("SĐT không hợp lệ");
      var isBlacklisted = await _orderService.IsPhoneBlacklisted(phone); // Cần khai báo trong IOrderService
      return Ok(new { isBlacklisted });
  }
  ```

---

### Task 3: Frontend Pop-up Chọn Quận/Huyện & Checkout Form Mở rộng (Location Gating UI)

**Files:**
- Create: `cms.frontend/src/components/LocationGatingModal.tsx`
- Modify: `cms.frontend/src/pages/cart/index.tsx`
- Modify: `cms.frontend/src/pages/checkout/index.tsx`
- Modify: `cms.frontend/src/schemas/checkoutSchema.ts`

- [ ] **Step 1: Tạo component Pop-up chọn Quận/Huyện**
  Tạo file [LocationGatingModal.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/components/LocationGatingModal.tsx):
  ```tsx
  import React, { useState } from 'react';
  import axiosClient from '../api/axiosClient';

  interface LocationGatingModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: (district: string) => void;
    items: { productId: number; quantity: number }[];
  }

  const HCM_DISTRICTS = [
    "Quận 1", "Quận 3", "Quận 4", "Quận 5", "Quận 6", "Quận 7", "Quận 8", "Quận 10", "Quận 11", "Quận 12",
    "Quận Bình Thạnh", "Quận Gò Vấp", "Quận Phú Nhuận", "Quận Tân Bình", "Quận Tân Phú", "Quận Bình Tân",
    "Thành phố Thủ Đức"
  ];

  export const LocationGatingModal: React.FC<LocationGatingModalProps> = ({ isOpen, onClose, onSuccess, items }) => {
    const [selectedDistrict, setSelectedDistrict] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    if (!isOpen) return null;

    const handleCheck = async () => {
      if (!selectedDistrict) {
        setError('Vui lòng chọn Quận/Huyện');
        return;
      }
      setLoading(true);
      setError('');
      try {
        const response: any = await axiosClient.post('/LocationGating/check-availability', {
          district: selectedDistrict,
          items: items.map(i => ({ productId: i.productId, quantity: i.quantity }))
        });

        if (response.available) {
          localStorage.setItem('delivery_district', selectedDistrict);
          onSuccess(selectedDistrict);
        } else {
          setError(response.message || 'Khu vực này hiện đang hết hàng.');
        }
      } catch (err) {
        setError('Lỗi kết nối máy chủ. Vui lòng thử lại.');
      } finally {
        setLoading(false);
      }
    };

    return (
      <div className="fixed inset-0 z-[110] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
        <div className="bg-surface border border-outline-variant p-xl max-w-md w-full shadow-2xl rounded-xl">
          <div className="text-center space-y-sm">
            <span className="material-symbols-outlined text-4xl text-primary">local_shipping</span>
            <h3 className="font-headline-sm text-headline-sm uppercase tracking-widest">Địa chỉ giao hàng</h3>
            <p className="text-secondary text-sm">Vui lòng chọn Quận/Huyện giao hàng tại TP.HCM để xác minh hỗ trợ vận chuyển.</p>
          </div>

          <div className="space-y-md mt-md">
            <select
              value={selectedDistrict}
              onChange={(e) => setSelectedDistrict(e.target.value)}
              className="w-full bg-surface-container-low border border-outline-variant px-md py-4 text-sm font-semibold tracking-widest outline-none rounded-lg"
            >
              <option value="">-- CHỌN QUẬN/HUYỆN --</option>
              {HCM_DISTRICTS.map((d) => (
                <option key={d} value={d}>{d}</option>
              ))}
            </select>
            {error && <p className="text-error text-[11px] font-bold text-center uppercase tracking-wider">{error}</p>}
          </div>

          <div className="flex gap-md mt-lg">
            <button
              onClick={onClose}
              disabled={loading}
              className="flex-1 border border-outline-variant py-3 text-label-sm uppercase tracking-widest font-bold bg-transparent hover:bg-surface-container cursor-pointer"
            >
              Hủy bỏ
            </button>
            <button
              onClick={handleCheck}
              disabled={loading}
              className="flex-1 bg-primary text-on-primary py-3 text-label-sm uppercase tracking-widest font-bold border border-primary hover:opacity-90 cursor-pointer"
            >
              {loading ? 'Đang kiểm tra...' : 'Xác nhận'}
            </button>
          </div>
        </div>
      </div>
    );
  };
  ```

- [ ] **Step 2: Cập nhật Form Checkout & Tích hợp kiểm tra Blacklist SĐT tự động**
  Sửa file [index.tsx (Checkout)](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/checkout/index.tsx):
  - Chia form làm 3 phần: Người mua, Người nhận (Họ tên, SĐT, Lời chúc), Chọn Ngày + Khung giờ giao hàng.
  - Sử dụng sự kiện `onBlur` trên ô nhập Số điện thoại người mua. Gọi API `/api/Orders/check-blacklist?phone=...`.
  - Nếu SĐT bị blacklist: Hiển thị cảnh báo màu đỏ *"Số điện thoại này có lịch sử bùng hàng. Bạn bắt buộc phải thanh toán Online."*, đồng thời ép buộc chọn phương thức chuyển khoản (OnlinePayment) và disable tùy chọn COD.

---

### Task 4: Slot Validation & Chặn Blacklist Backend (Order Validation)

**Files:**
- Modify: `CMS.Backend/Services/OrderService.cs`
- Modify: `CMS.Backend/Services/Interfaces/IOrderService.cs`

**Interfaces:**
- Consumes: Khung giờ giao hàng và SĐT người mua từ UI Checkout
- Produces: API tạo đơn hàng kiểm tra thợ cắm hoa quá tải (max 5 đơn/slot) và chặn COD nếu Blacklist

- [ ] **Step 1: Khai báo hàm check blacklist trong IOrderService**
  Sửa file [IOrderService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/Interfaces/IOrderService.cs):
  ```csharp
  Task<bool> IsPhoneBlacklisted(string phone);
  ```

- [ ] **Step 2: Cập nhật hàm Check quá tải thợ cắm hoa (Overload) và Blacklist trong OrderService**
  Sửa file [OrderService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/OrderService.cs):
  - Thêm phương thức `IsPhoneBlacklisted` gọi `_fraudDetectionService.IsPhoneBlacklisted`.
  - Trong phương thức `CreateOrder`:
    *   **Check Blacklist**: Nếu chọn COD mà SĐT người mua bị Blacklist, lập tức báo lỗi: *"Số điện thoại này đã bị chặn thanh toán COD do lịch sử bùng đơn hàng. Vui lòng thanh toán online."*
    *   **Check Kho động (Slot)**: Khi gọi `_deliverySlotService.TryLockSlot`, kiểm tra xem tổng số đơn cắm hoa đã nhận tại khung giờ đó đạt tối đa chưa. Nếu `CurrentBooked >= 5`, trả về lỗi: *"Khung giờ này đã bận, vui lòng chọn khung giờ khác."*
  - Khi thanh toán online, gọi `StockLockService.ReserveStock` giữ chỗ tạm trong 15 phút.

---

### Task 5: Giả lập MoMo Mock & Xử lý Webhook cập nhật đơn hàng

**Files:**
- Create: `cms.frontend/src/pages/checkout/MomoMock.tsx`
- Modify: `cms.frontend/src/App.tsx`
- Modify: `CMS.Backend/Services/PaymentService.cs`

- [ ] **Step 1: Tạo giao diện thanh toán giả lập MoMo**
  Đăng ký route `/momo-mock` trong [App.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/App.tsx).
  Tạo file [MomoMock.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/checkout/MomoMock.tsx) chứa 2 nút: "Giả lập: Thanh toán thành công" và "Giả lập: Thanh toán thất bại".
  *Khi nhấn Thành công*: Gọi API Webhook Backend cập nhật trạng thái đơn sang `Confirmed` và chuyển hướng về `/order-confirmation?orderId={id}`.

- [ ] **Step 2: Xử lý Webhook cập nhật đơn hàng**
  Sửa file [PaymentService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/PaymentService.cs#L98-L126):
  - Webhook nhận được trạng thái `success`: Trừ kho thực tế trong DB, giải phóng bộ nhớ đệm `StockLockService.ReleaseReservedStock`, chuyển trạng thái đơn hàng sang `Confirmed` và gọi dịch vụ gửi Gmail thông báo xác nhận đơn hàng thành công.
  - Webhook nhận trạng thái `failed`: Giải phóng bộ nhớ đệm giữ hàng và hủy đơn hàng.

---

### Task 6: Tự động gửi Email khi Xác nhận & Hoàn thành Đơn hàng (Gmail Integration)

**Files:**
- Modify: `CMS.Backend/Services/EmailService.cs`
- Modify: `CMS.Backend/Services/Interfaces/IEmailService.cs`
- Modify: `CMS.Backend/Services/OrderService.cs`
- Modify: `CMS.Backend/Controllers/OrderController.cs`

**Interfaces:**
- Consumes: SMTP cấu hình trong `appsettings.json`
- Produces: Email tự động gửi đến hòm thư khách hàng khi đơn hàng được Xác nhận (Confirmed) và Hoàn thành (Completed).

- [ ] **Step 1: Bổ sung các phương thức gửi Mail trong IEmailService**
  Sửa file [IEmailService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/Interfaces/IEmailService.cs):
  ```csharp
  Task SendOrderConfirmedEmailAsync(Order order, string customerEmail, string customerName);
  Task SendOrderCompletedEmailAsync(Order order, string customerEmail, string customerName);
  ```

- [ ] **Step 2: Viết nội dung mẫu Email Xác nhận & Email Hoàn thành**
  Sửa file [EmailService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/EmailService.cs):
  - Viết mẫu Email cho sự kiện **Xác nhận thành công (Confirmed)**: Thông báo *"Đơn hàng #{order.Id} đã được xác nhận và thợ cắm hoa đang tiến hành thiết kế hoa. Cảm ơn bạn!"*.
  - Viết mẫu Email cho sự kiện **Hoàn thành (Completed)**: Thông báo *"Đơn hàng #{order.Id} của bạn đã được giao thành công. AnhCMS Boutique xin trân trọng cảm ơn quý khách!"*.

- [ ] **Step 3: Kích hoạt gửi Mail khi thay đổi trạng thái đơn**
  - **Khi đơn hàng được Xác nhận**:
    *   Trong luồng thanh toán MoMo Webhook thành công (trạng thái đổi sang `Confirmed`): Gọi `SendOrderConfirmedEmailAsync`.
    *   Trong luồng duyệt COD thủ công của Admin tại [OrderController.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Controllers/OrderController.cs) (khi click nút "Đã gọi điện - Xác nhận đơn"): Gọi `SendOrderConfirmedEmailAsync`.
  - **Khi đơn hàng Hoàn thành (Completed)**:
    *   Trong phương thức cập nhật trạng thái đơn hàng sang `Completed` (do Admin cập nhật tại trang quản trị), tự động kích hoạt `SendOrderCompletedEmailAsync`.

---

### Task 7: Hosted Background Service quét hủy đơn quá hạn (Memory Cleanup Job)

**Files:**
- Create: `CMS.Backend/Services/OrderExpiryBackgroundService.cs`
- Modify: `CMS.Backend/Program.cs`

- [ ] **Step 1: Tạo Background Service tự động**
  Tạo file [OrderExpiryBackgroundService.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Services/OrderExpiryBackgroundService.cs):
  Quét dọn định kỳ mỗi phút:
  - Hủy đơn COD chưa được xác nhận quá 30 phút.
  - Hủy đơn Online chưa được thanh toán quá 15 phút.
  - Gọi giải phóng bộ nhớ đệm `StockLockService.ReleaseReservedStock`.

- [ ] **Step 2: Đăng ký Background Service ngầm**
  Sửa file [Program.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Program.cs#L136-L139):
  ```csharp
  builder.Services.AddHostedService<CMS.Backend.Services.OrderExpiryBackgroundService>();
  ```

---

## Kiểm thử luồng hoạt động (Testing & Verification)

### Kiểm thử Gửi Gmail
1. Cấu hình thông số Gmail SMTP thật của bạn trong `appsettings.json`.
2. Tạo đơn hàng và thanh toán thành công (hoặc nhấn nút xác nhận đơn COD trong Admin).
3. *Expected:* Khách hàng nhận được hòm thư thông báo Xác nhận đơn hàng thành công trong vòng vài giây.
4. Cập nhật trạng thái đơn hàng sang `Completed` trên trang quản lý Admin.
5. *Expected:* Khách hàng nhận được email thông báo đơn hàng đã giao thành công.

### Kiểm thử quá tải thợ cắm hoa (Overload)
1. Thêm 5 đơn hàng cùng một khung giờ (`08:00-10:00`) vào cùng một ngày giao.
2. Đặt tiếp đơn thứ 6 vào khung giờ đó.
3. *Expected:* Backend từ chối tạo đơn và trả lỗi: *"Khung giờ này đã bận, vui lòng chọn khung giờ khác."*
