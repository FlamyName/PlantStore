using PlantStore.ViewModels;

namespace PlantStore.Services.DBServices.IDBServices
{
    public interface INewsServices
    {
        Task<PagedResult<NewsViewModel>> GetAllNews(int page, int pageSize);
    }
}
