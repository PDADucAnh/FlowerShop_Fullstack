# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 8: PaymentMethods `active` endpoint (STEP 2)

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

---

## Task 8: PaymentMethods `active` endpoint (STEP 2)

**Files:**
- Create: `Flower.Backend/Models/DTOs/PaymentMethodDTOs.cs`
- Modify: `Flower.Backend/Models/DTOs/MappingExtensions.cs` (add `PaymentMethodDefinition.ToDTO()`)
- Create: `Flower.Backend/Controllers/Api/PaymentMethodsController.cs`

**Interfaces:**
- Consumes: `PaymentMethodDefinition` entity + `IApplicationDbContext`, `PaymentMethodDTO` (this task).
- Produces: `GET api/PaymentMethods/active` → `IEnumerable<PaymentMethodDTO>` where `IsActive == true`.

- [ ] **Step 1: Create `PaymentMethodDTOs.cs`**

```csharp
namespace Flower.Backend.Models.DTOs
{
    public class PaymentMethodDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsOnline { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
```

> Add `PaymentMethodDefinition.ToDTO()` to `MappingExtensions.cs` (Step 2).

- [ ] **Step 2: Add `PaymentMethodDefinition.ToDTO()` to `MappingExtensions.cs`**

```csharp
public static PaymentMethodDTO ToDTO(this PaymentMethodDefinition m)
{
    return new PaymentMethodDTO
    {
        Id = m.Id,
        Code = m.Code,
        Name = m.Name,
        Description = m.Description,
        IsOnline = m.IsOnline,
        IsActive = m.IsActive,
        DisplayOrder = m.DisplayOrder
    };
}
```

- [ ] **Step 3: Create `PaymentMethodsController.cs`**

```csharp
using Flower.Backend.Models.DTOs;
using Flower.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public PaymentMethodsController(IApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var methods = await _context.PaymentMethods
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.Id)
                .ToListAsync();

            return Ok(methods.Select(m => m.ToDTO()));
        }
    }
}
```

- [ ] **Step 4: Build + test + smoke-test**

```powershell
dotnet build
dotnet test Flower.Tests
```

Expected: build succeeds, `37` tests pass. Manual smoke: `GET /api/PaymentMethods/active` returns only methods with `isActive: true`, ordered by `displayOrder`.

- [ ] **Step 5: Commit**

```bash
git add Flower.Backend
git commit -m "feat: add payment methods active endpoint"
```

---