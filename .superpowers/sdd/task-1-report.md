# Task 1 Report — Backend New Service Methods + Controller Actions

## What was implemented

- **IOrderService.cs**: Added `UpdateStatus(int id, OrderStatus newStatus) → bool` and extended `GetPaged` with filters (statuses, search, dateFrom, dateTo, customerId)
- **OrderService.cs**: Implemented `UpdateStatus` with email notifications and customer notifications on status change; updated `GetPaged` with all filter parameters
- **IContactService.cs**: Added `GetPaged(int page, int pageSize, bool? isRead = null) → PagedResult<ContactDTO>`
- **ContactService.cs**: Implemented `GetPaged` with isRead filtering
- **CustomerDTOs.cs**: Added `CreatedAt` property + `using System;`
- **OrdersController.cs**: Added `GET api/orders/paged` (with status/search/date filters) and `PUT api/orders/{id}/status` endpoints; added `UpdateOrderStatusRequest` DTO
- **CustomersController.cs**: Added `GET api/customers/paged` (with search) and `GET api/customers/{id}/orders` endpoints; injected `IOrderService` dependency
- **ContactsController.cs**: Added `GET api/contacts/paged` (with isRead filter)
- **ICustomerService.cs**: Updated `GetPaged` signature to include `search` parameter
- **CustomerService.cs**: Updated `GetPaged` implementation with search filtering and explicit `CustomerDTO` mapping including `CreatedAt`

## Build test results

`dotnet build` from `Flower.Backend/` — **Build succeeded with 0 errors, 111 warnings** (all warnings are pre-existing, not from these changes)

## Files changed

- `Flower.Backend/Services/Interfaces/IOrderService.cs`
- `Flower.Backend/Services/OrderService.cs`
- `Flower.Backend/Services/Interfaces/IContactService.cs`
- `Flower.Backend/Services/ContactService.cs`
- `Flower.Backend/Services/Interfaces/ICustomerService.cs`
- `Flower.Backend/Services/CustomerService.cs`
- `Flower.Backend/Models/DTOs/CustomerDTOs.cs`
- `Flower.Backend/Controllers/Api/OrdersController.cs`
- `Flower.Backend/Controllers/Api/CustomersController.cs`
- `Flower.Backend/Controllers/Api/ContactsController.cs`

## Self-review findings

- `UpdateStatus` in `OrderService` mirrors the same pattern from `OrderService.Update()` — handles email notifications and customer notifications for confirmed/shipping/completed transitions
- `UpdateOrderStatusRequest` is placed in the namespace alongside `OrdersController` (as specified by the brief)
- `CustomerDTO.CreatedAt` requires `using System;` — added it
- `CustomersController` now has both `ICustomerService` and `IOrderService` injected for the customer orders endpoint
- Extended `IOrderService.GetPaged` signature includes all optional parameters, maintaining backward compatibility via default values (`null`)

## Issues or concerns

- `IOrderService` now uses `List<OrderStatus>` which requires `using Flower.Data.Entities;` — already present
- The existing `OrderService.GetPaged` overload (2-param) was replaced with the 7-param overload; the old 2-param version no longer exists (callers need updating)
