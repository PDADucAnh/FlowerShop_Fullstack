# UserService Test Plan

## Overview

Write unit tests for `CMS.Backend.Services.UserService` in the existing `CMS.Tests` project (xUnit + Moq + EF Core InMemory).

## Approach

**Hybrid:** InMemory database for CRUD tests, Moq for edge-case scenarios (concurrency exceptions).

- Follow the same pattern as `AuthServiceTests.cs` (direct service instantiation, InMemory context)
- Keep both patterns (InMemory + Mock) in the same file with clear helper methods

## Test Infrastructure

### Existing (reuse)
- `Microsoft.EntityFrameworkCore.InMemory` — for realistic EF behavior
- `Moq` — for edge-case control
- `PasswordHasher<User>` — used by the service itself

### New helpers
- `CreateInMemoryDbContext()` — fresh InMemory DB per test (same as AuthServiceTests)
- `CreateMockDbContext()` — Moq-based for concurrency tests
- `SampleUserDTO` — standard `CreateUserDTO` fixture

## Test Cases (22 total)

### Section 1: CRUD via InMemory (8 tests)

| # | Test | Method | Assertions |
|---|------|--------|------------|
| 1 | `GetAll_NoUsers_ReturnsEmptyList` | GetAll | Result is empty |
| 2 | `GetAll_ReturnsAllUsersAsDTOs` | GetAll | Count matches, fields mapped correctly |
| 3 | `GetById_ExistingUser_ReturnsUserDTO` | GetById | Not null, fields match |
| 4 | `GetById_NonExistingUser_ReturnsNull` | GetById | Result is null |
| 5 | `Create_ValidUser_ReturnsUserDTOWithHashedPassword` | Create | PasswordHash != input, all fields mapped |
| 6 | `Create_SetsAllFieldsCorrectly` | Create | Username, FullName, Role match |
| 7 | `Delete_NonExistingUser_ReturnsFalse` | Delete | Returns false, no exception |
| 8 | `Delete_ExistingUser_RemovesAndReturnsTrue` | Delete | Returns true, user gone from DB |

### Section 2: Update via InMemory + Mock (7 tests)

| # | Test | Method | Strategy | Assertions |
|---|------|--------|----------|------------|
| 9 | `Update_IdMismatch_ReturnsFalse` | Update | InMemory | False, no DB changes |
| 10 | `Update_UserNotFound_ReturnsFalse` | Update | InMemory | False |
| 11 | `Update_ValidUpdate_ReturnsTrue` | Update | InMemory | True, fields updated |
| 12 | `Update_WithNewPassword_HashesPassword` | Update | InMemory | PasswordHash changed != old |
| 13 | `Update_WithoutPassword_PreservesOldHash` | Update | InMemory | PasswordHash unchanged |
| 14 | `Update_ConcurrencyUserDeleted_ReturnsFalse` | Update | **Mock** | Catches concurrency, returns false |
| 15 | `Update_ConcurrencyUserStillExists_Throws` | Update | **Mock** | Re-throws DbUpdateConcurrencyException |

### Section 3: Helper Methods via InMemory (7 tests)

| # | Test | Method | Assertions |
|---|------|--------|------------|
| 16 | `GetUsersAsync_ReturnsAllUsers` | GetUsersAsync | Count matches, entity type |
| 17 | `GetUserByIdAsync_ExistingUser_ReturnsUser` | GetUserByIdAsync | Not null, correct Id |
| 18 | `GetUserByIdAsync_NonExistingUser_ReturnsNull` | GetUserByIdAsync | Null |
| 19 | `UserExistsAsync_ExistingUser_ReturnsTrue` | UserExistsAsync | True |
| 20 | `UserExistsAsync_NonExistingUser_ReturnsFalse` | UserExistsAsync | False |
| 21 | `CreateUserAsync_HashesPassword` | CreateUserAsync | Returns true, PasswordHash != input |
| 22 | `CreateUserAsync_NullPassword_NoHash` | CreateUserAsync | Returns true, PasswordHash preserved |

## Password Security Verification

All password-hash tests must verify:
1. `PasswordHash` is never null after creation/update
2. `PasswordHash != originalPassword` (actual hashing occurred)
3. `CreateUserAsync` only hashes when `PasswordHash` is non-empty

## File Structure

```
CMS.Tests/
  AuthServiceTests.cs       (existing — 4 tests)
  UserServiceTests.cs       (new — 22 tests)
```
