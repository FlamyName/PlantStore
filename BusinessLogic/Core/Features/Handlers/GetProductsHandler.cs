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

        public GetProductsHandler(ICatalogServices catalogServices)
        {
            _catalogServices = catalogServices;
        }

        /// <summary>
        /// Обрабатывает запрос на получение списка товаров с пагинацией
        /// </summary>
        public async Task<PagedResult<ProductsViewModels>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            if (request.SearchTerm?.Length > 50)
            {
                return new PagedResult<ProductsViewModels>
                {
                    Items = new List<ProductsViewModels>(),
                    TotalCount = 0
                };
            }

            return string.IsNullOrEmpty(request.SearchTerm)
                ? await _catalogServices.GetAllProductAsync(
                    request.Page, request.PageSize, request.Category, request.HideOutOfStock)
                : await _catalogServices.GetProductNameAsync(
                    request.SearchTerm, request.Page, request.PageSize, request.Category, request.HideOutOfStock);
        }
    }
}
