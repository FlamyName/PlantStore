using BusinessLogic.ViewModels;
using PlantStore.ViewModels;

namespace AdminPanel.ViewModels
{
    public class EditProductViewModel
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int Count { get; set; }
        public int CategoryId { get; set; }
        public int? UnitId { get; set; }         
        public List<ProductImageViewModel>? Images { get; set; } 
        public List<CategoryViewModel>? Category { get; set; } 
        public List<UnitsViewModel>? Units { get; set; } 
    }
}
