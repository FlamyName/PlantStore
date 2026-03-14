using MediatR;
using PlantStore.Core.Features.Queries;
using PlantStore.Services.DBServices.IDBServices;
using PlantStore.ViewModels;

namespace PlantStore.Core.Features.Handlers
{
    public class GetProductIdHandler : IRequestHandler<GetProductIdQuery, ProductIdViewModel?>
    {
		private readonly ICatalogServices _catalogService;
		private readonly ILogger<GetProductIdHandler> _logger;

		public GetProductIdHandler(ICatalogServices catalogService, ILogger<GetProductIdHandler> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }

        public async Task<ProductIdViewModel?> Handle(GetProductIdQuery request, CancellationToken cancellationToken)
        {
			try
			{
                return await _catalogService.GetProductByIdAsync(request.Id);
			}
			catch (Exception ex)
			{
                _logger.LogInformation(ex,"Ошибка при получении товара с Id {id}", request.Id);
				return null;
			}
        }
    }
}
