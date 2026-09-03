using BusinessLogic.DB;
using BusinessLogic.Services.BackgroundServices;
using BusinessLogic.Services.DBServices;
using BusinessLogic.Services.DBServices.AdminService;
using BusinessLogic.Services.DBServices.FileStorageService;
using BusinessLogic.Services.DBServices.IDBServices;
using BusinessLogic.Services.ImageServices;
using BusinessLogic.Services.ImageServices.IImageServices;
using BusinessLogic.Services.ValidationServices;
using BusinessLogic.Services.ValidationServices.IValidationServices;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<IImageResolver, ImageResolver>();
            services.AddScoped<IImageValidationService, ImageValidationService>();

            services.AddHostedService<TempFileCleanupService>();

            return services;
        }
    }
}
