using MediatR;
using PlantStore.ViewModels;

namespace PlantStore.Core.Features.Queries
{
    /// <summary>
    /// Запрос на получение конкретного товара по id
    /// </summary>
    public class GetProductIdQuery : IRequest<ProductIdViewModel>
    {
        public int Id { get; set; }
    }
}
