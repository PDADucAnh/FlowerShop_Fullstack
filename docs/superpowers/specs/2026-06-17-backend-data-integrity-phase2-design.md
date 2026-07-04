# Phase 2: Backend Data Integrity

## Overview

Add validation attributes to all entities and DTOs, fix API controller input validation, replace client-side filtering with server-side queries, and improve exception handling in OrderService.

---

## 1. Entity Validation Attributes

### Category.cs
```csharp
[Required, MaxLength(200)]
public string Name { get; set; }

[MaxLength(2000)]
public string? Description { get; set; }
```

### Post.cs
```csharp
[Required, MaxLength(500)]
public string Title { get; set; }

[Required]
public string Content { get; set; }

[MaxLength(1000)]
public string ImageUrl { get; set; }
```

### User.cs
```csharp
[Required, MaxLength(50)]
public string Username { get; set; }

[Required]
public string PasswordHash { get; set; }

[Required, MaxLength(200)]
public string FullName { get; set; }

[Required, MaxLength(50)]
public string Role { get; set; }
```

### Product.cs
```csharp
[Required, MaxLength(200)]
public string Name { get; set; }

[Required, Range(0, double.MaxValue)]
public decimal Price { get; set; }
```

### Customer.cs (already has [Required], [EmailAddress] — no changes needed)

### Order.Status — Enum Replacement
Replace magic number `int Status` with enum:
```csharp
public OrderStatus Status { get; set; } = OrderStatus.Pending;

public enum OrderStatus
{
    Pending = 0,    // Chờ duyệt
    Shipping = 1,   // Đang giao
    Completed = 2   // Đã xong
}
```

Update references in `OrderService.cs`, `OrderController.cs`, `MappingExtensions.cs`.

---

## 2. DTO Validation Attributes

### CategoryDTOs.cs
- `CreateCategoryDTO.Name`: `[Required(ErrorMessage = "...")]`
- `CreateCategoryDTO.Description`: `[MaxLength(2000)]`
- `UpdateCategoryDTO.Name`: `[Required]`
- `UpdateCategoryDTO.Description`: `[MaxLength(2000)]`
- `CategoryDTO.Name`: `[Required]`

### CategoryProductDTOs.cs
- Same pattern as CategoryDTOs

### ProductDTOs.cs
- `CreateProductDTO.Name`: `[Required]`
- `CreateProductDTO.Price`: `[Range(0, double.MaxValue)]`
- `UpdateProductDTO.Name`: `[Required]`
- `ProductDTO.Name`: `[Required]`

### CustomerDTOs.cs (already has [Required] on FullName, Email — add [EmailAddress] on Email)
- `CreateCustomerDTO.Email`: `[EmailAddress]`
- `UpdateCustomerDTO.Email`: `[EmailAddress]`

### UserDTOs.cs
- `CreateUserDTO.Username`: `[Required, MaxLength(50)]`
- `CreateUserDTO.Password`: `[Required, MinLength(6)]`
- `CreateUserDTO.FullName`: `[Required, MaxLength(200)]`
- `CreateUserDTO.Role`: `[Required]`
- `UpdateUserDTO.Username`: `[Required]`
- `UpdateUserDTO.FullName`: `[Required]`

### OrderDTOs
- `OrderInputDTO.CustomerId`: `[Required, Range(1, int.MaxValue)]`
- `OrderItemDTO.ProductId`: `[Required, Range(1, int.MaxValue)]`
- `OrderItemDTO.Quantity`: `[Required, Range(1, 10000)]`
- `OrderItemInput.ProductId`: `[Required, Range(1, int.MaxValue)]`
- `OrderItemInput.Quantity`: `[Required, Range(1, 10000)]`

---

## 3. API Controller Input Validation

### OrderDetailsController
- `Create(OrderDetailDTO dto)`: Add `[FromBody]` parameter attribute
- `Update(int id, OrderDetailDTO dto)`: Add `ModelState.IsValid` check, return `BadRequest(ModelState)` on invalid

### UsersController
- `Create(CreateUserDTO dto)`: Add `ModelState.IsValid` check (defense-in-depth despite [ApiController] auto-validation)

### CustomersController
- `Create(CreateCustomerDTO dto)`: Add `ModelState.IsValid` check

---

## 4. Client-side Filter → Server-side Query

### PostsController.GetByCategory(int categoryId)
Change from:
```csharp
var posts = await _postService.GetAll();
return Ok(posts.Where(p => p.CategoryId == categoryId));
```
To:
```csharp
var posts = await _postService.GetPostsByCategory(categoryId);
```

Add method to IPostService/PostService:
```csharp
Task<IEnumerable<PostDTO>> GetPostsByCategory(int categoryId);
```

Implementation:
```csharp
public async Task<IEnumerable<PostDTO>> GetPostsByCategory(int categoryId)
{
    var posts = await _context.Posts
        .Where(p => p.CategoryId == categoryId)
        .ToListAsync();
    return posts.Select(p => p.ToDTO());
}
```

### OrderDetailsController.GetByOrderId(int orderId)
Change from:
```csharp
var details = await _orderDetailService.GetAll();
return Ok(details.Where(d => d.OrderId == orderId));
```
To:
```csharp
var details = await _orderDetailService.GetByOrderId(orderId);
```

Add method to IOrderDetailService/OrderDetailService:
```csharp
Task<IEnumerable<OrderDetailDTO>> GetByOrderId(int orderId);
```

Implementation:
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

---

## 5. OrderService Exception Handling

### Inject ILogger<OrderService>
Add constructor parameter and field.

### Replace catch-all with specific handling
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

### Customer existence check
Before creating order, verify customer exists:
```csharp
var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
if (!customerExists)
    return (false, "Khách hàng không tồn tại", 0);
```

---

## Scope

### In Scope
- Entity validation attributes (Category, Post, User, Product, Order)
- DTO validation attributes (all DTOs)
- Order.Status magic number → enum
- API controller input validation fixes
- Client-side filter → server-side query (Posts, OrderDetails)
- OrderService exception logging + customer validation

### Out of Scope (Phase 3-4)
- Frontend bug fixes
- CRA → Vite migration
- Pagination
- Image upload service
- Repository pattern
- TypeScript/PropTypes
