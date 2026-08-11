using BusinessLogic.Services.DBServices.IDBServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.DBServices.FileStorageService
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _tempFolder;

        public FileStorageService(IWebHostEnvironment webHostEnvironment, ILogger<FileStorageService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _tempFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "temp");
        }
        /// <summary>
        /// Сохраняет файл во временную папку и возвращает временный URL.
        /// </summary>
        public async Task<string> UploadTempFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не выбран", nameof(file));

            // Проверка типа файла
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml" };
            if (!allowedTypes.Contains(file.ContentType))
                throw new InvalidOperationException("Недопустимый формат файла. Только изображения.");

            // Генерируем уникальное имя
            var fileName = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_tempFolder, fileName);

            // Создаём папку, если её нет
            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);

            // Сохраняем файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("Файл сохранён во временную папку: {FilePath}", filePath);
            return $"/images/temp/{fileName}";
        }

        /// <summary>
        /// Перемещает файл из временной папки в указанную постоянную папку.
        /// </summary>
        /// <param name="tempUrl">Временный URL.</param>
        /// <param name="destinationFolder">Папка назначения (например, "products" или "news").</param>
        public async Task<string> MoveTempToPermamentAsync(string tempUrl, string destinationFolder)
        {
            if (string.IsNullOrEmpty(tempUrl) || !tempUrl.StartsWith("/images/temp/"))
                return tempUrl; // не временный URL

            var fileName = Path.GetFileName(tempUrl);
            var tempPath = Path.Combine(_tempFolder, fileName);

            // Если временный файл не существует, возвращаем null
            if (!File.Exists(tempPath))
            {
                _logger.LogWarning("Временный файл не найден: {TempPath}", tempPath);
                return null;
            }

            // Определяем постоянную папку
            var permanentFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", destinationFolder);
            if (!Directory.Exists(permanentFolder))
                Directory.CreateDirectory(permanentFolder);

            var permanentPath = Path.Combine(permanentFolder, fileName);

            // Перемещаем файл
            File.Move(tempPath, permanentPath);
            _logger.LogInformation("Файл перемещён: {TempUrl} -> {PermanentPath}", tempUrl, permanentPath);

            return $"/images/{destinationFolder}/{fileName}";
        }

        /// <summary>
        /// Перемещает несколько временных файлов в указанную постоянную папку.
        /// </summary>
        public async Task<List<string>> MoveTempFilesToPermamentAsync(IEnumerable<string> tempUrls, string destinationFolder)
        {
            var result = new List<string>();
            foreach (var url in tempUrls)
            {
                var newUrl = await MoveTempToPermamentAsync(url, destinationFolder);
                if (newUrl != null)
                    result.Add(newUrl);
            }
            return result;
        }

        /// <summary>
        /// Удаляет временный файл.
        /// </summary>
        public Task<bool> DeleteTempFileAsync(string tempUrl)
        {
            if (string.IsNullOrEmpty(tempUrl) || !tempUrl.StartsWith("/images/temp/"))
                return Task.FromResult(false);

            var fileName = Path.GetFileName(tempUrl);
            var filePath = Path.Combine(_tempFolder, fileName);

            if (!File.Exists(filePath))
                return Task.FromResult(false);

            File.Delete(filePath);
            _logger.LogInformation("Удалён временный файл: {FilePath}", filePath);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Очищает временную папку от файлов, старше указанного возраста.
        /// </summary>
        public Task CleanTempFolderAsync(TimeSpan olderThan)
        {
            if (!Directory.Exists(_tempFolder))
                return Task.CompletedTask;

            var cutoff = DateTime.Now.Subtract(olderThan);
            var files = Directory.GetFiles(_tempFolder)
                .Where(f => File.GetCreationTime(f) < cutoff)
                .ToList();

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                    _logger.LogDebug("Удалён старый временный файл: {File}", file);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Не удалось удалить временный файл: {File}", file);
                }
            }

            return Task.CompletedTask;
        }

    }
}
