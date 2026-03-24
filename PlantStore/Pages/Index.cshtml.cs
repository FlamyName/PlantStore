using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantStore.Core.Features.Queries;
using PlantStore.DB.Models;
using PlantStore.ViewModels;

namespace PlantStore.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IndexModel> _logger;
        private const int PageSize = 12;

        public IEnumerable<NewsViewModel>? News { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; } = 1;
        public bool HasMorePage => TotalItems > CurrentPage * PageSize;

        public IndexModel(IMediator mediator ,ILogger<IndexModel> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentPage = 1;
            await LoadItemsAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetLoadMoreAsync([FromQuery] int page = 2)
        {
            CurrentPage = page;
            await LoadItemsAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Partial("_NewsItem", News);
            }

            return RedirectToPage(new { page });
        }

        public async Task LoadItemsAsync()
        {
            try
            {
                var result = await _mediator.Send(new GetNewsQuery
                {
                    PageSize = PageSize,
                    Page = CurrentPage,
                });

                News = result.Items.ToList();
                TotalItems = result.TotalCount;
                _logger.LogInformation($"TotalItems: {TotalItems}, CurrentPage: {CurrentPage}, PageSize: {PageSize}");
                _logger.LogInformation($"HasMorePage calculation: {TotalItems} > {CurrentPage} * {PageSize} = {TotalItems > CurrentPage * PageSize}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке товаров");
                News = new List<NewsViewModel>();
                TotalItems = 0;
            }
        }
    }
}
