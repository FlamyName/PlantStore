using BusinessLogic.Core.Features.Queries;
using BusinessLogic.Core.Notification;
using BusinessLogic.Core.Notification.Extensions;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantStore.Pages.Infrastructure.Abstractclass;
using System.ComponentModel.DataAnnotations;

namespace PlantStore.Pages
{
    public class CatalogModel : PagedPageModel<ProductsViewModels>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CatalogModel> _logger;
        public override int PageSize => 20;
        public IEnumerable<CategoryViewModel>? Categories { get; set; }

        [FromQuery]
        [StringLength(50, ErrorMessage = "Поисковый запрос должен содержать максимум 50 символов")]
        public string? Search {  get; set; }

        [FromQuery]
        [StringLength(20, ErrorMessage = "Название категории должен быть 20 символов")]
        public string? Category { get; set; }
        [FromQuery]
        public bool HideOutOfStock { get; set; } = false;

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
                return Partial("_ProductItem", Items);
            }

            return RedirectToPage(new { search = searchTerm, category, hideOutOfStock, page });
        }

        protected override async Task LoadItemsAsync()
        {
            try
            {
                var result = await _mediator.Send(new GetProductsQuery
                {
                    SearchTerm = Search,
                    Category = Category,
                    PageSize = PageSize,
                    Page = CurrentPage,
                    HideOutOfStock = HideOutOfStock
                });

                ApplyPage(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке товаров");
                Items = new List<ProductsViewModels>();
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
                _logger.LogError(ex, "Ошибка при загрузке категорий");
                Categories = new List<CategoryViewModel>();
            }
        }
    }
}
