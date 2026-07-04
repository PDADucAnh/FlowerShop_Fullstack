# Self-Delete Guard Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) for syntax tracking.

**Goal:** Prevent admin from deleting their own account and ensure session cleanup if self-delete occurs.

**Architecture:** Add a guard check in `UserController.Delete` that compares the target user's username against the currently logged-in user's username from `HttpContext.User` claims. Block with ModelState error if they match.

**Tech Stack:** ASP.NET Core MVC, Cookie Authentication

---

### Task 1: Add self-delete guard to UserController.Delete

**Files:**
- Modify: `CMS.Backend/Controllers/UserController.cs` (lines 76-80)
- Test: manual verification (no existing controller test infrastructure)

- [ ] **Step 1: Read current Delete method**

The current `Delete` action at line 76-80:
```csharp
public async Task<IActionResult> Delete(int id)
{
    await _userService.Delete(id);
    return RedirectToAction("Index");
}
```

- [ ] **Step 2: Add `using System.Security.Claims;` to imports**

```csharp
using CMS.Backend.Models.DTOs;
using CMS.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
```

- [ ] **Step 3: Add self-delete check**

```csharp
public async Task<IActionResult> Delete(int id)
{
    var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
    var targetUser = await _userService.GetById(id);
    if (targetUser != null && targetUser.Username == currentUsername)
    {
        ModelState.AddModelError("", "Bạn không thể tự xóa tài khoản của chính mình");
        var users = await _userService.GetAll();
        return View("Index", users);
    }

    await _userService.Delete(id);
    return RedirectToAction("Index");
}
```

- [ ] **Step 4: Build and verify no compilation errors**

```
dotnet build CMS.Backend\CMS.Backend.csproj
```
Expected: build succeeds (0 errors)

- [ ] **Step 5: Run existing tests**

```
dotnet test CMS.Tests\CMS.Tests.csproj --filter "FullyQualifiedName~UserServiceTests"
```
Expected: 22/22 pass (no regression)

- [ ] **Step 6: Commit**

```bash
git add CMS.Backend/Controllers/UserController.cs
git commit -m "fix: prevent admin self-deletion in UserController.Delete"
```
