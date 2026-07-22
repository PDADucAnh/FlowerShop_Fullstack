using Flower.Backend.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDTO>> GetAll();
        Task<ContactDTO?> GetById(int id);
        Task<ContactDTO> Create(CreateContactDTO dto);
        Task<bool> MarkRead(int id, bool isRead);
        Task<bool> Delete(int id);
        Task<int> GetUnreadCount();
    }
}
