using AutoMapper;
using PlantStore.DB.Models;
using PlantStore.ViewModels;

namespace PlantStore.Services.AutoMapper
{
    public class CatalogMappingProfile : Profile
    {
        /// <summary>
        /// Mapping Профиль
        /// </summary>
        public CatalogMappingProfile()
        {
            /// <summary>
            /// Преобразования данных из Products в ViewModel
            /// </summary>
            CreateMap<Products, ProductsViewModels>()
                .ForMember(x => x.Url,
                    y => y.MapFrom(a => a.Images.FirstOrDefault(i => i.IsMain)!.Url));

            /// <summary>
            /// Преобразования данных из News в ViewModel
            /// </summary>
            CreateMap<News, NewsViewModel>();
        }
    }
}
