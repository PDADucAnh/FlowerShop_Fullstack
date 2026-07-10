# 11. Các ca sử dụng hệ thống (Use Cases)

Tài liệu này liệt kê và mô tả danh sách các ca sử dụng (Use Cases) của hệ thống PDA FlowerShop, được phân nhóm theo các phân hệ chức năng và đối tượng tác nhân (Guest, Customer, Admin/Staff).

## 11.1. Phân hệ Xác thực và Quản lý tài khoản (Authentication & Account)

Phân hệ chịu trách nhiệm xác định danh tính tác nhân và bảo mật thông tin cá nhân.

* **Ca sử dụng dành cho Khách vãng lai:**
    * *Đăng ký tài khoản:* Khách hàng mới cung cấp thông tin email, số điện thoại, mật khẩu để tạo tài khoản mua hàng.
    * *Đăng nhập tài khoản:* Khách hàng nhập email và mật khẩu để xác thực quyền truy cập.
    * *Quên mật khẩu:* Khách hàng yêu cầu gửi mã khôi phục mật khẩu về email cá nhân khi quên mật khẩu đăng nhập.
* **Ca sử dụng dành cho Khách hàng đã đăng nhập:**
    * *Đăng xuất tài khoản:* Khách hàng hủy phiên làm việc hiện tại trên thiết bị.
    * *Cập nhật hồ sơ cá nhân:* Khách hàng cập nhật lại thông tin họ tên, số điện thoại hoặc địa chỉ nhận hàng mặc định.
    * *Thay đổi mật khẩu:* Khách hàng tiến hành đổi mật khẩu đăng nhập bằng cách cung cấp mật khẩu cũ và mật khẩu mới.
* **Ca sử dụng dành cho Quản trị viên (Admin):**
    * *Quản lý tài khoản nhân viên:* Admin thực hiện thêm mới, chỉnh sửa thông tin, phân vai trò hoặc khóa tài khoản của các nhân viên vận hành (Editor/Staff).

## 11.2. Phân hệ Sản phẩm và Mua sắm (Product & Shopping)

Phân hệ hỗ trợ khách hàng tìm kiếm thông tin hoa tươi và quản lý giỏ hàng mua sắm.

* **Ca sử dụng dành cho Khách vãng lai và Khách hàng:**
    * *Duyệt sản phẩm:* Duyệt danh sách các mẫu hoa tươi đang bán kèm theo bộ lọc danh mục và khoảng giá mong muốn.
    * *Tìm kiếm sản phẩm:* Tìm nhanh sản phẩm hoa theo từ khóa khớp tên sản phẩm hoặc mã SKU.
    * *Xem chi tiết sản phẩm:* Xem hình ảnh cận cảnh, mô tả chi tiết, giá bán chính thức, giá khuyến mãi và kiểm tra số lượng tồn kho khả dụng.
    * *Quản lý giỏ hàng:* Khách hàng thêm sản phẩm vào giỏ hàng, điều chỉnh số lượng mua hoặc xóa sản phẩm khỏi giỏ hàng.
    * *Quản lý danh sách yêu thích (Wishlist):* Thêm các mẫu hoa yêu thích vào trang danh sách riêng để thuận tiện theo dõi và mua sắm sau này.

## 11.3. Phân hệ Đơn hàng và Thanh toán (Order & Payment)

Phân hệ cốt lõi quản lý hoạt động kinh doanh trực tuyến của hệ thống.

* **Ca sử dụng dành cho Khách hàng:**
    * *Tiến hành đặt hàng (Checkout):* Điền thông tin người nhận, lời nhắn thiệp, chọn ngày giao, khung giờ giao hàng và chọn hình thức thanh toán.
    * *Thanh toán trực tuyến:* Thực hiện giao dịch chuyển tiền trực tiếp thông qua cổng thanh toán liên kết VNPay.
    * *Xác minh đơn hàng COD:* Nhận và nhập mã OTP gửi về email cá nhân để xác minh đơn hàng thanh toán khi nhận hàng.
    * *Tra cứu lịch sử đơn hàng:* Xem danh sách đơn hàng đã đặt và kiểm tra trạng thái vận chuyển thời gian thực.
    * *Hủy đơn hàng:* Gửi yêu cầu hủy đơn hàng đối với các đơn hàng chưa bàn giao giao nhận.
* **Ca sử dụng dành cho Quản trị viên và Nhân viên:**
    * *Quản lý đơn hàng:* Xem thông tin chi tiết các đơn đặt hàng toàn hệ thống và thực hiện cập nhật trạng thái đơn hàng (Xác nhận, Đang chuẩn bị, Giao hàng, Hoàn thành, Hủy đơn).
    * *Phê duyệt hoàn tiền (Chỉ Admin):* Xem xét các đơn hàng đã thanh toán online bị hủy, thực hiện giao dịch hoàn trả tiền và xác nhận trạng thái hoàn tiền thành công.
    * *Quản lý danh sách đen (Phone Blacklist):* Thêm hoặc xóa các số điện thoại gian lận bùng hàng khỏi blacklist của hệ thống.
    * *Quản lý khung giờ giao hàng (Delivery Slots):* Thiết lập năng lực chuẩn bị hoa tối đa của cửa hàng trong từng khung giờ cụ thể theo ngày.

## 11.4. Phân hệ Tin tức và Biên tập nội dung (Blog & Content)

Phân hệ quản lý các nội dung bổ trợ tiếp thị trên website.

* **Ca sử dụng dành cho Khách vãng lai và Khách hàng:**
    * *Đọc tin tức blog:* Xem danh sách các bài viết chia sẻ về ý nghĩa loài hoa, cẩm nang cắm hoa và hướng dẫn chăm sóc hoa tươi.
* **Ca sử dụng dành cho Quản trị viên và Nhân viên:**
    * *Quản lý bài viết blog:* Tạo mới, chỉnh sửa nội dung bài viết bằng trình soạn thảo văn bản CKEditor 5 và cập nhật hình ảnh đại diện bài viết.
    * *Quản lý quảng cáo:* Thêm, sửa hoặc xóa các banner quảng cáo xuất hiện trên trang chủ website.
