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

        public UpdateProductHandler(IAdminCatalogService adminCatalogService)
        {
            _adminCatalogService = adminCatalogService;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            return await _adminCatalogService.UpdateProductAsync(request);
        }
    }
}
