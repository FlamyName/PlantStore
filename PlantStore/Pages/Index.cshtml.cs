using BusinessLogic.Core.Features.Queries;
using BusinessLogic.DB.Models;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantStore.Pages.Infrastructure.Abstractclass;

namespace PlantStore.Pages
{
    public class IndexModel : PagedPageModel<NewsViewModel>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IndexModel> _logger;

        public override int PageSize => 12;

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
                return Partial("_NewsItem", Items);
            }

            return RedirectToPage(new { page });
        }

        protected override async Task LoadItemsAsync()
        {
            try
            {
                var result = await _mediator.Send(new GetNewsQuery
                {
                    PageSize = PageSize,
                    Page = CurrentPage,
                });

                ApplyPage(result);

                _logger.LogInformation($"TotalItems: {TotalItems}, CurrentPage: {CurrentPage}, PageSize: {PageSize}");
                _logger.LogInformation($"HasMorePage calculation: {TotalItems} > {CurrentPage} * {PageSize} = {TotalItems > CurrentPage * PageSize}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке новостей");
                Items = new List<NewsViewModel>();
                TotalItems = 0;
            }
        }
    }
}
