using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IAdvertisementService
    {
        Task<IEnumerable<AdvertisementDTO>> GetAllActive();
        Task<IEnumerable<AdvertisementDTO>> GetAll();
        Task<AdvertisementDTO?> GetById(int id);
        Task<AdvertisementDTO> Create(CreateAdvertisementDTO dto);
        Task<bool> Update(int id, UpdateAdvertisementDTO dto);
        Task<bool> Delete(int id);
    }
}
