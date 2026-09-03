using AutoMapper;
using BusinessLogic.DB.Models;
using BusinessLogic.ViewModels;
namespace BusinessLogic.Services.AutoMapper
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

            CreateMap<Products, ProductIdViewModel>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.NameCategory))
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.Category.Id))
                .ForMember(dest => dest.NameUnit,
                    opt => opt.MapFrom(src => src.Units.NameUnit))
                .ForMember(dest => dest.UnitId,
                    opt => opt.MapFrom(src => src.Units.Id))
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => src.Images.OrderBy(i => i.DisplayOrder)))
                .ForMember(dest => dest.Count,
                    opt => opt.MapFrom(src => src.Count));

            CreateMap<ProductImage, ProductImageViewModel>()
                .ForMember(dest => dest.Id, 
                    opt => opt.MapFrom(src => src.Id));

            /// <summary>
            /// Преобразования данных из News в ViewModel
            /// </summary>
            CreateMap<News, NewsViewModel>();
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Units, UnitsViewModel>();
        }
    }
}
