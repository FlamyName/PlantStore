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
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IAdminCatalogService _adminCatalogService;
        private readonly ILogger<UpdateProductHandler> _logger;

        public UpdateProductHandler(IAdminCatalogService adminCatalogService, ILogger<UpdateProductHandler> logger)
        {
            _adminCatalogService = adminCatalogService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _adminCatalogService.UpdateProductAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновлении товара с файлами Id {id}", request.Id);
                throw;
            }
        }
    }
}
