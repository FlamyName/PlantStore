using BusinessLogic.Core.Features.Queries;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;
using PlantStore.Services.DBServices.IDBServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Core.Features.Handlers
{
    public class GetUnitsHandler : IRequestHandler<GetUnitsQuery, List<UnitsViewModel>>
    {
        private readonly ICatalogServices _catalogServices;
        private readonly ILogger<GetUnitsHandler> _logger;
        public GetUnitsHandler(ICatalogServices catalogServices, ILogger<GetUnitsHandler> logger)
        {
            _catalogServices = catalogServices;
            _logger = logger;
        }

        public async Task<List<UnitsViewModel>> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _catalogServices.GetAllUnitsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получении Единиц измерения");
                return null;
            }
        }
    }
}
