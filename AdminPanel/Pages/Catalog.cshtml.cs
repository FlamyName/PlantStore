using AdminPanel.Pages.Infrastructure.Abstractclass;
using AdminPanel.ViewModels;
using AutoMapper;
using BusinessLogic.Core.Features.Commands;
using BusinessLogic.Core.Features.Queries;
using BusinessLogic.Core.Notification;
using BusinessLogic.Core.Notification.Extensions;
using BusinessLogic.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Pages
{
    public class CatalogModel : PagedPageModel<ProductsViewModels>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<CatalogModel> _logger;

        public override int PageSize => 20;

        [FromQuery]
        [StringLength(50, ErrorMessage = "Поисковый запрос должен содержать максимум 50 символов")]
        public string? Search {  get; set; }

        public CatalogModel(IMediator mediator, ILogger<CatalogModel> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
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
                return Partial("_ProductItem", Items);
            }

            return RedirectToPage(new { search = searchTerm, page });
        }

        public async Task<IActionResult> OnGetEditModal(int id)
        {
            var product = await _mediator.Send(new GetProductIdQuery
            {
                Id = id
            });

            if (product == null)
            {
                return NotFound();
            }

            var category = await _mediator.Send(new GetCategoryQuery());
            var units = await _mediator.Send(new GetUnitsQuery());

            var model = _mapper.Map<EditProductViewModel>(product);
            model.Category = category;
            model.Units = units;

            return Partial("_EditProductModal", model);
        }



        public async Task<IActionResult> OnPostUploadTempImage(IFormFile file, int index)
        {
            try
            {
                var command = new UploadTempImageCommand { File = file };
                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return Partial("_ImageSlot", new ImageSlotViewModel
                    {
                        Index = index,
                        Error = result.ErrorMessage ?? "Ошибка загрузки"
                    });
                }

                //  Возвращаем обновленный слот с временным URL
                return Partial("_ImageSlot", new ImageSlotViewModel
                {
                    Index = index,
                    TempUrl = result.Url,
                    IsMain = index == 1
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Ошибка валидации при загрузке изображения");
                return Partial("_ImageSlot", new ImageSlotViewModel
                {
                    Index = index,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке временного файла");
                return Partial("_ImageSlot", new ImageSlotViewModel
                {
                    Index = index,
                    Error = "Произошла ошибка при загрузке изображения"
                });
            }
        }

        //  НОВЫЙ МЕТОД: Удаление изображения из слота
        public IActionResult OnPostRemoveImage(int index)
        {
            return Partial("_ImageSlot", new ImageSlotViewModel
            {
                Index = index,
                Url = null,
                TempUrl = null,
                IsMain = index == 1
            });
        }


        public async Task<IActionResult> OnPostUpdateProductWithFiles(UpdateProductWithFilesRequest request)
        {
            try
            {
                var command = _mapper.Map<UpdateProductCommand>(request);
                var result = await _mediator.Send(command);

                if (result)
                {
                    return Partial("_Notification", new NotificationViewModel
                    {
                        Message = "Товар успешно обновлён!",
                        Type = NotificationType.Success.ToNotificationType()
                    });
                }
                else
                {
                    return Partial("_Notification", new NotificationViewModel
                    {
                        Message = "Товар не найден",
                        Type = NotificationType.Error.ToNotificationType()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении товара Id {Id}", request.Id);
                return Partial("_Notification", new NotificationViewModel
                {
                    Message = "Ошибка при обновлении: " + ex.Message,
                    Type = NotificationType.Error.ToNotificationType()
                });
            }

        }

        protected override async Task LoadItemsAsync()
        {
            try
            {
                var result = await _mediator.Send(new GetProductsQuery
                {
                    SearchTerm = Search,
                    PageSize = PageSize,
                    Page = CurrentPage,
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
    }
}
