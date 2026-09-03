using BusinessLogic.Core.Features.Queries;
using BusinessLogic.Services.DBServices.IDBServices;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Core.Features.Handlers
{
    /// <summary>
    /// Обработчик запроса <see cref="GetProductsQuery"/>
    /// </summary>
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductsViewModels>>
    {
        private readonly ICatalogServices _catalogServices;
        private readonly ILogger<GetProductsHandler> _logger;

        public GetProductsHandler(ICatalogServices catalogServices, ILogger<GetProductsHandler> logger)
        {
            _catalogServices = catalogServices;
            _logger = logger;
        }

        /// <summary>
        /// Обрабатывает запрос на получение списка товаров с пагинацией
        /// </summary>
        public async Task<PagedResult<ProductsViewModels>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.SearchTerm))
                {
                    return await _catalogServices.GetAllProductAsync(request.Page, request.PageSize, request.Category, request.HideOutOfStock);
                }
                else if (request.SearchTerm.Length > 50)
                {
                    return new PagedResult<ProductsViewModels>
                    {
                        Items = new List<ProductsViewModels>(),
                        TotalCount = 0
                    };
                }
                else
                {
                    return await _catalogServices.GetProductNameAsync(request.SearchTerm, request.Page, request.PageSize, request.Category, request.HideOutOfStock);
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
