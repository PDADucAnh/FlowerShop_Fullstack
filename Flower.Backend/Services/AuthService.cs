using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flower.Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApplicationDbContext _context;
        private readonly PasswordHasher<User> _userPasswordHasher;
        private readonly PasswordHasher<Customer> _customerPasswordHasher;

        public AuthService(IApplicationDbContext context)
        {
            _context = context;
            _userPasswordHasher = new PasswordHasher<User>();
            _customerPasswordHasher = new PasswordHasher<Customer>();
        }

        private static LoginResult MapUserToResult(User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Address = user.Address,
            Role = user.Role,
            PasswordHash = user.PasswordHash,
            AuthType = "User"
        };

        private static LoginResult MapCustomerToResult(Customer customer) => new()
        {
            Id = customer.Id,
            Username = customer.Email,
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            Role = "Customer",
            PasswordHash = customer.PasswordHash,
            AuthType = "Customer"
        };

        public async Task<LoginResult?> Login(string identifier, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == identifier);

            if (user != null)
            {
                var verificationResult = _userPasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (verificationResult == PasswordVerificationResult.Failed)
                    return null;

                if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.PasswordHash = _userPasswordHasher.HashPassword(user, password);
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }

                return MapUserToResult(user);
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == identifier);

            if (customer != null)
            {
                var verificationResult = _customerPasswordHasher.VerifyHashedPassword(customer, customer.PasswordHash, password);
                if (verificationResult == PasswordVerificationResult.Failed)
                    return null;

                if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    customer.PasswordHash = _customerPasswordHasher.HashPassword(customer, password);
                    _context.Customers.Update(customer);
                    await _context.SaveChangesAsync();
                }

                return MapCustomerToResult(customer);
            }

            return null;
        }

        public async Task<(bool Success, string Message)> Register(string username, string password, string fullName, string? email, string? phone, string? address)
        {
            var checkExist = await _context.Customers.AnyAsync(c => c.Email == email);
            if (checkExist)
            {
                return (false, "Email already exists!");
            }

            var newCustomer = new Customer
            {
                FullName = fullName,
                Email = email ?? string.Empty,
                Phone = phone,
                Address = address,
                PasswordHash = password
            };

            newCustomer.PasswordHash = _customerPasswordHasher.HashPassword(newCustomer, password);

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return (true, "Registration successful!");
        }

        public async Task<LoginResult?> GetProfile(string identifier, string authType)
        {
            if (authType == "User")
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == identifier);
                return user != null ? MapUserToResult(user) : null;
            }

            if (authType == "Customer")
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == identifier);
                return customer != null ? MapCustomerToResult(customer) : null;
            }

            return null;
        }
    }
}
