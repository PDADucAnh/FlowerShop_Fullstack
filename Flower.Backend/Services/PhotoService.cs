using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

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
            _logger.LogInformation("GetCloudinaryAsync: CloudName={Name}, ApiKeyPrefix={KeyPrefix}, ApiSecretLen={SecretLen}, Folder={Folder}",
                _settings.CloudName,
                _settings.ApiKey.Length >= 6 ? _settings.ApiKey[..6] + "..." : "too-short",
                _settings.ApiSecret.Length,
                _settings.Folder);
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

            using var compressedStream = new MemoryStream();
            using (var sourceStream = file.OpenReadStream())
            using (var image = await Image.LoadAsync(sourceStream))
            {
                var maxDimension = 1920;
                if (image.Width > maxDimension || image.Height > maxDimension)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new SixLabors.ImageSharp.Size(maxDimension, maxDimension)
                    }));
                }
                image.Mutate(x => x.AutoOrient());
                var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 80 };
                await image.SaveAsJpegAsync(compressedStream, encoder);
                compressedStream.Position = 0;
            }

            var cloudinary = await GetCloudinaryAsync();
            _logger.LogInformation("UploadPhotoAsync: FileName={Name}, OriginalLength={OriginalLength}, CompressedLength={CompressedLength}, CloudName={CloudName}, Folder={Folder}",
                file.FileName, file.Length, compressedStream.Length, _settings?.CloudName, _settings?.Folder);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, compressedStream),
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
