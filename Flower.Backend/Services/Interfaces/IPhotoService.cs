using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<string?> UploadPhotoAsync(IFormFile file, string? subfolder = null);
        Task<bool> DeletePhotoAsync(string imageUrl);
    }
}
