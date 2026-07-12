# Promotion Backend

Triển khai đầy đủ Service cho Promotion.

## Services

PromotionService

CouponService

PriceCalculationService

PromotionScheduler

---

## Repository

PromotionRepository

CouponRepository

CouponUsageRepository

---

## DTO

PromotionDTO

CouponDTO

ApplyCouponRequest

ApplyCouponResponse

PromotionResponse

---

## Dependency Injection

Đăng ký toàn bộ Service vào DI.

Không thay đổi cấu trúc hiện tại.

---

## Price Calculation

Backend luôn tính giá theo thứ tự:

Original Price

↓

Flash Sale

↓

Voucher

↓

Shipping

↓

Total

Không tính ở Frontend.

---

## Logging

Log:

Promotion Created

Promotion Updated

Coupon Applied

Coupon Rejected

Promotion Expired