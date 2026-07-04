# Phase 2: Backend Data Integrity — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add validation attributes to all entities/DTOs, fix API controller input validation, replace client-side filtering with server-side queries, and improve OrderService exception handling.

**Architecture:** Data annotations on entities + DTOs for model validation, explicit `ModelState.IsValid` checks in API controllers, new service methods for filtered queries, structured exception handling with logging.

**Tech Stack:** .NET 8, EF Core, ASP.NET Core MVC + API

---

### Task 1: Entity Validation Attributes

**Files:**
- Modify: `CMS.Data/Entities/Category.cs`
- Modify: `CMS.Data/Entities/Post.cs`
- Modify: `CMS.Data/Entities/User.cs`
- Modify: `CMS.Data/Entities/Product.cs`

- [ ] **Step 1: Add validation attributes to Category.cs**

```csharp
using System.ComponentModel.DataAnnotations;

// In Category.cs:
[Key]
public int Id { get; set; }

[Required(ErrorMessage = "Tên danh mục không được để trống")]
[MaxLength(200)]
public string Name { get; set; }

[MaxLength(2000)]
public string? Description { get; set; }
```

- [ ] **Step 2: Add validation attributes to Post.cs**

```csharp
[Key]
public int Id { get; set; }

[Required(ErrorMessage = "Tiêu đề không được để trống")]
[MaxLength(500)]
public string Title { get; set; }

[Required(ErrorMessage = "Nội dung không được để trống")]
public string Content { get; set; }

[MaxLength(1000)]
public string ImageUrl { get; set; }
```

- [ ] **Step 3: Add validation attributes to User.cs**

```csharp
[Key]
public int Id { get; set; }

[Required(ErrorMessage = "Tên đăng nhập không được để trống")]
[MaxLength(50)]
public string Username { get; set; }

[Required]
public string PasswordHash { get; set; }

[Required(ErrorMessage = "Họ tên không được để trống")]
[MaxLength(200)]
public string FullName { get; set; }

[Required(ErrorMessage = "Vai trò không được để trống")]
[MaxLength(50)]
public string Role { get; set; }
```

- [ ] **Step 4: Add validation attributes to Product.cs**

```csharp
[Key]
public int Id { get; set; }

[Required(ErrorMessage = "Tên sản phẩm không được để trống")]
[MaxLength(200)]
public string Name { get; set; }

[Required(ErrorMessage = "Giá sản phẩm không được để trống")]
[Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
public decimal Price { get; set; }
```

- [ ] **Step 5: Build and verify**

```bash
dotnet build CMS.Data
dotnet build CMS.Backend
```

Expected: 0 errors

- [ ] **Step 6: Commit**

```bash
git add CMS.Data/Entities/
git commit -m "feat: add validation attributes to Category, Post, User, Product entities"
```

---

### Task 2: Order.Status Magic Number → Enum

**Files:**
- Modify: `CMS.Data/Entities/Order.cs`
- Modify: `CMS.Backend/Services/OrderService.cs`
- Modify: `CMS.Backend/Controllers/OrderController.cs`
- Modify: `CMS.Backend/Models/DTOs/MappingExtensions.cs`
- Modify: `CMS.Backend/Models/DTOs/OrderDTOs.cs`

- [ ] **Step 1: Replace magic number with enum in Order.cs**

```csharp
// CMS.Data/Entities/Order.cs — add enum before Order class:
namespace CMS.Data.Entities;

public enum OrderStatus
{
    Pending = 0,    // Chờ duyệt
    Shipping = 1,   // Đang giao
    Completed = 2   // Đã xong
}

// In Order class, replace:
// public int Status { get; set; } // 0: Chờ duyệt, 1: Đang giao, 2: Đã xong
// with:
[Required]
public OrderStatus Status { get; set; } = OrderStatus.Pending;
```

- [ ] **Step 2: Update OrderService.cs Status references**

Search for all `Status = 0` and replace with `Status = OrderStatus.Pending`. Also check any `Status == 0`, `Status == 1`, `Status == 2` comparisons.

Current location in `OrderService.cs:53`:
```csharp
// Change:
Status = 0,
// To:
Status = OrderStatus.Pending,
```

- [ ] **Step 3: Update OrderDTOs.cs**

If `OrderDTO` or `CreateOrderDTO` or `UpdateOrderDTO` have `int Status`, change to `OrderStatus Status`.

Also update `MappingExtensions.cs` if it maps Status as int.

- [ ] **Step 4: Update OrderController.cs**

Check for any `model.Status != 0` comparisons (line 48):
```csharp
// Change:
if (success && model.Status != 0)
// To:
if (success && model.Status != OrderStatus.Pending)
```

- [ ] **Step 5: Build and verify**

```bash
dotnet build CMS.Data
dotnet build CMS.Backend
```

Expected: 0 errors

- [ ] **Step 6: Commit**

```bash
git add CMS.Data/Entities/Order.cs CMS.Backend/Services/OrderService.cs CMS.Backend/Controllers/OrderController.cs
git commit -m "feat: replace Order.Status magic numbers with OrderStatus enum"
```

---

### Task 3: DTO Validation Attributes

**Files:**
- Modify: `CMS.Backend/Models/DTOs/CategoryDTOs.cs`
- Modify: `CMS.Backend/Models/DTOs/CategoryProductDTOs.cs`
- Modify: `CMS.Backend/Models/DTOs/ProductDTOs.cs`
- Modify: `CMS.Backend/Models/DTOs/CustomerDTOs.cs`
- Modify: `CMS.Backend/Models/DTOs/UserDTOs.cs`
- Modify: `CMS.Backend/Models/DTOs/OrderDTOs.cs`

- [ ] **Step 1: Update CategoryDTOs.cs**

```csharp
public class CategoryDTO
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }
    public string? Description { get; set; }
}

public class CreateCategoryDTO
{
    [Required(ErrorMessage = "Tên danh mục không được để trống")]
    [MaxLength(200)]
    public string Name { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }
}

public class UpdateCategoryDTO
{
    public int Id { get; set; }
    [Required] [MaxLength(200)] public string Name { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
}
```

- [ ] **Step 2: Same pattern for CategoryProductDTOs.cs**

Add `[Required]`, `[MaxLength(200)]` on `Name` in all three DTOs.

- [ ] **Step 3: Update ProductDTOs.cs**

```csharp
public class ProductDTO
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    [Required] public decimal Price { get; set; }
}

public class CreateProductDTO
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [MaxLength(200)]
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }

    [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal Price { get; set; }
}

public class UpdateProductDTO
{
    public int Id { get; set; }
    [Required] [MaxLength(200)] public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    [Required] [Range(0, double.MaxValue)] public decimal Price { get; set; }
}
```

- [ ] **Step 4: Update CustomerDTOs.cs**

```csharp
public class CustomerDTO
{
    public int Id { get; set; }
    [Required] public string FullName { get; set; }
    [Required] [EmailAddress] public string Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class CreateCustomerDTO
{
    [Required] public string FullName { get; set; }
    [Required] [EmailAddress] public string Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    [Required] public string PasswordHash { get; set; }
}

public class UpdateCustomerDTO
{
    public int Id { get; set; }
    [Required] public string FullName { get; set; }
    [Required] [EmailAddress] public string Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? PasswordHash { get; set; }
}
```

- [ ] **Step 5: Update UserDTOs.cs**

```csharp
public class UserDTO
{
    public int Id { get; set; }
    [Required] public string Username { get; set; }
    [Required] public string FullName { get; set; }
    [Required] public string Role { get; set; }
}

public class CreateUserDTO
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [MaxLength(50)]
    public string Username { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [MaxLength(200)]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Vai trò không được để trống")]
    public string Role { get; set; }
}

public class UpdateUserDTO
{
    public int Id { get; set; }
    [Required] [MaxLength(50)] public string Username { get; set; }
    [Required] [MaxLength(200)] public string FullName { get; set; }
    [Required] public string Role { get; set; }
    public string? Password { get; set; }
}
```

- [ ] **Step 6: Update OrderDTOs.cs**

```csharp
// In OrderInputDTO:
[Required(ErrorMessage = "Khách hàng không được để trống")]
[Range(1, int.MaxValue)]
public int CustomerId { get; set; }

// In OrderItemDTO:
[Required] [Range(1, int.MaxValue)] public int ProductId { get; set; }
[Required] [Range(1, 10000)] public int Quantity { get; set; }

// In OrderItemInput:
[Required] [Range(1, int.MaxValue)] public int ProductId { get; set; }
[Required] [Range(1, 10000)] public int Quantity { get; set; }
```

- [ ] **Step 7: Build and verify**

```bash
dotnet build CMS.Backend
```

Expected: 0 errors

- [ ] **Step 8: Commit**

```bash
git add CMS.Backend/Models/DTOs/
git commit -m "feat: add validation attributes to all DTOs"
```

---

### Task 4: API Controller Input Validation

**Files:**
- Modify: `CMS.Backend/Controllers/Api/OrderDetailsController.cs`
- Modify: `CMS.Backend/Controllers/Api/UsersController.cs`
- Modify: `CMS.Backend/Controllers/Api/CustomersController.cs`

- [ ] **Step 1: Fix OrderDetailsController.Create — add [FromBody]**

Read the file and find the `Create(OrderDetailDTO dto)` method. The parameter needs `[FromBody]`:

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] OrderDetailDTO dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    // ... rest unchanged
}
```

- [ ] **Step 2: Fix OrderDetailsController.Update — add ModelState check**

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] OrderDetailDTO dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    // ... rest unchanged
}
```

- [ ] **Step 3: Fix UsersController.Create — add ModelState check**

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    // ... rest unchanged
}
```

- [ ] **Step 4: Fix CustomersController.Create — add ModelState check**

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateCustomerDTO dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    // ... rest unchanged
}
```

- [ ] **Step 5: Build and verify**

```bash
dotnet build CMS.Backend
```

Expected: 0 errors

- [ ] **Step 6: Commit**

```bash
git add CMS.Backend/Controllers/Api/OrderDetailsController.cs CMS.Backend/Controllers/Api/UsersController.cs CMS.Backend/Controllers/Api/CustomersController.cs
git commit -m "fix: add ModelState.IsValid and [FromBody] to API controllers"
```

---

### Task 5: Client-side Filter → Server-side Query

**Files:**
- Modify: `CMS.Backend/Services/Interfaces/IPostService.cs`
- Modify: `CMS.Backend/Services/PostService.cs`
- Modify: `CMS.Backend/Controllers/Api/PostsController.cs`
- Modify: `CMS.Backend/Services/Interfaces/IOrderDetailService.cs`
- Modify: `CMS.Backend/Services/OrderDetailService.cs`
- Modify: `CMS.Backend/Controllers/Api/OrderDetailsController.cs`

- [ ] **Step 1: Add GetByCategory to IPostService**

```csharp
// In IPostService.cs, add:
Task<IEnumerable<PostDTO>> GetByCategory(int categoryId);
```

- [ ] **Step 2: Implement in PostService**

```csharp
public async Task<IEnumerable<PostDTO>> GetByCategory(int categoryId)
{
    var posts = await _context.Posts
        .Where(p => p.CategoryId == categoryId)
        .Include(p => p.Category)
        .ToListAsync();
    return posts.Select(p => p.ToDTO());
}
```

- [ ] **Step 3: Fix PostsController.GetByCategory**

Replace:
```csharp
[AllowAnonymous]
[HttpGet("category/{categoryId}")]
public async Task<IActionResult> GetByCategory(int categoryId)
{
    var posts = await _postService.GetAll();
    return Ok(posts.Where(p => p.CategoryId == categoryId));
}
```

With:
```csharp
[AllowAnonymous]
[HttpGet("category/{categoryId}")]
public async Task<IActionResult> GetByCategory(int categoryId)
{
    var posts = await _postService.GetByCategory(categoryId);
    return Ok(posts);
}
```

- [ ] **Step 4: Add GetByOrderId to IOrderDetailService**

```csharp
// In IOrderDetailService.cs, add:
Task<IEnumerable<OrderDetailDTO>> GetByOrderId(int orderId);
```

- [ ] **Step 5: Implement in OrderDetailService**

```csharp
public async Task<IEnumerable<OrderDetailDTO>> GetByOrderId(int orderId)
{
    var details = await _context.OrderDetails
        .Where(od => od.OrderId == orderId)
        .Include(od => od.Product)
        .ToListAsync();
    return details.Select(od => od.ToDTO());
}
```

- [ ] **Step 6: Fix OrderDetailsController.GetByOrderId**

Replace:
```csharp
[AllowAnonymous]
[HttpGet("order/{orderId}")]
public async Task<IActionResult> GetByOrderId(int orderId)
{
    var details = await _orderDetailService.GetAll();
    return Ok(details.Where(d => d.OrderId == orderId));
}
```

With:
```csharp
[HttpGet("order/{orderId}")]
public async Task<IActionResult> GetByOrderId(int orderId)
{
    var details = await _orderDetailService.GetByOrderId(orderId);
    return Ok(details);
}
```

- [ ] **Step 7: Build and verify**

```bash
dotnet build CMS.Backend
```

Expected: 0 errors

- [ ] **Step 8: Commit**

```bash
git add CMS.Backend/Services/Interfaces/IPostService.cs CMS.Backend/Services/PostService.cs CMS.Backend/Controllers/Api/PostsController.cs CMS.Backend/Services/Interfaces/IOrderDetailService.cs CMS.Backend/Services/OrderDetailService.cs CMS.Backend/Controllers/Api/OrderDetailsController.cs
git commit -m "perf: replace client-side filtering with server-side queries for Posts and OrderDetails"
```

---

### Task 6: OrderService Exception Handling + Customer Validation

**Files:**
- Modify: `CMS.Backend/Services/OrderService.cs`
- Test: `CMS.Tests/OrderServiceTests.cs`

- [ ] **Step 1: Inject ILogger into OrderService**

```csharp
using Microsoft.Extensions.Logging;

// Add field:
private readonly IApplicationDbContext _context;
private readonly ILogger<OrderService> _logger;

// Update constructor:
public OrderService(IApplicationDbContext context, ILogger<OrderService> logger)
{
    _context = context;
    _logger = logger;
}
```

- [ ] **Step 2: Add customer existence check at start of CreateOrder**

After `var newOrder = new Order { ... }`, before the try block:

```csharp
var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
if (!customerExists)
    return (false, "Khách hàng không tồn tại", 0);
```

- [ ] **Step 3: Replace catch-all with specific exception handling**

```csharp
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Database error creating order for customer {CustomerId}", customerId);
    return (false, "Lỗi cơ sở dữ liệu khi tạo đơn hàng", 0);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error creating order for customer {CustomerId}", customerId);
    return (false, "Lỗi không xác định khi tạo đơn hàng", 0);
}
```

- [ ] **Step 4: Build and verify**

```bash
dotnet build CMS.Backend
dotnet test CMS.Tests
```

Expected: 0 errors, all tests pass

- [ ] **Step 5: Commit**

```bash
git add CMS.Backend/Services/OrderService.cs
git commit -m "fix: add customer validation and structured exception logging to OrderService"
```

---

### Task 7: Final Build Verification

- [ ] **Step 1: Build all projects**

```bash
dotnet build CMS.Data
dotnet build CMS.Backend
```

Expected: 0 errors

- [ ] **Step 2: Run all tests**

```bash
dotnet test CMS.Tests
```

Expected: 27 passed, 0 failed

- [ ] **Step 3: Commit any remaining changes**

```bash
git add -A
git status
git commit -m "chore: final verification for Phase 2"
```
