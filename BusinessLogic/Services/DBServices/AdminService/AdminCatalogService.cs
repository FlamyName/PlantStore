using AutoMapper;
using BusinessLogic.Core.Features.Commands;
using BusinessLogic.DB;
using BusinessLogic.DB.Models;
using BusinessLogic.Services.DBServices.IDBServices;
using BusinessLogic.Services.ImageServices.IImageServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Services.DBServices.AdminService
{
    public class AdminCatalogService : IAdminCatalogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminCatalogService> _logger;
        private readonly IFileStorageService _fileStorage;
        private readonly IImageResolver _imageResolver;

        public AdminCatalogService(ApplicationDbContext context, IMapper mapper, ILogger<AdminCatalogService> logger, IFileStorageService fileStorageService, IImageResolver imageResolver)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _fileStorage = fileStorageService;
            _imageResolver = imageResolver;
        }

        public async Task<bool> UpdateProductAsync(UpdateProductCommand command)
        {
            var urls = _imageResolver.Resolve(command.TempImages, command.CurrentImages);

            var permanentUrls = await _fileStorage.MoveTempFilesToPermamentAsync(urls, "products");

            return await UpdateProductInternalAsync(command, permanentUrls);
        }

        private async Task<bool> UpdateProductInternalAsync(UpdateProductCommand command, List<string> finalUrls)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == command.Id);

            if (product == null)
            {
                return false;
            }

            product.ProductName = command.Name;
            product.Description = command.Description;
            product.Price = command.Price;
            product.Count = command.Count;
            product.CategoryId = command.CategoryId;
            product.UnitsId = command.UnitId;

            var existingImages = product.Images.OrderBy(i => i.DisplayOrder).ToList();

            for(int i = 0; i < 5; i++)
            {
                int position = i + 1;
                string newUrl = finalUrls.ElementAtOrDefault(i);

                ProductImage existingImg = (i < existingImages.Count) ? existingImages[i] : null;

                if (string.IsNullOrEmpty(newUrl))
                {
                    if (existingImg != null) 
                        _context.ProductImages.Remove(existingImg);
                }
                else
                {
                    if (existingImg != null)
                    {
                        existingImg.Url = newUrl;
                        existingImg.DisplayOrder = position;
                        existingImg.IsMain = (position == 1);
                    }
                    else
                    {
                        product.Images.Add(new ProductImage
                        {
                            Url = newUrl,
                            DisplayOrder = position,
                            IsMain = (position == 1)
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
