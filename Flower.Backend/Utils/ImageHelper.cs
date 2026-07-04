using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Flower.Backend.Utils
{
    public static class ImageHelper
    {
        private const int _heroWidth = 1920;
        private const int _heroHeight = 819;
        private const int _quality = 85;

        public static async Task<string> SaveHeroImageAsync(IFormFile file, string uploadsDir)
        {
            var fileName = $"{Guid.NewGuid()}.jpg";
            var fullPath = Path.Combine(uploadsDir, fileName);

            using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);

            image.Mutate(ctx =>
            {
                var (w, h) = (image.Width, image.Height);
                var targetRatio = (double)_heroWidth / _heroHeight;
                var sourceRatio = (double)w / h;

                Rectangle cropRect;
                if (sourceRatio > targetRatio)
                {
                    var cropWidth = (int)(h * targetRatio);
                    cropRect = new Rectangle((w - cropWidth) / 2, 0, cropWidth, h);
                }
                else
                {
                    var cropHeight = (int)(w / targetRatio);
                    cropRect = new Rectangle(0, (h - cropHeight) / 2, w, cropHeight);
                }
                ctx.Crop(cropRect);
                ctx.Resize(_heroWidth, _heroHeight);
            });

            var encoder = new JpegEncoder { Quality = _quality };
            await image.SaveAsJpegAsync(fullPath, encoder);

            return $"/uploads/herobars/{fileName}";
        }

        public static void DeleteImage(string? imageUrl, string wwwrootDir)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            var path = Path.Combine(wwwrootDir, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
