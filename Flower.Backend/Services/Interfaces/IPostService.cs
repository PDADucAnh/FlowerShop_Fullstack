using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostDTO>> GetAll();
        Task<PostDTO?> GetById(int id);
        Task<PostDTO> Create(CreatePostDTO dto);
        Task<bool> Update(int id, UpdatePostDTO dto);
        Task<bool> Delete(int id);
        Task<IEnumerable<PostDTO>> GetByCategory(int categoryId);
    }
}
