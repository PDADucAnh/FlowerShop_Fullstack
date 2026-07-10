# Biểu Đồ Tuần Tự (Sequence Diagram) Hệ Thống FlowerShop

## Biểu đồ tuần tự Usecase đăng ký tài khoản

```plantuml
@startuml
actor "Khách Hàng" as User
participant "React SPA" as Frontend
participant "AuthController" as API
participant "AuthService" as Service
participant "ApplicationDbContext" as DB

User -> Frontend: Nhập email, password, confirmPassword
Frontend -> API: POST /api/auth/register
API -> Service: RegisterAsync(request)
Service -> DB: Kiểm tra email tồn tại
DB --> Service: Kết quả
alt Email đã tồn tại
  Service --> API: Lỗi "Email already exists"
  API --> Frontend: 400 Bad Request
  Frontend --> User: Hiển thị lỗi
else Email hợp lệ
  Service -> Service: Hash password (BCrypt)
  Service -> DB: Tạo Customer mới
  DB --> Service: Customer created
  Service -> Service: Tạo JWT token
  Service --> API: AuthResponse (token + profile)
  API --> Frontend: 200 OK (token + profile)
  Frontend -> Frontend: Lưu token vào localStorage
  Frontend --> User: Chuyển hướng đến trang chủ
end
@enduml
```
**Hình 13. Biểu đồ tuần tự đăng ký**

---

## Biểu đồ tuần tự Usecase đăng nhập

```plantuml
@startuml
actor "Khách Hàng" as User
participant "React SPA" as Frontend
participant "AuthController" as API
participant "AuthService" as Service
participant "ApplicationDbContext" as DB

User -> Frontend: Nhập email, password
Frontend -> API: POST /api/auth/login
API -> Service: LoginAsync(email, password)
Service -> DB: Tìm customer theo email
DB --> Service: Customer entity

alt Không tìm thấy
  Service --> API: Lỗi "Invalid credentials"
  API --> Frontend: 401 Unauthorized
  Frontend --> User: Hiển thị lỗi
else Tìm thấy
  Service -> Service: Verify password (BCrypt)
  alt Sai mật khẩu
    Service --> API: Lỗi "Invalid credentials"
    API --> Frontend: 401 Unauthorized
    Frontend --> User: Hiển thị lỗi
  else Đúng
    Service -> Service: Tạo JWT access token + refresh token
    Service --> API: AuthResponse
    API --> Frontend: 200 OK (token + profile)
    Frontend -> Frontend: Lưu token vào localStorage
    Frontend --> User: Chuyển hướng đến trang chủ
  end
end
@enduml
```
**Hình 14. Biểu đồ tuần tự đăng nhập**

---

## Biểu đồ tuần tự Usecase tìm kiếm sản phẩm

```plantuml
@startuml
actor "Khách Hàng" as User
participant "React SPA" as Frontend
participant "ProductsController" as API
participant "ProductService" as Service
participant "ApplicationDbContext" as DB

User -> Frontend: Nhập từ khóa tìm kiếm
Frontend -> API: GET /api/products/search?keyword=...
API -> Service: SearchProductsAsync(keyword)
Service -> DB: Query sản phẩm theo tên/SKU
DB --> Service: Danh sách Product
Service --> API: List<ProductDto>
API --> Frontend: 200 OK (danh sách sản phẩm)
Frontend -> Frontend: Render danh sách lên giao diện
Frontend --> User: Hiển thị kết quả tìm kiếm

alt Không có kết quả
  Frontend --> User: Hiển thị "Không tìm thấy sản phẩm"
end
@enduml
```
**Hình 15. Biểu đồ tuần tự tìm kiếm**

---

## Biểu đồ tuần tự Usecase quản lý giỏ hàng

```plantuml
@startuml
actor "Khách Hàng" as User
participant "React SPA" as Frontend
participant "ProductsController" as API
participant "ProductService" as Service
participant "localStorage" as Storage

== Thêm vào giỏ hàng ==
User -> Frontend: Click "Thêm vào giỏ" (productId, variant, qty)
Frontend -> API: POST /api/products/{id}/track-add-to-cart
API -> Service: TrackAddToCartAsync(productId)
Service --> API: OK
Frontend -> Storage: Lấy giỏ hàng hiện tại
Storage --> Frontend: Cart data (JSON)
Frontend -> Frontend: Thêm sản phẩm vào cart
Frontend -> Storage: Lưu giỏ hàng mới
Frontend --> User: Hiển thị badge + thông báo "Đã thêm vào giỏ"

== Xem giỏ hàng ==
User -> Frontend: Click vào biểu tượng giỏ hàng
Frontend -> Storage: Lấy giỏ hàng
Storage --> Frontend: Cart data
Frontend -> Frontend: Tính tổng tiền
Frontend --> User: Hiển thị danh sách + tổng tiền

== Cập nhật số lượng ==
User -> Frontend: Thay đổi số lượng
Frontend -> Storage: Cập nhật số lượng
Frontend -> Frontend: Tính lại tổng tiền
Frontend --> User: Cập nhật hiển thị

== Xóa sản phẩm ==
User -> Frontend: Click "Xóa"
Frontend -> Storage: Xóa sản phẩm khỏi cart
Frontend --> User: Cập nhật giỏ hàng
@enduml
```
**Hình 16. Biểu đồ tuần tự giỏ hàng**

---

## Biểu đồ tuần tự Usecase đặt hàng

```plantuml
@startuml
actor "Khách Hàng" as User
participant "React SPA" as Frontend
participant "OrdersController" as API
participant "OrderService" as Service
participant "ApplicationDbContext" as DB

== Bước 1: Nhập thông tin ==
User -> Frontend: Nhập thông tin người mua + người nhận
Frontend --> User: Hiển thị form

== Bước 2: Chọn khung giờ ==
User -> Frontend: Chọn ngày, quận/huyện
Frontend -> API: GET /api/delivery/slots?date=X&districtId=Y
API -> Service: GetAvailableSlots(date, districtId)
Service --> API: Danh sách khung giờ
API --> Frontend: 200 OK
Frontend --> User: Hiển thị khung giờ trống
User -> Frontend: Chọn khung giờ

== Bước 3: Chọn thanh toán ==
User -> Frontend: Chọn COD hoặc VNPay

== Bước 4: Xác nhận ==
User -> Frontend: Click "Đặt hàng"
Frontend -> API: POST /api/orders
API -> Service: CreateOrderAsync(request)
Service -> Service: Tính tổng tiền
Service -> DB: Kiểm tra tồn kho
DB --> Service: Stock OK

alt Hết hàng
  Service --> API: Lỗi "Out of stock"
  API --> Frontend: 400 Bad Request
  Frontend --> User: Thông báo lỗi
else Còn hàng
  Service -> DB: Tạo Order (Pending)
  Service -> DB: Tạo OrderDetails
  Service -> DB: Lock stock (15 phút TTL)
  Service -> DB: Tạo Payment (chờ thanh toán)
  DB --> Service: Order created
  Service --> API: OrderResponse
  API --> Frontend: 200 OK (order info)
  Frontend -> Frontend: Xóa giỏ hàng
  Frontend --> User: Chuyển đến trang xác nhận đơn hàng
end
@enduml
```
**Hình 17. Biểu đồ tuần tự đặt hàng**

---

## Biểu đồ tuần tự Usecase thanh toán VNPAY

```plantuml
@startuml
actor "Khách Hàng" as User
participant "React SPA" as Frontend
participant "PaymentController" as API
participant "PaymentService" as Service
participant "VNPay Gateway" as VNPay
participant "ApplicationDbContext" as DB

User -> Frontend: Click "Thanh toán VNPay"
Frontend -> API: POST /api/payment/create-vnpay-url
API -> Service: CreateVnPayUrlAsync(orderId)
Service -> DB: Lấy thông tin đơn hàng
DB --> Service: Order + Payment
Service -> Service: Tạo PaymentRequest (IPN URL, order info)
Service --> API: PaymentUrl
API --> Frontend: 200 OK (vnpayUrl)
Frontend -> Frontend: Redirect sang VNPay
Frontend -> VNPay: GET {vnpayUrl}

VNPay -> User: Hiển thị cổng thanh toán
User -> VNPay: Nhập thông tin thẻ + xác thực
VNPay -> VNPay: Xử lý giao dịch

alt Giao dịch thành công
  VNPay -> API: GET /api/payment/vnpay-callback (vnp_ResponseCode=00)
  API -> Service: ProcessVnPayCallbackAsync(params)
  Service -> DB: Cập nhật Payment (Success)
  Service -> DB: Cập nhật Order (Confirmed)
  DB --> Service: Updated
  Service --> API: Redirect URL
  API --> Frontend: 302 Redirect
  Frontend --> User: Hiển thị "Thanh toán thành công"
else Giao dịch thất bại
  VNPay -> API: GET /api/payment/vnpay-callback (vnp_ResponseCode!=00)
  API -> Service: ProcessVnPayCallbackAsync(params)
  Service -> DB: Cập nhật Payment (Failed)
  Service -> DB: Ghi nhận PaymentAttempt
  DB --> Service: Updated
  Service --> API: Redirect URL
  API --> Frontend: 302 Redirect
  Frontend --> User: Hiển thị "Thanh toán thất bại" + nút thanh toán lại
end
@enduml
```
**Hình 18. Biểu đồ tuần tự thanh toán VNPAY**

---

## Biểu đồ tuần tự Usecase thanh toán lại đơn hàng chưa thanh toán

```plantuml
@startuml
actor "Khách Hàng" as User
participant "React SPA" as Frontend
participant "PaymentController" as API
participant "PaymentService" as Service
participant "VNPay Gateway" as VNPay
participant "ApplicationDbContext" as DB

User -> Frontend: Click "Thanh toán lại"
Frontend -> API: POST /api/payment/retry/{orderId}
API -> Service: RetryPaymentAsync(orderId)
Service -> DB: Kiểm tra đơn hàng
DB --> Service: Order info

alt Đơn hàng không hợp lệ (đã thanh toán / hết hạn)
  Service --> API: Lỗi "Cannot retry payment"
  API --> Frontend: 400 Bad Request
  Frontend --> User: Thông báo lỗi
else Hợp lệ
  Service -> DB: Tạo PaymentAttempt mới
  Service -> Service: Tạo VNPay URL mới
  Service --> API: PaymentUrl
  API --> Frontend: 200 OK (vnpayUrl)
  Frontend -> Frontend: Redirect sang VNPay
  Frontend -> VNPay: GET {vnpayUrl}
  
  VNPay -> User: Hiển thị cổng thanh toán
  User -> VNPay: Nhập thông tin thẻ
  VNPay -> VNPay: Xử lý giao dịch
  
  alt Thành công
    VNPay -> API: Callback
    API -> Service: ProcessVnPayCallbackAsync(params)
    Service -> DB: Cập nhật Payment + Order
    API --> Frontend: 302 Redirect
    Frontend --> User: "Thanh toán thành công"
  else Thất bại
    VNPay -> API: Callback
    Service -> DB: Ghi nhận PaymentAttempt thất bại
    Frontend --> User: "Thanh toán thất bại" (có thể thử lại)
  end
end
@enduml
```
**Hình 19. Biểu đồ tuần tự thanh toán lại**

---

## Biểu đồ tuần tự Usecase hủy đơn hàng

```plantuml
@startuml
actor "Khách Hàng" as User
participant "React SPA" as Frontend
participant "OrdersController" as API
participant "OrderService" as Service
participant "CancellationPolicyService" as Policy
participant "ApplicationDbContext" as DB

User -> Frontend: Click "Hủy đơn hàng"
Frontend -> API: PUT /api/orders/{id}/cancel
API -> Service: CancelOrderAsync(orderId, reason)
Service -> DB: Lấy thông tin đơn hàng
DB --> Service: Order

Service -> Policy: Tính phí hủy theo CancellationPolicy
Policy --> Service: RefundPercentage, CancelFee

alt Không được phép hủy (trạng thái không hợp lệ)
  Service --> API: Lỗi "Cannot cancel order"
  API --> Frontend: 400 Bad Request
  Frontend --> User: Thông báo lỗi
else Được phép hủy
  Service -> DB: Cập nhật Order (Cancelled)
  Service -> DB: Ghi lý do hủy
  Service -> DB: Unlock stock (hoàn lại tồn kho)
  alt Có thanh toán VNPay
    Service -> Service: Tạo Refund request
    Service -> DB: Lưu Refund (Pending)
  end
  Service --> API: CancelResult (fee, refund amount)
  API --> Frontend: 200 OK
  Frontend --> User: Hiển thị "Đã hủy đơn hàng" + thông tin hoàn tiền
end
@enduml
```
**Hình 20. Biểu đồ tuần tự hủy đơn**

---

## Biểu đồ tuần tự Usecase hoàn tiền

```plantuml
@startuml
actor "Quản Trị Viên" as Admin
participant "ASP.NET MVC CMS" as CMS
participant "OrderController" as Controller
participant "OrderService" as Service
participant "PaymentService" as PaymentSvc
participant "VNPay Gateway" as VNPay
participant "ApplicationDbContext" as DB

Admin -> CMS: Vào trang chi tiết đơn hàng
CMS -> Controller: GET /Order/Detail/{id}
Controller -> Service: GetOrderByIdAsync(id)
Service -> DB: Query order + payment
DB --> Service: Order + Payment
Controller --> CMS: Render view

Admin -> CMS: Click "Xử lý hoàn tiền"
CMS -> Controller: POST /Order/ProcessRefund
Controller -> Service: ProcessRefundAsync(orderId, amount)
Service -> DB: Lấy thông tin payment
DB --> Service: Payment info

alt Thanh toán VNPay
  Service -> PaymentSvc: RefundViaVnPayAsync(transactionId, amount)
  PaymentSvc -> VNPay: Gọi API hoàn tiền
  VNPay --> PaymentSvc: RefundResponse
  alt Hoàn tiền thành công
    PaymentSvc -> DB: Cập nhật Refund (Approved)
    PaymentSvc -> DB: Cập nhật Payment (Refunded)
    Service -> Service: Gửi email thông báo hoàn tiền
    Controller --> CMS: 200 OK + thông báo thành công
    CMS --> Admin: Hiển thị "Đã hoàn tiền thành công"
  else Thất bại
    PaymentSvc --> Service: Lỗi refund
    Controller --> CMS: 400 Bad Request
    CMS --> Admin: Hiển thị lỗi
  end
else Thanh toán COD
  Service -> DB: Cập nhật Refund (Approved)
  Controller --> CMS: 200 OK
  CMS --> Admin: Hiển thị "Đã xác nhận hoàn tiền COD"
end
@enduml
```
**Hình 21. Biểu đồ tuần tự hoàn tiền**

---

## Biểu đồ tuần tự Usecase quản lý đơn hàng (Admin)

```plantuml
@startuml
actor "Quản Trị Viên" as Admin
participant "ASP.NET MVC CMS" as CMS
participant "OrderController" as Controller
participant "OrderService" as Service
participant "ApplicationDbContext" as DB

== Xem danh sách đơn hàng ==
Admin -> CMS: Vào trang Quản lý đơn hàng
CMS -> Controller: GET /Order
Controller -> Service: GetAllOrdersAsync(filters)
Service -> DB: Query orders (có phân trang, lọc)
DB --> Service: List<Order>
Controller --> CMS: Render view với danh sách
CMS --> Admin: Hiển thị bảng đơn hàng

== Xem chi tiết đơn hàng ==
Admin -> CMS: Click vào một đơn hàng
CMS -> Controller: GET /Order/Detail/{id}
Controller -> Service: GetOrderDetailAsync(id)
Service -> DB: Query order + orderDetails + payment + emailHistory
DB --> Service: Order detail
Controller --> CMS: Render detail view
CMS --> Admin: Hiển thị chi tiết đơn hàng

== Xác nhận đơn COD ==
Admin -> CMS: Click "Xác nhận COD"
CMS -> Controller: POST /Order/ConfirmCOD
Controller -> Service: ConfirmCodOrderAsync(orderId)
Service -> DB: Cập nhật Order (Confirmed)
Service -> DB: Cập nhật Payment (Success)
Service -> Service: Gửi email xác nhận
Controller --> CMS: 200 OK
CMS --> Admin: Thông báo "Đã xác nhận đơn hàng"

== Xem lịch sử email ==
Admin -> CMS: Click "Lịch sử email"
CMS -> Controller: GET /Order/EmailHistory/{orderId}
Controller -> Service: GetEmailHistoryAsync(orderId)
Service -> DB: Query EmailHistory
Controller --> CMS: Render partial view
CMS --> Admin: Hiển thị danh sách email đã gửi
@enduml
```
**Hình 22. Biểu đồ tuần tự quản lý đơn hàng**

---

## Biểu đồ tuần tự Usecase cập nhật trạng thái giao hàng

```plantuml
@startuml
actor "Quản Trị Viên" as Admin
actor "Biên Tập Viên" as Editor
participant "ASP.NET MVC CMS" as CMS
participant "OrderController" as Controller
participant "OrderService" as Service
participant "ApplicationDbContext" as DB

Admin -> CMS: Vào chi tiết đơn hàng
CMS -> Controller: GET /Order/Detail/{id}
Controller -> Service: GetOrderDetailAsync(id)
Service -> DB: Query order
Controller --> CMS: Render view
CMS --> Admin: Hiển thị trạng thái hiện tại

Admin -> CMS: Chọn trạng thái mới (Processing / Shipping / Completed)
Admin -> CMS: Click "Cập nhật"
CMS -> Controller: POST /Order/UpdateStatus
Controller -> Service: UpdateOrderStatusAsync(orderId, newStatus)
Service -> DB: Kiểm tra trạng thái hiện tại

alt Chuyển trạng thái hợp lệ
  Service -> DB: Cập nhật Order status
  Service -> DB: Ghi lại lịch sử
  alt Trạng thái = Shipping
    Service -> Service: Gửi email thông báo "Đơn hàng đang giao"
  else Trạng thái = Completed
    Service -> Service: Gửi email "Giao hàng thành công"
  end
  Service --> Controller: Success
  Controller --> CMS: 200 OK
  CMS --> Admin: Thông báo "Cập nhật trạng thái thành công"
else Chuyển trạng thái không hợp lệ
  Service --> Controller: Lỗi "Invalid status transition"
  Controller --> CMS: 400 Bad Request
  CMS --> Admin: Thông báo lỗi
end
@enduml
```
**Hình 23. Biểu đồ tuần tự giao hàng**

---

## Biểu đồ tuần tự Usecase Dashboard thống kê

```plantuml
@startuml
actor "Quản Trị Viên" as Admin
actor "Biên Tập Viên" as Editor
participant "ASP.NET MVC CMS" as CMS
participant "HomeController" as Controller
participant "DashboardService" as Service
participant "ApplicationDbContext" as DB

Admin -> CMS: Vào trang Dashboard
CMS -> Controller: GET /Home/Index
Controller -> Service: GetDashboardDataAsync()

Service -> DB: Đếm tổng số đơn hàng
DB --> Service: OrderCount

Service -> DB: Tính tổng doanh thu (tháng hiện tại)
DB --> Service: Revenue

Service -> DB: Đếm tổng số sản phẩm
DB --> Service: ProductCount

Service -> DB: Đếm tổng số khách hàng
DB --> Service: CustomerCount

Service -> DB: Lấy danh sách đơn hàng theo trạng thái
DB --> Service: OrdersByStatus

Service -> DB: Lấy sản phẩm bán chạy
DB --> Service: BestSellingProducts

Service --> Controller: DashboardViewModel
Controller --> CMS: Render view
CMS --> Admin: Hiển thị biểu đồ + số liệu thống kê
@enduml
```
**Hình 24. Biểu đồ tuần tự Dashboard**
