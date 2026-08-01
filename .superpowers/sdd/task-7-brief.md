# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 7: CustomerAddress service + controller (STEP 2)

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

---

## Task 7: CustomerAddress service + controller (STEP 2)

**Files:**
- Create: `Flower.Backend/Models/DTOs/CustomerAddressDTOs.cs`
- Create: `Flower.Backend/Services/Interfaces/ICustomerAddressService.cs`
- Create: `Flower.Backend/Services/CustomerAddressService.cs`
- Create: `Flower.Backend/Controllers/Api/CustomerAddressesController.cs`
- Modify: `Flower.Backend/Program.cs` (DI registration)

**Interfaces:**
- Consumes: `CustomerAddress` entity (unchanged).
- Produces: `ICustomerAddressService` with `GetByCustomerId`, `GetById`, `Create`, `Update`, `Delete`, `SetDefault`; routes under `api/CustomerAddresses`.

- [ ] **Step 1: Create `CustomerAddressDTOs.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class CustomerAddressDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? AddressLine { get; set; }
        public string? PostalCode { get; set; }
        public string? Note { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCustomerAddressDTO
    {
        public int CustomerId { get; set; }

        [MaxLength(200)]
        public string? ReceiverName { get; set; }

        [MaxLength(20)]
        public string? ReceiverPhone { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? Ward { get; set; }

        [MaxLength(500)]
        public string? AddressLine { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
    }

    public class UpdateCustomerAddressDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(200)]
        public string? ReceiverName { get; set; }

        [MaxLength(20)]
        public string? ReceiverPhone { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? Ward { get; set; }

        [MaxLength(500)]
        public string? AddressLine { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
    }
}
```

- [ ] **Step 2: Create `ICustomerAddressService.cs`**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface ICustomerAddressService
    {
        Task<IEnumerable<CustomerAddressDTO>> GetByCustomerId(int customerId);
        Task<CustomerAddressDTO?> GetById(int id);
        Task<CustomerAddressDTO> Create(CreateCustomerAddressDTO dto);
        Task<bool> Update(int id, UpdateCustomerAddressDTO dto);
        Task<bool> Delete(int id);
        Task<bool> SetDefault(int id, int customerId);
    }
}
```

- [ ] **Step 3: Create `CustomerAddressService.cs`**

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Flower.Backend.Services
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly IApplicationDbContext _context;

        public CustomerAddressService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerAddressDTO>> GetByCustomerId(int customerId)
        {
            var list = await _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && a.IsActive)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.Id)
                .ToListAsync();
            return list.Select(ToDTO);
        }

        public async Task<CustomerAddressDTO?> GetById(int id)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            return address == null ? null : ToDTO(address);
        }

        public async Task<CustomerAddressDTO> Create(CreateCustomerAddressDTO dto)
        {
            if (dto.IsDefault)
            {
                var others = _context.CustomerAddresses
                    .Where(a => a.CustomerId == dto.CustomerId && a.IsDefault);
                foreach (var a in others) a.IsDefault = false;
            }

            var address = new CustomerAddress
            {
                CustomerId = dto.CustomerId,
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,
                Province = dto.Province,
                District = dto.District,
                Ward = dto.Ward,
                AddressLine = dto.AddressLine,
                PostalCode = dto.PostalCode,
                Note = dto.Note,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsDefault = dto.IsDefault
            };

            if (!await _context.CustomerAddresses.AnyAsync(a => a.CustomerId == dto.CustomerId))
            {
                address.IsDefault = true;
            }

            _context.CustomerAddresses.Add(address);
            await _context.SaveChangesAsync();
            return ToDTO(address);
        }

        public async Task<bool> Update(int id, UpdateCustomerAddressDTO dto)
        {
            if (id != dto.Id) return false;

            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null) return false;

            if (dto.IsDefault)
            {
                var others = _context.CustomerAddresses
                    .Where(a => a.CustomerId == dto.CustomerId && a.IsDefault && a.Id != id);
                foreach (var a in others) a.IsDefault = false;
            }

            address.CustomerId = dto.CustomerId;
            address.ReceiverName = dto.ReceiverName;
            address.ReceiverPhone = dto.ReceiverPhone;
            address.Province = dto.Province;
            address.District = dto.District;
            address.Ward = dto.Ward;
            address.AddressLine = dto.AddressLine;
            address.PostalCode = dto.PostalCode;
            address.Note = dto.Note;
            address.Latitude = dto.Latitude;
            address.Longitude = dto.Longitude;
            address.IsDefault = dto.IsDefault;
            address.UpdatedAt = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null) return false;

            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefault(int id, int customerId)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null || address.CustomerId != customerId) return false;

            var others = _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && a.IsDefault && a.Id != id);
            foreach (var a in others) a.IsDefault = false;

            address.IsDefault = true;
            address.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private static CustomerAddressDTO ToDTO(CustomerAddress a)
        {
            return new CustomerAddressDTO
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                ReceiverName = a.ReceiverName,
                ReceiverPhone = a.ReceiverPhone,
                Province = a.Province,
                District = a.District,
                Ward = a.Ward,
                AddressLine = a.AddressLine,
                PostalCode = a.PostalCode,
                Note = a.Note,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                IsDefault = a.IsDefault,
                IsActive = a.IsActive
            };
        }
    }
}
```

- [ ] **Step 4: Create `CustomerAddressesController.cs`**

```csharp
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly ICustomerAddressService _addressService;

        public CustomerAddressesController(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("{customerId:int}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            return Ok(await _addressService.GetByCustomerId(customerId));
        }

        [HttpGet("by-id/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var address = await _addressService.GetById(id);
            if (address == null) return NotFound();
            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerAddressDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _addressService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerAddressDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _addressService.Update(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _addressService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPut("{id:int}/set-default")]
        public async Task<IActionResult> SetDefault(int id, [FromQuery] int customerId)
        {
            var updated = await _addressService.SetDefault(id, customerId);
            if (!updated) return NotFound();
            return NoContent();
        }
    }
}
```

- [ ] **Step 5: Register DI (`Program.cs`)**

```csharp
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.ICustomerAddressService, Flower.Backend.Services.CustomerAddressService>();
```

- [ ] **Step 6: Build + test + smoke-test**

```powershell
dotnet build
dotnet test Flower.Tests
```

Expected: build succeeds, `37` tests pass. Manual smoke (with a customer token): `POST /api/CustomerAddresses`, `GET /api/CustomerAddresses/{customerId}`, `PUT /api/CustomerAddresses/{id}/set-default?customerId=1`, `DELETE /api/CustomerAddresses/{id}`.

- [ ] **Step 7: Commit**

```bash
git add Flower.Backend
git commit -m "feat: add customer address API"
```

---