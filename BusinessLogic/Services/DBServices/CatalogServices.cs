using AutoMapper;
using BusinessLogic.Extensions;
using BusinessLogic.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlantStore.DB;
using PlantStore.DB.Models;
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

        public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
        {
            var category = await _context.Categories.ToListAsync();

            var categoryView = _mapper.Map<List<CategoryViewModel>>(category);

            _logger.LogInformation("Загружено {category.Count} категорий", category.Count);

            return categoryView;
        }

        /// <summary>
        /// Получение всего списка элементов из таблицы Products с пагинацией
        /// </summary>
        public async Task<PagedResult<ProductsViewModels>> GetAllProductAsync(int page, int pageSize, string? category, bool hideOutOfStock)
        {
            var query = _context.Products.AsNoTracking();

            // Фильтр по категории (если указана)
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.NameCategory == category);
            }

            if (hideOutOfStock)
            {
                query = query.Where(p => p.Count > 0); // 👈 Фильтр
            }

            var pagedResult = await query
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Reverse()
                .ToPagedResultAsync<Products, ProductsViewModels>(page, pageSize, _mapper);


            _logger.LogInformation(
                "Загружено {pagedResult.Items.Count} товаров из {pagedResult.TotalCount} (страница {page}), (категория - {category})",
                pagedResult.Items.Count(),
                pagedResult.TotalCount,
                pagedResult.CurrentPage,
                category);

            return pagedResult;
        }



        ///<summary>
        ///Получение списка элементов из таблицы Products по категории с пагинацией
        /// </summary> 
        //public async Task<PagedResult<ProductsViewModels>> GetProductsByCategoryAsync(int page, int pageSize)
        //{

        //}

        /// <summary>
        /// Получение списка элементов из таблицы Products по заданному значению name с пагинацией
        /// </summary>
        public async Task<PagedResult<ProductsViewModels>> GetProductNameAsync(string name,int page, int pageSize, string? category, bool hideOutOfStock)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(x => x.ProductName.ToLower().Contains(name));

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.NameCategory == category);
            }

            if (hideOutOfStock)
            {
                query = query.Where(p => p.Count > 0); // 👈 Фильтр
            }

            var pagedResult = await query
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Reverse()
                .ToPagedResultAsync<Products, ProductsViewModels>(page, pageSize, _mapper);

            _logger.LogInformation(
                "Загружено {pagedResult.Items.Count} товаров из {pagedResult.TotalCount} (страница {page}), (категория - {category})",
                pagedResult.Items.Count(),
                pagedResult.TotalCount,
                pagedResult.CurrentPage,
                category);

            return pagedResult;
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
                CategoryId = product.CategoryId,
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
