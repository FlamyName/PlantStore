namespace PlantStore.ViewModels
{
    /// <summary>
    /// ViewModel для отображения данных продуктов с пагинацией
    /// </summary>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
