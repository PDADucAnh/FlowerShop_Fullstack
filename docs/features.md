# Danh Sách Tính Năng — FlowerShop

> Dự án thực tập tốt nghiệp — Hệ thống bán hoa online
> Công nghệ: ASP.NET Core 8.0 + React 19 + TypeScript + SQL Server

---

## I. Người Dùng (Khách Hàng)

### 1.1 Xác thực & Tài khoản
| Tính năng | Mô tả |
|-----------|-------|
| Đăng ký tài khoản | Đăng ký bằng email, mật khẩu |
| Đăng nhập | JWT Bearer token authentication |
| Quên mật khẩu | Gửi email reset password (token trong email body) |
| Đặt lại mật khẩu | Nhập token thủ công + mật khẩu mới |
| Cập nhật hồ sơ | Sửa tên, SĐT, địa chỉ |
| Đổi mật khẩu | Yêu cầu mật khẩu hiện tại |

### 1.2 Sản phẩm
| Tính năng | Mô tả |
|-----------|-------|
| Danh mục sản phẩm | Lọc theo danh mục, phân trang |
| Chi tiết sản phẩm | Hình ảnh, mô tả, giá, tồn kho |
| Tìm kiếm sản phẩm | Tìm kiếm theo tên, mô tả |
| Lọc giá | Khoảng giá từ-thấp-đến-cao |
| Sản phẩm nổi bật | Trending products dựa trên view + add-to-cart |

### 1.3 Giỏ hàng
| Tính năng | Mô tả |
|-----------|-------|
| Thêm vào giỏ | Kiểm tra tồn kho |
| Cập nhật số lượng | Tăng/giảm số lượng từng item |
| Xóa sản phẩm | Xóa khỏi giỏ hàng |
| Áp mã giảm giá | Tính toán giảm giá (base price) |
| Wishlist | Thêm/xóa sản phẩm yêu thích |
| Stock lock | Giữ chỗ tồn kho trong 15 phút khi vào checkout |

### 1.4 Đặt hàng
| Tính năng | Mô tả |
|-----------|-------|
| Checkout | Chọn giờ giao, địa chỉ, ghi chú |
| Thanh toán COD | Thanh toán khi nhận hàng |
| Thanh toán online | MoMo mock (sandbox) |
| OTP xác minh COD | Gửi OTP qua email, xác minh trước khi xác nhận |
| Xác nhận đơn hàng | Email xác nhận + cập nhật trạng thái |
| Hủy đơn hàng | Hủy trước khi giao (tự động hoặc thủ công) |
| Theo dõi đơn hàng | Xem lịch sử và trạng thái |
| Webhook thanh toán | Xử lý callback thanh toán (HMAC-SHA256 signature) |

### 1.5 Bài viết
| Tính năng | Mô tả |
|-----------|-------|
| Xem blog | Danh sách bài viết phân trang |
| Chi tiết bài viết | Nội dung HTML, SEO metadata |
| Bài viết mới nhất | Sidebar bài viết gần đây |

---

## II. Admin (Quản Trị)

### 2.1 Quản lý
| Module | Tính năng |
|--------|-----------|
| Sản phẩm | CRUD sản phẩm, quản lý tồn kho, slug, SKU |
| Danh mục | CRUD danh mục sản phẩm + bài viết |
| Đơn hàng | Xem, cập nhật trạng thái, hủy |
| Bài viết | CRUD bài viết, CKEditor soạn thảo |
| Khách hàng | Xem danh sách, blacklist, fraud score |
| Người dùng | Quản lý tài khoản admin |
| Quảng cáo | CRUD banner quảng cáo trang chủ |
| Phân phối | Quản lý khung giờ giao hàng |
| Danh sách đen | Quản lý SĐT trong blacklist |

### 2.2 Admin Panel MVC
- Giao diện Razor Pages với layout admin
- DataTables với sort/search/pagination
- CKEditor 5 cho soạn thảo nội dung
- Toast notifications

---

## III. Kiến Trúc Hệ Thống

### Backend
| Thành phần | Công nghệ |
|-----------|-----------|
| Framework | ASP.NET Core 8.0 (MVC + Web API) |
| Database | SQL Server LocalDB, EF Core 8 |
| Authentication | JWT Bearer + Cookie (hybrid) |
| Real-time | SignalR (notifications) |
| Auto-cancel | BackgroundService (OrderExpiry) |

### Frontend
| Thành phần | Công nghệ |
|-----------|-----------|
| Framework | React 19, TypeScript 6 |
| Build tool | Vite |
| Routing | react-router-dom v7 |
| Data fetching | TanStack Query (React Query) |
| State | React Context (Auth, Cart, Wishlist) |
| Validation | Zod schemas |
| SEO | react-helmet-async |
| UI | Custom CSS (không UI framework) |

### Testing
- xUnit + Moq + EF Core InMemory
- 37 tests (AuthService, UserService, PaymentService)
- Coverlet coverage collector

---

## IV. API Endpoints

### Public
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/products` | Danh sách sản phẩm |
| GET | `/api/products/{id}` | Chi tiết sản phẩm |
| GET | `/api/products/search` | Tìm kiếm |
| GET | `/api/products/trending` | Sản phẩm nổi bật |
| GET | `/api/categories` | Danh mục |
| GET | `/api/category-products` | Danh mục sản phẩm |
| GET | `/api/posts` | Bài viết |
| GET | `/api/posts/{slug}` | Chi tiết bài viết |

### Auth
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/auth/login` | Đăng nhập |
| POST | `/api/auth/register` | Đăng ký |
| POST | `/api/auth/forgot-password` | Quên mật khẩu |
| POST | `/api/auth/reset-password` | Đặt lại mật khẩu |
| PUT | `/api/auth/change-password` | Đổi mật khẩu |
| PUT | `/api/auth/profile` | Cập nhật hồ sơ |
| POST | `/api/auth/refresh` | Refresh token |
| POST | `/api/auth/logout` | Đăng xuất |

### Order (cần auth)
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/orders` | Tạo đơn hàng |
| GET | `/api/orders` | Danh sách đơn hàng |
| GET | `/api/orders/{id}` | Chi tiết đơn hàng |
| POST | `/api/orders/{id}/cancel` | Hủy đơn hàng |
| POST | `/api/orders/{id}/verify-otp` | Xác minh OTP |

### Payment
| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/payment/create-payment` | Tạo thanh toán MoMo |
| POST | `/api/payment/webhook` | Webhook thanh toán |
| POST | `/api/payment/send-otp` | Gửi OTP COD |
| POST | `/api/payment/refund` | Hoàn tiền |

---

## V. Bảo Mật

| Biện pháp | Mô tả |
|-----------|-------|
| JWT Bearer + refresh token | Access token ngắn hạn (60 phút) |
| Rate limiting | Giới hạn forgot-password 1 request/60s per email |
| Password hashing | ASP.NET Core Identity PasswordHasher |
| Webhook signature | HMAC-SHA256 với constant-time comparison |
| Input validation | Model validation attributes + Zod |
| XSS protection | DOMPurify, output encoding |
| Token trong email body | Không expose token trong URL |
| Cascade delete protection | DeleteBehavior.Restrict trên FK quan trọng |
| Random OTP | OTP 6-digit random, cache 10 phút |
| Email credentials | User Secrets, không hardcode |

---

## VI. Công Nghệ Sử Dụng

### Backend
- ASP.NET Core 8.0
- Entity Framework Core 8.0 + SQL Server
- SignalR
- System.IdentityModel.Tokens.Jwt
- SixLabors.ImageSharp

### Frontend
- React 19
- TypeScript 6
- Vite
- react-router-dom v7
- TanStack Query v5
- react-helmet-async
- Zod
- Axios
- DOMPurify

### Testing
- xUnit
- Moq
- EF Core InMemory
- Coverlet
