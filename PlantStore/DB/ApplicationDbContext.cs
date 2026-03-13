using Microsoft.EntityFrameworkCore;
using PlantStore.DB.Models;

namespace PlantStore.DB
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
    }
}
