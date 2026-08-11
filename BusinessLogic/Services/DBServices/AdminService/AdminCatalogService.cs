using AutoMapper;
using BusinessLogic.Core.Features.Commands;
using BusinessLogic.Services.DBServices.IDBServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlantStore.DB;
using PlantStore.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.DBServices.AdminService
{
    public class AdminCatalogService : IAdminCatalogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminCatalogService> _logger;
        private readonly IFileStorageService _fileStorage;

        public AdminCatalogService(ApplicationDbContext context, IMapper mapper, ILogger<AdminCatalogService> logger, IFileStorageService fileStorageService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _fileStorage = fileStorageService;
        }

        public async Task<bool> UpdateProductAsync(UpdateProductCommand command)
        {
            var urls = command.ImageUrls
                .OrderBy(x => x.Position)
                .Select(x => x.Url)
                .ToList();

            var permanentUrls = await _fileStorage.MoveTempFilesToPermamentAsync(urls, "products");

            var updateCommand = new UpdateProductCommand
            {
                Id = command.Id,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Image1 = permanentUrls.ElementAtOrDefault(0),
                Image2 = permanentUrls.ElementAtOrDefault(1),
                Image3 = permanentUrls.ElementAtOrDefault(2),
                Image4 = permanentUrls.ElementAtOrDefault(3),
                Image5 = permanentUrls.ElementAtOrDefault(4)
            };

            return await UpdateProductInternalAsync(updateCommand);
        }

        private async Task<bool> UpdateProductInternalAsync(UpdateProductCommand command)
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

            var newUrls = command.ImageUrls;
            var existingImages = product.Images.OrderBy(i => i.DisplayOrder).ToList();

            for(int i = 0; i < 5; i++)
            {
                int position = i + 1;

                var urlEntry = newUrls.FirstOrDefault(x => x.Position == position);
                string newUrl = urlEntry.Url ?? null;

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
