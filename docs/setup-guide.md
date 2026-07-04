# Hướng dẫn Cài đặt Dự án AnhCMS_Solution

> **Môi trường:** Windows + Visual Studio 2022  
> **Phiên bản:** .NET 8.0 / React 19 / SQL Server LocalDB

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
dotnet --version
node --version
npm --version
git --version
```

---

## 2. Cài đặt môi trường phát triển

### 2.1 Visual Studio 2022

Cài đặt từ [visualstudio.microsoft.com](https://visualstudio.microsoft.com/vs/), chọn workload:
- **ASP.NET and web development**
- **.NET desktop development** (nếu cần)

### 2.2 .NET 8.0 SDK

Tải từ [dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

### 2.3 Node.js

Tải từ [nodejs.org](https://nodejs.org/) (bản LTS 20.x).  
Sau khi cài, mở PowerShell kiểm tra:

```powershell
node -v   # v20.x.x
npm -v    # 10.x.x
```

### 2.4 SQL Server LocalDB

LocalDB có sẵn khi cài Visual Studio với workload **ASP.NET and web development**.  
Kiểm tra:

```powershell
sqllocaldb info
# Phải thấy "MSSQLLocalDB"
```

Nếu chưa có, cài riêng từ [SQL Server Express Download](https://go.microsoft.com/fwlink/?linkid=866658).

---

## 3. Clone & Mở dự án

### 3.1 Clone repository

```powershell
git clone https://github.com/<your-repo>/AnhCMS_Solution.git
cd AnhCMS_Solution
```

### 3.2 Mở bằng Visual Studio

```powershell
start AnhCMS_Solution.sln
```

Hoặc mở thủ công: File > Open > Project/Solution > chọn `AnhCMS_Solution.sln`

---

## 4. Cấu hình Backend

### 4.1 File cấu hình chính

Mở `CMS.Backend/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AnhCMS_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "AnhCMS",
    "Audience": "AnhCMS.SPA",
    "SecretKey": "AnhCMS-SuperSecret-Key-256bit-Minimum-Length-Required!!",
    "ExpiryMinutes": 60
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "AnhCMS Boutique",
    "EnableSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-16-char-app-password"
  },
  "TimeSettings": {
    "TimeZone": "Asia/Ho_Chi_Minh",
    "LeadTimeHours": 2,
    "MaxOrdersPerSlot": 10,
    "PreShippingMinutes": 30
  },
  "ClientUrl": "http://localhost:3000"
}
```

### 4.2 Cấu hình Email (tùy chọn)

Xem hướng dẫn chi tiết tại [Gmail SMTP Setup Guide](./gmail-setup-guide.md).

### 4.3 Restore NuGet packages

Trong Visual Studio:右键 Solution > **Restore NuGet Packages**  
Hoặc dùng CLI:

```powershell
dotnet restore AnhCMS_Solution.sln
```

### 4.4 Build Backend

```powershell
dotnet build CMS.Backend\CMS.Backend.csproj
```

---

## 5. Cấu hình Frontend

### 5.1 Cài đặt dependencies

```powershell
cd cms.frontend
npm install
```

### 5.2 Cấu hình API endpoint

File `cms.frontend/src/utils/apiUtils.ts` chứa `API_BASE_URL`.  
Mặc định: `http://localhost:5064`. Nếu Backend chạy port khác, sửa lại cho phù hợp.

### 5.3 Kiểm tra build

```powershell
npm run build
```

---

## 6. Tạo Database & Seed dữ liệu

### 6.1 Migration tự động

Backend được cấu hình tự động chạy migration khi khởi động (dòng 170-188 trong `Program.cs`):

```csharp
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}
```

Chỉ cần **chạy Backend** lần đầu, database `AnhCMS_DB` sẽ tự động được tạo.

### 6.2 Seed Admin mặc định

Khi chạy lần đầu, `Program.cs` tự động tạo tài khoản Admin nếu chưa tồn tại:

```csharp
if (!context.Users.Any(u => u.Username == "admin"))
{
    // Tạo user admin với mật khẩu 123456
}
```

### 6.3 Migration thủ công (nếu cần)

```powershell
cd CMS.Backend
dotnet ef database update
```

Hoặc dùng Package Manager Console trong VS:
```
Default project: CMS.Data
PM> Update-Database
```

---

## 7. Chạy dự án

### 7.1 Chạy Backend (ASP.NET Core)

**Từ Visual Studio:**  
Set `CMS.Backend` làm Startup Project > Nhấn F5 (hoặc Ctrl+F5).

**Từ CLI:**

```powershell
cd CMS.Backend
dotnet run
```

Backend sẽ chạy tại:
- **MVC Admin:** `https://localhost:7000` (hoặc `http://localhost:5064`)
- **Swagger UI:** `http://localhost:5064/swagger`
- **SignalR Hub:** `http://localhost:5064/hubs/notifications`

### 7.2 Chạy Frontend (React)

**Mở terminal riêng** (không đóng terminal Backend):

```powershell
cd cms.frontend
npm start
```

Frontend sẽ chạy tại `http://localhost:3000`.

### 7.3 Kiểm tra kết nối

1. Mở trình duyệt vào `http://localhost:3000`
2. Trang chủ hiển thị danh sách sản phẩm
3. Backend API có thể test tại `http://localhost:5064/swagger`

---

## 8. Tài khoản mặc định

### 8.1 Admin Panel (MVC)

| Username | Password | Role | URL |
|----------|----------|------|-----|
| `admin` | `123456` | Admin | `http://localhost:5064/Admin` |

### 8.2 API (JWT)

| Endpoint | Method | Body |
|----------|--------|------|
| `/api/auth/login` | POST | `{"username": "admin", "password": "123456"}` |
| `/api/auth/register` | POST | `{"email": "...", "password": "..."}` |

### 8.3 Test User API (Postman / Swagger)

```json
POST /api/auth/login
{
    "username": "admin",
    "password": "123456"
}
```

Response sẽ trả về JWT token dùng để gọi các API khác.

---

## 9. Xử lý sự cố thường gặp

### Lỗi: `Cannot open database "AnhCMS_DB"`

```powershell
sqllocaldb start MSSQLLocalDB
sqllocaldb info MSSQLLocalDB
```

### Lỗi: CORS / Network Error

Đảm bảo:
1. Backend đang chạy (port 5064)
2. Frontend chạy ở port 3000
3. `appsettings.json` có `"ClientUrl": "http://localhost:3000"`

### Lỗi: Port đã được sử dụng

Sửa port trong `CMS.Backend/Properties/launchSettings.json`:

```json
"applicationUrl": "http://localhost:5064"
```

### Lỗi: NuGet restore thất bại

```powershell
dotnet restore
```

Hoặc xóa `obj` và `bin` trong từng project rồi build lại.

### Lỗi: `npm install` thất bại

Thử:

```powershell
npm cache clean --force
npm install --legacy-peer-deps
```

---

## Cấu trúc thư mục dự án

```
AnhCMS_Solution/
├── AnhCMS_Solution.sln              # Solution file
├── CMS.Backend/                      # ASP.NET Core Backend
│   ├── Controllers/                  # MVC Controllers (Admin)
│   │   └── Api/                      # RESTful API Controllers
│   ├── Services/                     # Business Logic Layer
│   │   └── Interfaces/               # Service interfaces
│   ├── Models/                       # DTOs, ViewModels
│   ├── Middleware/                    # Custom middleware
│   ├── Hubs/                         # SignalR hubs
│   ├── Utils/                        # Helpers
│   ├── Views/                        # Razor views (Admin)
│   └── wwwroot/                      # Static files
├── CMS.Data/                         # Data Layer
│   ├── Entities/                     # EF Core entities (13 tables)
│   ├── Migrations/                   # EF migrations
│   └── ApplicationDbContext.cs       # DbContext
├── cms.frontend/                     # React SPA
│   └── src/
│       ├── api/                      # Axios client
│       ├── components/               # Reusable components
│       ├── contexts/                 # React contexts (Auth, Cart, Wishlist)
│       ├── hooks/                    # Custom hooks
│       ├── pages/                    # Page components (17 routes)
│       ├── services/                 # API service modules
│       ├── types/                    # TypeScript interfaces
│       └── utils/                    # Utility functions
├── CMS.Tests/                        # xUnit unit tests
├── docs/                             # Documentation
├── scripts/                          # SQL scripts
└── README.md
```

---

<p align="center"><i>&copy; 2026 AnhCMS_Solution - Full-Stack ASP.NET Core &amp; React Documentation</i></p>
