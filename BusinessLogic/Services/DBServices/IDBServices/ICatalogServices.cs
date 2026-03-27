using PlantStore.ViewModels;

namespace PlantStore.Services.DBServices.IDBServices
{
    /// <summary>
    /// Interface реализующий сервис CatalogServices
    /// </summary>
    public interface ICatalogServices
    {
        /// <summary>
        /// Реализует метод получения всех продуктов с пагинацией
        /// </summary>
        Task<PagedResult<ProductsViewModels>> GetAllProductAsync(int page, int pageSize);

        /// <summary>
        /// Реализует метод получения всех продуктов по значению name с пагинацией
        /// </summary>
        Task<PagedResult<ProductsViewModels>> GetProductNameAsync(string name, int page, int pageSize);

        /// <summary>
        /// Реализует метод получения всех элементов опредленного продукта по значению id
        /// </summary>
        Task<ProductIdViewModel?> GetProductByIdAsync(int id);
    }
}
