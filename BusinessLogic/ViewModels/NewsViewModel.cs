namespace PlantStore.ViewModels
{
    /// <summary>
    /// ViewModel для отображения новостей
    /// </summary>
    public class NewsViewModel
    {
        public string NameNews { get; set; }
        public string UrlNews { get; set; }
        public string TitleNews { get; set; }
        public string DescriptionNews { get; set; }
        public DateTime DateNews { get; set; }
    }
}
