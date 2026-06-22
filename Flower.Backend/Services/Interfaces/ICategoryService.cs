using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAll();
        Task<CategoryDTO?> GetById(int id);
        Task<CategoryDTO> Create(CreateCategoryDTO dto);
        Task<bool> Update(int id, UpdateCategoryDTO dto);
        Task<bool> Delete(int id);
    }
}
