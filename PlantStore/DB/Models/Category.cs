using System.ComponentModel.DataAnnotations;

namespace PlantStore.DB.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string NameCategory { get; set; }
        public List<Products> Products { get; set; }
    }
}
