# Biểu Đồ UseCase Hệ Thống FlowerShop

## Biểu đồ Usecase tổng quát

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Khách Hàng" as Customer
actor "Quản Trị Viên" as Admin
actor "Biên Tập Viên" as Editor

rectangle "Hệ Thống Quản Lý Bán Hoa\nFlowerShop" {
  usecase "(Quản lý tài khoản)" as UC_ACC
  usecase "(Quản lý sản phẩm)" as UC_PROD
  usecase "(Quản lý giỏ hàng)" as UC_CART
  usecase "(Đặt hàng)" as UC_ORDER
  usecase "(Thanh toán VNPAY)" as UC_PAY
  usecase "(Quản lý đơn hàng)" as UC_MGR_ORDER
  usecase "(Quản lý thanh toán)" as UC_PAY_MGR
  usecase "(Quản lý khách hàng)" as UC_CUS
  usecase "(Quản lý bài viết)" as UC_POST
  usecase "(Quản lý khuyến mãi)" as UC_PROMO
  usecase "(Dashboard thống kê)" as UC_DASH
}

Customer --> UC_ACC
Customer --> UC_PROD
Customer --> UC_CART
Customer --> UC_ORDER
Customer --> UC_PAY
Customer --> UC_MGR_ORDER

Admin --> UC_MGR_ORDER
Admin --> UC_PAY_MGR
Admin --> UC_CUS
Admin --> UC_POST
Admin --> UC_PROMO
Admin --> UC_DASH
Admin --> UC_PROD

Editor --> UC_PROD
Editor --> UC_POST
Editor --> UC_PROMO
Editor --> UC_DASH

@enduml
```
**Hình 1. Biểu đồ Usecase tổng quát**

---

## Biểu đồ Usecase quản lý tài khoản

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Khách Hàng" as Customer
actor "Quản Trị Viên" as Admin

rectangle "Quản Lý Tài Khoản" {
  usecase "(Đăng ký tài khoản)" as UC_REG
  usecase "(Đăng nhập)" as UC_LOGIN
  usecase "(Quên mật khẩu)" as UC_FORGOT
  usecase "(Đặt lại mật khẩu)" as UC_RESET
  usecase "(Xem hồ sơ cá nhân)" as UC_PROFILE
  usecase "(Cập nhật hồ sơ)" as UC_UPDATE
  usecase "(Đổi mật khẩu)" as UC_CHPWD
  usecase "(Đăng nhập quản trị)" as UC_ADMIN_LOGIN
  usecase "(Đăng xuất)" as UC_LOGOUT
  usecase "(Phân quyền người dùng)" as UC_ROLE
}

Customer --> UC_REG
Customer --> UC_LOGIN
Customer --> UC_FORGOT
Customer --> UC_RESET
Customer --> UC_PROFILE
Customer --> UC_UPDATE
Customer --> UC_CHPWD

Admin --> UC_ADMIN_LOGIN
Admin --> UC_LOGOUT
Admin --> UC_ROLE

@enduml
```
**Hình 2. Biểu đồ Usecase quản lý tài khoản**

---

## Biểu đồ Usecase quản lý sản phẩm

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Khách Hàng" as Customer
actor "Quản Trị Viên" as Admin
actor "Biên Tập Viên" as Editor

rectangle "Quản Lý Sản Phẩm" {
  usecase "(Xem danh sách sản phẩm)" as UC_LIST
  usecase "(Lọc sản phẩm theo danh mục)" as UC_FILTER_CAT
  usecase "(Lọc sản phẩm theo giá)" as UC_FILTER_PRICE
  usecase "(Sắp xếp sản phẩm)" as UC_SORT
  usecase "(Tìm kiếm sản phẩm)" as UC_SEARCH
  usecase "(Xem chi tiết sản phẩm)" as UC_DETAIL
  usecase "(Xem sản phẩm xu hướng)" as UC_TREND
  usecase "(Xem sản phẩm bán chạy)" as UC_BEST
  usecase "(Thêm sản phẩm mới)" as UC_CREATE
  usecase "(Sửa thông tin sản phẩm)" as UC_UPDATE
  usecase "(Xóa sản phẩm)" as UC_DELETE
  usecase "(Tải lên hình ảnh)" as UC_IMAGE
  usecase "(Quản lý tồn kho)" as UC_STOCK
  usecase "(Quản lý biến thể)" as UC_VARIANT
  usecase "(Thêm vào giỏ hàng)" as UC_ADD_CART
}

Customer --> UC_LIST
Customer --> UC_FILTER_CAT
Customer --> UC_FILTER_PRICE
Customer --> UC_SORT
Customer --> UC_SEARCH
Customer --> UC_DETAIL
Customer --> UC_TREND
Customer --> UC_BEST
Customer --> UC_ADD_CART

Admin --> UC_LIST
Admin --> UC_CREATE
Admin --> UC_UPDATE
Admin --> UC_DELETE
Admin --> UC_IMAGE
Admin --> UC_STOCK
Admin --> UC_VARIANT

Editor --> UC_LIST
Editor --> UC_CREATE
Editor --> UC_UPDATE
Editor --> UC_DELETE
Editor --> UC_IMAGE
Editor --> UC_STOCK
Editor --> UC_VARIANT

@enduml
```
**Hình 3. Biểu đồ Usecase quản lý sản phẩm**

---

## Biểu đồ Usecase quản lý giỏ hàng

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Khách Hàng" as Customer

rectangle "Quản Lý Giỏ Hàng" {
  usecase "(Thêm sản phẩm vào giỏ)" as UC_ADD
  usecase "(Xem giỏ hàng)" as UC_VIEW
  usecase "(Cập nhật số lượng)" as UC_QTY
  usecase "(Xóa sản phẩm khỏi giỏ)" as UC_REMOVE
  usecase "(Thêm vào danh sách yêu thích)" as UC_WISHLIST
  usecase "(Xem danh sách yêu thích)" as UC_VIEW_WISH
  usecase "(Xóa khỏi danh sách yêu thích)" as UC_REMOVE_WISH
  usecase "(Tiến hành đặt hàng)" as UC_CHECKOUT
}

Customer --> UC_ADD
Customer --> UC_VIEW
Customer --> UC_QTY
Customer --> UC_REMOVE
Customer --> UC_WISHLIST
Customer --> UC_VIEW_WISH
Customer --> UC_REMOVE_WISH
Customer --> UC_CHECKOUT

@enduml
```
**Hình 4. Biểu đồ Usecase quản lý giỏ hàng**

---

## Biểu đồ Usecase đặt hàng

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Khách Hàng" as Customer

rectangle "Đặt Hàng" {
  usecase "(Nhập thông tin người mua)" as UC_BUYER
  usecase "(Nhập thông tin người nhận)" as UC_RECIPIENT
  usecase "(Chọn địa chỉ giao hàng)" as UC_ADDRESS
  usecase "(Chọn khung giờ giao)" as UC_SLOT
  usecase "(Chọn phương thức thanh toán)" as UC_PAY_METHOD
  usecase "(Thanh toán COD)" as UC_COD
  usecase "(Thanh toán VNPAY)" as UC_VNPAY
  usecase "(Xác thực COD qua OTP)" as UC_OTP
  usecase "(Xác nhận đặt hàng)" as UC_CONFIRM
  usecase "(Hủy đặt hàng)" as UC_CANCEL
}

Customer --> UC_BUYER
Customer --> UC_RECIPIENT
Customer --> UC_ADDRESS
Customer --> UC_SLOT
Customer --> UC_PAY_METHOD
UC_PAY_METHOD --> UC_COD : <<extend>>
UC_PAY_METHOD --> UC_VNPAY : <<extend>>
UC_COD --> UC_OTP : <<extend>>
Customer --> UC_CONFIRM
Customer --> UC_CANCEL

@enduml
```
**Hình 5. Biểu đồ Usecase đặt hàng**

---

## Biểu đồ Usecase thanh toán VNPAY

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Khách Hàng" as Customer
actor "Hệ Thống VNPay" as VNPay

rectangle "Thanh Toán VNPAY" {
  usecase "(Tạo yêu cầu thanh toán)" as UC_CREATE
  usecase "(Tạo URL thanh toán)" as UC_URL
  usecase "(Chuyển hướng sang VNPay)" as UC_REDIRECT
  usecase "(Nhập thông tin thẻ)" as UC_CARD
  usecase "(Xác thực giao dịch)" as UC_AUTH
  usecase "(Nhận kết quả callback)" as UC_CALLBACK
  usecase "(Xử lý kết quả thanh toán)" as UC_PROCESS
  usecase "(Cập nhật trạng thái đơn hàng)" as UC_UPDATE
  usecase "(Thanh toán thất bại)" as UC_FAIL
  usecase "(Hoàn tiền qua VNPay)" as UC_REFUND
}

Customer --> UC_CREATE
UC_CREATE --> UC_URL : <<include>>
UC_URL --> UC_REDIRECT : <<include>>
Customer --> UC_CARD
UC_CARD --> UC_AUTH : <<include>>
VNPay --> UC_CALLBACK
UC_CALLBACK --> UC_PROCESS : <<include>>
UC_PROCESS --> UC_UPDATE : <<include>>
UC_PROCESS --> UC_FAIL : <<extend>>
Admin --> UC_REFUND

@enduml
```
**Hình 6. Biểu đồ Usecase thanh toán VNPAY**

---

## Biểu đồ Usecase quản lý đơn hàng

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Khách Hàng" as Customer
actor "Quản Trị Viên" as Admin
actor "Biên Tập Viên" as Editor

rectangle "Quản Lý Đơn Hàng" {
  usecase "(Xem lịch sử đơn hàng)" as UC_HISTORY
  usecase "(Xem chi tiết đơn hàng)" as UC_DETAIL
  usecase "(Hủy đơn hàng)" as UC_CANCEL
  usecase "(Xem danh sách đơn hàng)" as UC_LIST
  usecase "(Xem chi tiết đơn hàng - Admin)" as UC_ADMIN_DETAIL
  usecase "(Cập nhật trạng thái đơn hàng)" as UC_STATUS
  usecase "(Xác nhận đơn COD)" as UC_CONFIRM_COD
  usecase "(Hủy đơn hàng - Admin)" as UC_ADMIN_CANCEL
  usecase "(Xem lịch sử email)" as UC_EMAIL_HIST
  usecase "(Quản lý chi tiết đơn hàng)" as UC_LINE_ITEM
}

Customer --> UC_HISTORY
Customer --> UC_DETAIL
Customer --> UC_CANCEL

Admin --> UC_LIST
Admin --> UC_ADMIN_DETAIL
Admin --> UC_STATUS
Admin --> UC_CONFIRM_COD
Admin --> UC_ADMIN_CANCEL
Admin --> UC_EMAIL_HIST
Admin --> UC_LINE_ITEM

Editor --> UC_LIST
Editor --> UC_ADMIN_DETAIL
Editor --> UC_STATUS
Editor --> UC_ADMIN_CANCEL

@enduml
```
**Hình 7. Biểu đồ Usecase quản lý đơn hàng**

---

## Biểu đồ Usecase quản lý thanh toán

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Quản Trị Viên" as Admin
actor "Hệ Thống VNPay" as VNPay

rectangle "Quản Lý Thanh Toán" {
  usecase "(Xem danh sách giao dịch)" as UC_LIST
  usecase "(Xem chi tiết giao dịch)" as UC_DETAIL
  usecase "(Xem yêu cầu hoàn tiền)" as UC_REFUND_LIST
  usecase "(Phê duyệt hoàn tiền)" as UC_APPROVE
  usecase "(Từ chối hoàn tiền)" as UC_REJECT
  usecase "(Xem lịch sử thanh toán đơn)" as UC_PAY_HIST
  usecase "(Xem nhật ký thanh toán thất bại)" as UC_FAIL_LOG
}

Admin --> UC_LIST
Admin --> UC_DETAIL
Admin --> UC_REFUND_LIST
Admin --> UC_APPROVE
Admin --> UC_REJECT
Admin --> UC_PAY_HIST
Admin --> UC_FAIL_LOG

@enduml
```
**Hình 8. Biểu đồ Usecase quản lý thanh toán**

---

## Biểu đồ Usecase quản lý khách hàng

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Quản Trị Viên" as Admin

rectangle "Quản Lý Khách Hàng" {
  usecase "(Xem danh sách khách hàng)" as UC_LIST
  usecase "(Xem chi tiết khách hàng)" as UC_DETAIL
  usecase "(Xem lịch sử mua hàng)" as UC_HISTORY
  usecase "(Xem fraud score)" as UC_FRAUD
  usecase "(Sửa thông tin khách hàng)" as UC_UPDATE
  usecase "(Xóa khách hàng)" as UC_DELETE
  usecase "(Quản lý địa chỉ khách hàng)" as UC_ADDRESS
  usecase "(Quản lý số điện thoại blacklist)" as UC_BLACKLIST
}

Admin --> UC_LIST
Admin --> UC_DETAIL
Admin --> UC_HISTORY
Admin --> UC_FRAUD
Admin --> UC_UPDATE
Admin --> UC_DELETE
Admin --> UC_ADDRESS
Admin --> UC_BLACKLIST

@enduml
```
**Hình 9. Biểu đồ Usecase quản lý khách hàng**

---

## Biểu đồ Usecase quản lý bài viết

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Khách Hàng" as Customer
actor "Quản Trị Viên" as Admin
actor "Biên Tập Viên" as Editor

rectangle "Quản Lý Bài Viết" {
  usecase "(Xem danh sách bài viết)" as UC_LIST
  usecase "(Xem bài viết theo danh mục)" as UC_CAT
  usecase "(Xem chi tiết bài viết)" as UC_DETAIL
  usecase "(Thêm bài viết mới)" as UC_CREATE
  usecase "(Sửa bài viết)" as UC_UPDATE
  usecase "(Xóa bài viết)" as UC_DELETE
  usecase "(Soạn thảo nội dung rich-text)" as UC_EDITOR
  usecase "(Tải lên hình ảnh bài viết)" as UC_IMAGE
  usecase "(Quản lý danh mục blog)" as UC_MGR_CAT
}

Customer --> UC_LIST
Customer --> UC_CAT
Customer --> UC_DETAIL

Admin --> UC_LIST
Admin --> UC_CREATE
Admin --> UC_UPDATE
Admin --> UC_DELETE
Admin --> UC_EDITOR
Admin --> UC_IMAGE
Admin --> UC_MGR_CAT

Editor --> UC_LIST
Editor --> UC_CREATE
Editor --> UC_UPDATE
Editor --> UC_DELETE
Editor --> UC_EDITOR
Editor --> UC_IMAGE
Editor --> UC_MGR_CAT

@enduml
```
**Hình 10. Biểu đồ Usecase quản lý bài viết**

---

## Biểu đồ Usecase quản lý khuyến mãi

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Quản Trị Viên" as Admin
actor "Biên Tập Viên" as Editor

rectangle "Quản Lý Khuyến Mãi (Quảng Cáo)" {
  usecase "(Xem danh sách quảng cáo)" as UC_LIST
  usecase "(Thêm quảng cáo mới)" as UC_CREATE
  usecase "(Sửa quảng cáo)" as UC_UPDATE
  usecase "(Xóa quảng cáo)" as UC_DELETE
  usecase "(Tải lên hình ảnh banner)" as UC_IMAGE
  usecase "(Sắp xếp thứ tự hiển thị)" as UC_REORDER
  usecase "(Đặt thời gian hiệu lực)" as UC_DATE
  usecase "(Bật / Tắt quảng cáo)" as UC_TOGGLE
}

Admin --> UC_LIST
Admin --> UC_CREATE
Admin --> UC_UPDATE
Admin --> UC_DELETE
Admin --> UC_IMAGE
Admin --> UC_REORDER
Admin --> UC_DATE
Admin --> UC_TOGGLE

Editor --> UC_LIST
Editor --> UC_CREATE
Editor --> UC_UPDATE
Editor --> UC_DELETE
Editor --> UC_IMAGE
Editor --> UC_DATE
Editor --> UC_TOGGLE

@enduml
```
**Hình 11. Biểu đồ Usecase quản lý khuyến mãi**

---

## Biểu đồ Usecase Dashboard thống kê

```plantuml
@startuml
left to right direction
skinparam packageStyle rectangle

actor "Quản Trị Viên" as Admin
actor "Biên Tập Viên" as Editor

rectangle "Dashboard Thống Kê" {
  usecase "(Xem tổng quan dashboard)" as UC_DASH
  usecase "(Xem tổng doanh thu)" as UC_REVENUE
  usecase "(Xem số lượng đơn hàng)" as UC_ORDER_CNT
  usecase "(Xem số lượng sản phẩm)" as UC_PROD_CNT
  usecase "(Xem số lượng khách hàng)" as UC_CUS_CNT
  usecase "(Xem biểu đồ doanh thu)" as UC_CHART
  usecase "(Xem đơn hàng theo trạng thái)" as UC_STATUS
  usecase "(Xem sản phẩm bán chạy)" as UC_BEST
}

Admin --> UC_DASH
Admin --> UC_REVENUE
Admin --> UC_ORDER_CNT
Admin --> UC_PROD_CNT
Admin --> UC_CUS_CNT
Admin --> UC_CHART
Admin --> UC_STATUS
Admin --> UC_BEST

Editor --> UC_DASH

@enduml
```
**Hình 12. Biểu đồ Usecase Dashboard thống kê**
