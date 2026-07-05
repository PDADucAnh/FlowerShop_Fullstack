# Hướng dẫn Cài đặt Dự án FlowerShop

> **Môi trường:** Windows + Visual Studio 2022  
> **Phiên bản:** .NET 8.0 / React 19 / TypeScript 6 / SQL Server LocalDB

---

## Mục lục

1. [Yêu cầu hệ thống](#1-yêu-cầu-hệ-thống)
2. [Cài đặt môi trường phát triển](#2-cài-đặt-môi-trường-phát-triển)
3. [Clone & Mở dự án](#3-clone--mở-dự-án)
4. [Cấu hình Backend](#4-cấu-hình-backend)
5. [Cấu hình Frontend](#5-cấu-hình-frontend)
6. [Tạo Database & Seed dữ liệu](#6-tạo-database--seed-dữ-liệu)
7. [Chạy dự án](#7-chạy-dự-án)
8. [Tài khoản mặc định](#8-tài-khoản-mặc-định)
9. [Xử lý sự cố thường gặp](#9-xử-lý-sự-cố-thường-gặp)

---

## 1. Yêu cầu hệ thống

| Công cụ | Phiên bản tối thiểu | Mục đích |
|---------|---------------------|----------|
| Windows | 10 / 11 | Hệ điều hành |
| Visual Studio 2022 | 17.8+ | IDE chính (Workload: ASP.NET and web development) |
| .NET SDK | 8.0.x | Build và chạy Backend |
| Node.js | 18.x / 20.x LTS | Chạy Frontend React |
| SQL Server | LocalDB (tự động cài cùng VS) | Cơ sở dữ liệu |
| Git | 2.30+ | Quản lý phiên bản |

### Kiểm tra phiên bản đã cài

```powershell
dotnet --version   # 8.0.x
node --version     # v20.x.x
npm --version      # 10.x.x
git --version      # 2.30+
sqllocaldb info    # Phải thấy "MSSQLLocalDB"
```

---

## 2. Cài đặt môi trường phát triển

### 2.1 Visual Studio 2022
Cài đặt từ [visualstudio.microsoft.com](https://visualstudio.microsoft.com/vs/), chọn workload **ASP.NET and web development**.

### 2.2 .NET 8.0 SDK
Tải từ [dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

### 2.3 Node.js
Tải từ [nodejs.org](https://nodejs.org/) (bản LTS 20.x).

### 2.4 SQL Server LocalDB
Có sẵn khi cài Visual Studio với workload ASP.NET and web development. Kiểm tra:
```powershell
sqllocaldb info MSSQLLocalDB
```

---

## 3. Clone & Mở dự án

### 3.1 Clone repository
```powershell
git clone https://github.com/PDADucAnh/FlowerShop_Fullstack.git
cd FlowerShop_Fullstack
```

### 3.2 Mở bằng Visual Studio
```powershell
start Flower-Shop.sln
```

---

## 4. Cấu hình Backend

### 4.1 File cấu hình chính
Mở `Flower.Backend/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FlowerShop_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "FlowerShop",
    "Audience": "FlowerShop.SPA",
    "SecretKey": "FlowerShop-SuperSecret-Key-256bit-Minimum-Length-Required!!",
    "ExpiryMinutes": 60
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreply@flowershop.com",
    "SenderName": "FlowerShop",
    "EnableSsl": true
  },
  "TimeSettings": {
    "TimeZone": "Asia/Ho_Chi_Minh",
    "LeadTimeHours": 2,
    "MaxOrdersPerSlot": 10,
    "PreShippingMinutes": 30
  },
  "WebhookSettings": {
    "SecretKey": "flowershop-webhook-secret-change-in-production"
  },
  "ClientUrl": "http://localhost:3000"
}
```

### 4.2 Cấu hình Email (tùy chọn)
Chức năng gửi email yêu cầu App Password Gmail. Xem hướng dẫn tại [gmail-setup-guide.md](./gmail-setup-guide.md). \
**Không bắt buộc** để chạy demo — các chức năng chính vẫn hoạt động không cần email.

### 4.3 Restore NuGet packages
```powershell
dotnet restore Flower-Shop.sln
```

### 4.4 Build Backend
```powershell
dotnet build Flower.Backend\Flower.Backend.csproj
```

---

## 5. Cấu hình Frontend

### 5.1 Cài đặt dependencies
```powershell
cd Flower-shop.frontend
npm install
```

### 5.2 Biến môi trường
File `.env.example` → copy thành `.env.local`:
```
VITE_API_URL=http://localhost:7224
VITE_IMAGE_BASE_URL=http://localhost:7224
```
(Backend HTTPS profile mặc định chạy port 7224. Dùng HTTP: port 5165.)

### 5.3 Kiểm tra build
```powershell
npm run build
```

---

## 6. Tạo Database & Seed dữ liệu

### 6.1 Migration tự động
Backend được cấu hình tự động chạy migration khi khởi động. \
Chỉ cần **chạy Backend** lần đầu, database `FlowerShop_DB` sẽ tự động được tạo với đầy đủ bảng.

### 6.2 Seed Admin mặc định
Khi chạy lần đầu, `Program.cs` tự động tạo tài khoản Admin nếu chưa tồn tại.

### 6.3 Migration thủ công (nếu cần)
```powershell
dotnet ef database update --project Flower.Data --startup-project Flower.Backend
```
Hoặc trong Visual Studio Package Manager Console:
```
Default project: Flower.Data
PM> Update-Database
```

---

## 7. Chạy dự án

### 7.1 Chạy Backend (ASP.NET Core)
**Từ CLI:**
```powershell
cd Flower.Backend
dotnet run --launch-profile https
```
Backend chạy tại:
- **API:** `https://localhost:7224` (hoặc `http://localhost:5165`)
- **Admin MVC:** `https://localhost:7224/Admin`
- **Swagger UI:** `https://localhost:7224/swagger`

### 7.2 Chạy Frontend (React)
**Mở terminal riêng:**
```powershell
cd Flower-shop.frontend
npm run dev
```
Frontend chạy tại `http://localhost:3000`.

### 7.3 Kiểm tra kết nối
1. Mở `http://localhost:3000` → Trang chủ hiển thị danh sách sản phẩm
2. API test tại `http://localhost:7224/swagger`

---

## 8. Tài khoản mặc định

### 8.1 Admin Panel
| Username | Password | Role | URL |
|----------|----------|------|-----|
| `admin` | `123456` | Admin | `http://localhost:7224/Admin` |

### 8.2 API (JWT)
| Endpoint | Method | Body |
|----------|--------|------|
| `/api/auth/login` | POST | `{"username": "admin", "password": "123456"}` |
| `/api/auth/register` | POST | `{"email": "...", "password": "..."}` |

---

## 9. Cấu trúc thư mục dự án

```
FlowerShop_Fullstack/
├── Flower-Shop.sln                    # Solution file
├── Flower.Backend/                     # ASP.NET Core Backend
│   ├── Controllers/Api/                # RESTful API Controllers
│   ├── Services/                       # Business Logic Layer
│   │   └── Interfaces/                 # Service interfaces
│   ├── Models/DTOs/                    # Data Transfer Objects
│   ├── Middleware/                      # Custom middleware
│   ├── Hubs/                           # SignalR hubs
│   └── Views/                          # Razor views (Admin)
├── Flower.Data/                        # Data Layer
│   ├── Entities/                       # EF Core entities
│   ├── Migrations/                     # EF migrations
│   └── ApplicationDbContext.cs         # DbContext
├── Flower-shop.frontend/               # React SPA (Vite)
│   └── src/
│       ├── components/                 # Reusable components
│       ├── pages/                      # Page components (19 routes)
│       ├── services/                   # API service modules
│       └── utils/                      # Utility functions
├── Flower.Tests/                       # xUnit unit tests
├── docs/                               # Documentation
│   └── audits/                         # Audit reports
├── scripts/                            # SQL scripts
└── README.md
```

---

## 10. Xử lý sự cố thường gặp

### Lỗi: `Cannot open database "FlowerShop_DB"`
```powershell
sqllocaldb start MSSQLLocalDB
sqllocaldb info MSSQLLocalDB
```

### Lỗi: CORS / Network Error
Đảm bảo Backend đang chạy trước Frontend, và cổng khớp với `.env.local`.

### Lỗi: Port đã được sử dụng
Sửa port trong `Flower.Backend/Properties/launchSettings.json`.

### Lỗi: NuGet restore thất bại
```powershell
dotnet restore
```

### Lỗi: `npm install` thất bại
```powershell
npm cache clean --force
npm install --legacy-peer-deps
```

---

<p align="center"><i>&copy; 2026 FlowerShop - Full-Stack ASP.NET Core &amp; React Project</i></p>
