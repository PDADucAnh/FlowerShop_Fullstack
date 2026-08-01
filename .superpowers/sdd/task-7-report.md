# Task 7 Report: CustomerAddress service + controller (STEP 2)

## Status: DONE

## What I implemented

Built the `CustomerAddresses` feature exactly per the brief (verbatim code):

- **`Flower.Backend/Models/DTOs/CustomerAddressDTOs.cs`** (new) — `CustomerAddressDTO`, `CreateCustomerAddressDTO`, `UpdateCustomerAddressDTO` with the exact `[MaxLength]` annotations from the brief.
- **`Flower.Backend/Services/Interfaces/ICustomerAddressService.cs`** (new) — interface with `GetByCustomerId`, `GetById`, `Create`, `Update`, `Delete`, `SetDefault`.
- **`Flower.Backend/Services/CustomerAddressService.cs`** (new) — implementation against `IApplicationDbContext` (`_context.CustomerAddresses`). Default-address handling: un-set other defaults when creating/updating/setting a default, and first address of a customer auto-becomes default. Hard-delete via `Remove` (per brief).
- **`Flower.Backend/Controllers/Api/CustomerAddressesController.cs`** (new) — `[Authorize]` `api/CustomerAddresses` controller with `GET {customerId}`, `GET by-id/{id}`, `POST`, `PUT {id}`, `DELETE {id}`, `PUT {id}/set-default?customerId=`.
- **`Flower.Backend/Program.cs`** (modified) — DI: `AddScoped<ICustomerAddressService, CustomerAddressService>()`, inserted after the `ICustomerService` registration.

No migration created; the `CustomerAddress` entity, `CustomerAddresses` DbSet, and DbContext config were all already present and untouched.

## Verification

- `dotnet build Flower.Backend` → **0 errors** (131 pre-existing warnings, none from new code).
- `dotnet test Flower.Tests` → **37 passed, 0 failed** (baseline matches).
- Grep on all touched files confirmed every brief symbol/route is present.
- Committed only code files; `.superpowers/` and `docs/` NOT staged.

## Files changed

- Created: `Flower.Backend/Models/DTOs/CustomerAddressDTOs.cs`
- Created: `Flower.Backend/Services/Interfaces/ICustomerAddressService.cs`
- Created: `Flower.Backend/Services/CustomerAddressService.cs`
- Created: `Flower.Backend/Controllers/Api/CustomerAddressesController.cs`
- Modified: `Flower.Backend/Program.cs` (one DI line)

## Commit

- `456c1f8` feat: add customer address service and controller

## Self-review findings / concerns

- **Brief-verbatim**: All code copied exactly from the brief (the brief's own Step 7 commit message differs from the dispatch instructions; used the dispatch message `feat: add customer address service and controller`).
- **Delete is hard-delete** even though the entity has `IsActive` — matches the brief's service code exactly; a soft-delete would be a future enhancement.
- **No authorization/ownership scoping beyond customerId matching** in `SetDefault`/ownership — matches brief; `[Authorize]` only. Noted as-is since the brief mandates the exact code.
- Manual smoke test not performed (no running app/customer token) — build + unit tests pass, endpoints match brief.
