using AutoMapper;
using BusinessLogic.ViewModels;
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
                    y => y.MapFrom(a => a.Images.FirstOrDefault(i => i.IsMain)!.Url))
                .ForMember(x => x.NameCategory,
                    y => y.MapFrom(a => a.Category.NameCategory))
                .ForMember(x => x.UnitName,
                    y => y.MapFrom(a => a.Units!.NameUnit));


            /// <summary>
            /// Преобразования данных из News в ViewModel
            /// </summary>
            CreateMap<News, NewsViewModel>();
            CreateMap<Category, CategoryViewModel>();
        }
    }
}
