using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantStore.Core.Features.Queries;
using PlantStore.ViewModels;
using System.Runtime.InteropServices;

namespace PlantStore.Pages
{
    public class CatalogModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CatalogModel> _logger;
        private const int PageSize = 6;

        public IEnumerable<ProductsViewModels>? Products { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; } = 1;

        [FromQuery]
        public string? Search {  get; set; }
        public bool HasMorePage => TotalItems > CurrentPage * PageSize;

        public CatalogModel(IMediator mediator, ILogger<CatalogModel> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentPage = 1;
            await LoadItemsAsync();
            return Page();
            
        }

        public async Task<IActionResult> OnGetLoadMoreAsync([FromQuery] string? searchTerm, [FromQuery] int page = 2)
        {
            Search = searchTerm;
            CurrentPage = page;
            await LoadItemsAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Partial("_ProductItem", Products);
            }

            return RedirectToPage(new { search = searchTerm, page });
        }

        public async Task LoadItemsAsync()
        {
            try
            {
                var result = await _mediator.Send(new GetProductsQuery
                {
                    SearchTerm = Search,
                    PageSize = PageSize,
                    Page = CurrentPage,
                });

                Products = result.Items.ToList();
                TotalItems = result.TotalCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке товаров");
                Products = new List<ProductsViewModels>();
                TotalItems = 0;
            }
        }
    }
}
