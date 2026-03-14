using MediatR;
using PlantStore.ViewModels;

namespace PlantStore.Core.Features.Queries
{
    public class GetProductIdQuery : IRequest<ProductIdViewModel>
    {
        public int Id { get; set; }
    }
}
