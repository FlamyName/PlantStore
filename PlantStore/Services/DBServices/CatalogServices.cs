using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PlantStore.DB;
using PlantStore.Services.DBServices.IDBServices;
using PlantStore.ViewModels;
using System.Runtime.InteropServices;

namespace PlantStore.Services.DBServices
{
    public class CatalogServices : ICatalogServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CatalogServices> _logger;

        public CatalogServices(ApplicationDbContext context, IMapper mapper, ILogger<CatalogServices> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<PagedResult<ProductsViewModels>> GetAllProductAsync(int page, int pageSize)
        {
          
            var totalCount = await _context.Products.CountAsync();

            var product = await _context.Products
                .AsNoTracking()
                .Include(x => x.Images.Where(x => x.IsMain))
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            var clothView = _mapper.Map<List<ProductsViewModels>>(product);

            return new PagedResult<ProductsViewModels>
            {
                Items = clothView,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
            };
        }
        public async Task<PagedResult<ProductsViewModels>> GetProductNameAsync(string name,int page, int pageSize)
        {
            var totalCount = await _context.Products.CountAsync(
                x => x.ProductName.ToLower().Contains(name));

            var product = await _context.Products
                .AsNoTracking()
                .Where(x => x.ProductName.ToLower().Contains(name))
                .Include(x => x.Images.Where(x => x.IsMain))
                .OrderBy (x => x.Id)
                .Skip((page - 1) * pageSize)
                .AsSplitQuery()
                .ToListAsync();

            var productView = _mapper.Map<List<ProductsViewModels>>(product);

            _logger.LogInformation("Поиск {name}: Загружено {cloth.Count} товаров из {totalCount} (страница {page})", name, product.Count, totalCount, page);

            return new PagedResult<ProductsViewModels>
            {
                Items = productView,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
            };
        }
    }
}
