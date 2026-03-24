using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlantStore.DB;
using PlantStore.Services.DBServices.IDBServices;
using PlantStore.ViewModels;

namespace PlantStore.Services.DBServices
{
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

        public async Task<PagedResult<NewsViewModel>> GetAllNews(int page, int pageSize)
        {
            var totalCount = await _context.News.CountAsync();

            var news = await _context.News
                .AsNoTracking()
                .OrderBy(x => x.DateNews)
                .Reverse()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var newsView = _mapper.Map<List<NewsViewModel>>(news);

            _logger.LogInformation("Загружено {news.Count} новостей из {totalCount} (страница {page})", news.Count, totalCount, page);

            return new PagedResult<NewsViewModel>()
            {
                Items = newsView,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
            };
        }
    }
}
