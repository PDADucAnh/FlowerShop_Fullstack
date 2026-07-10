# 15. Tổng kết báo cáo và hướng phát triển

Tài liệu này tổng kết toàn bộ kết quả đạt được của dự án phát triển hệ thống PDA FlowerShop, đánh giá các ưu điểm, các hạn chế còn tồn đọng và đề xuất các hướng nghiên cứu, phát triển tiếp theo.

## 15.1. Tổng quan kết quả đạt được của dự án

Dự án đã nghiên cứu, thiết kế và xây dựng thành công hệ thống thương mại điện tử kết hợp quản trị nội dung bán hoa tươi trực tuyến PDA FlowerShop đáp ứng đầy đủ các yêu cầu nghiệp vụ thực tế đặt ra.

* Xây dựng giao diện khách hàng bằng công nghệ React 19 và TypeScript mượt mà, hiện đại, tối ưu hóa tốc độ phản hồi và hiển thị.
* Phát triển hệ thống máy chủ RESTful Web API bằng ASP.NET Core 8.0 bảo đảm tính chịu tải cao và tổ chức mã nguồn khoa học theo mô hình phân tầng.
* Thiết lập cơ sở dữ liệu quan hệ đồng bộ trên SQL Server thông qua Entity Framework Core Code-First.
* Tích hợp thành công cổng thanh toán VNPay Sandbox bảo đảm tính bảo mật và chính xác cao của luồng giao dịch tài chính.
* Triển khai hệ thống gửi thư tự động Email Service, hệ thống SignalR thông báo thời gian thực và dịch vụ chạy ngầm OrderExpiry xử lý tự động.

## 15.2. Các ưu điểm nổi bật của hệ thống

Hệ thống sở hữu nhiều giải pháp công nghệ nổi bật giúp giải quyết triệt để các bài toán khó khăn trong vận hành thực tế của cửa hàng hoa.

* **Tính phân tách và độc lập cao:** Việc tách biệt hoàn toàn ứng dụng Frontend và Backend giúp tăng cường tính bảo mật, dễ dàng nâng cấp hoặc thay đổi công nghệ ở từng phần mà không ảnh hưởng đến phần còn lại.
* **Cơ chế chống overselling thông minh (Stock Lock):** Sử dụng Memory Cache để khóa giữ chỗ tồn kho tạm thời trong 15 phút khi đặt hàng, giúp giải quyết triệt để vấn đề tranh chấp mua hàng khi có lượng truy cập thanh toán đồng thời lớn, bảo vệ uy tín của cửa hàng.
* **Hệ thống phòng chống gian lận đa tầng:** Kết hợp bộ kiểm tra điểm gian lận khách hàng (Fraud Score), khóa số điện thoại trong danh sách đen (Phone Blacklist) và gửi mã xác thực OTP qua thư điện tử trước khi chấp nhận đơn hàng COD, giúp giảm thiểu tối đa rủi ro thiệt hại tài chính do bị bùng hoa tươi.
* **Bảo mật hệ thống vững chắc:** Triển khai cơ chế xác thực kép (JWT cho API và Cookie cho CMS), bảo vệ dữ liệu bằng băm mật khẩu PBKDF2, ngăn chặn các nguy cơ SQL Injection, XSS, CSRF và thiết lập giới hạn tần suất gửi yêu cầu Rate Limiting.
* **Quản trị vận hành khoa học:** Trang Admin CMS cung cấp đầy đủ các công cụ quản lý nghiệp vụ, tự động cập nhật thông tin thời gian thực qua SignalR và Dashboard thống kê doanh thu biểu đồ trực quan.

## 15.3. Các hạn chế hiện tại của hệ thống

Bên cạnh các kết quả đạt được, hệ thống vẫn tồn tại một số điểm hạn chế cần được cải thiện.

* **Phí vận chuyển cố định:** Hệ thống chưa tích hợp API địa lý của Google Maps để đo khoảng cách thực tế từ cửa hàng đến người nhận, dẫn đến việc phí giao hàng đang được cấu hình cố định theo quận/huyện thay vì tính toán động chính xác.
* **Quy trình logistics thủ công:** Chưa kết nối API với các hãng vận chuyển công nghệ (Ahamove, Lalamove), nhân viên cửa hàng vẫn phải thực hiện liên hệ shipper bên ngoài một cách thủ công.
* **Quy trình hoàn tiền thủ công:** Chức năng hoàn tiền khi hủy đơn hàng yêu cầu quản trị viên thực hiện chuyển khoản thủ công bên ngoài hệ thống ngân hàng rồi nhấn xác nhận trên trang quản trị, chưa tích hợp gọi trực tiếp API hoàn tiền tự động (Refund API) của cổng VNPay.
* **Thiếu tính năng đánh giá sản phẩm:** Hệ thống chưa triển khai chức năng cho phép khách hàng viết nhận xét và chấm điểm sao đánh giá chất lượng sản phẩm hoa tươi sau khi nhận hàng.

## 15.4. Định hướng nghiên cứu và phát triển tiếp theo

Để phát triển hệ thống PDA FlowerShop trở nên hoàn thiện và mạnh mẽ hơn trong tương lai, các hướng nghiên cứu tiếp theo sẽ tập trung vào các điểm sau.

* Tích hợp các API của các hãng vận chuyển công nghệ hàng đầu để tự động hóa việc đẩy đơn vận chuyển, tìm kiếm tài xế giao hoa và cập nhật trạng thái di chuyển của shipper trên bản đồ cho khách hàng theo dõi.
* Nghiên cứu tích hợp các giải pháp trí tuệ nhân tạo (AI) để phân tích hành vi duyệt sản phẩm của khách hàng, từ đó đề xuất các mẫu hoa tươi phù hợp nhất theo dịp lễ hoặc khoảng giá ưa thích.
* Hoàn thiện module đánh giá sản phẩm (Product Reviews) và tích hợp hệ thống quản lý mã giảm giá nâng cao cùng phân hạng thành viên thân thiết.
* Liên kết trực tiếp với các API hoàn tiền tự động của VNPay để thực hiện hoàn tiền tức thời về tài khoản khách hàng ngay khi đơn hàng được phê duyệt hủy hợp lệ trên trang quản trị CMS.
