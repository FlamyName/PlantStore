using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.DB.Models
{
    /// <summary>
    /// Таблица категорий в БД
    /// </summary>
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string NameCategory { get; set; }
        public List<Products> Products { get; set; }
    }
}
