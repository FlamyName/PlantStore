using BusinessLogic.Services.DBServices.IDBServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.BackgroundServices
{
    public class TempFileCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TempFileCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5); // Каждые 6 часов
        private readonly TimeSpan _maxFileAge = TimeSpan.FromMinutes(5); // Удаляем файлы старше 24 часов

        public TempFileCleanupService(
            IServiceProvider serviceProvider,
            ILogger<TempFileCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Сервис очистки временных файлов запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanTempFilesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при очистке временных файлов");
                }

                // Ждем интервал перед следующей очисткой
                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Сервис очистки временных файлов остановлен");
        }

        private async Task CleanTempFilesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorageService>();

            _logger.LogInformation("Начинается очистка временных файлов (старше {Age} часов)", _maxFileAge.TotalHours);

            await fileStorage.CleanTempFolderAsync(_maxFileAge);

            _logger.LogInformation("Очистка временных файлов завершена");
        }

    }
}
