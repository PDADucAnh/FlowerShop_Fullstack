using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategoryDTO>> GetAll();
        Task<PagedResult<ProductCategoryDTO>> GetPaged(int page, int pageSize);
        Task<ProductCategoryDTO?> GetById(int id);
        Task<ProductCategoryDTO> Create(CreateProductCategoryDTO dto);
        Task<bool> Update(int id, UpdateProductCategoryDTO dto);
        Task<bool> Delete(int id);
    }
}
