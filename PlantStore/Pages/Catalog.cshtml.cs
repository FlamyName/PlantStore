using BusinessLogic.Core.Features.Queries;
using BusinessLogic.Core.Notification;
using BusinessLogic.Core.Notification.Extensions;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantStore.Core.Features.Queries;
using PlantStore.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace PlantStore.Pages
{
    public class CatalogModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CatalogModel> _logger;
        private const int PageSize = 20;

        public IEnumerable<ProductsViewModels>? Products { get; set; }
        public IEnumerable<CategoryViewModel>? Categories { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; } = 1;

        [FromQuery]
        [StringLength(50, ErrorMessage = "Поисковый запрос должен содержать максимум 50 символов")]
        public string? Search {  get; set; }

        [FromQuery]
        [StringLength(20, ErrorMessage = "Поисковый запрос должен содержать максимум 20 символов")]
        public string? Category { get; set; }
        [FromQuery]
        public bool HideOutOfStock { get; set; } = false;
        public bool HasMorePage => TotalItems > CurrentPage * PageSize;

        public CatalogModel(IMediator mediator, ILogger<CatalogModel> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            CurrentPage = 1;
            await LoadItemsAsync();
            await LoadCategoriesAsync();
            return Page();
            
        }

        public async Task<IActionResult> OnGetLoadMoreAsync([FromQuery] string? searchTerm, [FromQuery] string? category, [FromQuery] bool hideOutOfStock, [FromQuery] int page = 2)
        {
            Search = searchTerm;
            Category = category;
            CurrentPage = page;
            HideOutOfStock = hideOutOfStock;
            await LoadItemsAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Partial("_ProductItem", Products);
            }

            return RedirectToPage(new { search = searchTerm, category, hideOutOfStock, page });
        }

        public async Task LoadItemsAsync()
        {
            try
            {
                var products = await _mediator.Send(new GetProductsQuery
                {
                    SearchTerm = Search,
                    Category = Category,
                    PageSize = PageSize,
                    Page = CurrentPage,
                    HideOutOfStock = HideOutOfStock
                });

                Products = products.Items.ToList();
                TotalItems = products.TotalCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке товаров");
                Products = new List<ProductsViewModels>();
                TotalItems = 0;
            }
        }

        public async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _mediator.Send(new GetCategoryQuery { });

                Categories = categories.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке Категорий");
                Categories = new List<CategoryViewModel>();
            }
        }
    }
}
