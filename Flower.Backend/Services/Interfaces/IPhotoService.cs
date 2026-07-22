using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<string?> UploadPhotoAsync(IFormFile file);
        Task<bool> DeletePhotoAsync(string imageUrl);
    }
}
