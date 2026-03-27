using System.ComponentModel.DataAnnotations;

namespace PlantStore.DB.Models
{
    /// <summary>
    /// Таблица новостей в БД
    /// </summary>
    public class News
    {
        [Key]
        public int Id { get; set; }
        public string NameNews { get; set; }
        public string UrlNews { get; set; }
        public string TitleNews { get; set; }
        public string DescriptionNews { get; set; }
        public DateTime DateNews { get; set; }
        
    }
}
