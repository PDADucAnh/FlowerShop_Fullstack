# Promotion Business Rules

## Flash Sale

Điều kiện:

CurrentTime >= StartDate

CurrentTime <= EndDate

IsActive = true

---

## Voucher

Điều kiện:

IsActive

StartDate

EndDate

UsageLimit

UsagePerCustomer

MinimumOrderAmount

---

## Promotion Priority

Nếu nhiều Promotion:

Chỉ Promotion có Priority cao nhất được áp dụng.

---

## Stack Promotion

Nếu IsStackable=true

Flash Sale + Voucher

Nếu false

Chỉ Flash Sale.

---

## Validation

Discount không vượt 100%

Discount không âm

EndDate phải lớn hơn StartDate

Coupon Code phải UNIQUE

Promotion phải có Product