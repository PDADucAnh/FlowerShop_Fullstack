### Task 1: Backend â€” New Service Methods + Controller Actions

**Files:**
- Modify: `Flower.Backend/Services/Interfaces/IOrderService.cs`
- Modify: `Flower.Backend/Services/OrderService.cs`
- Modify: `Flower.Backend/Services/Interfaces/IContactService.cs`
- Modify: `Flower.Backend/Services/ContactService.cs`
- Modify: `Flower.Backend/Models/DTOs/CustomerDTOs.cs`
- Modify: `Flower.Backend/Controllers/Api/OrdersController.cs`
- Modify: `Flower.Backend/Controllers/Api/CustomersController.cs`
- Modify: `Flower.Backend/Controllers/Api/ContactsController.cs`

**Interfaces:**
- Consumes: existing `IOrderService`, `IContactService`, `ICustomerService`, existing `PagedResult<T>`, existing `ContactDTO`, `CustomerDTO`, `OrderDTO`
- Produces: `IOrderService.UpdateStatus(int id, OrderStatus newStatus) â†’ bool`, `IContactService.GetPaged(int page, int pageSize, bool? isRead) â†’ PagedResult<ContactDTO>`, new controller actions listed below

- [ ] **Step 1: Add `UpdateStatus` to IOrderService**

Open `Flower.Backend/Services/Interfaces/IOrderService.cs`. Add after `CancelWithReason` line:

```csharp
Task<bool> UpdateStatus(int id, OrderStatus newStatus);
```

- [ ] **Step 2: Implement `UpdateStatus` in OrderService**

Open `Flower.Backend/Services/OrderService.cs`. Add before the `Delete` method:

```csharp
public async Task<bool> UpdateStatus(int id, OrderStatus newStatus)
{
    var order = await _context.Orders.FindAsync(id);
    if (order == null) return false;

    var oldStatus = order.Status;
    order.Status = newStatus;

    try
    {
        await _context.SaveChangesAsync();

        var statusChangedToConfirmed = oldStatus != OrderStatus.Confirmed && order.Status == OrderStatus.Confirmed;
        var statusChangedToCompleted = oldStatus != OrderStatus.Completed && order.Status == OrderStatus.Completed;
        var statusChangedToShipping = oldStatus != OrderStatus.Shipping && order.Status == OrderStatus.Shipping;

        if (statusChangedToConfirmed || statusChangedToCompleted || statusChangedToShipping)
        {
            await _context.Entry(order).Reference(o => o.Customer).LoadAsync();
            await _context.Entry(order).Collection(o => o.OrderDetails).LoadAsync();
            if (order.OrderDetails != null)
            {
                foreach (var detail in order.OrderDetails)
                {
                    await _context.Entry(detail).Reference(d => d.Product).LoadAsync();
                }
            }

            if (order.Customer != null && !string.IsNullOrEmpty(order.Customer.Email))
            {
                if (statusChangedToConfirmed)
                    await _emailService.SendOrderConfirmedEmailAsync(order, order.Customer.Email, order.Customer.FullName);
                else if (statusChangedToShipping)
                    await _emailService.SendOrderShippingEmailAsync(order, order.Customer.Email, order.Customer.FullName);
                else if (statusChangedToCompleted)
                    await _emailService.SendOrderCompletedEmailAsync(order, order.Customer.Email, order.Customer.FullName);
            }

            if (order.CustomerId > 0)
            {
                var (notifTitle, notifType, notifIcon) = statusChangedToConfirmed
                    ? ($"ÄÆ¡n hÃ ng #{order.Id} Ä‘Ã£ Ä‘Æ°á»£c xÃ¡c nháº­n", "OrderConfirmed", "Verified")
                    : statusChangedToShipping
                        ? ($"ÄÆ¡n hÃ ng #{order.Id} Ä‘ang Ä‘Æ°á»£c giao", "OrderShipping", "LocalShipping")
                        : ($"ÄÆ¡n hÃ ng #{order.Id} Ä‘Ã£ hoÃ n thÃ nh", "OrderCompleted", "CheckCircle");

                await _notificationService.CreateCustomerNotification(
                    customerId: order.CustomerId,
                    title: notifTitle,
                    content: $"Tráº¡ng thÃ¡i Ä‘Æ¡n hÃ ng #{order.Id} Ä‘Ã£ Ä‘Æ°á»£c cáº­p nháº­t.",
                    type: notifType,
                    orderId: order.Id,
                    referenceType: "OrderStatusChanged",
                    icon: notifIcon,
                    priority: "High",
                    navigationUrl: $"/my-orders/{order.Id}"
                );
            }
        }

        if (oldStatus != order.Status && order.CustomerId > 0)
        {
            await _notificationService.NotifyCustomerEvent(order.CustomerId, "OrderChanged", new { orderId = order.Id, status = order.Status.ToString() });
        }

        return true;
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!await _context.Orders.AnyAsync(e => e.Id == id))
            return false;
        throw;
    }
}
```

- [ ] **Step 3: Add `GetPaged` to IContactService**

Open `Flower.Backend/Services/Interfaces/IContactService.cs`. Add after `GetAll` line:

```csharp
Task<PagedResult<ContactDTO>> GetPaged(int page, int pageSize, bool? isRead = null);
```

- [ ] **Step 4: Implement `GetPaged` in ContactService**

Open `Flower.Backend/Services/ContactService.cs`. Add the method:

```csharp
public async Task<PagedResult<ContactDTO>> GetPaged(int page, int pageSize, bool? isRead = null)
{
    IQueryable<Contact> query = _context.Contacts.OrderByDescending(c => c.CreatedAt);

    if (isRead.HasValue)
        query = query.Where(c => c.IsRead == isRead.Value);

    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var dtos = items.Select(c => new ContactDTO
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone,
        Subject = c.Subject,
        Message = c.Message,
        IsRead = c.IsRead,
        ReadAt = c.ReadAt,
        CreatedAt = c.CreatedAt
    }).ToList();

    return new PagedResult<ContactDTO>
    {
        Items = dtos,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

- [ ] **Step 5: Add `CreatedAt` to CustomerDTO**

Open `Flower.Backend/Models/DTOs/CustomerDTOs.cs`. Add to `CustomerDTO`:

```csharp
public DateTime CreatedAt { get; set; }
```

- [ ] **Step 6: Add paginated + status endpoint actions to OrdersController**

Open `Flower.Backend/Controllers/Api/OrdersController.cs`. Add after `GetAll`:

```csharp
[HttpGet("paged")]
public async Task<IActionResult> GetPaged(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? status = null,
    [FromQuery] string? search = null,
    [FromQuery] DateTime? dateFrom = null,
    [FromQuery] DateTime? dateTo = null)
{
    List<OrderStatus>? statuses = null;
    if (!string.IsNullOrEmpty(status))
    {
        var parts = status.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        statuses = new List<OrderStatus>();
        foreach (var part in parts)
        {
            if (Enum.TryParse<OrderStatus>(part, true, out var parsed))
                statuses.Add(parsed);
        }
    }

    var result = await _orderService.GetPaged(page, pageSize, statuses, search, dateFrom, dateTo);
    return Ok(result);
}

[HttpPut("{id}/status")]
public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
{
    var updated = await _orderService.UpdateStatus(id, request.Status);
    if (!updated) return NotFound();
    return NoContent();
}
```

Also add the `UpdateOrderStatusRequest` DTO at the bottom of the file (or in a separate file):

```csharp
public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}
```

Now update `IOrderService` to add the extended `GetPaged` overload. Add to the interface:

```csharp
Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize, List<OrderStatus>? statuses = null, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null);
```

Then update the `OrderService.GetPaged(int page, int pageSize)` implementation to accept the new parameters. Replace the existing `GetPaged` method:

```csharp
public async Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize, List<OrderStatus>? statuses = null, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null)
{
    IQueryable<Order> query = _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
        .Include(o => o.Promotion)
        .Include(o => o.Coupon)
        .OrderByDescending(o => o.OrderDate);

    query = ApplyOwnershipFilter(query);

    if (statuses != null && statuses.Count > 0)
        query = query.Where(o => statuses.Contains(o.Status));

    if (!string.IsNullOrEmpty(search))
        query = query.Where(o =>
            (o.Customer != null && o.Customer.FullName.Contains(search)) ||
            (o.Customer != null && o.Customer.Phone != null && o.Customer.Phone.Contains(search)));

    if (dateFrom.HasValue)
        query = query.Where(o => o.OrderDate >= dateFrom.Value);

    if (dateTo.HasValue)
        query = query.Where(o => o.OrderDate <= dateTo.Value);

    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var dtos = items.Select(o => o.ToDTO()).ToList();
    return new PagedResult<OrderDTO>
    {
        Items = dtos,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

- [ ] **Step 7: Add paginated + orders actions to CustomersController**

Open `Flower.Backend/Controllers/Api/CustomersController.cs`. Add after `GetAll`:

```csharp
[HttpGet("paged")]
public async Task<IActionResult> GetPaged(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? search = null)
{
    var result = await _customerService.GetPaged(page, pageSize, search);
    return Ok(result);
}

[HttpGet("{id}/orders")]
public async Task<IActionResult> GetCustomerOrders(
    int id,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    var result = await _orderService.GetPaged(page, pageSize, customerId: id);
    return Ok(result);
}
```

Now update `ICustomerService` to add the extended `GetPaged` overload. Replace:

```csharp
Task<PagedResult<CustomerDTO>> GetPaged(int page, int pageSize);
```

With:

```csharp
Task<PagedResult<CustomerDTO>> GetPaged(int page, int pageSize, string? search = null);
```

Update `CustomerService.GetPaged(int page, int pageSize)` to accept search:

```csharp
public async Task<PagedResult<CustomerDTO>> GetPaged(int page, int pageSize, string? search = null)
{
    IQueryable<Customer> query = _context.Customers.OrderByDescending(c => c.CreatedAt);

    if (!string.IsNullOrEmpty(search))
    {
        query = query.Where(c =>
            c.FullName.Contains(search) ||
            c.Email.Contains(search) ||
            (c.Phone != null && c.Phone.Contains(search)));
    }

    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var dtos = items.Select(c => new CustomerDTO
    {
        Id = c.Id,
        FullName = c.FullName,
        Email = c.Email,
        Phone = c.Phone,
        Address = c.Address,
        TotalOrders = c.TotalOrders,
        SuccessfulDeliveries = c.SuccessfulDeliveries,
        FailedDeliveries = c.FailedDeliveries,
        IsBlacklisted = c.IsBlacklisted,
        FraudScore = c.FraudScore,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt
    }).ToList();

    return new PagedResult<CustomerDTO>
    {
        Items = dtos,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

Also update `IOrderService` to add customerId filter to `GetPaged`:

```csharp
Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize, List<OrderStatus>? statuses = null, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null, int? customerId = null);
```

And update the `OrderService.GetPaged` implementation to include the customerId filter, add after the `dateTo` filter block:

```csharp
if (customerId.HasValue)
    query = query.Where(o => o.CustomerId == customerId.Value);
```

- [ ] **Step 8: Add paginated endpoint to ContactsController**

Open `Flower.Backend/Controllers/Api/ContactsController.cs`. Add after the `GetUnreadCount` action:

```csharp
[Authorize(Policy = "StaffOnly")]
[HttpGet("paged")]
public async Task<IActionResult> GetPaged(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] bool? isRead = null)
{
    var result = await _contactService.GetPaged(page, pageSize, isRead);
    return Ok(result);
}
```

- [ ] **Step 9: Build and verify backend**

Run:

```bash
cd Flower.Backend
dotnet build
```

Expected: Build succeeded with 0 errors.

---

