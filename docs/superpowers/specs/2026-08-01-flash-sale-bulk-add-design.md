# Thiết kế — Thêm hàng loạt sản phẩm vào Flash Sale (Bulk Add)

Ngày: 2026-08-01

## Mục tiêu

Cho phép Admin thêm nhiều sản phẩm vào một Flash Sale có sẵn theo 3 phương pháp:
1. Lọc theo ProductCategories (kết hợp `IsActive`).
2. Lọc theo TopBestSeller trong 30 ngày (dựa trên lịch sử OrderDetails) + MinStockQuantity.
3. Đọc file Excel (.xlsx) gồm các cột SKU, Giá Flash Sale, Số lượng.

Mọi phương pháp đều đi qua bước **Preview** (xem trước, chỉnh sửa giá/số lượng inline, xóa dòng) trước khi commit **Bulk Add**.

## Quyết định chính

| Chủ đề | Quyết định |
|---|---|
| Khái niệm "Tag" | Không tồn tại trong codebase → dùng `ProductCategoryIds` + `IsActive` (bỏ IsFeatured/IsNew). |
| Cột `Quantity` | Thêm vào `FlashSaleProduct` (migration `AddFlashSaleProductQuantity`) — giới hạn số lượng bán. |
| Thư viện Excel | Tái sử dụng **EPPlus 8.6.2** (đã là dependency của Flower.Backend, dùng trong ImportService). Không thêm MiniExcel. |
| TopBestSeller | Top N (tham số, mặc định 10) trong 30 ngày, chỉ tính đơn `OrderStatus.Completed`. |
| Cơ chế add | **Append** theo FlashSaleId; nếu sản phẩm đã có → upsert (cập nhật SalePrice/Quantity/DiscountPercent). |
| Cấu trúc API | 3 endpoint preview + 1 endpoint bulk-add chung. |
| Map SKU | Theo `Product.Sku` (không theo ProductVariant.Sku). |
| Sản phẩm đã có trong FlashSale | Preview loại bỏ (chỉ trả sản phẩm mới). |
| DiscountPercent | Backend tự tính lại chính xác từ `SalePrice` gửi lên so với `OriginalPrice`; lưu vào DB. |
| Gợi ý giá mặc định | `SuggestedSalePrice = OriginalPrice × (1 − DefaultDiscountPercent/100)`, `DefaultDiscountPercent` mặc định 15, admin chỉnh được. |

## Thay đổi Backend (.NET Core 8 / EF Core 8)

### Data model
- `Flower.Data/Entities/FlashSaleProduct.cs`: thêm `public int Quantity { get; set; }`.
- Migration `20260801005516_AddFlashSaleProductQuantity`: `AddColumn<int>("Quantity", "FlashSaleProducts", defaultValue: 0)`. Provider-agnostic (`int` tương thích SQL Server + Postgres).

### DTOs (`Flower.Backend/Models/DTOs/FlashSaleDTOs.cs`)
- `FlashSalePreviewRequestDto` — `FlashSaleId` (required), `ProductCategoryIds?`, `MinStockQuantity?`, `TopCount?`, `DefaultDiscountPercent? = 15`.
- `FlashSaleProductPreviewDto` — `ProductId`, `Sku`, `ProductName`, `ProductImageUrl`, `OriginalPrice`, `StockQuantity`, `SuggestedSalePrice`, `Quantity`, `DiscountPercent`.
- `BulkAddFlashSaleProductsDto` — `FlashSaleId` (required) + `Products` (required).
- `BulkAddFlashSaleProductDto` — `ProductId` (required), `SalePrice` (required, ≥ 0), `Quantity`.

### Service (`FlashSaleService` + `IFlashSaleService`)
- `PreviewByCategory(dto)` — lọc `IsActive` + `ProductCategoryIds` (contains), loại bỏ SP đã có, gợi ý giá theo `DefaultDiscountPercent`.
- `PreviewByBestSeller(dto)` — top N (mặc định 10) theo tổng `OrderDetail.Quantity` trong 30 ngày (đơn `Completed`), lọc `MinStockQuantity`, loại bỏ SP đã có.
- `PreviewByExcel(flashSaleId, defaultDiscountPercent, file)` — EPPlus đọc 3 cột (SKU / Giá FS / SL), map `Product.Sku`, SKU không có trong file bỏ qua, dùng giá từ file nếu có, ngược lại gợi ý theo `DefaultDiscountPercent`.
- `BulkAdd(dto)` — upsert FlashSaleProduct; tính lại `DiscountPercent = (Price − SalePrice)/Price × 100`; validate SalePrice ≤ Price, Quantity ≥ 0.
- Helper: `FlashSaleExists`, `GetExistingProductIds`, `NormalizeDiscountPercent` (clamp 0–100), `BuildPreview`.

### Endpoints (`Flower.Backend/Controllers/Api/FlashSalesController.cs`, `[Authorize(Policy = "AdminOnly")]`)
| Method | Route | Body |
|---|---|---|
| POST | `api/FlashSales/preview/category` | `FlashSalePreviewRequestDto` |
| POST | `api/FlashSales/preview/bestseller` | `FlashSalePreviewRequestDto` |
| POST | `api/FlashSales/preview/excel` | multipart: `flashSaleId` + `defaultDiscountPercent` + `file` |
| POST | `api/FlashSales/bulk-add` | `BulkAddFlashSaleProductsDto` → `{ added: n }` |

- `KeyNotFoundException` → 404; `InvalidOperationException` → 400.
- Sau bulk-add thành công: `NotifyEntityChanged("FlashSale")`.

## Thay đổi Frontend Admin (Next.js / React)

### Types (`src/types/flashSale.ts`)
- `FlashSalePreviewRequest`, `FlashSaleProductPreview`, `BulkAddFlashSaleProductRequest`, `BulkAddFlashSaleRequest`.

### API client (`src/api/flashSales.ts`)
- `previewByCategory`, `previewByBestSeller`, `previewByExcel(flashSaleId, file, defaultDiscountPercent?)` (FormData), `bulkAdd`.

### Component mới `src/pages/marketing/BulkAddFlashSaleModal.tsx`
- Props: `open`, `onOpenChange`, `flashSale: FlashSale | null`, `onSuccess`.
- **3 Tabs**: Danh mục (checkbox categories + % giảm), Bán chạy (MinStock, TopCount, % giảm), Excel (file .xlsx + hướng dẫn cột).
- **Preview Table** (hiện sau khi xem trước): Ảnh, Tên, SKU, Giá gốc, **Tồn kho**, Giá Flash Sale (inline Input), Số lượng (inline Input), Giảm % (tính realtime), Xóa dòng.
- Footer: nút "Thêm vào Flash Sale" → `bulkAdd` → toast "Đã thêm N sản phẩm", close, `onSuccess` (invalidate `['flash-sales']`).

### Trigger (`src/pages/marketing/FlashSalesTab.tsx`)
- Thêm nút "Thêm hàng loạt" (icon `ListPlus`) cạnh nút Sửa/Xóa mỗi dòng Flash Sale.

## Verify
- Backend: `dotnet build Flower.Backend` 0 errors; `dotnet test Flower.Tests` 37/37.
- Frontend admin: `npm run build` ✓; `npx oxlint src/pages/marketing` 0 warnings.
- Logic % giảm giá: client tính realtime `(OriginalPrice − SalePrice)/OriginalPrice × 100`; backend tính lại và lưu `DiscountPercent`.
- Map SKU: `Product.Sku` so sánh với cột SKU trong file Excel (case-insensitive).

## Ghi chú
- Migration `AddFlashSaleProductQuantity` chưa apply vào DB (chờ bước deploy/test).
- `FlashSaleProduct.DiscountPercent` trước đây chưa bao giờ được set trong Create — giờ được tính đầy đủ qua BulkAdd; có thể cân nhắc backfill cho dữ liệu cũ sau.
