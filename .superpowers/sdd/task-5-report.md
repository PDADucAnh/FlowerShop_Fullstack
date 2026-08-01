# Task 5 Report: Data-preserving migration `RefactorAndRenameTables`

**Date:** 2026-07-31
**Status:** DONE_WITH_CONCERNS (1 pre-existing, 1 deviation noted below)

## What I did

1. **Surveyed the repo + DB pre-state.**
   - Confirmed Tasks 1-4 committed; `Flower.Data` builds; 37/37 tests pass.
   - LocalDB `(localdb)\MSSQLLocalDB`, db `FlowerShop_DB`, connection string in `Flower.Backend/appsettings.json`.
   - Pre-state: 32 tables (old schema), **no `__EFMigrationsHistory`**, all rows 0 except `Users`=1.
   - Captured pre-state schema (columns/FKs/indexes of `Posts`, `Products`, `ProductVariants`, `Notifications`, `Categories`, `CategoriesProducts`) — confirmed it matches the 4 historical migrations verbatim.

2. **Installed local EF tool** (pinned to 8.0.23, since global `dotnet-ef` is 10.0.10 and the project is EF Core 8.0.23 / net8.0):
   ```
   dotnet new tool-manifest
   dotnet tool install dotnet-ef --version 8.0.23
   ```
   Manifest created at repo root; **moved to `.config/dotnet-tools.json`** (standard location, and the commit step targets `.config`). Verified `dotnet dotnet-ef --version` → 8.0.23.

3. **Generated the migration** with the local tool:
   ```
   dotnet dotnet-ef migrations add RefactorAndRenameTables --project Flower.Data --startup-project Flower.Backend
   ```
   Scaffolded `Up()` contained the expected (wrong) `DropTable`/`CreateTable` ops (plus `Avatar` `AlterColumn` ops) — NOT executed.

4. **Replaced `Up()`** with the brief's exact rename operations. **Replaced `Down()`** with the brief's exact reverse operations. No `DropTable`/`CreateTable` ops kept. `ProductVariants.ProductId1` untouched.

5. **Verified `.Designer.cs`**: target model contains `Flower.Data.Entities.PostCategory`, `ProductCategory`, `CustomerNotification`, and `ProductVariant` with `Price` + `Sku`. No old-type references (`rg` for `Entities.(Category|CategoryProduct|Notification)` → 0 hits). Snapshot updated (351 changed lines).

6. **Backed up DB** to `C:\Users\PDA Admin\AppData\Local\Temp\opencode\FlowerShop_DB_backup\FlowerShop_DB_pre_refactor.bak` (COPY_ONLY; 4.2 MB), seeded `__EFMigrationsHistory` with the 4 historical migrations (ProductVersion `8.0.23`), then applied:
   ```
   dotnet dotnet-ef database update --project Flower.Data --startup-project Flower.Backend
   ```
   Result: `Done.` — only `20260731154245_RefactorAndRenameTables` applied.

7. **Verified schema + data** (see below). Builds + tests green.

8. **Committed** (see Commits).

## Migration timestamp filename

`Flower.Data/Migrations/20260731154245_RefactorAndRenameTables.cs`
(+ `.Designer.cs`), timestamp lexically AFTER `20260730105026_AddCategoryImageAndAvatar`. ✔

## Deviation from brief (MUST READ)

The brief's verbatim `Up()`/`Down()` call `migrationBuilder.RenameForeignKey(...)`, which **does not exist in EF Core 8.0.23** (only `RenameColumn`/`RenameIndex`/`RenameTable` exist; `RenameForeignKey` was introduced in EF Core 9/10 — verified against the `Microsoft.EntityFrameworkCore.Relational.dll`/`.xml` shipped in the 8.0.23 NuGet package).

Fix: added a tiny private helper `RenameForeignKey(MigrationBuilder, name, table, newName)` that emits provider-aware raw SQL:
- **PostgreSQL:** `ALTER TABLE "<table>" RENAME CONSTRAINT "<old>" TO "<new>";` (works on Render)
- **SQL Server:** `EXEC sp_rename N'<old>', N'<new>', N'OBJECT';` (bare two-part constraint name)

The 6 `RenameForeignKey(...)` call sites in `Up()`/`Down()` are otherwise byte-identical to the brief.

### During the first apply attempt (before the fix) the migration FAILED

- First attempt used the three-part SQL Server form `EXEC sp_rename N'[dbo].[<table>].[<fk>]', ...`. SQL Server raised error **15248** ("@objname is ambiguous or @objtype wrong"), which EF treats as a fatal exception → `database update` aborted mid-migration.
- Investigation proved: (a) renaming an FK *constraint* via `sp_rename` must use the **bare** name; the three-part form errors; (b) EF's own `RenameIndex` uses `sp_rename N'[<table>].[<index>]', ... N'INDEX'` which is correct and works.
- The partial state was rolled back via **restore of the COPY_ONLY backup** (safe: pre-backup rows were empty except `Users`=1, which was confirmed restored), then `__EFMigrationsHistory` was re-seeded and the fixed migration re-applied cleanly.

## Verification output

### Tables (32 + history; old names gone)
`PostCategories`, `ProductCategories`, `CustomerNotifications` present. `Categories`, `CategoriesProducts`, `Notifications` absent. (`__EFMigrationsHistory` now exists.)

### Columns
- `Posts.PostCategoryId` int NOT NULL ✔
- `Products.ProductCategoryId` int NOT NULL ✔
- `ProductVariants.Price` decimal(18,2) ✔ (renamed from `PriceAdjustment`)
- `ProductVariants.Sku` **varchar(50) NULL** ✔ (added; `character varying(50)` stored as `varchar(50)` on SQL Server)
- `PriceAdjustment` gone ✔

### Foreign keys
`FK_Posts_PostCategories_PostCategoryId` ✔, `FK_Products_ProductCategories_ProductCategoryId` ✔, `FK_CustomerNotifications_Customers_CustomerId` ✔.
Untouched: `FK_ProductVariants_Products_ProductId`, `FK_ProductVariants_Products_ProductId1` (shadow FK left as-is per brief).

### Indexes
`IX_Posts_PostCategoryId`, `IX_Products_ProductCategoryId`, `IX_CustomerNotifications_CustomerId`, `IX_CustomerNotifications_CustomerId_IsRead` ✔ (+ pre-existing `IX_Products_Sku`).

### Data preservation
All renamed tables keep identical row counts (0). `Users` still 1 (untouched). No drop+create performed.

### Migration history
```
20260722230554_AddPagesAndContacts|8.0.23
20260729004843_AddProductImages|8.0.23
20260729101500_EnsureProductImagesTable|8.0.23
20260730105026_AddCategoryImageAndAvatar|8.0.23
20260731154245_RefactorAndRenameTables|8.0.23
```

### Builds / tests
- `dotnet build Flower.Data` → Build succeeded, 0 errors
- `dotnet build Flower.Backend` → Build succeeded, 0 errors
- `dotnet test Flower.Tests` → Passed: 37, Failed: 0

## Files changed (committed)
- `.config/dotnet-tools.json` (new — local `dotnet-ef` 8.0.23 manifest)
- `Flower.Data/Migrations/20260731154245_RefactorAndRenameTables.cs` (new)
- `Flower.Data/Migrations/20260731154245_RefactorAndRenameTables.Designer.cs` (new)
- `Flower.Data/Migrations/ApplicationDbContextModelSnapshot.cs` (updated)

## Self-review findings / concerns

1. **[DEV-ONLY] Failed first attempt.** The first `database update` attempt aborted on the three-part `sp_rename` (error 15248). DB was restored from the COPY_ONLY backup and the fixed migration re-applied cleanly. Final DB state is exactly the brief's target. Backup file retained in `%LOCALAPPDATA%\Temp\opencode\FlowerShop_DB_backup\`.

2. **[PRE-EXISTING, NOT BLOCKING] `Avatar` model drift.** The current `User`/`Customer` entities map `Avatar` as `MaxLength(2000)` → the snapshot/Designer now record `Avatar nvarchar(2000)`, but the DB column is still `text` (from the historical migration). The migration does **not** reconcile this (brief's `Up()`/`Down()` don't touch Avatar, and per global constraints the migration must only rename + AddColumn). The scaffolded migration (before replacement) contained `AlterColumn` ops for Avatar that I removed per the brief. If anyone generates the *next* migration, EF will want to emit those `AlterColumn` ops — flag to a later task if the current entities are intended. (No row-count or correctness impact.)

3. **PostgreSQL path is untested here** (Render production not reachable from this task). The Npgsql branch `ALTER TABLE ... RENAME CONSTRAINT` is standard Postgres and matches how the existing Npgsql-typed migrations were authored; the brief explicitly targets provider-agnostic SQL. Recommend a smoke test against Render before the deploy cutover.

4. **`character varying(50)` on SQL Server** resolves to `varchar(50)` (correct). On Postgres it resolves to `varchar(50)` too. Fine.

5. Migration helper uses `migrationBuilder.ActiveProvider` — available in EF Core 8.0.23 (verified). `suppressTransaction: true` is used for the raw `sp_rename` (sp_rename is fine outside the migration transaction; it is a metadata operation).

6. No files under `.superpowers/sdd/` or `docs/` were committed (per constraints).

## Commits
- `feat: add RefactorAndRenameTables migration` (includes `.config/dotnet-tools.json` + 3 migration files)
