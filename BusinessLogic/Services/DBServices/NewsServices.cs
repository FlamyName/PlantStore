using AutoMapper;
using BusinessLogic.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlantStore.DB;
using PlantStore.DB.Models;
using PlantStore.Services.DBServices.IDBServices;
using PlantStore.ViewModels;

namespace PlantStore.Services.DBServices
{
    /// <summary>
    /// Service для работы или преобразования данных из таблицы News
    /// </summary>
    public class NewsServices : INewsServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<NewsServices> _logger;
        public NewsServices(ApplicationDbContext context, IMapper mapper, ILogger<NewsServices> logger) 
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Получение всех элементов новостей их таблицы News
        /// </summary>
        public async Task<PagedResult<NewsViewModel>> GetAllNews(int page, int pageSize)
        {
            var query = _context.News.AsNoTracking();

            var pagedResult = await query
                .AsNoTracking()
                .OrderBy(x => x.DateNews)
                .Reverse()
                .ToPagedResultAsync<News, NewsViewModel>(page, pageSize, _mapper);

            _logger.LogInformation(
                "Загружено {pagedResult.Items.Count()} новостей из {pagedResult.TotalCount} (страница {pagedResult.CurrentPage})", 
                pagedResult.Items.Count(), 
                pagedResult.TotalCount, 
                pagedResult.CurrentPage);

            return pagedResult;
        }
    }
}
