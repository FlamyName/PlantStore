namespace PlantStore.Pages.Infrastructure.ViewModels
{
    public class ContentErrorViewModel
    {
        public string Message { get; set; } 

        public string RetryAction { get; set; } = "reload";

        public string? Region { get; set; }
    }
}
