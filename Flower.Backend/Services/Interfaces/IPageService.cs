using Flower.Backend.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IPageService
    {
        Task<IEnumerable<PageDTO>> GetAll();
        Task<IEnumerable<PageDTO>> GetAllActive();
        Task<PageDTO?> GetById(int id);
        Task<PageDTO?> GetBySlug(string slug);
        Task<PageDTO> Create(CreatePageDTO dto);
        Task<bool> Update(int id, UpdatePageDTO dto);
        Task<bool> Delete(int id);
    }
}
