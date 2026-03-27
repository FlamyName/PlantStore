using MediatR;
using PlantStore.ViewModels;

namespace PlantStore.Core.Features.Queries
{
    /// <summary>
    /// Запрос на получение списка новостей с пагинацией
    /// </summary>
    public class GetNewsQuery : IRequest<PagedResult<NewsViewModel>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
