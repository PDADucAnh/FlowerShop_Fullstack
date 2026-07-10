# 1. Người dùng hệ thống

Tài liệu này mô tả chi tiết các nhóm người dùng và phân quyền tương ứng trong hệ thống bán hoa tươi trực tuyến PDA FlowerShop. Hệ thống phân chia quyền truy cập rõ ràng giữa giao diện khách hàng (React SPA) và giao diện quản trị (ASP.NET Core MVC).

## 1.1. Khách vãng lai (Guest)

Khách vãng lai là người dùng chưa đăng nhập tài khoản trên hệ thống. Nhóm người dùng này được phép thực hiện các chức năng cơ bản liên quan đến tìm kiếm thông tin và trải nghiệm sản phẩm.

* Xem danh sách sản phẩm và lọc theo danh mục sản phẩm.
* Xem chi tiết sản phẩm bao gồm hình ảnh, giá cả, mô tả chi tiết và số lượng tồn kho.
* Tìm kiếm sản phẩm theo từ khóa (tên hoặc mã sản phẩm SKU).
* Xem danh sách bài viết blog và chi tiết bài viết blog.
* Thêm sản phẩm vào giỏ hàng và cập nhật số lượng trong giỏ hàng (dữ liệu được lưu trữ tạm thời tại bộ nhớ cục bộ LocalStorage của trình duyệt).
* Đăng ký tài khoản khách hàng mới.
* Đăng nhập vào hệ thống.

## 1.2. Khách hàng (Customer)

Khách hàng là người dùng đã đăng ký tài khoản và đăng nhập thành công vào giao diện khách hàng (React SPA). Quyền hạn của khách hàng bao gồm toàn bộ các chức năng của khách vãng lai và các chức năng cá nhân hóa sau.

* Quản lý thông tin hồ sơ cá nhân (cập nhật họ tên, số điện thoại, địa chỉ mặc định).
* Quản lý danh sách sản phẩm yêu thích (Wishlist) lưu trữ đồng bộ.
* Tiến hành đặt hàng (Checkout) từ giỏ hàng hiện tại.
* Lựa chọn phương thức thanh toán trực tuyến qua cổng thanh toán VNPay hoặc thanh toán khi nhận hàng (COD).
* Nhận mã OTP qua email để xác minh đơn hàng COD đối với tài khoản thuộc nhóm cần kiểm tra gian lận.
* Xem lịch sử đơn hàng cá nhân và theo dõi trạng thái đơn hàng thời gian thực.
* Thực hiện yêu cầu hủy đơn hàng đối với các đơn hàng ở trạng thái cho phép (chưa chuyển sang trạng thái đang giao hàng hoặc đã hoàn thành) theo chính sách hủy đơn.
* Đổi mật khẩu tài khoản khách hàng.

## 1.3. Quản trị viên và Nhân viên (Admin và Editor)

Nhóm người dùng quản trị truy cập vào hệ thống qua giao diện MVC Admin Panel riêng biệt, sử dụng cơ chế xác thực Cookie Authentication kết hợp phân quyền chi tiết (Authorization Policies).

* **Quản trị viên (Admin):** Có toàn bộ quyền hạn cao nhất trong hệ thống quản trị nội dung (CMS).
    * Quản lý danh mục sản phẩm và danh sách sản phẩm (Thêm, Sửa, Xóa).
    * Quản lý danh mục bài viết và bài viết blog.
    * Quản lý danh sách khách hàng, cập nhật điểm đánh giá gian lận (Fraud Score) và đưa các số điện thoại gian lận vào danh sách đen (Blacklist).
    * Quản lý danh sách đơn hàng toàn hệ thống, cập nhật trạng thái đơn hàng (Xác nhận, Đang chuẩn bị, Đang giao, Hoàn thành, Hủy đơn).
    * Quản lý các giao dịch thanh toán và thực hiện phê duyệt hoàn tiền (Refund) trực tiếp cho khách hàng.
    * Quản lý danh sách tài khoản nội bộ (Admin/Editor) và phân quyền tương ứng.
    * Quản lý quảng cáo, banner hiển thị trên website chính thức.
    * Cấu hình và thiết lập các khung giờ giao hàng (Delivery Slots).
* **Biên tập viên/Nhân viên (Editor/Staff):** Có quyền hỗ trợ vận hành và biên tập nội dung, nhưng bị giới hạn đối với các chức năng liên quan đến tài chính hoặc bảo mật tài khoản.
    * Xem danh sách đơn hàng và hỗ trợ cập nhật trạng thái đơn hàng (chuẩn bị hoa).
    * Biên tập, tạo mới hoặc cập nhật bài viết blog và danh mục bài viết.
    * Thêm mới hoặc chỉnh sửa thông tin sản phẩm, danh mục sản phẩm.
    * Không có quyền quản lý người dùng quản trị, không có quyền duyệt hoàn tiền hoặc can thiệp trực tiếp vào danh sách đen khách hàng nếu không được cấp phép đặc biệt.
