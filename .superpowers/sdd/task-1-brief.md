# PLAN PREAMBLE (from docs/superpowers/plans/2026-07-31-refactor-and-rename-tables.md)

## Task 1: Rename entities + DbContext (+ ProductVariant columns)

## Global Constraints
- Keep the 4 currently-unused tables **untouched**: `ProductVariants`, `CustomerAddresses`, `PaymentMethods`, `CustomerPaymentPreferences` — they exist to support future features. (Their *entities* may be renamed internally only where specified, and their table names stay.)
- Migration must be **exactly** named `RefactorAndRenameTables` and must preserve all data (use `RenameTable`/`RenameColumn`/`RenameForeignKey`/`RenameIndex`, never drop+create).
- Do **NOT** rename: `Order.PaymentMethod` enum (in `Flower.Data/Entities/Order.cs:26`), the `NotificationHub` SignalR route `/hubs/notifications`, `AdminNotification`/`NotificationController` (MVC admin), the `ProductImage` table, the `Product.ImageUrl` column, the Cloudinary folder constant `CloudinaryFolders.Categories`.
- Route pattern stays `Route("api/[controller]")` unless otherwise stated, so route changes follow controller renames.
- Entity-name strings sent via `NotifyEntityChanged("...")` must match frontend `entityQueryMap` keys in `Flower-shop.frontend/src/hooks/useRealtimeUpdates.ts`.
- Commit style (repo convention): lowercase prefix — `refactor:`, `feat:`, `fix:`.
- Do not add comments to code unless a file already has them in that style.

---

## Task 1: Rename entities + DbContext (+ ProductVariant columns)

**Files:**
- Rename: `Flower.Data/Entities/Category.cs` → `PostCategory.cs`
- Rename: `Flower.Data/Entities/CategoryProduct.cs` → `ProductCategory.cs`
- Rename: `Flower.Data/Entities/Notification.cs` → `CustomerNotification.cs`
- Modify: `Flower.Data/ApplicationDbContext.cs:14,17,34`
- Modify: `Flower.Data/IApplicationDbContext.cs:13,16,33`
- Modify: `Flower.Data/Entities/ProductVariant.cs:17-18` (add `Sku`, rename `PriceAdjustment`→`Price`)
- Modify: `Flower.Data/Entities/Product.cs:35-38` (nav + FK property rename), `Flower.Data/Entities/Post.cs` (nav + FK property rename)

**Interfaces:**
- Consumes: existing entity classes (contents preserved verbatim except names).
- Produces: entity CLR types `PostCategory`, `ProductCategory`, `CustomerNotification`; DbSets `PostCategories`, `ProductCategories`, `CustomerNotifications`; `ProductVariant` with `Price` + `Sku`; nav property names `Post.PostCategory`/`Post.PostCategoryId`, `Product.ProductCategory`/`Product.ProductCategoryId`.

- [ ] **Step 1: Rename `Category.cs` → `PostCategory.cs`**

Create `Flower.Data/Entities/PostCategory.cs` with **exactly** this content (identical to old `Category.cs`, only class name changed):

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class PostCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
```

Then delete `Flower.Data/Entities/Category.cs`.

- [ ] **Step 2: Rename `CategoryProduct.cs` → `ProductCategory.cs`**

Create `Flower.Data/Entities/ProductCategory.cs` with **exactly** this content (identical to old `CategoryProduct.cs`, only class name changed):

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        [MaxLength(2000)]
        public string? ImageUrl { get; set; }

        public virtual ICollection<Product>? Products { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
```

Then delete `Flower.Data/Entities/CategoryProduct.cs`.

- [ ] **Step 3: Rename `Notification.cs` → `CustomerNotification.cs`**

Create `Flower.Data/Entities/CustomerNotification.cs` with **exactly** this content (identical to old `Notification.cs`, only class name changed):

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class CustomerNotification
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int? OrderId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Content { get; set; }

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ReferenceType { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(20)]
        public string? Priority { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? NavigationUrl { get; set; }

        public string? Metadata { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}
```

Then delete `Flower.Data/Entities/Notification.cs`.

- [ ] **Step 4: Update `Post.cs` navigation + FK properties**

Replace the `CategoryId`/`Category` members in `Flower.Data/Entities/Post.cs` (old lines 30-33):

```csharp
        public int PostCategoryId { get; set; }

        [ForeignKey("PostCategoryId")]
        public virtual PostCategory PostCategory { get; set; }
```

- [ ] **Step 5: Update `Product.cs` navigation + FK properties**

In `Flower.Data/Entities/Product.cs:35-38`:
- `public int CategoryProductId { get; set; }` → `public int ProductCategoryId { get; set; }`
- `public virtual CategoryProduct? CategoryProduct { get; set; }` → `public virtual ProductCategory? ProductCategory { get; set; }`

- [ ] **Step 6: Update `ProductVariant.cs`**

```csharp
[Column(TypeName = "decimal(18,2)")]
public decimal Price { get; set; }

[MaxLength(50)]
public string? Sku { get; set; }
```

(Replace `PriceAdjustment` with `Price`; add `Sku` directly after it. The `[ForeignKey("ProductId")]` on the nav stays as-is.)

- [ ] **Step 7: Update `ApplicationDbContext.cs`**

```csharp
public DbSet<PostCategory> PostCategories { get; set; }   // was Categories (line 14)
public DbSet<ProductCategory> ProductCategories { get; set; } // was CategoriesProducts (line 17)
public DbSet<CustomerNotification> CustomerNotifications { get; set; } // was Notifications (line 34)
```

- [ ] **Step 8: Update `IApplicationDbContext.cs`**

Same three renames at lines 13, 16, 33.

- [ ] **Step 9: Commit**

```bash
git add Flower.Data
git commit -m "refactor: rename Category/CategoryProduct/Notification entities"
```

> Note: the solution will NOT compile until Task 4 finishes. Do not stop here — continue through Task 4 before building.

---