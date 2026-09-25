using BusinessLogic.Core.Features.Queries;
using BusinessLogic.DB.Models;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantStore.Pages.Infrastructure.Abstractclass;
using PlantStore.Pages.Infrastructure.ViewModels;

namespace PlantStore.Pages
{
    public class IndexModel : PagedPageModel<NewsViewModel>
    {
        private readonly IMediator _mediator;

        public override int PageSize => 12;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentPage = 1;
            await LoadItemsSafeAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetLoadMoreAsync([FromQuery] int page = 2)
        {
            CurrentPage = page;
            await LoadItemsSafeAsync();

            if (LoadFailed)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return Partial("_ContentError", new ContentErrorViewModel
                {
                    Message = "Не удалось загрузить Новости",
                    RetryAction = "loadMore",
                    Region = "news"
                });
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Partial("_NewsItem", Items);
            }

            return RedirectToPage(new { page });
        }

        protected override async Task LoadItemsAsync()
        {
            var result = await _mediator.Send(new GetNewsQuery
            {
                PageSize = PageSize,
                Page = CurrentPage,
            });

            ApplyPage(result);
        }
    }
}
