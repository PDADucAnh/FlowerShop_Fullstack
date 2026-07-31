using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IPostCategoryService
    {
        Task<IEnumerable<PostCategoryDTO>> GetAll();
        Task<PagedResult<PostCategoryDTO>> GetPaged(int page, int pageSize);
        Task<PostCategoryDTO?> GetById(int id);
        Task<PostCategoryDTO> Create(CreatePostCategoryDTO dto);
        Task<bool> Update(int id, UpdatePostCategoryDTO dto);
        Task<bool> Delete(int id);
    }
}
