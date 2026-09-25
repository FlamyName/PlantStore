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
        public GetUnitsHandler(ICatalogServices catalogServices)
        {
            _catalogServices = catalogServices;
        }

        public async Task<List<UnitsViewModel>> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
        {
            return await _catalogServices.GetAllUnitsAsync();
        }
    }
}
