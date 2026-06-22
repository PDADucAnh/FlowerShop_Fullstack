# FlowerShop 🏪🌷

> Full-stack e-commerce platform for flower shop with integrated CMS & blog — built with ASP.NET Core 8 + React 19.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![React](https://img.shields.io/badge/React-19-61DAFB?style=flat&logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-6-3178C6?style=flat&logo=typescript)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019-CC2927?style=flat&logo=microsoft-sql-server)
![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=flat&logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Features](#-features)
- [Database Schema](#-database-schema)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [API Documentation](#-api-documentation)
- [Development Roadmap](#-development-roadmap)

---

## 🎯 Overview

**FlowerShop** is a hybrid-architecture web application combining:

- **🛒 E-commerce**: Full shopping experience for flower products — browse categories, manage cart, checkout, track orders
- **📝 CMS & Blog**: Admin panel with rich content management, user authentication & role-based authorization
- **🔌 RESTful API**: Exposes JSON endpoints consumed by the React SPA frontend

Built as a **3-layer solution** with clear separation of concerns between data access, business logic, and presentation.

---

## ⚡ Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | ASP.NET Core 8 (MVC + Web API Hybrid), SignalR |
| **Frontend** | React 19, TypeScript 6, React Router 7, TanStack Query 5 |
| **Database** | SQL Server 2019+ / LocalDB, Entity Framework Core 8 (Code-First) |
| **Auth** | JWT Bearer (API) + Cookie Authentication (Admin MVC) |
| **Validation** | React Hook Form 7 + Zod 4 |
| **Real-time** | SignalR for order notifications |
| **API Docs** | Swagger / OpenAPI |
| **Tools** | Visual Studio 2022, SSMS, Postman, Node.js LTS |

---

## 🏗 Architecture

### Solution Structure (3-Layer)

```
Flower-Shop.sln
├── Flower.Data          # Data Layer — EF Core DbContext, Entities, Migrations
├── Flower.Backend       # Backend — MVC Admin + Web API Controllers, Services, DTOs
└── Flower-shop.frontend # Frontend — React SPA, Components, Pages, Hooks, State
```

### Hybrid Request Pipeline

```
┌──────────┐     ┌──────────────────┐     ┌──────────────┐
│  Browser │────▶│  ASP.NET Core 8  │────▶│  SQL Server  │
│ (React)  │     │  PolicyScheme    │     │  (LocalDB)   │
└──────────┘     │  ┌────────────┐  │     └──────────────┘
                 │  │ /api/*  →  │  │
┌──────────┐     │  │ JWT Auth   │  │
│  Browser │────▶│  │            │  │
│ (Admin)  │     │  │ / (MVC) → │  │
└──────────┘     │  │ Cookie     │  │
                 │  └────────────┘  │
                 └──────────────────┘
```

- **MVC routes** → Cookie authentication → Admin panel (Razor views)
- **API routes** (`/api/*`) → JWT Bearer authentication → JSON responses for React SPA
- **SignalR Hub** (`/hubs/notifications`) → Real-time order updates

---

## ✨ Features

### Customer-facing (React SPA)
- Browse flower products by category
- Product detail pages with images & descriptions
- Shopping cart with quantity management
- Checkout & order placement
- Order history tracking
- Wishlist management
- Blog with categorized articles
- User registration & login

### Admin Panel (MVC)
- Full CRUD for products, categories, posts, users, customers, orders
- Image upload with GUID-based file naming
- Rich text editing with CKEditor 5
- Role-based authorization (`Admin`, `Editor`)
- Order status management (Pending → Shipping → Completed)

### API & Infrastructure
- RESTful JSON API with Swagger documentation
- Real-time notifications via SignalR
- JWT-based stateless authentication for API consumers
- Database seeding with sample data on first run

---

## 🗄 Database Schema

| Table | Description |
|-------|-------------|
| **Categories** | Blog post categories |
| **Posts** | Blog articles (FK → Categories) |
| **Users** | Admin & staff accounts |
| **CategoriesProducts** | Product categories (e.g., Roses, Orchids) |
| **Products** | Flower products (FK → CategoriesProducts) |
| **Customers** | E-commerce customer accounts |
| **Orders** | Customer orders (FK → Customers) |
| **OrderDetails** | Order line items (FK → Orders, Products) |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with VS 2022)
- [Node.js LTS](https://nodejs.org/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with ASP.NET & web development workload

### Setup

```bash
# 1. Clone the repository
git clone <repo-url>
cd FlowerShop

# 2. Database — apply migrations
cd Flower.Backend
dotnet ef database update

# 3. Run backend (starts on https://localhost:7224)
dotnet run

# 4. Frontend — install dependencies
cd ../Flower-shop.frontend
npm install

# 5. Run frontend (starts on http://localhost:3000)
npm start
```

The application will seed default data on first run including an admin account:

| Role | Username | Password |
|------|----------|----------|
| Admin | `admin` | `admin123` |

---

## 📁 Project Structure

```
FlowerShop/
├── Flower.Data/                # Data layer
│   ├── Entities/               # EF Core entity classes
│   ├── Migrations/             # Code-First migrations
│   ├── ApplicationDbContext.cs
│   └── IApplicationDbContext.cs
├── Flower.Backend/             # Backend (MVC + API)
│   ├── Controllers/
│   │   ├── Api/                # RESTful API controllers
│   │   └── *Controller.cs      # MVC admin controllers
│   ├── Hubs/                   # SignalR hub
│   ├── Models/
│   │   ├── DTOs/               # Data transfer objects
│   │   └── MappingExtensions.cs
│   ├── Services/               # Business logic layer
│   │   ├── Interfaces/
│   │   └── *Service.cs
│   ├── Views/                  # Razor admin views
│   └── Program.cs
├── Flower-shop.frontend/       # React SPA
│   ├── src/
│   │   ├── api/                # Axios client, TanStack Query
│   │   ├── components/         # Reusable UI components
│   │   ├── context/            # Auth, Cart, Wishlist state
│   │   ├── hooks/              # Custom React hooks
│   │   ├── pages/              # Route pages (home, shop, blog, etc.)
│   │   ├── schemas/            # Zod validation schemas
│   │   ├── services/           # API service modules
│   │   ├── types/              # TypeScript type definitions
│   │   └── App.tsx
│   └── package.json
└── Flower-Shop.sln
```

---

## 📖 API Documentation

When running in development mode, Swagger UI is available at:

```
https://localhost:7224/swagger
```

### Key API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Authenticate & receive JWT |
| GET | `/api/products` | List all products |
| GET | `/api/products/{id}` | Get product details |
| GET | `/api/categories` | List product categories |
| GET | `/api/posts` | List blog posts |
| GET | `/api/posts/{id}` | Get post details |
| POST | `/api/orders` | Place a new order |
| GET | `/api/orders/customer/{id}` | Get customer orders |
| POST | `/api/customers/register` | Register new customer |

---

## 🗺 Development Roadmap

### Phase 1 — Backend & Database Foundation
- [x] 3-layer solution setup
- [x] Entity design & EF Core migrations
- [x] LINQ queries & CRUD operations
- [x] Seed data

### Phase 2 — Admin Panel, Security & API
- [x] Bootstrap admin layout with sidebar
- [x] Image upload & CKEditor integration
- [x] Cookie authentication & role-based authorization
- [x] RESTful Web API with Swagger
- [x] JWT authentication for API

### Phase 3 — React Frontend
- [x] CORS configuration
- [x] Axios client setup
- [x] Pages: Home, Shop, Blog, Cart, Checkout, Auth
- [x] TanStack Query integration
- [x] Cart & wishlist state management

### Phase 4 — Advanced Features
- [x] SignalR real-time notifications
- [x] Form validation with Zod
- [x] Protected routes & auth flow
- [ ] Unit tests
- [ ] CI/CD pipeline

---

## 👨‍💻 Author

**Pham Duc Anh** — 2123110135 — CCQ2311D

---

<p align="center">
  <sub>© 2026 FlowerShop — Built with ASP.NET Core 8 & React 19</sub>
</p>
