using BusinessLogic.Core.Features.Queries;
using BusinessLogic.Services.DBServices.IDBServices;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Core.Features.Handlers
{
    /// <summary>
    /// Обработчик запроса <see cref="GetProductIdQuery"/>
    /// </summary>
    public class GetProductIdHandler : IRequestHandler<GetProductIdQuery, ProductIdViewModel?>
    {
		private readonly ICatalogServices _catalogService;

		public GetProductIdHandler(ICatalogServices catalogService)
        {
            _catalogService = catalogService;
        }

        /// <summary>
        /// Обрабатывает запрос на получение списка элементов опредленного товара
        /// </summary>
        public async Task<ProductIdViewModel?> Handle(GetProductIdQuery request, CancellationToken cancellationToken)
        {
            return await _catalogService.GetProductByIdAsync(request.Id);
        }
    }
}
