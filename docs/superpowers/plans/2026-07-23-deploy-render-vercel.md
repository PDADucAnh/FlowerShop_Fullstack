# Deploy Backend (Render) + Frontend (Vercel) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make the FlowerShop project deployable: backend to Render.com, frontend to Vercel.

**Architecture:** Backend is ASP.NET Core 8.0 with EF Core SQL Server (switch to PostgreSQL for Render compatibility). Frontend is React+Vite, connects via API URL. Dockerfile builds and runs the backend. Render PostgreSQL for production DB.

**Tech Stack:** ASP.NET Core 8.0, EF Core + Npgsql (PostgreSQL), Render, Vercel, Cloudinary, Vite

## Global Constraints

- .NET 8.0 SDK for build
- Backend listens on `http://+:8080` (Render assigns `$PORT` via `ASPNETCORE_URLS`)
- Render PostgreSQL for database (SQL Server localdb for dev only)
- All secrets via environment variables, never hardcoded
- Frontend build reads `VITE_API_URL` at build time

---

### Task 1: Fix Dockerfile

**Files:**
- Modify: `Flower.Backend/Dockerfile`

- [ ] **Step 1: Rewrite Dockerfile with correct base images**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Flower.Backend.dll"]
```

- [ ] **Step 2: Verify Dockerfile parses**

Run: `docker build -f Flower.Backend/Dockerfile -t test-build . 2>&1 | Select-String -Pattern "error"`

---

### Task 2: Switch Database Provider to PostgreSQL + Add Env Var Support

**Files:**
- Modify: `Flower.Backend/Flower.Backend.csproj`
- Modify: `Flower.Backend/Program.cs`
- Modify: `Flower.Backend/appsettings.json`

- [ ] **Step 1: Add Npgsql NuGet package**

```xml
<!-- Add to csproj ItemGroup -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
```

- [ ] **Step 2: Update Program.cs to support both SQL Server (dev) and PostgreSQL (production)**

Replace the existing `AddDbContext` block in Program.cs:

Old:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

New:
```csharp
var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
var dbProvider = Environment.GetEnvironmentVariable("DB_PROVIDER") ?? "SqlServer";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (dbProvider == "PostgreSQL")
        options.UseNpgsql(connectionString);
    else
        options.UseSqlServer(connectionString);
});
```

Also add using at top:
```csharp
using Npgsql;
```

- [ ] **Step 3: Update appsettings.json connection string to be dev-only**

Keep SQL Server connection string for local dev. Production will use env var.

- [ ] **Step 4: Verify build**

Run: `dotnet build Flower.Backend/Flower.Backend.csproj`

---

### Task 3: Add Cloudinary Fallback + Read Secrets from Env Vars

**Files:**
- Modify: `Flower.Backend/Services/SystemSettingService.cs`
- Modify: `Flower.Backend/appsettings.json`

- [ ] **Step 1: Add Cloudinary fallback in GetFallbackSetting**

Add before `return new T();` in `GetFallbackSetting`:

```csharp
if (key == "Cloudinary" && typeof(T) == typeof(CloudinarySettings))
{
    var configSection = _configuration.GetSection("CloudinarySettings");
    var cloudinary = new CloudinarySettings
    {
        CloudName = Environment.GetEnvironmentVariable("CLOUDINARY__CLOUDNAME")
            ?? configSection["CloudName"] ?? "",
        ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY__APIKEY")
            ?? configSection["ApiKey"] ?? "",
        ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY__APISECRET")
            ?? configSection["ApiSecret"] ?? "",
        Folder = configSection["Folder"] ?? "flowershop_products"
    };
    return (T)(object)cloudinary;
}
```

- [ ] **Step 2: Remove hardcoded secrets from appsettings.json**

Replace the dev-only placeholder values. Keep `Jwt:SecretKey` as dev-only (will use env var in prod). `Program.cs` already reads `JWT_SECRET_KEY` env var.

---

### Task 4: Make CORS Configurable via Env Var

**Files:**
- Modify: `Flower.Backend/Program.cs`

- [ ] **Step 1: Read CORS origins from env var with fallback**

Replace the CORS section:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var origins = Environment.GetEnvironmentVariable("CORS_ORIGINS") ?? "http://localhost:3000";
        policy.WithOrigins(origins.Split(';', StringSplitOptions.RemoveEmptyEntries))
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

- [ ] **Step 2: Update the CORS usage**

Change line:
```csharp
app.UseCors("AllowReactApp");
```
to:
```csharp
app.UseCors("AllowFrontend");
```

---

### Task 5: Update Frontend for Vercel Deployment

**Files:**
- Modify: `Flower-shop.frontend/.env.production`
- Modify: `Flower-shop.frontend/.env`

- [ ] **Step 1: Update .env.production to use env var at build time**

```env
VITE_API_URL=https://flowershop-api.onrender.com
```

- [ ] **Step 2: Update .env (dev) to keep localhost**

Already correct: `VITE_API_URL=https://localhost:7224`

- [ ] **Step 3: Add vercel.json for SPA routing**

Create `Flower-shop.frontend/vercel.json`:

```json
{
  "rewrites": [
    { "source": "/(.*)", "destination": "/index.html" }
  ]
}
```

---

### Task 6: Add render.yaml for Render Deployment

**Files:**
- Create: `render.yaml`

```yaml
services:
  - type: web
    name: flowershop-api
    env: docker
    dockerfilePath: ./Flower.Backend/Dockerfile
    repo: https://github.com/your-username/FlowerShop
    branch: main
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: DB_PROVIDER
        value: PostgreSQL
      - key: CONNECTIONSTRINGS__DEFAULTCONNECTION
        fromDatabase:
          name: flowershop-db
          property: ConnectionString
      - key: JWT_SECRET_KEY
        generateValue: true
      - key: WEBHOOK_SECRET_KEY
        generateValue: true
      - key: CORS_ORIGINS
        value: https://flowershop.vercel.app
      - key: CLOUDINARY__CLOUDNAME
        sync: false
      - key: CLOUDINARY__APIKEY
        sync: false
      - key: CLOUDINARY__APISECRET
        sync: false

databases:
  - name: flowershop-db
    type: postgresql
    plan: free
```

---

### Task 7: Create Render Post-Deploy Script (Seed Data)

**Files:**
- Create: `Flower.Backend/seed.sql`

This file is for manual seeding of Cloudinary settings + other required settings on first deploy.

```sql
-- Seed initial settings (run once after first deploy)
-- Access admin panel at /Settings to configure Cloudinary, SMTP, VNPay, etc.
```

Note: Settings are managed via the admin UI. After first login, go to Settings page to configure Cloudinary.

---

## Post-Deploy Checklist

After deploying:

1. **Render**: Set `CLOUDINARY__CLOUDNAME`, `CLOUDINARY__APIKEY`, `CLOUDINARY__APISECRET` env vars in Render dashboard
2. **Render**: Set `CORS_ORIGINS` to your Vercel frontend URL
3. **Vercel**: Set `VITE_API_URL` env var to your Render backend URL
4. **Admin**: Login at `https://your-app.onrender.com/Account/Login` (admin/123456)
5. **Settings**: Go to `/Settings` and configure Cloudinary, SMTP, VNPay
6. **Verify**: Upload a test image to verify Cloudinary upload works
