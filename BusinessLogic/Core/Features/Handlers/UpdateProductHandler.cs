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
    public class UpdateProductHandler : IRequestHandler<UpdateProductFilesCommand, bool>
    {
        private readonly IAdminCatalogService _adminCatalogService;
        private readonly ILogger<UpdateProductHandler> _logger;

        public UpdateProductHandler(IAdminCatalogService adminCatalogService, ILogger<UpdateProductHandler> logger)
        {
            _adminCatalogService = adminCatalogService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateProductFilesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string GetImageUrl(string tempUrl, string currentUrl)
                {
                    if (!string.IsNullOrEmpty(tempUrl)) return tempUrl;
                    if (!string.IsNullOrEmpty(currentUrl)) return currentUrl;
                    return null;
                }

                var command = new UpdateProductCommand
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Image1 = GetImageUrl(request.TempImage1, request.CurrentImage1),
                    Image2 = GetImageUrl(request.TempImage2, request.CurrentImage2),
                    Image3 = GetImageUrl(request.TempImage3 , request.CurrentImage3),
                    Image4 = GetImageUrl(request.TempImage4 , request.CurrentImage4),
                    Image5 = GetImageUrl(request.TempImage5 , request.CurrentImage5),
                };

                return await _adminCatalogService.UpdateProductAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновлении товара с файлами Id {id}", request.Id);
                throw;
            }
        }
    }
}
