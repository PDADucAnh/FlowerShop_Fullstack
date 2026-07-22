using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Flower.Backend.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly ISystemSettingService _settingService;
        private Cloudinary? _cloudinary;

        public PhotoService(ISystemSettingService settingService)
        {
            _settingService = settingService;
        }

        private CloudinarySettings? _settings;

        private async Task<Cloudinary> GetCloudinaryAsync()
        {
            if (_cloudinary != null && _settings != null)
                return _cloudinary;

            _settings = await _settingService.GetSetting<CloudinarySettings>("Cloudinary");
            var acc = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
            _cloudinary = new Cloudinary(acc);
            return _cloudinary;
        }

        public async Task<string?> UploadPhotoAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                var cloudinary = await GetCloudinaryAsync();
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = _settings!.Folder
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
            return null;
        }

        public async Task<bool> DeletePhotoAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return false;

            var cloudinary = await GetCloudinaryAsync();
            var uri = new System.Uri(imageUrl);
            var segments = uri.Segments;
            var lastSegment = segments[^1];
            var publicId = _settings!.Folder + "/" + System.IO.Path.GetFileNameWithoutExtension(lastSegment);

            var deleteParams = new DeletionParams(publicId);
            var result = await cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }
    }
}
