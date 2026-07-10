# 3. Mục tiêu của hệ thống

Tài liệu này trình bày các mục tiêu phát triển của hệ thống PDA FlowerShop bao gồm mục tiêu kinh doanh, mục tiêu kỹ thuật và định hướng mở rộng trong tương lai.

## 3.1. Mục tiêu kinh doanh

Hệ thống được thiết kế nhằm hỗ trợ cửa hàng hoa tươi chuyển đổi số toàn diện, nâng cao hiệu quả vận hành và mở rộng thị trường tiêu thụ.

* **Mở rộng kênh bán hàng trực tuyến:** Tiếp cận tập khách hàng thế hệ mới năng động, có thói quen mua sắm trực tuyến và tặng quà từ xa.
* **Tối ưu hóa quy trình tiếp nhận đơn hàng:** Giảm thiểu thời gian trao đổi, thương lượng thủ công giữa nhân viên và khách hàng nhờ hệ thống hiển thị thông tin sản phẩm đầy đủ, giá cả minh bạch và giỏ hàng tự động.
* **Kiểm soát rủi ro tài chính:** Hạ thấp tỷ lệ bùng đơn hàng COD bằng cách khuyến khích thanh toán trước qua cổng VNPay và áp dụng các biện pháp xác thực nghiêm ngặt.
* **Xây dựng thương hiệu chuyên nghiệp:** Cung cấp trải nghiệm mua sắm mượt mà, dịch vụ giao hàng đúng hẹn và chăm sóc khách hàng tự động qua email xác nhận chuyên nghiệp.
* **Nâng cao năng lực quản lý kinh doanh:** Giúp chủ cửa hàng nắm bắt chính xác xu hướng tiêu dùng, biến động doanh thu và hiệu quả tồn kho thông qua báo cáo số liệu cụ thể.

## 3.2. Mục tiêu kỹ thuật

Về mặt công nghệ, hệ thống phải đảm bảo các tiêu chí về hiệu năng, bảo mật, tính toàn vẹn dữ liệu và khả năng chịu tải.

* **Ứng dụng mô hình kiến trúc hiện đại:** Sử dụng kiến trúc tách biệt hoàn toàn giữa Frontend (React SPA) và Backend (ASP.NET Core Web API) để tăng khả năng bảo trì, nâng cấp độc lập và nâng cao hiệu năng tải trang.
* **Đồng bộ hóa dữ liệu tồn kho thời gian thực:** Áp dụng giải pháp giữ chỗ tồn kho tạm thời (Stock Lock Service) thông qua bộ nhớ đệm Memory Cache để tránh lỗi overselling khi có nhiều khách hàng cùng thanh toán một sản phẩm tại cùng một thời điểm.
* **Tích hợp thanh toán trực tuyến an toàn:** Triển khai tích hợp thành công cổng thanh toán VNPay Sandbox với thuật toán mã hóa chữ ký số bảo mật, đảm bảo tính chính xác và an toàn của các giao dịch tài chính.
* **Cơ chế bảo mật đa lớp:**
    * Bảo vệ giao diện API bằng JWT Bearer Token kết hợp cơ chế Refresh Token.
    * Sử dụng Cookie Authentication bảo vệ các trang quản trị CMS.
    * Áp dụng mã hóa mật khẩu một chiều bằng thuật toán băm (Password Hasher) theo tiêu chuẩn bảo mật của ASP.NET Core Identity.
    * Tích hợp chống tấn công giả mạo yêu cầu chéo trang (CSRF) bằng Antiforgery Token và ngăn chặn mã độc thực thi (XSS) qua DOMPurify.
    * Thiết lập giới hạn tần suất yêu cầu (Rate Limiting) trên các API nhạy cảm để tránh tấn công từ chối dịch vụ.
* **Tương tác thời gian thực:** Sử dụng thư viện SignalR để đồng bộ hóa và phát thông báo tức thời đến giao diện quản trị khi có đơn hàng mới hoặc sản phẩm thay đổi.

## 3.3. Định hướng mở rộng tương lai

Hệ thống được thiết kế với tính module cao, sẵn sàng cho các nâng cấp nâng cao trong các giai đoạn tiếp theo.

* **Đa dạng hóa phương thức thanh toán:** Tích hợp thêm các ví điện tử phổ biến như MoMo, ZaloPay, ShopeePay hoặc các cổng thanh toán quốc tế như Stripe và PayPal phục vụ khách kiều bào.
* **Tự động hóa logistics:** Kết nối API trực tiếp với các đơn vị giao hàng công nghệ như Ahamove, GrabExpress, Lalamove để tự động đẩy đơn vận chuyển và cập nhật trạng thái giao hàng tức thời.
* **Hệ thống đề xuất thông minh:** Ứng dụng trí tuệ nhân tạo (AI) để gợi ý các mẫu hoa phù hợp dựa trên dịp lễ, sở thích và lịch sử mua sắm trước đó của khách hàng.
* **Triển khai hệ thống khách hàng thân thiết:** Tích hợp tính năng tích lũy điểm thưởng (Loyalty Points), phân hạng thành viên (Vàng, Bạc, Đồng) và phát hành mã giảm giá tự động nhân dịp sinh nhật khách hàng.
