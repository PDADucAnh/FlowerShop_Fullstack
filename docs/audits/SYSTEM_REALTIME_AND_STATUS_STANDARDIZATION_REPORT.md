# SYSTEM REALTIME AND STATUS STANDARDIZATION REPORT

## 1. Overview
The goal of this architectural update was to completely standardize the realtime event integration (SignalR) and synchronize frontend state management (React Query) without modifying the database schema or REST API contracts. Furthermore, all scattered status terminologies were unified across the codebase.

## 2. Root Cause Analysis
Prior to this update, the application suffered from the following fundamental flaws:
1. **Manual Polling vs. Realtime**: The frontend relied heavily on manual data refreshing. When a user completed checkout or the admin updated settings, the frontend state became stale, requiring a page reload.
2. **Hardcoded and Inconsistent Status Texts**: Texts like "Chờ xác minh", "Chờ xử lý" were scattered across both Backend DTOs and Frontend mappers, leading to mismatched statuses on the user profile vs. the admin panel.
3. **Decoupled Notification & Entity States**: Changing a System Setting did not broadcast to active users. Locking a customer did not log them out in realtime.

## 3. SignalR Architecture Integration
The `NotificationHub` was extended to serve as a unified event bus for frontend clients.

### 3.1. Hub Groups
- `AdminGroup`: For broadcasting events explicitly to admins.
- `Customer_{id}`: Dedicated group for user-specific events.

### 3.2. Events Implemented
- `OrderChanged(data)`: Dispatched by `OrderService` and `PaymentService` when order status or payment status changes.
- `EntityChanged(entityName)`: Dispatched for generic entities. Specifically added for `SystemSettings` in `SettingsController`.
- `CustomerLocked(data)`: Dispatched when an admin deactivates a user account, forcing immediate logout.
- `AdminAnnouncement(data)`: Global push messages from admin to active users.
- `ReceiveNotification(notification)`: Standard personalized notifications.

## 4. React Query Invalidation Strategy
To eliminate manual polling, `useNotifications` and `useRealtimeUpdates` hook directly into SignalR.
Instead of mutating state locally, the hooks aggressively invoke `queryClient.invalidateQueries(key)`.

- `SystemSettings` triggers `invalidateQueries({ queryKey: ['settings'] })`.
- `OrderChanged` triggers `invalidateQueries({ queryKey: ['orders'] })` and `['order', id]`.

## 5. UI Enhancements (Toast & Badges)
1. **Toast Notifications**: Integrated `react-hot-toast` for non-intrusive alerts (`AdminAnnouncement`, `ReceiveNotification`).
2. **Notification Bell**: Created `NotificationBell.tsx` to display unread badge counts, dropdown history, and Mark as Read functionality.
3. **100% Tiếng Việt**: Translated all remaining hardcoded English text (e.g., date formats, navigation links) in the Next.js frontend to `vi-VN`.

## 6. Zero Regression Constraints Met
- Database schema remained untouched.
- Existing Payment flows (VNPay, COD) remain fully intact and operational.
- REST API boundaries were preserved; SignalR acts strictly as a cache-invalidation signal layer.
