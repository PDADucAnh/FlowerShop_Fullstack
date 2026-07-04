# Tài liệu Đặc tả Thiết kế: Nâng cấp Giao diện Chi tiết Sản phẩm (Product Details)

Tài liệu này đặc tả luồng nâng cấp giao diện trang chi tiết sản phẩm theo phong cách thiết kế sang trọng, hiện đại từ mẫu thiết kế `docs/design/productdetails.md`.

---

## 1. Mục tiêu nâng cấp

Nâng cấp trang chi tiết sản phẩm hiện tại từ giao diện cơ bản lên chuẩn giao diện cao cấp:
1. **Thiết lập cột kép (Grid 12 cột)**: 7 cột bên trái cho thư viện ảnh, 5 cột bên phải cho thông tin chi tiết cố định (Sticky).
2. **Thư viện ảnh tương tác (Interactive Gallery)**:
   - Hiển thị ảnh lớn chính (hỗ trợ chuyển đổi khi bấm thumbnail).
   - Dàn hàng 4 thumbnail gồm: Ảnh gốc sản phẩm, ảnh chụp cận cảnh chi tiết cánh hoa, ảnh góc chụp flat-lay từ trên xuống, và ảnh bố trí trong không gian phòng khách thực tế (lifestyle).
3. **Bộ chọn cấu hình kích thước (Select Size)**:
   - Cho phép chọn 3 cỡ: Classic (giá gốc), Deluxe (Cộng thêm giá), Grand (Cộng thêm giá).
   - Tự động phản ánh giá tăng thêm lên UI và truyền dữ liệu tùy biến vào giỏ hàng.
4. **Khối accordion chi tiết (Floral Care & Delivery)**:
   - Sử dụng thẻ `<details>` và `<summary>` CSS chuẩn để đóng mở thông tin Chăm sóc hoa & Giao hàng.
5. **Gợi ý sản phẩm liên quan (You May Also Love)**:
   - Hiển thị danh sách 4 sản phẩm cùng danh mục từ Backend một cách động.
   - Thêm hiệu ứng hover phóng to ảnh nhẹ và đổ bóng cao cấp.

---

## 2. Đặc tả thay đổi giao diện (Tailwind CSS)

### 2.1. Thư viện ảnh tương tác
```tsx
const [activeImage, setActiveImage] = useState(product.imageUrl);

const galleryImages = [
  product.imageUrl,
  "https://lh3.googleusercontent.com/aida-public/AB6AXuB9CddqN2JsuVI2rYrLjfhM9YJEFtmNp6f_UaBbD8XuKJ2qE5FLElGt1sSIizcpFIBWITclUw4cq9Zhpzs1vGtTEpSTzy6UOcn8Uf3146Ih0LPSnin2xvbSLqAc08l1_MwIKWQmPF5wXwQrMQBKupE_0bN9EZ4UW86h9zRflczjzRqvbIbsUFzIELmDwiL61nlxefYQguY7IW2PnRp72LshlZWLRnxebPiOJ0fpdgIhYGXjGLuVtDt4aPlSGy5hNcdAWNjn4O8NxTs",
  "https://lh3.googleusercontent.com/aida-public/AB6AXuARzZ2qpRYpsU19whOUnfBzCxrqw3zwzDMv9zTU5J2TAmeacj0BaWjrlo-IrlQjJBljoQbGoSoIzF21u9dh3bo3b2jHjQy2W8jd31qKC3K7kd0UPuDp3iUuEIUkfpGc1e_GJKTgrk9ZcdqGWOgIDg80Ulq6XjEjJwfpZxB9zidUfXHEwrxinQBnbjR5Cly7HOst4MdMJ_fdSnZ4arKWILEdaahJabulvl9C0Ro1R3yq3W49q7veNH8L0L6P1YrTeExOXmoSnGQXuus",
  "https://lh3.googleusercontent.com/aida-public/AB6AXuA8_xORpxwk86bL3I4VJlBsIa21JwThIYnfT_IvKL09XAhTuhrJHtHVTYz3lv02Uj2dU8HpKpZNGGsh4ULoq3Sf9ZZoFSqsoCcjVd8PdjIl8hPNpARjUw9RlqbsPb4b-tUKUfv8kb8ZMBB-QxF2PzIgAvpINuwnbQKOTvnXgZioxC-lZ1b13_z8DIbBBKuD7TGNLS6RPFGP5zE5X9SyhVB7zS_3FSH5utYQwqGx-gBglQ-m9DBJBpbcNJ_mvsxXQFVdTzgZjW--ZVk"
];
```

### 2.2. Xử lý logic chọn kích thước (Size)
- **Classic**: Giữ nguyên giá.
- **Deluxe**: Cộng thêm 300,000 VND.
- **Grand**: Cộng thêm 600,000 VND.
Khi thêm vào giỏ hàng, thông tin kích cỡ sẽ được gộp vào tên sản phẩm để hiển thị rõ ràng trong giỏ và trang thanh toán.

### 2.3. Sản phẩm liên quan (Related Products)
Gọi hook `useProductsPaged` với tham số `categoryProductId` từ sản phẩm hiện tại để lấy 4 sản phẩm cùng loại.
 Giao diện hiển thị sản phẩm liên quan sẽ áp dụng thẻ thiết kế đồng bộ với trang cửa hàng chính.
