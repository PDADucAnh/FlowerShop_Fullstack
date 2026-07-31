# SDD Progress Ledger — refactor/rename-tables (plan 2026-07-31-refactor-and-rename-tables)

Baseline (branch refactor/rename-tables created from main@0a562a6):
- backend build: 0 errors (131 pre-existing warnings)
- Flower.Tests: 37/37 passed

Task 0: complete (controller-verified baseline + branch, no code changes)
Task 1: complete — 9ba9074 + dc61bb1 (concern fix: index names IX_CustomerNotifications_*). Flower.Data builds, 0 stale refs. Spot-checked by controller.
Task 2: complete — 9b02ed2. Concerns reviewed: DTOs live in Flower.Backend (not Flower.Shared), CreatePostDTO/UpdatePostDTO field renames required for compile (OK), DashboardDTOs CategoryName left untouched (correct, out of scope). Spot-checked by controller.
Task 3: complete — 2c29d0f. Extended rename to INotificationService consumers (Customer/OrderCancellation/Payment/OrderService) + OrderDetailService nav so service layer compiles — correct. Spot-checked by controller.
Task 4: complete — 40299f8. GATE PASSED: dotnet build Flower.Backend 0 errors; dotnet test Flower.Tests 37/37. (First attempt interrupted; resumed by fresh implementer which inherited partial renames, reverted out-of-scope CategoriesApiController rename, finished content, committed.)

## ENVIRONMENT REALITY CHECK (controller, pre-Task 5)
- Local DB = SQL Server LocalDB `(localdb)\MSSQLLocalDB` / `FlowerShop_DB` (default provider SqlServer). Production = PostgreSQL (Render).
- LocalDB has 32 tables (old schema: Categories, CategoriesProducts, Notifications) but NO `__EFMigrationsHistory` and ALL rows empty.
- No `ProductImages` table locally (pre-existing drift; migrations AddProductImages/EnsureProductImagesTable were no-op'd/patch-rendered historically) — OUT OF SCOPE.
- Global `dotnet-ef` tool = 10.0.10 (MISMATCH vs EF Core 8.0.23) — will use local tool manifest (dotnet-ef 8.0.23).
- USER DECISION: Option 1 — seed `__EFMigrationsHistory` (4 existing migrations) then `dotnet ef database update` applies only RefactorAndRenameTables; keep migration provider-agnostic (character varying(50)) for SQL Server local + Postgres prod.

Task 5: complete — 39f8311 (migration `20260731154245_RefactorAndRenameTables` + `.config/dotnet-tools.json` pinning dotnet-ef 8.0.23).
- Deviation (reviewed + verified): EF Core 8 has NO `MigrationBuilder.RenameForeignKey` (brief was written for EF 10). Implementer used private provider-agnostic helper (`ALTER TABLE RENAME CONSTRAINT` for Npgsql / `sp_rename` for SqlServer). First apply attempt failed on 3-part sp_rename (15248); restored DB from COPY_ONLY backup, re-applied clean.
- Verified DB LocalDB directly: tables PostCategories/ProductCategories/CustomerNotifications present (old gone), 3 FKs renamed, 4 indexes renamed, ProductVariants.Price + Sku (PriceAdjustment gone), __EFMigrationsHistory = 5 rows. Builds green, 37/37 tests.
- Pre-existing drift noted: User/Customer.Avatar is text in DB vs nvarchar(2000) snapshot — NOT reconciled (out of scope); next migrations add will emit AlterColumns.
- Postgres branch untested locally — recommend Render smoke test later.

Task 6: complete — 2e97a5c. Variant CRUD in ProductService (DTOs, ProductDTO.Variants, 3 service methods, 3 controller endpoints). Build 0 errors, 37/37 tests. Note: implementer accidentally committed `.superpowers/sdd/task-6-report.md` — SDD tracking file; cleanup at final review.
Task 7: complete — 456c1f8. CustomerAddressService + CustomerAddressesController (GetByCustomerId, GetById, Create, Update, Delete hard-delete, SetDefault) + DTOs + mappings + DI. Build 0 errors, 37/37 tests. No SDD files committed.
Task 8: complete — 15da7a3. GET /api/PaymentMethods/active (IsActive==true) + DTO/mapping + controller. Build 0 errors, 37/37 tests.
Task 9: complete — bf9ee57. Admin frontend: api/productCategories.ts, api/postCategories.ts (kept), types postCategory.ts + productCategory.ts, renamed refs in categories/products pages. npm run build + oxlint clean, grep sweep 0 stale. Query key ['categories'] internal — OK.
Task 10: complete — 77524a8. Shop frontend: services/types/useCategories/useNotifications/usePosts/useProducts/useRealtimeUpdates (ProductCategory map) updated. npm run build clean, grep sweep 0 stale. Left categoryName/categoryId cosmetic props (out of scope per brief) — OK.

Task 11: complete — NO code changes (all gates green). Backend 0 errors, 37/37 tests; admin npm run build 0 (3.76s); shop npm run build 0 (1.43s); stale sweep clean (only Migrations/ history + plan-sanctioned categoryProductService module name); DB verified (3 renamed tables, Price/Sku, RefactorAndRenameTables applied, 0 data loss).
- **NEW PRE-EXISTING RISK flagged by Task 11:** `Flower.Backend/Program.cs:426-432` — startup catch-all on ANY migration failure does `db.Database.EnsureDeleted()` + `Migrate()` (drops the WHOLE database, incl. data). Pre-dates refactor (commit 2d9acd5), Postgres-oriented. During the smoke-test attempt it dropped the (empty) LocalDB; implementer fully restored to Task-5 verified state (confirmed via sqlcmd: 5 history rows, renamed tables present). RECOMMENDED as separate follow-up fix (should log + throw, never drop DB).
- Follow-ups outstanding: (1) ~~remove `.superpowers/sdd/task-6-report.md` from git~~ — CANCELLED: verified `.superpowers/sdd/` files are tracked repo-wide (pre-existing practice from prior sessions), so SDD files in commits are intentional; (2) Render/Postgres smoke test for the new migration (Postgres branch untested locally); (3) pre-existing Avatar text-vs-nvarchar drift will emit AlterColumns on next migrations add.

Task 12 (final): complete — EnsureDeleted fix + merge.
- Program.cs startup migration block fixed (commit `3ed2ae4`): removed `db.Database.EnsureDeleted()` + retry (was commit 2d9acd5 behavior); the PostgreSQL-only ProductImages CREATE TABLE safety-net is now guarded by `ProviderName?.Contains("Npgsql")` (else logs skip); catch → `logger.LogCritical(...)` + `throw` (never drops DB). Build 0 errors, 37/37 tests.
- Merged `refactor/rename-tables` → `main` via fast-forward (user choice), 12 commits, 0 conflicts. Pushed: `0a562a6..3ed2ae4 main -> main`. No merge commit created (repo stays linear).

## PLAN COMPLETE — refactor/rename-tables merged into main@3ed2ae4 (pushed)
