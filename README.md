# PDA FlowerShop — Full-Stack E-Commerce Platform

Graduation internship project: a full-stack e-commerce website for flower retail, built with **ASP.NET Core 8.0** (Backend + Admin Panel) and **React 19** (Frontend SPA).

**Live Demo:**
- Backend API + Admin Panel: https://flowershop-api-4i1d.onrender.com
- Frontend Store: *(deploy on Vercel)*

---

## Documentation

| Document | Description |
|----------|-------------|
| [Setup Guide](./docs/setup-guide.md) | Local development setup (SQL Server) |
| [Deploy Guide](./docs/deploy-guide.md) | Production deployment (Render + Vercel) |
| [Architecture Overview](./docs/architecture-overview.md) | System architecture, ER Diagram, workflows |
| [Features](./docs/features.md) | Full feature list |
| [Gmail SMTP Configuration](./docs/gmail-setup-guide.md) | Email sending setup |

---

## Tech Stack

| Layer | Local Development | Production |
|:------|:-----------------|:-----------|
| **Backend** | .NET 8.0, ASP.NET Core MVC + Web API | .NET 8.0 (Docker on Render) |
| **Frontend** | React 19, TypeScript, TanStack Query 5 | React 19 (Vercel) |
| **Database** | SQL Server LocalDB | PostgreSQL 16 (Render) |
| **Auth** | JWT + Cookie | Same |
| **File Storage** | Local uploads | Cloudinary |
| **Real-time** | SignalR | SignalR |
| **CI/CD** | — | GitHub → Auto Deploy |

---

## Quick Start (Local Development)

### Prerequisites
- .NET 8.0 SDK, Node.js 20+, SQL Server LocalDB

### Backend
```powershell
git clone https://github.com/PDADucAnh/FlowerShop_Fullstack.git
cd FlowerShop_Fullstack
dotnet restore
dotnet run --project Flower.Backend --launch-profile https
```
- Admin Panel: `https://localhost:7224/Admin`
- API: `https://localhost:7224/api`
- Swagger: `https://localhost:7224/swagger`

### Frontend
```powershell
cd Flower-shop.frontend
npm install
npm run dev
```
- Store: `http://localhost:3000`

### Default Account
| Username | Password | Role |
|----------|----------|------|
| `admin` | `123456` | Admin |

---

## Project Structure

```
FlowerShop/
├── Flower.Backend/         ASP.NET Core (MVC + Web API + SignalR)
├── Flower.Data/            EF Core DbContext, Entities, Migrations
├── Flower-shop.frontend/   React SPA (Vite)
├── Flower.Tests/           xUnit unit tests
├── Dockerfile              Production container build
├── render.yaml             Render blueprint
└── docs/                   Documentation
```

---

## Key Features

- **Storefront** — Product catalog, cart, checkout, order tracking, wishlist
- **Admin Panel** — Dashboard, CRUD for products/orders/posts/categories/users
- **Auth** — JWT (API) + Cookie (Admin), role-based (Admin/Staff)
- **Real-time** — SignalR notifications
- **Payments** — COD + VNPay integration
- **Promotions** — Campaigns, flash sales, coupons
- **Email** — Order status notifications via SMTP/Gmail
- **Background Jobs** — Auto-cancel expired orders, auto-activate promotions

---

&copy; 2026 PDA FlowerShop — Internship Project
