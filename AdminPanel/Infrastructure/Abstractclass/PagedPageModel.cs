using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.Infrastructure.Abstractclass
{
    public abstract class PagedPageModel<TItem> : PageModel
    {
        public abstract int PageSize { get; }

        public IEnumerable<TItem>? Items { get; protected set; }
        public int TotalItems { get; protected set; }
        public int CurrentPage { get; protected set; } = 1;

        public bool HasMorePage => TotalItems > CurrentPage * PageSize;

        /// <summary>
        /// Заполняет состояние из PagedResult<TItem>, который возвращает
        /// ToPagedResultAsync в BusinessLogic.
        /// </summary>
        protected void ApplyPage(PagedResult<TItem> result)
        {
            Items = result.Items.ToList();
            TotalItems = result.TotalCount;
        }

        /// <summary>
        /// Загружает данные текущей страницы. Наследник реализует свой запрос.
        /// </summary>
        protected abstract Task LoadItemsAsync();
    }
}
