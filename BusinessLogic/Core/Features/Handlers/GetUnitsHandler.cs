using BusinessLogic.Core.Features.Queries;
using BusinessLogic.Services.DBServices.IDBServices;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;

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
