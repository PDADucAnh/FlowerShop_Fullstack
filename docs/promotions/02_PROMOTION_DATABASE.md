# Promotion Database

## Tạo bảng PromotionCampaigns

Các cột

Id

Name

Description

PromotionType

DiscountType

DiscountValue

StartDate

EndDate

Priority

BannerImage

IsStackable

IsActive

CreatedAt

UpdatedAt

PromotionType:

- FlashSale
- Seasonal

DiscountType:

- Percent
- FixedAmount

---

## Tạo bảng PromotionProducts

Id

PromotionId

ProductId

CreatedAt

Một Promotion có nhiều Product.

Một Product có thể thuộc nhiều Promotion.

Nếu nhiều Promotion cùng áp dụng thì chọn Priority cao nhất.

---

## Tạo bảng Coupons

Id

Code

Description

DiscountType

DiscountValue

MinimumOrderAmount

MaximumDiscountAmount

UsageLimit

UsedCount

UsagePerCustomer

CustomerId (nullable)

StartDate

EndDate

IsPublic

IsActive

CreatedAt

UpdatedAt

---

## Tạo bảng CouponUsages

Id

CouponId

CustomerId

OrderId

DiscountAmount

UsedAt

---

## Entity Framework

CLI phải tạo:

- Entity
- Fluent API
- DbSet
- Migration
- Relationship

Không sửa Entity cũ.