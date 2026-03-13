using MediatR;
using PlantStore.Core.Features.Queries;
using PlantStore.Services.DBServices.IDBServices;
using PlantStore.ViewModels;

namespace PlantStore.Core.Features.Handlers
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductsViewModels>>
    {
        private readonly ICatalogServices _catalogServices;
        private readonly ILogger<GetProductsHandler> _logger;

        public GetProductsHandler(ICatalogServices catalogServices, ILogger<GetProductsHandler> logger)
        {
            _catalogServices = catalogServices;
            _logger = logger;
        }

        public async Task<PagedResult<ProductsViewModels>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if(string.IsNullOrEmpty(request.SearchTerm))
                {
                    return await _catalogServices.GetAllProductAsync(request.Page, request.PageSize);
                }
                else
                {
                    return await _catalogServices.GetProductNameAsync(request.SearchTerm, request.Page, request.PageSize);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка товаров");
                return new PagedResult<ProductsViewModels>
                {
                    Items = new List<ProductsViewModels>(),
                    TotalCount = 0
                };
            }
        }
    }
}
