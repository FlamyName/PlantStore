using BusinessLogic.DB.Models;
using Microsoft.EntityFrameworkCore;
using PlantStore.DB.Models;

namespace PlantStore.DB
{
    /// <summary>
    /// DbContext для подключения к базе данных 
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Units> Units { get; set; }
    }
}
