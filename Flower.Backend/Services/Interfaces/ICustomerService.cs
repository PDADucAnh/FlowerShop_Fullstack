using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAll();
        Task<PagedResult<CustomerDTO>> GetPaged(int page, int pageSize);
        Task<CustomerDTO?> GetById(int id);
        Task<CustomerDTO> Create(CreateCustomerDTO dto);
        Task<bool> Update(int id, UpdateCustomerDTO dto);
        Task<bool> Delete(int id);
    }
}
