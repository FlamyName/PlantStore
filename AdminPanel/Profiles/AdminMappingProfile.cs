using AdminPanel.ViewModels;
using AutoMapper;
using BusinessLogic.Core.Features.Commands;
using BusinessLogic.ViewModels;

namespace AdminPanel.Profiles
{
    public class AdminMappingProfile : Profile
    {
        public AdminMappingProfile()
        {
            CreateMap<ProductIdViewModel, EditProductViewModel>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())  
                .ForMember(dest => dest.Units, opt => opt.Ignore());

            CreateMap<UpdateProductWithFilesRequest, UpdateProductCommand>();
        }
    }
}
