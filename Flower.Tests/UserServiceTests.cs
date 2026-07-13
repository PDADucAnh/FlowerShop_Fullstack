using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace Flower.Tests
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

        [Fact]
        public async Task GetAll_NoUsers_ReturnsEmptyList()
        {
            using var context = CreateInMemoryDbContext();
            var service = new UserService(context);

            var result = await service.GetAll();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_ReturnsAllUsersAsDTOs()
        {
            using var context = CreateInMemoryDbContext();
            context.Users.AddRange(
                new User { Username = "admin", FullName = "Admin User", Role = "Admin", PasswordHash = "h1" },
                new User { Username = "staff", FullName = "Staff User", Role = "Staff", PasswordHash = "h2" }
            );
            await context.SaveChangesAsync();
            var service = new UserService(context);

            var result = (await service.GetAll()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Username == "admin" && u.FullName == "Admin User" && u.Role == "Admin");
            Assert.Contains(result, u => u.Username == "staff" && u.FullName == "Staff User" && u.Role == "Staff");
        }

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
                Role = "Staff"
            };

            var result = await service.Create(dto);

            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("New User", result.FullName);
            Assert.Equal("Staff", result.Role);

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

        private (IApplicationDbContext context, User user) CreateConcurrencyMock(bool userStillExists)
        {
            var user = new User { Id = 1, Username = "u", FullName = "U", Role = "Customer", PasswordHash = "h" };

            var data = userStillExists
                ? new List<User> { user }.AsQueryable()
                : new List<User>().AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IAsyncEnumerable<User>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(data.GetEnumerator()));
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(data.Provider));
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            mockSet.Setup(s => s.FindAsync(It.IsAny<object[]>())).ReturnsAsync(user);

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            return (mockContext.Object, user);
        }

        [Fact]
        public async Task Update_ConcurrencyUserDeleted_ReturnsFalse()
        {
            var (context, _) = CreateConcurrencyMock(userStillExists: false);
            var service = new UserService(context);
            var dto = new UpdateUserDTO { Id = 1, Username = "u", FullName = "U", Role = "Customer" };

            var result = await service.Update(1, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task Update_ConcurrencyUserStillExists_Throws()
        {
            var (context, _) = CreateConcurrencyMock(userStillExists: true);
            var service = new UserService(context);
            var dto = new UpdateUserDTO { Id = 1, Username = "u", FullName = "U", Role = "Customer" };

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => service.Update(1, dto));
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsAllUsers()
        {
            using var context = CreateInMemoryDbContext();
            context.Users.AddRange(
                new User { Username = "a", FullName = "A", Role = "Admin", PasswordHash = "h1" },
                new User { Username = "b", FullName = "B", Role = "Staff", PasswordHash = "h2" }
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
    }

    internal class TestAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        internal TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<T>(expression);
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
        public object Execute(Expression expression) => _inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })
                .MakeGenericMethod(resultType)
                .Invoke(this, new[] { expression });
            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
        public T Current => _inner.Current;
        public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());
    }
}
