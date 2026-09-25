using BusinessLogic.Core.Features.Queries;
using BusinessLogic.Services.DBServices.IDBServices;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Core.Features.Handlers
{
    public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, List<CategoryViewModel>>
    {
        private readonly ICatalogServices _catalogServices;

        public GetCategoryHandler(ICatalogServices catalogServices)
        {
            _catalogServices = catalogServices;
        }

        public async Task<List<CategoryViewModel>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            return await _catalogServices.GetAllCategoryAsync();
        }
    }
}
