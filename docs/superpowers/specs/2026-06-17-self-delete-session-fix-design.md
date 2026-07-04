# Self-Delete & Session Fix Design

## Problem

When an admin user deletes their own account via `UserController.Delete(id)`, the auth cookie is not invalidated. The user remains logged in with a valid session even after their database record is deleted — a "zombie session" (STIG V-222549).

## Approach

Enterprise pattern (validated against Better Auth, OWASP ASVS, STIG, Ping Identity):

1. **Prevent admin self-deletion** — guard in `UserController.Delete` that returns an error if the target user matches the currently logged-in user
2. **Auto-logout on self-delete bypass** — if the currently logged-in user's account is somehow deleted (e.g., direct DB manipulation), ensure their session is terminated

## Affected Files

| File | Change |
|------|--------|
| `CMS.Backend/Controllers/UserController.cs` | Add `IHttpContextAccessor` dependency; add self-delete check before calling service |

## Implementation

### UserController.Delete — new flow

```
1. Get current logged-in username from HttpContext.User.FindFirst(ClaimTypes.Name)
2. Fetch target user from IUserService (using existing GetById or GetUserByIdAsync)
3. If target user's username == current username:
   → return BadRequest("Bạn không thể tự xóa tài khoản của chính mình")
4. Otherwise:
   → Call _userService.Delete(id) as before
   → RedirectToAction("Index")
```

### Dependency

Add `IHttpContextAccessor` to `UserController` constructor (or use `HttpContext.User` directly since it inherits from `Controller`).

Actually, `Controller` base class already has `HttpContext.User` — no need for `IHttpContextAccessor`.

### Edge Cases

- **User deleted by another admin** — no session issue for the deleting admin; the deleted user's session will live until cookie expiry (acceptable in most applications; full session revocation requires server-side session store which this project doesn't have)
- **Concurrent delete** — if two admins try to delete the same user, one succeeds and the other gets `false` from `UserService.Delete` (existing behavior)
- **Non-existent user** — `UserService.Delete` already returns false silently (existing behavior)
