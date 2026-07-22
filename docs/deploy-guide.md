# Hướng dẫn Triển khai Production

> Kiến trúc: **Backend** (.NET 8 Docker) trên Render.com — **Frontend** (React SPA) trên Vercel

---

## Mục lục

1. [Yêu cầu](#1-yêu-cầu)
2. [Tổng quan kiến trúc](#2-tổng-quan-kiến-trúc)
3. [Triển khai Backend (Render)](#3-triển-khai-backend-render)
   - 3.1 [Tạo PostgreSQL Database](#31-tạo-postgresql-database)
   - 3.2 [Tạo Web Service bằng Docker](#32-tạo-web-service-bằng-docker)
   - 3.3 [Cấu hình Environment Variables](#33-cấu-hình-environment-variables)
   - 3.4 [Deploy & Kiểm tra](#34-deploy--kiểm-tra)
4. [Triển khai Frontend (Vercel)](#4-triển-khai-frontend-vercel)
5. [Cập nhật sau khi thay đổi code](#5-cập-nhật-sau-khi-thay-đổi-code)
6. [Xử lý sự cố thường gặp](#6-xử-lý-sự-cố-thường-gặp)

---

## 1. Yêu cầu

- Tài khoản [Render.com](https://render.com) (GitHub login)
- Tài khoản [Vercel](https://vercel.com) (GitHub login)
- Tài khoản [Cloudinary](https://cloudinary.com) (lưu ảnh sản phẩm)
- Repository GitHub: `PDADucAnh/FlowerShop_Fullstack`

---

## 2. Tổng quan kiến trúc

```
                        Render.com
┌─────────────────────────────────────────┐
│  Web Service (Docker)                   │
│  ┌─────────────────────────────────┐    │
│  │ Flower.Backend (.NET 8)        │    │
│  │ Port 8080                       │    │
│  │ ASPNETCORE_ENVIRONMENT=Production│    │
│  └──────────┬──────────────────────┘    │
│             │                            │
│  ┌──────────▼──────────────────────┐    │
│  │ PostgreSQL 16 (Render)         │    │
│  │ flowershop-db                   │    │
│  └─────────────────────────────────┘    │
│                                         │
│  Cloudinary (image upload)              │
└─────────────────────────────────────────┘
          │
          │ HTTPS
          ▼
┌─────────────────────────────────────────┐
│  Vercel                                 │
│  Flower-shop.frontend (React SPA)       │
│  VITE_API_URL=https://flowershop-api... │
└─────────────────────────────────────────┘
```

**Lưu ý:** Render tự động cấp domain `<service-name>.onrender.com`. Có thể dùng custom domain nếu nâng cấp lên paid plan.

---

## 3. Triển khai Backend (Render)

### 3.1 Tạo PostgreSQL Database

1. Vào [Render Dashboard](https://dashboard.render.com) → **New** → **PostgreSQL**
2. Điền:
   - **Name**: `flowershop-db`
   - **Database**: `flowershop_db_fpdm`
   - **User**: `flowershop_db_fpdm_user`
   - **Region**: `Singapore (Southeast Asia)`
   - **Plan**: Free
3. Click **Create Database**
4. Sau khi tạo xong, copy các giá trị sau (cần cho bước 3.3):
   - `Internal Database URL`
   - Hoặc các thông số: Hostname, Port, Database, Username, Password

### 3.2 Tạo Web Service bằng Docker

> **Quan trọng:** Dự án đã có sẵn `Dockerfile` ở thư mục gốc. Render sẽ build Docker image từ file này.

**Cách 1 — Tạo từ Dashboard (khuyến nghị):**
1. **New** → **Web Service**
2. Kết nối GitHub repo `PDADucAnh/FlowerShop_Fullstack`
3. Điền:
   - **Name**: `flowershop-api`
   - **Runtime**: `Docker`
   - **Branch**: `main`
   - **Plan**: Free
4. Click **Create Web Service**

**Cách 2 — Dùng render.yaml (blueprint):**
1. **New** → **Blueprint**
2. Kết nối GitHub repo
3. Render tự động đọc `render.yaml` và tạo service + database

> Nếu dùng blueprint, database được linked tự động. Skip bước 3.3.

### 3.3 Cấu hình Environment Variables

Sau khi tạo Web Service, vào tab **Environment** và thêm các biến sau:

| Key | Value | Bắt buộc |
|-----|-------|----------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | ✅ |
| `DB_PROVIDER` | `PostgreSQL` | ✅ |
| `PGHOST` | Hostname từ PostgreSQL dashboard | ✅ |
| `PGPORT` | `5432` | ✅ |
| `PGDATABASE` | Database name | ✅ |
| `PGUSER` | Username | ✅ |
| `PGPASSWORD` | Password | ✅ |
| `JWT_SECRET_KEY` | Chuỗi bí mật (tự tạo) | ✅ |
| `WEBHOOK_SECRET_KEY` | Chuỗi bí mật (tự tạo) | ✅ |
| `CORS_ORIGINS` | `https://flowershop.vercel.app` | ✅ |
| `CLOUDINARY__CLOUDNAME` | Từ Cloudinary dashboard | ⚠️ (nếu dùng upload ảnh) |
| `CLOUDINARY__APIKEY` | Từ Cloudinary dashboard | ⚠️ |
| `CLOUDINARY__APISECRET` | Từ Cloudinary dashboard | ⚠️ |

**Giải thích:**
- `PGHOST/PGPORT/PGDATABASE/PGUSER/PGPASSWORD`: Thông tin kết nối PostgreSQL. Code sẽ tự động build connection string từ các biến này.
- `JWT_SECRET_KEY`: Dùng để ký JWT token. Nên tạo chuỗi ngẫu nhiên dài tối thiểu 32 ký tự.
- `CORS_ORIGINS`: Danh sách origin được phép gọi API, cách nhau bằng `;`.

### 3.4 Deploy & Kiểm tra

1. Sau khi cấu hình xong, vào **Manual Deploy** → **Deploy latest commit**
2. Chờ build (~5-10 phút, free plan có thể lâu hơn)
3. Khi thấy log `Your service is live 🎉` là thành công
4. Kiểm tra: `https://flowershop-api-4i1d.onrender.com/Account/Login`

**Tài khoản mặc định:** `admin` / `123456`

---

## 4. Triển khai Frontend (Vercel)

### 4.1 Import project

1. Vào [Vercel Dashboard](https://vercel.com) → **Add New** → **Project**
2. Import GitHub repo `PDADucAnh/FlowerShop_Fullstack`
3. **Root Directory**: Chọn `Flower-shop.frontend`
4. **Framework Preset**: `Vite`

### 4.2 Environment Variables

| Key | Value |
|-----|-------|
| `VITE_API_URL` | `https://flowershop-api-4i1d.onrender.com` |
| `VITE_IMAGE_BASE_URL` | `https://flowershop-api-4i1d.onrender.com` |

> **Lưu ý:** Cloudinary URL được cấu hình động qua admin Settings UI, không cần env var cho frontend.

### 4.3 Deploy

1. Click **Deploy**
2. Sau khi hoàn tất, Vercel cấp domain `flowershop.vercel.app`
3. Cập nhật `CORS_ORIGINS` trên Render thành `https://flowershop.vercel.app`

---

## 5. Cập nhật sau khi thay đổi code

### Backend
```bash
git add .
git commit -m "mô tả thay đổi"
git push origin main
```
Render tự động build lại (Auto-Deploy). Hoặc vào Dashboard → **Manual Deploy** → **Deploy latest commit**.

### Frontend
Tương tự: push lên `main`, Vercel tự động deploy.

---

## 6. Xử lý sự cố thường gặp

### Lỗi: `Bad Request - Invalid Hostname`
**Nguyên nhân:** `AllowedHosts` trong `appsettings.json` quá hẹp.
**Fix:** Đặt thành `"AllowedHosts": "*"`.

### Lỗi: `inotify instances limit reached`
**Nguyên nhân:** Container giới hạn file watchers.
**Fix:** Dockerfile đã có `DOTNET_USE_POLLING_FILE_WATCHER=true` và `ASPNETCORE_ENVIRONMENT=Production`.

### Lỗi: `syntax error at or near "["`
**Nguyên nhân:** Dùng cú pháp SQL Server (`[ColumnName]`) với PostgreSQL.
**Fix:** Sử dụng conditional filter — code đã xử lý qua `IsPostgres` helper.

### Lỗi: `Cannot write DateTime with Kind=Unspecified`
**Nguyên nhân:** Npgsql yêu cầu DateTime.Kind = Utc.
**Fix:** `Program.cs` đã có `AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true)`.

### Lỗi: `Format of the initialization string does not conform...`
**Nguyên nhân:** Connection string sai format.
**Fix:** Dùng các biến `PGHOST/PGPORT/PGDATABASE/PGUSER/PGPASSWORD` riêng lẻ thay vì connection string gộp.

### Lỗi: Container crash / OOM
**Nguyên nhân:** Free plan chỉ có 512MB RAM.
**Fix:** Tối ưu query, tránh load toàn bộ bảng lớn. Nâng cấp plan nếu cần.

### Lỗi: Kết nối database chậm / timeout
**Nguyên nhân:** Free PostgreSQL trên Render có thể spin down.
**Fix:** Đợi ~30s cho lần kết nối đầu tiên sau thời gian idle.

---

&copy; 2026 PDA FlowerShop — Internship Project
