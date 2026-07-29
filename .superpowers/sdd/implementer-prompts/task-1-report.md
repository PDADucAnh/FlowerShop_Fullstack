# Task 1 Report — Pagination + Settings Write Endpoints

## Implemented

### Interfaces (5 files)
- Added `Task<PagedResult<AdvertisementDTO>> GetPaged(int page, int pageSize)` to `IAdvertisementService.cs`
- Added `Task<PagedResult<PageDTO>> GetPaged(int page, int pageSize)` to `IPageService.cs`
- Added `Task<PagedResult<PromotionCampaignDTO>> GetPaged(int page, int pageSize)` to `IPromotionService.cs`
- Added `Task<PagedResult<CouponDTO>> GetPaged(int page, int pageSize)` to `ICouponService.cs`
- Updated `IPostService.GetPaged` signature to include `string? search = null` parameter

### Service Implementations (5 files)
- Implemented `GetPaged` in `AdvertisementService.cs`, `PageService.cs`, `PromotionService.cs`, `CouponService.cs` — each queries its respective DbSet with `OrderByDescending`, skip/take paging, and returns `PagedResult<T>`
- Replaced `PostService.GetPaged` with search-enabled version that filters on `Title` and `Summary`

### Controller Actions (7 files)
- Added `[HttpGet("paged")]` to `AdvertisementsController`, `PagesController`, `PromotionsController`, `CouponsController` — all return `PagedResult<T>`
- Updated `PostsController.GetPaged` to accept optional `search` query parameter
- Added `[HttpPut("header")]` and `[HttpPut("footer")]` to `LayoutApiController`
- Replaced `SettingsApiController` with full implementation: `GET /api/settings`, `PUT /api/settings/store-info`, `PUT /api/settings/smtp`, `PUT /api/settings/vnpay`, `PUT /api/settings/shipping`, `PUT /api/settings/order`

## Build Result
**Build succeeded with 0 errors** (131 warnings, all pre-existing).

## Files Changed (17 files)
```
Flower.Backend/Services/Interfaces/IAdvertisementService.cs
Flower.Backend/Services/Interfaces/IPageService.cs
Flower.Backend/Services/Interfaces/IPromotionService.cs
Flower.Backend/Services/Interfaces/ICouponService.cs
Flower.Backend/Services/Interfaces/IPostService.cs
Flower.Backend/Services/AdvertisementService.cs
Flower.Backend/Services/PageService.cs
Flower.Backend/Services/PromotionService.cs
Flower.Backend/Services/CouponService.cs
Flower.Backend/Services/PostService.cs
Flower.Backend/Controllers/Api/AdvertisementsController.cs
Flower.Backend/Controllers/Api/PagesController.cs
Flower.Backend/Controllers/Api/PromotionsController.cs
Flower.Backend/Controllers/Api/CouponsController.cs
Flower.Backend/Controllers/Api/PostsController.cs
Flower.Backend/Controllers/Api/LayoutApiController.cs
Flower.Backend/Controllers/Api/SettingsApiController.cs
```

##  Commits Created
- `13ff0d4` — `feat(backend): add pagination + settings write endpoints for Phase 4`

## Issues / Concerns
- The solution file is named `Flower-Shop.sln` (not `FlowerShop.sln`). The solution contains a website project `Flower-shop.frontend` that cannot be built with the `dotnet` CLI (ASP.NET compiler only works on .NET Framework MSBuild). The backend project builds cleanly independently.
- No concerns with the implementation itself — all code matched the plan exactly.
