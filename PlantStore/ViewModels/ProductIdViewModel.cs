using PlantStore.DB.Models;

namespace PlantStore.ViewModels
{
    public class ProductIdViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public List<ProductImageViewModel> Images { get; set; }
    }
}
