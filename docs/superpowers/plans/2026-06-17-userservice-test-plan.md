# UserService Tests Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) for syntax tracking.

**Goal:** Write 22 unit tests for `UserService` in `CMS.Tests`, using InMemory DB for CRUD and Moq for concurrency edge cases.

**Architecture:** Hybrid pattern — `CreateInMemoryDbContext()` helper for 20 tests, `Mock<IApplicationDbContext>` for 2 concurrency tests. Follows existing `AuthServiceTests.cs` pattern.

**Tech Stack:** xUnit, Moq 4.20.72, EF Core InMemory 8.0.23, .NET 8

---

### Task 1: Scaffold + GetAll + GetById Tests

**Files:**
- Create: `CMS.Tests/UserServiceTests.cs`
- Reference: `CMS.Tests/AuthServiceTests.cs` (existing pattern)

- [ ] **Step 1.1: Add using statements and class scaffold with InMemory helper**

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using CMS.Backend.Models.DTOs;
using CMS.Backend.Services;
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CMS.Tests
{
    public class UserServiceTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }
    }
}
```

- [ ] **Step 1.2: Write GetAll — empty list test**

```csharp
        [Fact]
        public async Task GetAll_NoUsers_ReturnsEmptyList()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);

            var result = await service.GetAll();

            Assert.Empty(result);
        }
```

- [ ] **Step 1.3: Write GetAll — returns users test**

```csharp
        [Fact]
        public async Task GetAll_ReturnsAllUsersAsDTOs()
        {
            using var context = CreateInMemoryDbContext();
            context.Users.AddRange(
                new User { Username = "admin", FullName = "Admin User", Role = "Admin", PasswordHash = "h1" },
                new User { Username = "editor", FullName = "Editor User", Role = "Editor", PasswordHash = "h2" }
            );
            await context.SaveChangesAsync();
            var service = new UserService(context);

            var result = (await service.GetAll()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Username == "admin" && u.FullName == "Admin User" && u.Role == "Admin");
            Assert.Contains(result, u => u.Username == "editor" && u.FullName == "Editor User" && u.Role == "Editor");
        }
```

- [ ] **Step 1.4: Write GetById — existing and non-existing**

```csharp
        [Fact]
        public async Task GetById_ExistingUser_ReturnsUserDTO()
        {
            using var context = CreateInMemoryDbContext();
            var entity = new User { Username = "test", FullName = "Test User", Role = "Customer", PasswordHash = "h" };
            context.Users.Add(entity);
            await context.SaveChangesAsync();
            var service = new UserService(context);

            var result = await service.GetById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal("test", result.Username);
            Assert.Equal("Test User", result.FullName);
            Assert.Equal("Customer", result.Role);
        }

        [Fact]
        public async Task GetById_NonExistingUser_ReturnsNull()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);

            var result = await service.GetById(999);

            Assert.Null(result);
        }
```

- [ ] **Step 1.5: Build and run to verify**

```
dotnet build CMS.Tests\CMS.Tests.csproj
dotnet test CMS.Tests\CMS.Tests.csproj --filter "FullyQualifiedName~UserServiceTests" --no-build
```
Expected: 4 tests pass

- [ ] **Step 1.6: Commit**

```bash
git add CMS.Tests/UserServiceTests.cs
git commit -m "test: add UserService scaffold, GetAll, and GetById tests"
```

---

### Task 2: Create + Delete Tests

**Files:**
- Modify: `CMS.Tests/UserServiceTests.cs`

- [ ] **Step 2.1: Write Create — hashes password and returns DTO**

```csharp
        [Fact]
        public async Task Create_ValidUser_ReturnsUserDTOWithHashedPassword()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);
            var dto = new CreateUserDTO
            {
                Username = "newuser",
                Password = "PlainText123!",
                FullName = "New User",
                Role = "Editor"
            };

            var result = await service.Create(dto);

            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("New User", result.FullName);
            Assert.Equal("Editor", result.Role);

            var saved = await context.Users.FindAsync(result.Id);
            Assert.NotNull(saved);
            Assert.NotEqual("PlainText123!", saved.PasswordHash);
            Assert.NotNull(saved.PasswordHash);
        }

        [Fact]
        public async Task Create_SetsAllFieldsCorrectly()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);
            var dto = new CreateUserDTO
            {
                Username = "johndoe",
                Password = "P@ssw0rd",
                FullName = "John Doe",
                Role = "Customer"
            };

            var result = await service.Create(dto);

            Assert.Equal("johndoe", result.Username);
            Assert.Equal("John Doe", result.FullName);
            Assert.Equal("Customer", result.Role);
        }
```

- [ ] **Step 2.2: Write Delete — non-existing and existing**

```csharp
        [Fact]
        public async Task Delete_NonExistingUser_ReturnsFalse()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);

            var result = await service.Delete(999);

            Assert.False(result);
        }

        [Fact]
        public async Task Delete_ExistingUser_RemovesAndReturnsTrue()
        {
            using var context = CreateInMemoryDbContext();
            var entity = new User { Username = "todelete", FullName = "Delete Me", Role = "Customer", PasswordHash = "h" };
            context.Users.Add(entity);
            await context.SaveChangesAsync();
            var service = new UserService(context);

            var result = await service.Delete(entity.Id);

            Assert.True(result);
            Assert.Null(await context.Users.FindAsync(entity.Id));
        }
```

- [ ] **Step 2.3: Build and run**

```
dotnet build CMS.Tests\CMS.Tests.csproj
dotnet test CMS.Tests\CMS.Tests.csproj --filter "FullyQualifiedName~UserServiceTests" --no-build
```
Expected: 8 tests pass

- [ ] **Step 2.4: Commit**

```bash
git add CMS.Tests/UserServiceTests.cs
git commit -m "test: add UserService Create and Delete tests"
```

---

### Task 3: Update Tests (InMemory + Mock)

**Files:**
- Modify: `CMS.Tests/UserServiceTests.cs`

- [ ] **Step 3.1: Write Update — id mismatch and user not found**

```csharp
        [Fact]
        public async Task Update_IdMismatch_ReturnsFalse()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);
            var dto = new UpdateUserDTO { Id = 2, Username = "u", FullName = "U", Role = "Customer" };

            var result = await service.Update(1, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task Update_UserNotFound_ReturnsFalse()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);
            var dto = new UpdateUserDTO { Id = 999, Username = "u", FullName = "U", Role = "Customer" };

            var result = await service.Update(999, dto);

            Assert.False(result);
        }
```

- [ ] **Step 3.2: Write Update — successful update (no password, with password)**

```csharp
        [Fact]
        public async Task Update_ValidUpdate_ReturnsTrue()
        {
            using var context = CreateInMemoryDbContext();
            var entity = new User { Username = "old", FullName = "Old Name", Role = "Customer", PasswordHash = "oldhash" };
            context.Users.Add(entity);
            await context.SaveChangesAsync();
            var service = new UserService(context);
            var dto = new UpdateUserDTO
            {
                Id = entity.Id,
                Username = "new",
                FullName = "New Name",
                Role = "Admin",
                Password = null
            };

            var result = await service.Update(entity.Id, dto);

            Assert.True(result);
            var updated = await context.Users.FindAsync(entity.Id);
            Assert.Equal("new", updated.Username);
            Assert.Equal("New Name", updated.FullName);
            Assert.Equal("Admin", updated.Role);
            Assert.Equal("oldhash", updated.PasswordHash);
        }

        [Fact]
        public async Task Update_WithNewPassword_HashesPassword()
        {
            using var context = CreateInMemoryDbContext();
            var entity = new User { Username = "u", FullName = "U", Role = "Customer", PasswordHash = "oldhash" };
            context.Users.Add(entity);
            await context.SaveChangesAsync();
            var service = new UserService(context);
            var dto = new UpdateUserDTO
            {
                Id = entity.Id,
                Username = "u",
                FullName = "U",
                Role = "Customer",
                Password = "NewPass123!"
            };

            var result = await service.Update(entity.Id, dto);

            Assert.True(result);
            var updated = await context.Users.FindAsync(entity.Id);
            Assert.NotEqual("oldhash", updated.PasswordHash);
            Assert.NotEqual("NewPass123!", updated.PasswordHash);
        }

        [Fact]
        public async Task Update_WithoutPassword_PreservesOldHash()
        {
            using var context = CreateInMemoryDbContext();
            var entity = new User { Username = "u", FullName = "U", Role = "Customer", PasswordHash = "preserveme" };
            context.Users.Add(entity);
            await context.SaveChangesAsync();
            var service = new UserService(context);
            var dto = new UpdateUserDTO
            {
                Id = entity.Id,
                Username = "u",
                FullName = "U",
                Role = "Customer",
                Password = ""
            };

            var result = await service.Update(entity.Id, dto);

            Assert.True(result);
            var updated = await context.Users.FindAsync(entity.Id);
            Assert.Equal("preserveme", updated.PasswordHash);
        }
```

- [ ] **Step 3.3: Write Mock helper and concurrency tests**

`AnyAsync` is an EF Core extension method on `IQueryable<T>`. To mock it, set up the `DbSet` as `IQueryable<User>` backed by an in-memory list.

```csharp
        private (Mock<IApplicationDbContext> mockContext, User user) CreateConcurrencyMock(bool userStillExists)
        {
            var user = new User { Id = 1, Username = "u", FullName = "U", Role = "Customer", PasswordHash = "h" };

            var data = userStillExists
                ? new List<User> { user }.AsQueryable()
                : new List<User>().AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.Setup(s => s.FindAsync(It.IsAny<object[]>())).ReturnsAsync(user);

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            return (mockContext, user);
        }

        [Fact]
        public async Task Update_ConcurrencyUserDeleted_ReturnsFalse()
        {
            var (mockContext, _) = CreateConcurrencyMock(userStillExists: false);
            var service = new UserService(mockContext.Object);
            var dto = new UpdateUserDTO { Id = 1, Username = "u", FullName = "U", Role = "Customer" };

            var result = await service.Update(1, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task Update_ConcurrencyUserStillExists_Throws()
        {
            var (mockContext, _) = CreateConcurrencyMock(userStillExists: true);
            var service = new UserService(mockContext.Object);
            var dto = new UpdateUserDTO { Id = 1, Username = "u", FullName = "U", Role = "Customer" };

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => service.Update(1, dto));
        }
```

- [ ] **Step 3.4: Build and run**

```
dotnet build CMS.Tests\CMS.Tests.csproj
dotnet test CMS.Tests\CMS.Tests.csproj --filter "FullyQualifiedName~UserServiceTests" --no-build
```
Expected: 15 tests pass

- [ ] **Step 3.5: Commit**

```bash
git add CMS.Tests/UserServiceTests.cs
git commit -m "test: add UserService Update tests (InMemory + Mock concurrency)"
```

---

### Task 4: Helper Methods Tests

**Files:**
- Modify: `CMS.Tests/UserServiceTests.cs`

- [ ] **Step 4.1: Write GetUsersAsync and GetUserByIdAsync tests**

```csharp
        [Fact]
        public async Task GetUsersAsync_ReturnsAllUsers()
        {
            using var context = CreateInMemoryDbContext();
            context.Users.AddRange(
                new User { Username = "a", FullName = "A", Role = "Admin", PasswordHash = "h1" },
                new User { Username = "b", FullName = "B", Role = "Editor", PasswordHash = "h2" }
            );
            await context.SaveChangesAsync();
            var service = new UserService(context);

            var result = (await service.GetUsersAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, u => Assert.IsType<User>(u));
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
        {
            using var context = CreateInMemoryDbContext();
            var entity = new User { Username = "test", FullName = "Test", Role = "Customer", PasswordHash = "h" };
            context.Users.Add(entity);
            await context.SaveChangesAsync();
            var service = new UserService(context);

            var result = await service.GetUserByIdAsync(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.IsType<User>(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingUser_ReturnsNull()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);

            var result = await service.GetUserByIdAsync(999);

            Assert.Null(result);
        }
```

- [ ] **Step 4.2: Write UserExistsAsync tests**

```csharp
        [Fact]
        public async Task UserExistsAsync_ExistingUser_ReturnsTrue()
        {
            using var context = CreateInMemoryDbContext();
            context.Users.Add(new User { Username = "exists", FullName = "Exists", Role = "Customer", PasswordHash = "h" });
            await context.SaveChangesAsync();
            var service = new UserService(context);

            var result = await service.UserExistsAsync("exists");

            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_NonExistingUser_ReturnsFalse()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);

            var result = await service.UserExistsAsync("nobody");

            Assert.False(result);
        }
```

- [ ] **Step 4.3: Write CreateUserAsync tests**

```csharp
        [Fact]
        public async Task CreateUserAsync_HashesPassword()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);
            var user = new User
            {
                Username = "asyncuser",
                FullName = "Async User",
                Role = "Customer",
                PasswordHash = "PlainPassword"
            };

            var result = await service.CreateUserAsync(user);

            Assert.True(result);
            var saved = await context.Users.FirstOrDefaultAsync(u => u.Username == "asyncuser");
            Assert.NotNull(saved);
            Assert.NotEqual("PlainPassword", saved.PasswordHash);
        }

        [Fact]
        public async Task CreateUserAsync_EmptyPassword_NoHash()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);
            var user = new User
            {
                Username = "nopass",
                FullName = "No Pass",
                Role = "Customer",
                PasswordHash = ""
            };

            var result = await service.CreateUserAsync(user);

            Assert.True(result);
            var saved = await context.Users.FirstOrDefaultAsync(u => u.Username == "nopass");
            Assert.NotNull(saved);
            Assert.Equal("", saved.PasswordHash);
        }
```

- [ ] **Step 4.4: Build and run all 22 tests**

```
dotnet build CMS.Tests\CMS.Tests.csproj
dotnet test CMS.Tests\CMS.Tests.csproj --filter "FullyQualifiedName~UserServiceTests" --no-build
```
Expected: 22 tests pass

- [ ] **Step 4.5: Commit**

```bash
git add CMS.Tests/UserServiceTests.cs
git commit -m "test: add UserService helper method tests (GetUsersAsync, GetUserByIdAsync, UserExistsAsync, CreateUserAsync)"
```
