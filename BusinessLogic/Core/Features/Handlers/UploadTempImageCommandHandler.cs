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

        public UploadTempImageCommandHandler(IFileStorageService fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public async Task<UploadTempImageResult> Handle(UploadTempImageCommand request, CancellationToken cancellationToken)
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
    }
}
