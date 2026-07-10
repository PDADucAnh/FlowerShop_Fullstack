# 6. Các chức năng quản trị hệ thống

Tài liệu này mô tả chi tiết các chức năng quản trị, biên tập và vận hành hệ thống được cung cấp trên giao diện quản lý MVC Admin Panel (CMS) dành cho quản trị viên (Admin) và nhân viên vận hành (Editor).

## 6.1. Quản lý sản phẩm (Products Management)

Module hỗ trợ kiểm soát toàn bộ vòng đời của sản phẩm hoa tươi được hiển thị trên hệ thống.

* Xem danh sách sản phẩm với các chức năng tìm kiếm, phân trang và sắp xếp.
* Thêm mới sản phẩm hoa: Nhập tên sản phẩm, mô tả chi tiết, giá bán, giá khuyến mãi, số lượng tồn kho và chọn danh mục sản phẩm tương ứng. Hệ thống tự động tạo mã SKU và tạo đường dẫn thân thiện (Slug) dựa trên tên sản phẩm.
* Chỉnh sửa thông tin sản phẩm và cập nhật số lượng tồn kho.
* Tải lên hình ảnh sản phẩm mới: Hệ thống tích hợp thư viện xử lý hình ảnh SixLabors.ImageSharp để kiểm tra tính hợp lệ của file tải lên (chỉ chấp nhận các định dạng file ảnh chuẩn) trước khi lưu vào thư mục lưu trữ `/uploads/products/`.
* Xóa sản phẩm khỏi hệ thống (thực hiện xóa vật lý trong cơ sở dữ liệu nếu sản phẩm chưa phát sinh đơn hàng).

## 6.2. Quản lý danh mục (Categories Management)

Chức năng phân loại sản phẩm và bài viết để tối ưu trải nghiệm duyệt web của khách hàng.

* **Quản lý danh mục sản phẩm (CategoryProduct):** Thêm, sửa, xóa các danh mục sản phẩm hoa (ví dụ: Hoa sinh nhật, Hoa cưới, Bó hoa, Kệ hoa).
* **Quản lý danh mục bài viết (Category):** Thêm, sửa, xóa các danh mục cho bài viết blog tin tức (ví dụ: Ý nghĩa loài hoa, Hướng dẫn cắm hoa, Tin tức cửa hàng).
* Hệ thống tự động phát sinh slug và kiểm tra ràng buộc khóa ngoại để ngăn chặn việc xóa các danh mục đang chứa sản phẩm hoặc bài viết.

## 6.3. Quản lý đơn hàng (Orders Management)

Module cốt lõi phục vụ xử lý và vận hành đơn hàng hàng ngày của cửa hàng.

* Xem danh sách toàn bộ đơn hàng của hệ thống kèm theo bộ lọc tìm kiếm theo trạng thái đơn hàng.
* Xem thông tin chi tiết của từng đơn hàng: Danh sách mặt hàng đã mua, số tiền, ghi chú của khách hàng, thông tin người nhận, địa chỉ nhận và khung giờ giao hoa mong muốn.
* Cập nhật trạng thái đơn hàng theo quy trình xử lý thực tế: Xác nhận đơn hàng (`Confirmed`), đánh dấu đang cắm hoa (`Preparing`), giao vận chuyển (`Shipping`), hoàn thành đơn hàng (`Completed`) hoặc hủy đơn hàng (`Cancelled`).
* Giao diện tích hợp thông báo thời gian thực sử dụng SignalR để phát cảnh báo tức thì khi khách hàng đặt đơn hàng mới, giúp nhân viên không bỏ lỡ đơn hàng.

## 6.4. Quản lý giao dịch thanh toán và hoàn tiền (Payments & Refunds)

Hỗ trợ đối soát tài chính và giải quyết khiếu nại của khách hàng.

* Xem lịch sử toàn bộ các giao dịch thanh toán trên hệ thống (bảng `Payments` và `PaymentAttempts`) để đối chiếu mã giao dịch VNPay, số tiền và thời gian thanh toán.
* Phê duyệt các yêu cầu hoàn tiền (Refund) đối với đơn hàng bị hủy do lỗi của cửa hàng hoặc do khách hàng hủy hợp lệ trước giờ giao. Hệ thống tự động tính toán số tiền được hoàn dựa trên chính sách hủy đơn, cập nhật trạng thái đơn hàng sang `Refunded` và gửi email thông báo kết quả hoàn tiền cho khách hàng.

## 6.5. Quản lý khách hàng và chống gian lận (Customers & Fraud Prevention)

Phân hệ theo dõi mức độ uy tín của khách hàng nhằm bảo vệ cửa hàng khỏi các rủi ro tài chính.

* Xem danh sách khách hàng đăng ký trên hệ thống, theo dõi tổng số đơn hàng đã đặt, tỷ lệ đơn hàng giao thành công và tỷ lệ đơn hàng giao thất bại (bị bùng đơn).
* Theo dõi điểm số đánh giá độ tin cậy của khách hàng (Fraud Score): Hệ thống tự động tăng Fraud Score dựa trên số lần giao hàng thất bại hoặc các hành vi đặt đơn hàng COD liên tục bất thường.
* Quản lý danh sách đen số điện thoại (Phone Blacklist): Admin có quyền thêm các số điện thoại có lịch sử bùng đơn vào blacklist để hệ thống tự động từ chối phương thức thanh toán COD khi phát hiện số điện thoại này được sử dụng trong khâu đặt hàng.

## 6.6. Quản lý tài khoản nội bộ (Users Management)

* Chỉ dành riêng cho quyền Admin cao nhất.
* CRUD danh sách các tài khoản nhân viên vận hành hệ thống (Admin, Editor/Staff).
* Hệ thống áp dụng chính sách bảo mật nghiêm ngặt: Không hiển thị mật khẩu trong danh sách thành viên, mật khẩu nhân viên được băm trước khi lưu trữ và cung cấp chức năng đổi mật khẩu riêng biệt.

## 6.7. Quản lý bài viết blog (Posts Management)

Phân hệ biên tập nội dung marketing và tư vấn cho website.

* Xem danh sách, tạo mới, chỉnh sửa hoặc xóa bài viết blog tin tức.
* Trình soạn thảo văn bản tích hợp thư viện CKEditor 5 cho phép nhân viên định dạng văn bản chuyên nghiệp, chèn hình ảnh, tạo liên kết và soạn thảo nội dung HTML trực quan.
* Hỗ trợ cập nhật các trường SEO metadata (tóm tắt, hình ảnh đại diện, tiêu đề bài viết) phục vụ tối ưu hóa công cụ tìm kiếm.

## 6.8. Quản lý quảng cáo (Advertisements Management)

Chức năng chỉnh sửa banner truyền thông hiển thị ở trang chủ website khách hàng.

* Thêm mới, cập nhật thông tin hoặc xóa banner quảng cáo.
* Cấu hình hình ảnh banner, liên kết đích (LinkUrl), tiêu đề quảng cáo, phụ đề quảng cáo, thứ tự sắp xếp hiển thị (SortOrder) và trạng thái hoạt động (IsActive) để bật hoặc tắt quảng cáo tùy thời điểm.

## 6.9. Báo cáo thống kê và Dashboard (Reports & Analytics)

* Trang chủ quản trị hiển thị biểu đồ thống kê doanh số trực quan thu được từ các đơn hàng đã hoàn thành.
* Thống kê tổng số lượng đơn hàng mới phát sinh, tổng số khách hàng đăng ký mới và tổng doanh thu tích lũy.
* Liệt kê danh sách các sản phẩm hoa tươi bán chạy nhất và danh sách các đơn hàng mới nhất cần xử lý nhanh trên trang chủ.
