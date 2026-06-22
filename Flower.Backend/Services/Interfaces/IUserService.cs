using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;
using Flower.Data.Entities;

namespace Flower.Backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAll();
        Task<UserDTO?> GetById(int id);
        Task<UserDTO> Create(CreateUserDTO dto);
        Task<bool> Update(int id, UpdateUserDTO dto);
        Task<bool> Delete(int id);
        
        // Helper queries needed strictly for username checks or internal fetches
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UserExistsAsync(string username);
        Task<bool> CreateUserAsync(User user);
    }
}
