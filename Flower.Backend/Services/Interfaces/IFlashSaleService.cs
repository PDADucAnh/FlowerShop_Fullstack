using Flower.Backend.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IFlashSaleService
    {
        Task<IEnumerable<FlashSaleActiveDTO>> GetActiveFlashSales();
        Task<IEnumerable<FlashSaleDTO>> GetAll();
        Task<FlashSaleDTO?> GetById(int id);
        Task<FlashSaleDTO> Create(CreateFlashSaleDTO dto);
        Task<bool> Update(int id, UpdateFlashSaleDTO dto);
        Task<bool> Delete(int id);
    }
}
