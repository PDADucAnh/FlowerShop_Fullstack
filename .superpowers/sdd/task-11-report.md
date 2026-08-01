# Task 11 Report: Final verification

**Date:** 2026-07-31
**Status:** DONE_WITH_CONCERNS (E2E smoke blocked by a pre-existing LocalDB/PostgreSQL startup bug; DB restored to Task-5 state)

## Summary

Task 11 is verification-only. All build/test/frontend/DB gates **pass**. The brief's Step 3 (E2E smoke) **could not be executed** because the backend cannot start against LocalDB (SQL Server) — a **pre-existing** startup defect unrelated to this refactor (added in commit `2d9acd5`, 2026-07-29, before the refactor began). Attempting to start it dropped the (empty) LocalDB; it was **fully restored** to Task 5's verified state and re-verified. No code changes were made, so no commit was created.

## Step 1: Backend build + tests — PASS

- `dotnet build Flower.Backend` → **0 errors, 131 warnings** (all pre-existing nullable-annotation warnings; none introduced by the refactor).
- `dotnet test Flower.Tests` → **Passed: 37, Failed: 0, Skipped: 0** (matches expected 37).

## Step 2: Both frontends typecheck — PASS

- `flower-admin.frontend` → `tsc -b && vite build` **exit 0**, `✓ built in 3.76s` (3131 modules). Only pre-existing chunk-size advisory + Rolldown plugin-timings notice.
- `Flower-shop.frontend` → `tsc -b && vite build` **exit 0**, `✓ built in 1.43s` (359 modules). Only pre-existing `@microsoft/signalr` `/*#__PURE__*/` annotation notice (non-fatal).

## Step 3: End-to-end smoke — NOT RUN (pre-existing blocker, investigated)

`dotnet run` was attempted. Startup crashed inside the DB-init block in `Flower.Backend/Program.cs`:

- Line 409-423 runs an **unconditional `ExecuteSqlRaw` of Postgres-only DDL** (`CREATE TABLE IF NOT EXISTS "ProductImages" ... SERIAL ... NOW()`).
- On LocalDB (SQL Server) that SQL always throws ("Incorrect syntax near the keyword 'IF'").
- The surrounding `catch` (line 426-432) then calls `db.Database.EnsureDeleted()` (drops the DB) and re-runs `Migrate()`, which fails again because the historical EF migrations were regenerated as Npgsql-flavored (`character varying`, `boolean`, `timestamp without time zone`) in commit `afc6dd0` (2026-07-23) — invalid T-SQL on a fresh SQL Server DB.

**This is pre-existing, not a regression:** both commits (`2d9acd5` safety-net, `afc6dd0` Postgres migrations) predate the refactor (Tasks 1-10, all 2026-07-31). It is also consistent with the repo's own history — Tasks 6, 7 and 8 all explicitly recorded "smoke test not run (no running app)" for the same reason.

**DB impact & restoration:** The `EnsureDeleted()` path dropped `FlowerShop_DB`. The database was empty except `Users=1` (all business tables 0 rows), so **no data was lost**. It was restored from Task 5's `COPY_ONLY` backup (`%LOCALAPPDATA%\Temp\opencode\FlowerShop_DB_backup\FlowerShop_DB_pre_refactor.bak`), the 4 historical migrations were re-seeded into `__EFMigrationsHistory`, and `RefactorAndRenameTables` was re-applied via `dotnet dotnet-ef database update` (local tool 8.0.23). The restored state is byte-for-byte the Task-5 verified state (verified below).

Because the app cannot start on LocalDB, none of the smoke assertions (PostCategories/ProductCategories data, CustomerNotifications, productCategory filter, PaymentMethods/active, variant/address CRUD, UI loads) could be exercised with HTTP calls. Per the task's rule ("if a fix is non-trivial, STOP and report instead"), I did **not** alter the pre-existing startup code — fixing it is out of scope for a verification task and risks the Render/PostgreSQL path.

## Step 4: Stale-identifier sweep — PASS

Swept all code (excluding `node_modules`, `Migrations/`, `dist/`, `.git/`, `.superpowers/`, `docs/`):

| Check | Result |
|---|---|
| `CategoryProduct` / `CategoriesProducts` (case-sensitive) | **Clean** in all code. Only hits are: (a) `Migrations/*` historical files (allowed), (b) the shop frontend service **filename/variable** `categoryProductService` (see note), (c) `docs/` + `.superpowers/` historical/plan files. |
| bare `categoryProductId` | **Clean** in all code (only `docs/` plan/spec files). |
| `/api/Categories`, `/api/notifications` (customer), `/api/CategoriesProducts` URLs in frontends | **Clean** — zero hits in both frontends. |
| `NotificationsController` (customer) | **Clean** — only `AdminNotificationsController` + `CustomerNotificationsController` exist. MVC admin `NotificationController` untouched (allowed). |
| `PriceAdjustment` | **Clean** — zero hits in all code. |

Note on the one remaining literal `categoryProduct` substring: `Flower-shop.frontend/src/services/categoryProductService.ts` (filename + the `categoryProductService` const) and its import in `hooks/useCategories.ts`. The plan explicitly scoped this file as "Modify" (not rename) in Task 10, its content is fully renamed (`/ProductCategories` URL, `getAllProductCategories()` method), and it is not in the plan's exit-criteria list. It is an internal module/variable name only (no API/route/type surface). Left untouched as plan-specified.

## Step 5: DB verification (sqlcmd, LocalDB `FlowerShop_DB`) — PASS

- Tables present: `PostCategories`, `ProductCategories`, `CustomerNotifications`, `ProductVariants`.
- Old tables absent: `Categories`, `CategoriesProducts`, `Notifications`.
- `ProductVariants` columns: `Price` (renamed from `PriceAdjustment`) + `Sku` present; `PriceAdjustment` gone.
- FKs: `FK_Posts_PostCategories_PostCategoryId`, `FK_Products_ProductCategories_ProductCategoryId`, `FK_CustomerNotifications_Customers_CustomerId` present; `FK_ProductVariants_Products_ProductId1` (shadow) untouched as required.
- Indexes: `IX_Posts_PostCategoryId`, `IX_Products_ProductCategoryId`, `IX_CustomerNotifications_CustomerId`, `IX_CustomerNotifications_CustomerId_IsRead` present.
- Migration history: all 5 entries, `...RefactorAndRenameTables` last.
- Row counts: all renamed tables 0 (matching pre-refactor), `Users` = 1. **No data loss.**
- Known dev-env state (pre-existing, matches Task 5): `ProductImages` table does not exist on LocalDB — its `AddProductImages` migration was recorded as a no-op and only the (Postgres-only) startup safety-net was meant to create it. This is unrelated to the renames.

## Fixes & commits

**None.** No code changes were made; no commit was created. (Restoring the LocalDB is an environment action, not a repo change.)

## Remaining concerns (pre-existing / out of scope)

1. **Backend cannot start on LocalDB (SQL Server).** Program.cs:409-423 runs Postgres-only DDL unconditionally, and its `catch` (line 428-430) drops the DB before retrying. On Postgres (Render) this path works; on SQL Server it self-destructs. Pre-dates the refactor. Recommended follow-up: guard the safety-net `ExecuteSqlRaw` with a provider check (e.g. only run when `Database.ProviderName` is Npgsql), and remove/soften the `EnsureDeleted()` fallback. **Do this in a separate non-verification task; it touches production startup logic.**
2. Because of (1), the E2E smoke test could not run against LocalDB; it should be run against the Postgres target (Render) or after the startup fix.
3. `Avatar` column model drift (Task 5 note 2) — unchanged, still pending reconciliation if a future migration is generated.
4. `ProductImages` table absent on LocalDB (dev-env only; see above).
5. `ProductVariants.ProductId1` shadow FK deliberately left (per brief).
6. Duplicate blog-category API controller pair (`CategoriesController` + `CategoriesApiController`, routes `api/PostCategories` + `api/postcategories`) left as-is per brief.

## Exit-criteria re-check

- [x] Tables renamed (data intact): `PostCategories`, `ProductCategories`, `CustomerNotifications`.
- [x] `ProductVariants` has `Price` (old values) + `Sku`.
- [x] No stale identifiers in code (grep-clean; only historical `Migrations/`, `docs/`, `.superpowers/`, and the plan-sanctioned `categoryProductService` module name remain).
- [x] `dotnet build` + `dotnet test` pass (37 tests).
- [x] Both frontends `npm run build` clean (exit 0).
- [ ] New endpoints functional — **code-verified only** (variant CRUD, address CRUD + set-default, `/api/PaymentMethods/active`); runtime smoke blocked by (1).
