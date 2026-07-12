# Promotion API

## Promotion

GET /api/promotions

GET /api/promotions/{id}

POST /api/promotions

PUT /api/promotions/{id}

DELETE /api/promotions/{id}

PATCH /api/promotions/{id}/enable

PATCH /api/promotions/{id}/disable

POST /api/promotions/{id}/products

DELETE /api/promotions/{id}/products/{productId}

---

## Coupon

GET /api/coupons

GET /api/coupons/{id}

POST /api/coupons

PUT /api/coupons/{id}

DELETE /api/coupons/{id}

PATCH /api/coupons/{id}/enable

PATCH /api/coupons/{id}/disable

POST /api/coupons/validate

POST /api/coupons/apply

---

## Response

Tất cả API phải:

- Validate Model
- Trả về HTTP Status đúng chuẩn
- Trả JSON thống nhất
- Xử lý Exception
- Ghi Log khi có lỗi