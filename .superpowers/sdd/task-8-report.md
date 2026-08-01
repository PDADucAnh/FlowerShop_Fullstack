# Task 8 Report: PaymentMethods `active` endpoint

**Status:** DONE
**Commit:** `15da7a3` — `feat: add payment methods active endpoint`

## What was implemented

Added a public GET `/api/PaymentMethods/active` endpoint that returns all active payment methods ordered by `DisplayOrder` then `Id`, exactly per the task brief:

1. **Created `Flower.Backend/Models/DTOs/PaymentMethodDTOs.cs`** — `PaymentMethodDTO` (Id, Code, Name, Description, IsOnline, IsActive, DisplayOrder), verbatim from brief.
2. **Modified `Flower.Backend/Models/DTOs/MappingExtensions.cs`** — added `PaymentMethodDefinition.ToDTO()` extension, verbatim from brief (appended after the `Contact` mappings).
3. **Created `Flower.Backend/Controllers/Api/PaymentMethodsController.cs`** — `[Route("api/[controller]")]`, `[ApiController]`, injects `IApplicationDbContext`, `[AllowAnonymous]` `[HttpGet("active")]` returns `Ok(methods.Select(m => m.ToDTO()))` with `Where(m => m.IsActive)`, verbatim from brief.

No service/interface/DI additions were required: `IApplicationDbContext` is already registered in `Program.cs:184` and this direct-injection controller pattern matches existing controllers (`OrdersController`, `ProductsController`).

## Verification

- `dotnet build Flower.Backend` → **Build succeeded, 0 errors** (131 warnings, all pre-existing — nullable-reference and SixLabors.ImageSharp NU1902/NU1903 advisory, none introduced by this task).
- `dotnet test Flower.Tests` → **Passed: 37, Failed: 0, Skipped: 0** (baseline 37).
- Grep of touched files confirms `PaymentMethodDTO`, `PaymentMethodDefinition.ToDTO`, and `PaymentMethodsController` present and referenced as expected.

## Files changed

- `Flower.Backend/Models/DTOs/PaymentMethodDTOs.cs` (new)
- `Flower.Backend/Controllers/Api/PaymentMethodsController.cs` (new)
- `Flower.Backend/Models/DTOs/MappingExtensions.cs` (modified)

Commit contains only these 3 files. Nothing under `.superpowers/` was staged or committed.

## Self-review findings / concerns

- Scope respected: no migration created; `Order.PaymentMethod` enum untouched; `PaymentMethods` table untouched.
- Brief code used verbatim, including the `.ToDTO()` method lacking a null-guard (consistent with the DB-entity use case here; controller maps only non-null queried entities). Other `ToDTO` overloads in the file use a `if (x == null) return null;` guard, but the brief specified this exact body — followed the brief.
- No comments added (matches repo style).
- Smoke test not run against a live DB (no local run requested; build + unit tests cover compilation and regression). `GET /api/PaymentMethods/active` behavior (only `isActive: true`, ordered by `displayOrder`) is fully expressed by the LINQ in the controller.
