using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => u.ToDTO());
        }

        public async Task<UserDTO?> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user?.ToDTO();
        }

        public async Task<UserDTO> Create(CreateUserDTO dto)
        {
            var user = dto.ToEntity();
            
            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.ToDTO();
        }

        public async Task<bool> Update(int id, UpdateUserDTO dto)
        {
            if (id != dto.Id)
                return false;

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return false;

            existingUser.Username = dto.Username;
            existingUser.FullName = dto.FullName;
            existingUser.Role = dto.Role;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                existingUser.PasswordHash = _passwordHasher.HashPassword(existingUser, dto.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Users.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
            }
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
