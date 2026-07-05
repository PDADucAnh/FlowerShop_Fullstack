# PDA FlowerShop — Full-Stack E-Commerce Platform

Graduation internship project: a full-stack e-commerce website for flower retail, built with **ASP.NET Core 8.0** (Backend + Admin Panel) and **React 19** (Frontend SPA).

---

## Documentation

| Document | Description |
|----------|-------------|
| [Setup Guide](./docs/setup-guide.md) | System requirements, environment setup, configuration, database creation, running the project |
| [Architecture Overview](./docs/architecture-overview.md) | System architecture, ER Diagram, workflows (auth, cart, checkout, order), API reference |
| [Features](./docs/features.md) | Full feature list: customer-facing storefront, admin panel, advanced features |
| [Gmail SMTP Configuration](./docs/gmail-setup-guide.md) | Google App Password generation and email sending setup |

---

## Table of Contents

1. [Overview](#1-overview)
2. [Tech Stack](#2-tech-stack)
3. [Solution Architecture](#3-solution-architecture)
4. [Database Schema](#4-database-schema)
5. [Key Features](#5-key-features)
6. [Project Structure](#6-project-structure)

---

## 1. Overview

The system uses a **hybrid architecture** combining:
- **Backend (ASP.NET Core MVC + Web API):** Serves the Admin management panel via traditional MVC, while exposing RESTful Web APIs for the React frontend
- **Frontend (React SPA):** Client-side rendering with Axios-based API calls, TanStack Query caching, and real-time updates via SignalR

**Business domains:** E-Commerce (flower shop) and Content Management (news/blog).

---

## 2. Tech Stack

| Layer | Technologies |
|:-----|:-------------|
| **Backend** | .NET 8.0, ASP.NET Core MVC, ASP.NET Core Web API, SignalR |
| **Frontend** | React 19, TypeScript 6, Custom CSS, TanStack React Query 5, Axios |
| **Database** | SQL Server (Express/LocalDB), Entity Framework Core (Code-First Migrations) |
| **Authentication** | JWT Bearer (API) + Cookie Authentication (Admin MVC), Dual Policy Scheme |
| **Real-time** | SignalR WebSocket (`/hubs/notifications`) |
| **Testing** | xUnit + InMemory Database |
| **Tools** | Visual Studio 2022, SSMS, Node.js LTS, Postman, Swagger UI |

---

## 3. Solution Architecture

The solution consists of four projects:

1. **Flower.Data** — Class library containing `ApplicationDbContext` and 13 entity definitions
2. **Flower.Backend** — Hybrid server: 17 MVC Controllers (Admin UI) + 12 API Controllers (RESTful) + SignalR Hub + Middleware + Background Services
3. **Flower-shop.frontend** — React SPA with 17 pages, TanStack Query, Context API (Auth, Cart, Wishlist), SignalR client
4. **Flower.Tests** — xUnit unit tests with InMemory Database

### Architecture Diagram

```
Browser (React SPA) ───Axios───→ Flower.Backend (Web API + MVC)
        │                             │
        │                             ├──→ Flower.Data (EF Core + SQL Server)
        │                             │
        └──── SignalR ────────────────┘
```

---

## 4. Database Schema

13 tables with relational integrity:

| Table | Description |
|:------|:------------|
| **Users** | Admin/Editor management, role-based authorization |
| **Categories** | Blog news categories (hierarchical via ParentId) |
| **Posts** | Blog articles with rich HTML content (CKEditor 5) |
| **CategoriesProducts** | Product categories |
| **Products** | Flower products (unique SKU, indexed Price) |
| **Customers** | Customer accounts (unique Email, password reset support) |
| **Orders** | Orders with 7 statuses, delivery info, payment info |
| **OrderDetails** | Order line items (product + quantity + price) |
| **DeliverySlots** | Delivery time slots per product (capacity, day-of-week) |
| **Advertisements** | Banner ads (position, validity period) |
| **Payments** | Payment transactions (webhook, 4 statuses) |
| **RefreshTokens** | JWT refresh token management |
| **PhoneBlacklists** | Blacklisted phone numbers (fraud prevention) |

---

## 5. Key Features

### Customer-Facing (React SPA)

- **Homepage** — Hero banner slider, best-sellers, paginated products, latest posts
- **Store** — Category filter, price range filter, sorting, pagination (9/page)
- **Search** — Full-text search by product name/SKU
- **Product Detail** — Image lightbox, size variants (Classic/Deluxe/Grand), quantity selector, add-to-cart / buy-now
- **Cart** — LocalStorage persistence, stock validation, quantity update, removal
- **Checkout** — Multi-step form (buyer info, recipient info, delivery time, payment method)
- **Orders** — History, detail, cancellation (if eligible)
- **Wishlist** — LocalStorage-based favorites
- **Auth** — Login/register, forgot password, password reset via email
- **Blog** — Paginated articles, category sidebar, rich content display
- **Contact** — Message form + store information

### Admin Panel (ASP.NET Core MVC)

- **Dashboard** — Overview statistics (orders, revenue, products, customers)
- **Product Management** — CRUD with image upload
- **Order Management** — Approval, confirmation, status update, cancellation
- **Post Management** — Rich text editing with CKEditor 5
- **Category Management** — Blog & product categories
- **User Management** — Admins/Editors and customers
- **Advertisement Management** — Banner slider, positions, validity periods
- **Authorization** — Admin (full) / Staff (Editor — limited)

### Advanced

- **SignalR Real-time** — CRUD notifications to all connected clients
- **Fraud Detection** — Phone blacklist check, forced online payment on suspicion
- **Stock Locking** — Temporary hold during checkout (15-minute TTL)
- **Auto-expiry** — Automatic cancellation of unverified orders after 30 minutes
- **Email Notifications** — SMTP via Gmail on order status changes
- **Product Sizing** — Classic (base), Deluxe (+300k), Grand (+600k)
- **Delivery Slots** — Flexible time windows with 2-hour lead-time check

---

## 6. Project Structure

```
FlowerShop/
├── Flower.Backend/         # ASP.NET Core Backend (MVC + Web API)
│   ├── Controllers/        # MVC & API controllers
│   ├── Hubs/               # SignalR hubs
│   ├── Services/           # Business logic services
│   ├── wwwroot/            # Static files (uploads, assets)
│   └── Program.cs          # Application entry point
├── Flower.Data/            # Data access layer
│   ├── Entities/           # EF Core entity models
│   ├── Migrations/         # EF Core migrations
│   └── ApplicationDbContext.cs
├── Flower-shop.frontend/   # React SPA
│   ├── src/
│   │   ├── components/     # Reusable UI components
│   │   ├── pages/          # Page components
│   │   ├── services/       # API service layer
│   │   ├── contexts/       # React contexts (Auth, Cart, Wishlist)
│   │   └── hooks/          # Custom hooks
│   └── package.json
├── Flower.Tests/           # xUnit test project
├── docs/                   # Project documentation
└── scripts/                # SQL scripts
```

---

© 2026 PDA FlowerShop — Internship Project
