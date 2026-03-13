using PlantStore.ViewModels;

namespace PlantStore.Services.DBServices.IDBServices
{
    public interface ICatalogServices
    {
        Task<PagedResult<ProductsViewModels>> GetAllProductAsync(int page, int pageSize);
    }
}
