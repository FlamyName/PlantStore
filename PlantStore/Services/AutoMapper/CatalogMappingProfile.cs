using AutoMapper;
using PlantStore.DB.Models;
using PlantStore.ViewModels;

namespace PlantStore.Services.AutoMapper
{
    public class CatalogMappingProfile : Profile
    {
        public CatalogMappingProfile()
        {
            CreateMap<Products, ProductsViewModels>()
                .ForMember(x => x.Url,
                    y => y.MapFrom(a => a.Images.FirstOrDefault(i => i.IsMain)!.Url));
        }
    }
}
