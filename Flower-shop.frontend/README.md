# FlowerShop Frontend

Frontend cho ứng dụng FlowerShop — xây dựng bằng Vite + React 19 + TypeScript.

## Yêu cầu

- Node.js 20+
- npm 9+

## Cài đặt

```bash
npm install
```

## Scripts

| Lệnh | Mô tả |
|------|-------|
| `npm run dev` | Chạy development server tại `http://localhost:3000` |
| `npm run build` | Build production vào thư mục `dist/` |
| `npm run preview` | Preview bản build production |
| `npm run lint` | Kiểm tra code với ESLint |

## Cấu trúc thư mục

```
src/
  components/     — Component dùng chung (Header, Footer, SEO, ...)
  pages/          — Các page theo route
  utils/          — Hàm tiện ích
  hooks/          — Custom hooks
```

## Biến môi trường

Sao chép `.env.example` thành `.env.local`:

| Biến | Mô tả | Mặc định |
|------|-------|----------|
| `VITE_API_BASE_URL` | Backend API URL | `http://localhost:5000` |
