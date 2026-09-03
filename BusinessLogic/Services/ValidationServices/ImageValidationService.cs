using BusinessLogic.Services.ValidationServices.IValidationServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ValidationServices
{
    public class ImageValidationService : IImageValidationService
    {
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 МБ
        private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml", "image/bmp"
        };
            private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp"
        };

        public string? Validate(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return "Файл не выбран";

            if (file.Length > MaxFileSize)
                return "Файл слишком большой (макс 5 МБ)";

            if (!IsAllowedType(file.ContentType))
                return "Недопустимый формат изображения";

            using var stream = file.OpenReadStream();
            if (!IsValidSignature(stream))
                return "Файл не является изображением";

            return null; // валиден
        }

        public bool IsAllowedType(string contentType)
            => !string.IsNullOrWhiteSpace(contentType) && AllowedTypes.Contains(contentType);

        public bool IsValidSignature(Stream stream)
        {
            byte[] header = new byte[8];
            int read = stream.Read(header, 0, header.Length);
            if (read < 4) return false;

            // JPEG
            if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) return true;
            // PNG
            if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47) return true;
            // GIF
            if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38) return true;
            // WebP
            if (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46) return true;
            // BMP
            if (header[0] == 0x42 && header[1] == 0x4D) return true;

            return false;
        }
    }
}
