using BusinessLogic.ViewModels;
using MediatR;

namespace BusinessLogic.Core.Features.Queries
{
    /// <summary>
    /// Запрос на получение конкретного товара по id
    /// </summary>
    public class GetProductIdQuery : IRequest<ProductIdViewModel>
    {
        public int Id { get; set; }
    }
}
