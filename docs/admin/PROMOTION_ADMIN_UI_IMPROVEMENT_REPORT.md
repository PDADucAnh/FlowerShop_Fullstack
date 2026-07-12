# Báo Cáo Cải Tiến Giao Diện Quản Trị Khuyến Mãi (Admin Promotion UI Improvement)

Báo cáo chi tiết về việc nâng cấp giao diện, nâng cao trải nghiệm người dùng (UX), bổ sung ràng buộc kiểm lỗi (Validation) và cảnh báo xung đột nghiệp vụ cho module **Quản lý Khuyến mãi (Promotion Management)** trong trang quản trị Admin của dự án FlowerShop.

---

## 1. Các tập tin đã chỉnh sửa (Modified Files)

- [Create.cshtml](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Views/Promotion/Create.cshtml): Tái cấu trúc giao diện thêm mới chương trình khuyến mãi.
- [Edit.cshtml](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Views/Promotion/Edit.cshtml): Tái cấu trúc giao diện cập nhật chương trình khuyến mãi.

*Lưu ý: Không thay đổi bất kỳ tập tin mã nguồn C# (.cs), Controller, Route hay Schema cơ sở dữ liệu nào.*

---

## 2. Những thay đổi đã thực hiện (Implemented Changes)

### A. Thay thế ô nhập ID sản phẩm thủ công bằng Bộ chọn thông minh (UX)
- **Trước cải tiến**: Người dùng phải tự nhập ID sản phẩm cách nhau bởi dấu phẩy (`1, 2, 3`), rất dễ xảy ra sai sót và không có thông tin sản phẩm trực quan.
- **Sau cải tiến**:
  - Tích hợp ô tìm kiếm tự động hoàn thành (Search Autocomplete) liên kết thời gian thực với API `/api/Products`.
  - Hiển thị danh sách kết quả trực quan gồm: Ảnh thumbnail sản phẩm, Tên sản phẩm, mã SKU, và Đơn giá (định dạng VND).
  - Danh sách các sản phẩm đã chọn được trình bày riêng biệt kèm ảnh, tên, SKU, giá và nút "Xóa" trực quan.
  - Đồng bộ danh sách ID sản phẩm được chọn sang input ẩn `productIdsCsv` để gửi về Backend xử lý đúng như cơ chế cũ.

### B. Xem trước ảnh Banner (Preview Banner)
- Tích hợp thẻ hiển thị xem trước ảnh Banner ngay bên dưới ô nhập đường dẫn ảnh.
- Khi người dùng nhập/thay đổi URL ảnh hoặc đường dẫn nội bộ (ví dụ: `/uploads/banners/promo.jpg`), JavaScript sẽ cập nhật hiển thị xem trước tức thời.
- Nếu đường dẫn trống hoặc tải ảnh lỗi (URL không hợp lệ), hệ thống tự động hiển thị ảnh SVG Placeholder đẹp mắt để tránh lỗi giao diện.

### C. Bổ sung trường Trạng thái (Status Mapping)
- Tích hợp trường chọn **Trạng thái (Status)** thay cho checkbox đơn điệu:
  - **Draft (Bản nháp)**: Ánh xạ tương ứng với `IsActive = false`.
  - **Active (Kích hoạt)**: Ánh xạ tương ứng với `IsActive = true`.
- Hoàn toàn tương thích và lưu trữ trực tiếp vào cột `IsActive` hiện có trong database mà không cần chỉnh sửa schema.

### D. Disable nút bấm khi Submit (Prevent Double Submit)
- Khi người dùng nhấn nút "Tạo khuyến mãi" hoặc "Cập nhật", nút bấm sẽ bị vô hiệu hóa (`disabled`) và hiển thị hiệu ứng xoay tròn loading kèm text "Đang xử lý...".
- Ngăn chặn triệt để hành vi double-submit (gửi yêu cầu nhiều lần) gây trùng lặp dữ liệu hoặc quá tải hệ thống.

### E. Hiển thị Validation trực quan ngay dưới từng trường dữ liệu
- Chuyển đổi hiển thị thông báo lỗi chung thành các thông báo đỏ dưới từng trường dữ liệu cụ thể nhờ tích hợp và mở rộng `jQuery.validate` và `jQuery.validate.unobtrusive`.
- Bổ sung các quy tắc kiểm tra nâng cao ở phía Client:
  - **Tên chiến dịch**: Bắt buộc nhập.
  - **Giá trị giảm**: Bắt buộc nhập. Nếu chọn loại giảm giá là "Phần trăm (%)", hệ thống kiểm tra và cảnh báo nếu giá trị vượt quá `100%`.
  - **Ngày kết thúc**: Phải lớn hơn ngày bắt đầu (`EndDate > StartDate`). Cảnh báo ngay lập tức nếu chọn thời gian không hợp lệ.
  - **Đường dẫn Banner**: Phải đúng định dạng bắt đầu bằng `/` (đường dẫn nội bộ) hoặc `http://` / `https://` / `data:image/` (đường dẫn tuyệt đối/base64).

### F. Chú thích trường Độ ưu tiên (Priority Help Text)
- Bổ sung dòng chú thích (Help Text) nhỏ, tinh tế dưới trường nhập Độ ưu tiên: *"Giá trị càng lớn thì chiến dịch càng được ưu tiên khi nhiều chương trình cùng áp dụng."*.
- Giúp quản trị viên dễ dàng hiểu cơ chế cộng dồn khuyến mãi mà không cần xem tài liệu hướng dẫn.

### G. Cảnh báo xung đột Flash Sale (Conflict Warning)
- Khi trang quản lý được tải, hệ thống gọi API `/api/FlashSales/active` để lấy danh sách các sản phẩm đang nằm trong các đợt Flash Sale đang chạy.
- Khi người dùng chọn thêm sản phẩm vào chương trình khuyến mãi hiện tại, JavaScript sẽ tự động kiểm tra chéo:
  - Nếu sản phẩm đã nằm trong một Flash Sale khác, hệ thống sẽ hiển thị một khối Alert Warning màu vàng hổ phách rất đẹp ở bên dưới: *"Cảnh báo: Sản phẩm [Tên sản phẩm] đang thuộc chương trình Flash Sale [Tên Flash Sale] (Thời gian kết thúc: [Thời gian])."*.
  - Cảnh báo này chỉ có mục đích thông báo cho Admin biết để cân nhắc, **không chặn lưu** đơn hàng/khuyến mãi đúng theo yêu cầu.

---

## 3. Những thay đổi chưa thực hiện và lý do (Unimplemented Changes)

Các hạng mục dưới đây được chuẩn bị giao diện hoặc để nhãn đánh dấu (TODO) định hướng phát triển, chưa triển khai logic backend vì nằm ngoài phạm vi yêu cầu chỉ sửa đổi giao diện MVC/Razor và giữ nguyên cấu trúc Backend hiện tại:

1. **Áp dụng khuyến mãi theo danh mục**:
   - Giao diện được bổ sung nút chọn loại áp dụng (Radio: "Theo sản phẩm" / "Theo danh mục"). Nút chọn "Theo danh mục" được để ở trạng thái vô hiệu hóa kèm nhãn `TODO` (Tính năng đang phát triển).
   - *Lý do*: Backend hiện tại (`CreatePromotionCampaignDTO` và cơ sở dữ liệu bảng `PromotionProducts`...) mới chỉ có quan hệ liên kết trực tiếp giữa Chiến dịch với từng `ProductId` cụ thể thông qua bảng `PromotionProducts`. Để áp dụng theo danh mục cần thay đổi cơ chế tính giá khuyến mãi ở Backend (lấy danh sách sản phẩm thuộc Category khi tính tiền đơn hàng), việc này sẽ phá vỡ business logic hiện có nếu chỉ chỉnh sửa giao diện.
2. **Mức giảm tối đa (Maximum Discount)**:
   - Giao diện hiển thị trường nhập "Mức giảm tối đa" ở trạng thái disabled kèm nhãn `TODO`.
   - *Lý do*: Thực thể chiến dịch khuyến mãi `PromotionCampaign` hiện chưa có thuộc tính lưu trữ mức giảm tối đa (trường này mới chỉ hỗ trợ ở thực thể Coupon). Cần bổ sung cột trong database và cập nhật API tính tiền đơn hàng ở Backend mới có thể chạy chính thức.

---

## 4. Ảnh hưởng tới hệ thống (System Impact)

- **Biên dịch dự án**: Dự án biên dịch (Build) thành công 100% không phát sinh bất kỳ lỗi cú pháp C# hay lỗi phân tích cú pháp Razor View nào.
- **Hiệu năng Client**: Tối ưu hóa việc tìm kiếm sản phẩm. Client-side chỉ cần fetch danh sách sản phẩm một lần khi tải trang và thực hiện tìm kiếm lọc tức thì trên RAM ở trình duyệt, giảm tải tối đa số lượng HTTP request gửi lên server khi người dùng gõ phím tìm kiếm.
- **Tải trọng Backend**: Giảm thiểu các lỗi dữ liệu không hợp lệ gửi lên server nhờ cơ chế Validate chặt chẽ từ phía Client trước khi submit.

---

## 5. Khả năng tương thích (Backward Compatibility)

- **Tương thích 100%**:
  - Giao diện sau khi người dùng chọn sản phẩm vẫn tự động phân tách mảng ID thành chuỗi ngăn cách bằng dấu phẩy và gán vào thẻ `<input name="productIdsCsv" />`. Cơ chế này đảm bảo Model Binder của ASP.NET Core MVC nhận diện đúng tham số `productIdsCsv` như ban đầu và xử lý lưu vào DB y hệt.
  - Không làm thay đổi bất kỳ API nào, do đó không gây ra bất cứ ảnh hưởng nào tới ứng dụng Frontend chạy bằng Next.js.

---

## 6. Đề xuất hướng phát triển tiếp theo (Future Development Recommendations)

1. **Nâng cấp Entity Framework & Database**: Bổ sung cột `MaximumDiscountAmount` vào thực thể `PromotionCampaign` để đưa tính năng giới hạn giảm tối đa vào hoạt động thực tế.
2. **Hỗ trợ áp dụng khuyến mãi theo danh mục**: Thiết lập bảng liên kết `PromotionCategories` và cập nhật hàm tính giá sản phẩm khuyến mãi trong `PromotionService` để tự động áp dụng mức giảm cho mọi sản phẩm thuộc danh mục được chọn.
3. **Phân trang danh sách sản phẩm tìm kiếm**: Nếu số lượng sản phẩm trong hệ thống tăng lên hàng chục nghìn, nên chuyển đổi từ việc tải toàn bộ sản phẩm trên RAM sang gọi API tìm kiếm có phân trang `/api/products/paged` kèm tham số lọc từ khóa để tối ưu bộ nhớ trình duyệt.
