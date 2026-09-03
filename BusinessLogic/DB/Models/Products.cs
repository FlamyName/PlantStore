using BusinessLogic.DB.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.DB.Models
{
    /// <summary>
    /// Таблица продуктов в БД
    /// </summary>
    public class Products
    {
        [Key]
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int? Count { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int? UnitsId { get; set; }
        public Units? Units { get; set; }
        public List<ProductImage> Images {  get; set; }
    }
}
