using System.ComponentModel.DataAnnotations;

namespace PlantStore.DB.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsMain { get; set; }
        public int ProductsId { get; set; }
        public Products Products { get; set; }
    }
}
