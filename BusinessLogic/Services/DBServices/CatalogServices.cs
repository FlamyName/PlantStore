using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlantStore.DB;
using PlantStore.Services.DBServices.IDBServices;
using PlantStore.ViewModels;
using System.Runtime.InteropServices;

namespace PlantStore.Services.DBServices
{
    /// <summary>
    /// Service для работы или преобразования данных из таблицы Products и связанных с ней 
    /// </summary>
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

        /// <summary>
        /// Получение всего списка элементов из таблицы Products с пагинацией
        /// </summary>
        public async Task<PagedResult<ProductsViewModels>> GetAllProductAsync(int page, int pageSize)
        {
          
            var totalCount = await _context.Products.CountAsync();

            var product = await _context.Products
                .AsNoTracking()
                .Include(x => x.Images.Where(x => x.IsMain))
                .OrderBy(x => x.Id)
                .Reverse()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync();

            var clothView = _mapper.Map<List<ProductsViewModels>>(product);

            _logger.LogInformation(" Загружено {cloth.Count} товаров из {totalCount} (страница {page})", product.Count, totalCount, page);

            return new PagedResult<ProductsViewModels>
            {
                Items = clothView,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
            };
        }

        /// <summary>
        /// Получение списка элементов из таблицы Products по заданному значению name с пагинацией
        /// </summary>
        public async Task<PagedResult<ProductsViewModels>> GetProductNameAsync(string name,int page, int pageSize)
        {
            var totalCount = await _context.Products.CountAsync(
                x => x.ProductName.ToLower().Contains(name));

            var product = await _context.Products
                .AsNoTracking()
                .Where(x => x.ProductName.ToLower().Contains(name))
                .Include(x => x.Images.Where(x => x.IsMain))
                .OrderBy (x => x.Id)
                .Reverse()
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

        /// <summary>
        /// Получение списка элементов определенного продукта по значению id
        /// </summary>
        public async Task<ProductIdViewModel?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(x => x.Images.OrderBy(x => x.DisplayOrder))
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                _logger.LogInformation("Товар с Id {id} не найден", id);
                return null!;
            }

            var productView = new ProductIdViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                Images = product.Images.Select(x => new ProductImageViewModel
                {
                    Id = x.Id,
                    DisplayOrder = x.DisplayOrder,
                    IsMain = x.IsMain,
                    Url = x.Url,
                }).ToList()
            };

            _logger.LogInformation("Загружен товар {productName} (Id {id} c {imageCount} изображениями)", product.ProductName, id, product.Images.Count);

            return productView;
        }
    }
}
