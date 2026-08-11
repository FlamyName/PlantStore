using BusinessLogic.Services.DBServices.AdminService;
using BusinessLogic.Services.DBServices.FileStorageService;
using BusinessLogic.Services.DBServices.IDBServices;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlantStore.DB;
using PlantStore.Services.DBServices;
using PlantStore.Services.DBServices.IDBServices;
using System.Reflection;

namespace BusinessLogic
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
            });

            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
            });
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped<ICatalogServices, CatalogServices>();
            services.AddScoped<INewsServices, NewsServices>();
            services.AddScoped<IAdminCatalogService, AdminCatalogService>();
            services.AddScoped<IFileStorageService, FileStorageService>();

            return services;
        }
    }
}
