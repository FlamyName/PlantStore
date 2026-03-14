using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlantStore.Core.Features.Queries;
using PlantStore.ViewModels;

namespace PlantStore.Pages
{
    public class PlantModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PlantModel> _logger;
        
        public ProductIdViewModel? Product { get; set; }
        
        public PlantModel(IMediator mediator, ILogger<PlantModel> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Product = await _mediator.Send(new GetProductIdQuery
                {
                    Id = id
                });

                if (Product == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
