# 2. Đặt vấn đề và thực trạng nghiệp vụ

Tài liệu này trình bày chi tiết về thực trạng hoạt động kinh doanh, quy trình nghiệp vụ hiện tại, các hạn chế còn tồn đọng và lý do cần thiết phải xây dựng hệ thống bán hoa tươi trực tuyến PDA FlowerShop.

## 2.1. Thực trạng hoạt động kinh doanh hiện tại

Cửa hàng kinh doanh hoa tươi PDA FLOWER trước đây hoạt động chủ yếu theo mô hình bán lẻ truyền thống trực tiếp tại cửa hàng và kết hợp nhận đơn hàng trực tuyến qua các kênh mạng xã hội như Facebook, Zalo hoặc nhận cuộc gọi trực tiếp từ khách hàng. Quy mô kinh doanh ngày càng mở rộng kéo theo lượng đơn hàng tăng lên, dẫn đến việc quản lý thủ công bộc lộ nhiều điểm yếu nghiêm trọng trong khâu vận hành.

## 2.2. Quy trình nghiệp vụ hiện tại

Quy trình tiếp nhận và xử lý đơn hàng trực tuyến hiện tại được thực hiện thủ công qua các bước sau.

* Khách hàng tham khảo các mẫu hoa qua album ảnh trên Fanpage hoặc Zalo, sau đó nhắn tin hoặc gọi điện trực tiếp để hỏi giá và thỏa thuận.
* Nhân viên cửa hàng tiếp nhận thông tin đơn hàng, ghi chép thủ công vào sổ ghi chép hoặc nhập vào file Excel bao gồm tên khách hàng, số điện thoại, địa chỉ giao hoa, thời gian giao và lời nhắn ghi trên thiệp.
* Nhân viên chuẩn bị hoa dựa trên số lượng tồn kho nguyên liệu ước tính bằng mắt thường tại cửa hàng.
* Nhân viên liên hệ đơn vị vận chuyển bên ngoài (Shipper) để giao hoa và thu hộ tiền mặt (COD).
* Cuối ngày, chủ cửa hàng đối chiếu tiền mặt nhận được từ shipper với sổ ghi chép để kiểm tra doanh thu.

## 2.3. Các hạn chế của quy trình hiện tại

Quy trình vận hành thủ công gây ra nhiều tổn thất về chi phí, nhân lực và ảnh hưởng xấu đến uy tín thương hiệu của cửa hàng.

* **Tồn kho nguyên liệu không đồng bộ:** Do không quản lý lượng tồn kho hoa tươi và phụ kiện thời gian thực, cửa hàng thường gặp tình trạng nhận đơn đặt hàng của khách nhưng khi cắm hoa mới phát hiện thiếu nguyên liệu. Điều này buộc nhân viên phải gọi điện xin lỗi khách hàng để đổi mẫu hoa hoặc hủy đơn, làm giảm mức độ hài lòng của khách hàng.
* **Sai lệch thông tin giao nhận:** Việc ghi chép thủ công thông tin người nhận, địa chỉ và thời gian giao hoa rất dễ xảy ra sai sót. Đối với mặt hàng hoa tươi phục vụ sự kiện, sinh nhật, khai trương hoặc lễ kỷ niệm, việc giao hoa chậm trễ hoặc sai địa chỉ sẽ làm hỏng hoàn toàn ý nghĩa món quà của khách hàng.
* **Rủi ro bùng đơn hàng COD cao:** Hoa tươi là mặt hàng đặc thù có tính chất nhanh hỏng, thời gian bảo quản ngắn và chi phí thiết kế cao. Khi khách hàng đặt đơn hàng thu hộ (COD) qua điện thoại hoặc tin nhắn ảo rồi từ chối nhận hoa khi shipper giao tới, cửa hàng phải gánh chịu toàn bộ chi phí nguyên liệu và vận chuyển do sản phẩm hoa đã cắm không thể tái sử dụng hoặc hoàn trả kho.
* **Khó khăn trong thống kê báo cáo:** Việc tổng hợp dữ liệu doanh thu, lợi nhuận, thống kê mặt hàng bán chạy và quản lý hiệu quả làm việc của nhân viên từ các nguồn ghi chép rời rạc mất rất nhiều thời gian và dễ xảy ra nhầm lẫn.

## 2.4. Sự cần thiết của việc xây dựng hệ thống bán hoa tươi trực tuyến

Để khắc phục triệt để các hạn chế nêu trên, việc phát triển một hệ thống bán hoa trực tuyến đồng bộ (PDA FlowerShop) là vô cùng cấp thiết nhằm đạt được các mục tiêu vận hành chuyên nghiệp.

* **Đồng bộ hóa dữ liệu tồn kho:** Hệ thống tự động cập nhật và kiểm tra số lượng tồn kho của từng mẫu hoa trước khi khách hàng tiến hành thanh toán, ngăn ngừa tình trạng quá tải đơn hàng vượt quá năng lực đáp ứng nguyên liệu.
* **Tự động hóa quản lý khung giờ giao hàng (Delivery Slots):** Hệ thống giới hạn công suất chuẩn bị và giao hoa trong từng khung giờ cụ thể, giúp bộ phận cắm hoa và vận chuyển chủ động điều phối công việc, bảo đảm hoa được giao đúng giờ và tươi mới nhất.
* **Giảm thiểu tỷ lệ bùng đơn hàng bằng công nghệ:** Tích hợp cổng thanh toán trực tuyến quốc gia VNPay giúp khuyến khích khách hàng thanh toán trước. Đồng thời, hệ thống áp dụng bộ lọc đánh giá mức độ tin cậy của khách hàng (Fraud Score), quản lý danh sách đen số điện thoại từng bùng đơn (Phone Blacklist) và gửi mã xác thực OTP qua email để kiểm soát nghiêm ngặt các đơn hàng COD có dấu hiệu nghi ngờ.
* **Quản trị khoa học:** Cung cấp giao diện Dashboard quản trị trực quan với các chỉ số báo cáo doanh thu, sản phẩm bán chạy, quản lý bài viết giới thiệu ý nghĩa hoa, giúp nâng cao năng lực tiếp thị và đưa ra quyết định kinh doanh chính xác.
