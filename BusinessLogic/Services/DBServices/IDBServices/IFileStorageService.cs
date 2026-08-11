using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.DBServices.IDBServices
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Сохраняет файл во временную папку и возвращает временный URL.
        /// </summary>
        Task<string> UploadTempFileAsync(IFormFile file);

        /// <summary>
        /// Перемещает файл из временной папки в указанную постоянную папку.
        /// Если URL не является временным, возвращает его без изменений.
        /// </summary>
        /// <param name="tempUrl">Временный URL файла.</param>
        /// <param name="destinationFolder">Папка назначения (например, "products" или "news").</param>
        Task<string> MoveTempToPermamentAsync(string tempUrl, string destionationFolder);

        /// <summary>
        /// Перемещает несколько временных файлов в указанную постоянную папку.
        /// </summary>
        Task<List<string>> MoveTempFilesToPermamentAsync(IEnumerable<string> tempUrls, string destinationFolder);

        /// <summary>
        /// Удаляет временный файл.
        /// </summary>
        Task<bool> DeleteTempFileAsync(string tempUrl);

        /// <summary>
        /// Очищает временную папку от файлов, старше указанного возраста.
        /// </summary>
        Task CleanTempFolderAsync(TimeSpan olderThan);
    }
}
