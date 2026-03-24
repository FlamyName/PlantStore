using MediatR;
using PlantStore.Core.Features.Queries;
using PlantStore.Services.DBServices.IDBServices;
using PlantStore.ViewModels;

namespace PlantStore.Core.Features.Handlers
{
    public class GetNewsHandler : IRequestHandler<GetNewsQuery, PagedResult<NewsViewModel>>
    {
        private readonly INewsServices _service;
        private readonly ILogger<GetNewsHandler> _logger;

        public GetNewsHandler(INewsServices service, ILogger<GetNewsHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<PagedResult<NewsViewModel>> Handle(GetNewsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _service.GetAllNews(request.Page, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка товаров");
                return new PagedResult<NewsViewModel>()
                {
                    Items = new List<NewsViewModel>(),
                    TotalCount = 0,
                };
            }
        }
    }
}
