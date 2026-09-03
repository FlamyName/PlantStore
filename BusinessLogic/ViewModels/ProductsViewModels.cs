namespace BusinessLogic.ViewModels
{
    /// <summary>
    /// ViewModel для отображения данных продуктов
    /// </summary>
    public class ProductsViewModels
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public string UnitName { get; set; }
        public int Count { get; set; }
        public string Url { get; set; }
        public string NameCategory { get; set; }
    }
}
