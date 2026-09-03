using BusinessLogic.ViewModels;

namespace BusinessLogic.Services.DBServices.IDBServices
{
    /// <summary>
    /// Interface реализующий сервис NewsServices
    /// </summary>
    public interface INewsServices
    {
        /// <summary>
        /// Реализует метод получения всех новостей с пагинацией
        /// </summary>
        Task<PagedResult<NewsViewModel>> GetAllNews(int page, int pageSize);
    }
}
