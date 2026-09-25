using BusinessLogic.Core.Features.Queries;
using BusinessLogic.Services.DBServices.IDBServices;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Core.Features.Handlers
{
    /// <summary>
    /// Обработчик запроса <see cref="GetNewsQuery"/>
    /// </summary>
    public class GetNewsHandler : IRequestHandler<GetNewsQuery, PagedResult<NewsViewModel>>
    {
        private readonly INewsServices _service;

        public GetNewsHandler(INewsServices service)
        {
            _service = service;
        }

        /// <summary>
        /// Обработывает запрос на получения новостей с пагинацией
        /// </summary>
        public async Task<PagedResult<NewsViewModel>> Handle(GetNewsQuery request, CancellationToken cancellationToken)
        {
            if (request.Page == 2) throw new InvalidOperationException("Тест: догрузка");

            return await _service.GetAllNews(request.Page, request.PageSize);
        }
    }
}
