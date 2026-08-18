using BusinessLogic.Core.Features.Queries;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;
using PlantStore.Services.DBServices.IDBServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Core.Features.Handlers
{
    public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, List<CategoryViewModel>>
    {
        private readonly ICatalogServices _catalogServices;
        private readonly ILogger<GetCategoryHandler> _logger;

        public GetCategoryHandler(ICatalogServices catalogServices, ILogger<GetCategoryHandler> logger)
        {
            _catalogServices = catalogServices;
            _logger = logger;
        }

        public async Task<List<CategoryViewModel>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _catalogServices.GetAllCategoryAsync();  
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Ошибка при получении Категорий");
                return null;
            }
        }
    }
}
