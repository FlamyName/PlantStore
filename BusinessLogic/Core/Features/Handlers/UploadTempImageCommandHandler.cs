using BusinessLogic.Core.Features.Commands;
using BusinessLogic.Services.DBServices.IDBServices;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Core.Features.Handlers
{
    public class UploadTempImageCommandHandler : IRequestHandler<UploadTempImageCommand, UploadTempImageResult>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly ILogger<UploadTempImageCommandHandler> _logger;

        public UploadTempImageCommandHandler(IFileStorageService fileStorage, ILogger<UploadTempImageCommandHandler> logger)
        {
            _fileStorage = fileStorage;
            _logger = logger;
        }

        public async Task<UploadTempImageResult> Handle(UploadTempImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    return new UploadTempImageResult
                    {
                        Success = false,
                        ErrorMessage = "Файл не выбран"
                    };
                }

                var url = await _fileStorage.UploadTempFileAsync(request.File);

                return new UploadTempImageResult
                {
                    Success = true,
                    Url = url
                };
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Ошибка валидации файла");
                return new UploadTempImageResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Недопустимый тип файла");
                return new UploadTempImageResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке временного файла");
                return new UploadTempImageResult
                {
                    Success = false,
                    ErrorMessage = "Внутренняя ошибка сервера"
                };
            }
        }
    }
}
