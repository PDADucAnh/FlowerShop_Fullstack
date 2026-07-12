# Database Review & Evaluation Report (FlowerShop)

Tài liệu này cung cấp báo cáo đánh giá toàn diện về thiết kế cơ sở dữ liệu hiện tại của hệ thống FlowerShop dựa trên các tiêu chí chuẩn hóa, tối ưu hiệu năng, tính toàn vẹn dữ liệu và đề xuất các phương án cải thiện chi tiết.

---

## 1. Chuẩn hóa Database (Database Normalization)

Hệ thống cơ sở dữ liệu của FlowerShop được thiết kế tương đối tốt, hầu hết các bảng đạt tiêu chuẩn **3NF (Third Normal Form)**. Tuy nhiên, có một số điểm thiết kế phi chuẩn hóa (Denormalization) có chủ đích và không chủ đích:

- **Phi chuẩn hóa có chủ đích (Hợp lý)**:
  - Lưu thông tin Snapshot (`ProductName`, `ProductImage`) trực tiếp trong bảng `OrderDetails`. Điều này hoàn toàn hợp lý trong nghiệp vụ E-commerce để bảo toàn dữ liệu lịch sử mua hàng khi thông tin sản phẩm thay đổi hoặc bị xóa trong tương lai.
  - Lưu thông tin địa chỉ giao hàng Snapshot (`DeliveryReceiverName`, `DeliveryReceiverPhone`, `DeliveryAddressLine`, v.v.) trong bảng `Orders` thay vì chỉ tham chiếu khóa ngoại tới bảng `CustomerAddresses`. Điều này giúp lưu giữ chính xác địa chỉ tại thời điểm giao hàng.
- **Phi chuẩn hóa không chủ đích (Cần khắc phục)**:
  - Tồn tại song song thông tin địa chỉ cũ (`Orders.DeliveryAddress`) và các trường địa chỉ chi tiết mới được bổ sung sau này trong bảng `Orders`.
  - Tồn tại cột `Customer.Address` cũ trong khi hệ thống đã chuyển sang quản lý nhiều địa chỉ qua bảng `CustomerAddresses`.

---

## 2. Thiếu bảng (Missing Tables)

Hệ thống đang thiếu một số bảng quan trọng để đảm bảo tính mở rộng và khả năng giám sát:

1. **Bảng Lịch sử Kho hàng (Inventory Transactions / Logs)**:
   - Hiện tại, số lượng tồn kho `Products.StockQuantity` được cập nhật trực tiếp (cộng/trừ). Hệ thống thiếu bảng `InventoryLogs` để theo dõi nguồn gốc thay đổi kho (Nhập hàng, Khách mua, Hủy đơn hoàn kho, Hao hụt...), gây khó khăn cho việc kiểm toán.
2. **Bảng Đánh giá & Phản hồi (Product Reviews / Ratings)**:
   - Là một website bán hoa, việc thiếu bảng lưu trữ đánh giá sản phẩm từ khách hàng (`ProductReviews` / `ProductFeedback`) làm hạn chế tính tương tác và uy tín của sản phẩm.
3. **Bảng Nhật ký Hệ thống (Audit Logs / Admin Action Logs)**:
   - Thiếu bảng ghi nhận hành vết tác động của nhân viên quản trị (Ai đã thay đổi giá sản phẩm, ai đã blacklist số điện thoại, ai đã sửa đổi chương trình khuyến mãi và thời gian cụ thể).
4. **Bảng Giỏ hàng Cơ sở dữ liệu (Database-backed Shopping Cart)**:
   - Hiện hệ thống chưa lưu giỏ hàng của người dùng dưới cơ sở dữ liệu (`Carts` & `CartItems`). Nếu người dùng đổi thiết bị hoặc xóa cache trình duyệt, giỏ hàng sẽ bị mất.

---

## 3. Khóa ngoại thiếu (Missing Foreign Keys)

Có một số mối quan hệ thực tế tồn tại trong logic code nhưng cơ sở dữ liệu chưa được cấu hình ràng buộc khóa ngoại (Foreign Key Constraints), dẫn tới nguy cơ mất toàn vẹn tham chiếu:

1. **Orders -> DeliverySlots**:
   - Trường `Orders.DeliverySlotId` (`int?`) tồn tại để xác định ca giao hàng, nhưng không có ràng buộc khóa ngoại trỏ tới `DeliverySlots(Id)`. Điều này có thể dẫn đến việc đơn hàng tham chiếu tới một ca giao hàng không tồn tại hoặc đã bị xóa.
2. **Customers -> CustomerAddresses**:
   - Trường `Customers.DefaultAddressId` (`int?`) lưu địa chỉ mặc định của khách hàng nhưng không có FK trỏ sang bảng `CustomerAddresses(Id)`. Nếu địa chỉ đó bị xóa, trường này sẽ trỏ vào một bản ghi rỗng (Orphan ID).
3. **EmailHistories -> Orders**:
   - Bảng `EmailHistories` có trường `OrderId` (`int?`) và có đánh chỉ mục, nhưng thiếu ràng buộc khóa ngoại trỏ sang `Orders(Id)`.
4. **Notifications -> Orders**:
   - Tương tự, bảng `Notifications` chứa trường `OrderId` (`int?`) để liên kết thông báo với đơn hàng cụ thể, nhưng thiếu ràng buộc khóa ngoại trỏ sang `Orders(Id)`.

---

## 4. Chỉ mục thiếu (Missing Indexes)

Việc thiếu chỉ mục trên các trường tìm kiếm và lọc thường xuyên sẽ gây ra tình trạng quét toàn bộ bảng (Table Scan), làm suy giảm nghiêm trọng tốc độ phản hồi khi dữ liệu lớn lên:

1. **Các trường URL Slug**:
   - Các trường `Slug` trong các bảng `Products`, `Categories`, `CategoriesProducts`, và `Posts` đều được dùng để định tuyến URL thân thiện cho SEO. Hệ thống thiếu hoàn toàn chỉ mục trên các trường này (ngoại trừ `Products.Price` bao gồm `Slug` trong phần Include, không thể dùng để tìm kiếm trực tiếp bằng Slug).
   - *Hậu quả*: Mỗi lần khách hàng click xem chi tiết sản phẩm hoặc danh mục, SQL Server phải thực hiện quét toàn bộ bảng để tìm theo Slug.
2. **Username của Admin (`Users.Username`)**:
   - Trường đăng nhập chính của hệ thống quản trị quản lý bằng `Users` không được đánh chỉ mục Unique hay chỉ mục thường.
3. **Mã đơn hàng và mã giao dịch**:
   - `Orders.PaymentTransactionId` và `Payments.TransactionId` cần được đánh chỉ mục để tối ưu hóa việc đối soát thanh toán trực tuyến từ Webhook của đối tác (VNPAY/MOMO).
4. **Tìm kiếm sản phẩm theo tên (`Products.Name`)**:
   - Cần bổ sung chỉ mục thường (hoặc Full-Text Index) trên `Products.Name` phục vụ tính năng tìm kiếm sản phẩm trên thanh tìm kiếm của khách hàng.

---

## 5. Cấu hình Nullable chưa hợp lý

1. **Thông tin người nhận trong Đơn hàng (`Orders.RecipientName`, `Orders.RecipientPhone`)**:
   - Hiện tại đang cấu hình cho phép `NULL`. Thực tế nghiệp vụ giao hoa yêu cầu bắt buộc phải có thông tin người nhận để shipper liên hệ giao hàng. Nên đổi thành `NOT NULL` (hoặc bắt buộc thông qua validation chặt chẽ).
2. **Thời gian chạy Coupon (`Coupons.StartDate`, `Coupons.EndDate`)**:
   - Cho phép `NULL`, nghĩa là một Coupon có thể có hiệu lực vĩnh viễn không giới hạn thời gian. Điều này tiềm ẩn rủi ro thất thoát doanh thu nếu quên thu hồi Coupon cũ.

---

## 6. Kiểu dữ liệu chưa hợp lý (Data Types)

1. **Lạm dụng kiểu dữ liệu kích thước lớn (`nvarchar(max)`)**:
   - `Customer.FullName` và `Customer.Address` sử dụng `nvarchar(max)`. Tên khách hàng chỉ cần tối đa `nvarchar(200)` và địa chỉ tối đa `nvarchar(500)`. Sử dụng `nvarchar(max)` làm giảm hiệu năng lưu trữ trang (Page allocation) của SQL Server.
   - `Customer.Password` (lưu PasswordHash) sử dụng `nvarchar(max)`. Chuỗi băm mật khẩu luôn có độ dài cố định hoặc giới hạn nhỏ (thường từ 60 đến 128 ký tự). Nên giới hạn lại thành `nvarchar(256)`.
2. **Lãng phí bộ nhớ trên các trường số nguyên**:
   - `OrderDetails.Quantity` (Số lượng mua của 1 bông hoa/bó hoa trong đơn hàng) dùng kiểu `int` (4 bytes). Kiểu `smallint` (2 bytes - hỗ trợ đến 32,767) là quá đủ.
   - `DeliverySlot.MaxCapacity` và `CurrentBooked` dùng kiểu `int`. Số lượng giao tối đa một ca khó vượt quá vài chục đơn. Nên tối ưu thành `tinyint` (1 byte - lên tới 255) hoặc `smallint`.
   - `FlashSaleProduct.DiscountPercent` dùng kiểu `decimal(18,2)`. Phần trăm chiết khấu Flash Sale nên dùng kiểu `int` (ví dụ: 10%, 20%) hoặc `decimal(5,2)`.

---

## 7. Cột dư thừa (Redundant Columns)

1. **Orders.DeliveryAddress**:
   - Cột này là dư thừa và trùng lặp thông tin với tổ hợp cột địa chỉ chi tiết mới được thêm ở Migration 009 (`DeliveryAddressLine`, `DeliveryProvince`, `DeliveryDistrict`, `DeliveryWard`).
2. **Customer.Address**:
   - Trở nên dư thừa sau khi bảng `CustomerAddresses` được đưa vào sử dụng.
3. **Payments.Method vs Payments.PaymentMethodId**:
   - Cột `Method` lưu giá trị Enum (`0: OnlinePayment`, `1: COD`) nằm ngay cạnh cột liên kết ngoại `PaymentMethodId` trỏ tới bảng `PaymentMethods`. Việc duy trì cả hai cột này dễ gây ra xung đột logic dữ liệu thanh toán.

---

## 8. Vi phạm chuẩn hóa cơ sở dữ liệu

- **Shadow Key redundant của Entity Framework Core (`ProductVariants.ProductId1`)**:
  - Do cấu hình mối quan hệ trong Fluent API của `ApplicationDbContext.cs` bị thiếu tham số dẫn hướng ngược:
    `modelBuilder.Entity<ProductVariant>().HasOne(pv => pv.Product).WithMany().HasForeignKey(pv => pv.ProductId)`
    (Thiếu `.WithMany(p => p.ProductVariants)`)
  - Khiến EF Core tự hiểu là có 2 mối quan hệ khác nhau và tự động sinh ra cột Shadow Key `ProductId1` trong bảng `ProductVariants`. Đây là lỗi cấu hình Fluent API nghiêm trọng làm phát sinh dữ liệu rác trong DB.

---

## 9. Đề xuất cải thiện chi tiết (Actionable Recommendations)

### A. Sửa lỗi cấu hình EF Core & Xóa cột rác
Sửa cấu hình liên kết của `ProductVariant` trong `ApplicationDbContext.cs` để loại bỏ cột `ProductId1`:
```csharp
modelBuilder.Entity<ProductVariant>()
    .HasOne(pv => pv.Product)
    .WithMany(p => p.ProductVariants) // Thêm thuộc tính điều hướng ngược
    .HasForeignKey(pv => pv.ProductId)
    .OnDelete(DeleteBehavior.Cascade);
```

### B. Bổ sung các ràng buộc Khóa ngoại (FK Constraints)
Tạo các Migration bổ sung khóa ngoại cho các trường tham chiếu tự do:
- `Orders(DeliverySlotId) -> DeliverySlots(Id)`
- `Customers(DefaultAddressId) -> CustomerAddresses(Id)`
- `EmailHistories(OrderId) -> Orders(Id)`
- `Notifications(OrderId) -> Orders(Id)`

### C. Bổ sung các Index quan trọng phục vụ tìm kiếm và định tuyến SEO
```sql
-- Đánh chỉ mục Unique cho Username của Admin
CREATE UNIQUE INDEX IX_Users_Username ON Users(Username);

-- Đánh chỉ mục cho các trường Slug URL phục vụ SEO
CREATE INDEX IX_Products_Slug ON Products(Slug);
CREATE INDEX IX_Categories_Slug ON Categories(Slug);
CREATE INDEX IX_CategoriesProducts_Slug ON CategoriesProducts(Slug);
CREATE INDEX IX_Posts_Slug ON Posts(Slug);

-- Đánh chỉ mục hỗ trợ tra cứu lịch sử địa chỉ khách hàng
CREATE INDEX IX_CustomerAddresses_CustomerId ON CustomerAddresses(CustomerId);
```

### D. Tối ưu kiểu dữ liệu & Thu gọn dung lượng bảng
- Đổi kiểu dữ liệu của `Customer.FullName` từ `nvarchar(max)` thành `nvarchar(250)`.
- Đổi kiểu dữ liệu của `Customer.Password` từ `nvarchar(max)` thành `nvarchar(256)`.
- Thu nhỏ kiểu dữ liệu của các trường số lượng/dung lượng nhỏ từ `int` xuống `smallint`/`tinyint`.
