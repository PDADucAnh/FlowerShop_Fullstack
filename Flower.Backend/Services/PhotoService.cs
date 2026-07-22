using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Flower.Backend.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly ISystemSettingService _settingService;
        private readonly ILogger<PhotoService> _logger;
        private Cloudinary? _cloudinary;

        public PhotoService(ISystemSettingService settingService, ILogger<PhotoService> logger)
        {
            _settingService = settingService;
            _logger = logger;
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
            if (file.Length <= 0)
            {
                _logger.LogWarning("UploadPhotoAsync: file is empty");
                return null;
            }

            var cloudinary = await GetCloudinaryAsync();
            _logger.LogInformation("UploadPhotoAsync: FileName={Name}, Length={Length}, CloudName={CloudName}, Folder={Folder}",
                file.FileName, file.Length, _settings?.CloudName, _settings?.Folder);

            using var stream = file.OpenReadStream();
            _logger.LogInformation("UploadPhotoAsync: StreamPosition={Position}, StreamLength={Length}, CanSeek={CanSeek}",
                stream.Position, stream.Length, stream.CanSeek);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = _settings!.Folder
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            _logger.LogInformation("UploadPhotoAsync: ResultStatusCode={StatusCode}, SecureUrl={Url}, Error={Error}",
                uploadResult.StatusCode,
                uploadResult.SecureUrl?.ToString(),
                uploadResult.Error?.Message);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK && uploadResult.SecureUrl != null)
                return uploadResult.SecureUrl.ToString();

            _logger.LogError("Cloudinary upload failed: StatusCode={StatusCode}, Error={Error}, CloudName={CloudName}",
                uploadResult.StatusCode,
                uploadResult.Error?.Message,
                _settings?.CloudName);
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
