using MediatR;
using PlantStore.ViewModels;

namespace PlantStore.Core.Features.Queries
{
    public class GetProductsQuery : IRequest<PagedResult<ProductsViewModels>>
    {
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;
    }
}
