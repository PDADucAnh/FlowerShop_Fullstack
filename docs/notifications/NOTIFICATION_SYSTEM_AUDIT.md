# NOTIFICATION SYSTEM ARCHITECTURE AUDIT

## 1. Current Architecture Overview
The current Notification system is fragmented and partially implemented. There is an existing workflow for Admin notifications, but it relies on HTTP polling instead of real-time push. The Customer notification workflow is entirely missing from the frontend and API layers, though a basic database entity exists.

### Existing Entities
- **AdminNotification**: Contains `Id`, `Title`, `Message`, `Type`, `ReferenceId`, `UserId` (Admin), `IsRead`, `CreatedAt`, `CreatedBy`.
- **Notification**: Intended for customers. Contains `Id`, `CustomerId`, `OrderId`, `Title`, `Content`, `Type`, `IsRead`, `CreatedAt`. (Incomplete compared to enterprise requirements).

### Existing Services
- **AdminNotificationService** / `IAdminNotificationService`: Handles CRUD for Admin notifications via EF Core.
- **NotificationService** / `INotificationService`: Currently only used to broadcast a generic SignalR `EntityChanged` string. Does NOT save to database.

### Existing Controllers
- **NotificationController (MVC)**: Located at `Flower.Backend/Controllers/NotificationController.cs`. Used by Admin Views. Includes AJAX endpoints for polling (`GetUnreadCount`, `GetLatest`, `MarkAsRead`, etc.).
- **Missing**: No REST API controller exists in `Flower.Backend/Controllers/Api` for the Next.js Customer frontend.

### Existing SignalR Usage
- **NotificationHub**: Currently an empty Hub class.
- **Usage**: Used only via `NotificationService.NotifyEntityChanged(string entityName)`. This pushes `await _hubContext.Clients.All.SendAsync("EntityChanged", entityName);` which is broadcasted to all clients blindly without Groups or specific payloads.

### Existing Admin Workflow
- The Admin UI (`_LayoutAdmin.cshtml`) contains a Notification Bell and Dropdown.
- It uses JavaScript `setInterval(updateUnreadCount, 60000)` to poll the MVC controller every 60 seconds.
- Clicking the bell fetches the latest notifications via AJAX.

### Existing Customer Workflow
- None. Customers currently have no Notification Center in the Next.js frontend, no backend API to fetch notifications, and no SignalR connection to receive realtime updates.

---

## 2. Problems & Limitations
1. **No Realtime Push for Admin**: Admin UI relies on 60-second polling which causes delays and unnecessary server load. SignalR is barely utilized.
2. **Missing Customer Workflow**: Next.js frontend lacks any notification integration.
3. **Fragmented Schema**: `AdminNotification` and `Notification` are two separate tables with different column names (e.g., `Message` vs `Content`, `ReferenceId` vs `OrderId`).
4. **SignalR Broadcasts Everything**: `NotificationHub` broadcasts to `Clients.All`, which is bad for security and performance. It needs proper Groups (`Admin`, `Customer_{Id}`).
5. **No Extensibility**: Existing schemas lack fields like `Icon`, `NavigationUrl`, `Priority`, `Metadata` required for a Shopee-style enterprise system.

---

## 3. Reuse Opportunities
- **Admin UI**: The existing Notification Bell and Dropdown in `_LayoutAdmin.cshtml` can be reused but must be upgraded from polling to SignalR.
- **Controllers**: The MVC `NotificationController` can be reused to serve the Admin views.
- **Entities**: We can extend the existing `Notification` and `AdminNotification` entities with the missing enterprise fields instead of dropping them, ensuring backward compatibility.

---

## 4. Recommended Enterprise Design

### Database Schema Extension
Extend both `Notification` and `AdminNotification` (or unify them under a base pattern, but to ensure 100% backward compatibility and no broken migration, we will keep both tables and add missing fields).

**Add to `Notification`:**
- `Message` (map to `Content` or keep `Content` and add `NavigationUrl`, `ReferenceType`, `Priority`, `Icon`, `ReadAt`, `Metadata`).

### SignalR Architecture
Upgrade `NotificationHub` to support Groups:
- Admins join `AdminGroup`.
- Customers join `Customer_{CustomerId}`.
- When an event occurs, push exactly to the target group.

### Backend API
Create `Flower.Backend/Controllers/Api/NotificationsController.cs` for the Next.js Customer app:
- `GET /api/notifications` (Paged)
- `GET /api/notifications/unread-count`
- `PUT /api/notifications/{id}/read`
- `PUT /api/notifications/read-all`

### Service Layer
Create an Enterprise `INotificationService` that handles:
1. Validating and saving the Notification to the Database.
2. Pushing the typed payload to `NotificationHub` in real-time.
3. Handling both Customer and Admin logic if possible, or keep `AdminNotificationService` for Admins and use `NotificationService` for Customers.

### Frontend (Next.js)
Build a `NotificationCenter` component matching Shopee's UI. Connect to `NotificationHub` via `@microsoft/signalr`.
