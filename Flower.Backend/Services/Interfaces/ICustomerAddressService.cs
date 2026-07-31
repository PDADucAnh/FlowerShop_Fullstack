using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface ICustomerAddressService
    {
        Task<IEnumerable<CustomerAddressDTO>> GetByCustomerId(int customerId);
        Task<CustomerAddressDTO?> GetById(int id);
        Task<CustomerAddressDTO> Create(CreateCustomerAddressDTO dto);
        Task<bool> Update(int id, UpdateCustomerAddressDTO dto);
        Task<bool> Delete(int id);
        Task<bool> SetDefault(int id, int customerId);
    }
}
