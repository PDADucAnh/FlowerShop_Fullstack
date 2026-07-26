# Database-Agnostic Backend Refactor

**Date:** 2026-07-26
**Status:** Approved

## Goal

Refactor the .NET 8 backend to be fully database-agnostic — enabling seamless switching between PostgreSQL, SQL Server, and MySQL without runtime errors or business-logic changes.

## Principles

1. **Zero Raw SQL** — Every database operation goes through LINQ / EF Core APIs. Raw SQL (`ExecuteSqlRaw`, `FromSqlRaw`) is eliminated.
2. **Snake_case Convention** — `EFCore.NamingConventions` auto-maps C# PascalCase to SQL snake_case, eliminating quoted-identifier mismatches between providers.
3. **UTC-Normalized DateTime** — All `DateTime` values are stored as UTC; a value converter ensures consistency regardless of input `DateTimeKind`.
4. **Single-Config Provider Switch** — Changing the database provider is a one-line edit in `appsettings.json` or an environment variable.

---

## 1. Zero Raw SQL

### Current State

Two `ExecuteSqlRawAsync` calls in the codebase — both are atomic stock-decrement queries:

| File | Line | Purpose |
|------|------|---------|
| `Flower.Backend/Services/OrderService.cs` | 412 | Decrement stock on COD payment atomically |
| `Flower.Backend/Services/OrderExpiryBackgroundService.cs` | 113 | Restore stock on order expiry |

### Solution

Replace both with `ExecuteUpdateAsync` (EF Core 7+, provider-agnostic):

```csharp
var affected = await _context.Products
    .Where(p => p.Id == item.ProductId && p.StockQuantity >= item.Quantity)
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(p => p.StockQuantity, p => p.StockQuantity - item.Quantity));
```

- `ExecuteUpdateAsync` translates to the provider's native `UPDATE ... WHERE ...` SQL.
- Returns `int` (affected rows), preserving the existing `affected == 0` out-of-stock check.
- No quoting concerns — EF Core handles identifier quoting per provider.

**Files changed:** 2 (OrderService.cs, OrderExpiryBackgroundService.cs)

---

## 2. Snake_case Naming Convention

### Package

Add `EFCore.NamingConventions` v8.x to both `Flower.Data` and `Flower.Backend`.

### Configuration

In `Program.cs`, after the provider-specific options:

```csharp
options.UseSnakeCaseNamingConvention();
```

### Migration

Run after the code change:

```
dotnet ef migrations add SwitchToSnakeCaseNaming -p Flower.Data -s Flower.Backend
```

The migration renames all database objects via `RENAME` operations (data-preserving):

| C# PascalCase | SQL snake_case |
|---------------|----------------|
| `Products` | `products` |
| `ProductId` | `product_id` |
| `StockQuantity` | `stock_quantity` |
| `PK_Products` | `pk_products` |
| `IX_Products_Sku` | `ix_products_sku` |
| `FK_Products_CategoriesProducts_CategoryProductId` | `fk_products_categories_products_category_product_id` |

---

## 3. DateTime Auto-UTC

### Current State

Zero occurrences of `DateTime.Now` found — project already uses `DateTime.UtcNow` everywhere.

### Safety-Net Configuration

In `ApplicationDbContext.OnModelCreating`, add a value converter for all `DateTime` / `DateTime?` properties:

```csharp
foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    foreach (var property in entityType.GetProperties())
        if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
            property.SetValueConverter(...);
```

- **On save:** Converts non-UTC values to UTC.
- **On read:** Marks `DateTimeKind` as `Utc`.

---

## 4. Multi-Provider Configuration

### Configuration Source Priority

1. `appsettings.json` → `"DbProvider": "PostgreSQL"` / `"SqlServer"` / `"MySQL"`
2. Environment variable `DB_PROVIDER` (for Render / Docker)

```csharp
var dbProvider = builder.Configuration.GetValue<string>("DbProvider")
    ?? Environment.GetEnvironmentVariable("DB_PROVIDER") ?? "SqlServer";
```

### Provider Switch in Program.cs

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (dbProvider)
    {
        case "PostgreSQL":
            options.UseNpgsql(connectionString);
            break;
        case "MySQL":
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            break;
        default:
            options.UseSqlServer(connectionString);
            break;
    }
    options.UseSnakeCaseNamingConvention();
});
```

### Connection String Resolution

Update `GetConnectionString()` to support MySQL environment variables:

```
case "MySQL":
    var myHost = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
    var myPort = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
    var myDb = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
    var myUser = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
    var myPass = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
    // Build connection string
```

### Package

Add `Pomelo.EntityFrameworkCore.MySql` v8.x to `Flower.Backend`.

---

## Implementation Order

1. Add NuGet packages (`EFCore.NamingConventions`, `Pomelo.EntityFrameworkCore.MySql`)
2. Replace raw SQL with `ExecuteUpdateAsync` in both files
3. Configure `UseSnakeCaseNamingConvention` + DateTime value converter in `ApplicationDbContext`
4. Update `Program.cs` — multi-provider switch + MySQL connection string
5. Generate snake_case migration
6. Build & verify
7. Commit and push

---

## Files Changed

| File | Change |
|------|--------|
| `Flower.Data/Flower.Data.csproj` | +`EFCore.NamingConventions` |
| `Flower.Data/ApplicationDbContext.cs` | +DateTime value converter |
| `Flower.Backend/Flower.Backend.csproj` | +`EFCore.NamingConventions`, +`Pomelo.EntityFrameworkCore.MySql` |
| `Flower.Backend/Program.cs` | Multi-provider switch, MySQL conn string, snake_case config |
| `Flower.Backend/Services/OrderService.cs` | `ExecuteSqlRawAsync` → `ExecuteUpdateAsync` |
| `Flower.Backend/Services/OrderExpiryBackgroundService.cs` | `ExecuteSqlRawAsync` → `ExecuteUpdateAsync` |
| `Flower.Data/Migrations/20260726_SwitchToSnakeCaseNaming.cs` | New migration (generated) |

## Risks & Mitigations

| Risk | Mitigation |
|------|------------|
| Migration renames break running queries on deploy | Deploy during low-traffic; migration runs at startup before any request |
| Npgsql's `UseLowerCaseNamingConventions` conflicts with `UseSnakeCaseNamingConvention` | Remove any existing lowercase config; `EFCore.NamingConventions` is the single source of truth |
| MySQL `ServerVersion.AutoDetect` fails at runtime | Add `appsettings.json` fallback for MySQL version; user can hardcode if needed |
