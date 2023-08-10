using AutoMapper;
using FaterRasanehMarket.Entities;
using FaterRasanehMarket.Entities.identity;
using FaterRasanehMarket.ViewModels.Brand;
using FaterRasanehMarket.ViewModels.Category;
using FaterRasanehMarket.ViewModels.Discount;
using FaterRasanehMarket.ViewModels.Identity;

namespace FaterRasanehMarket.IocConfig.AutoMapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //identity
            CreateMap<Role, RolesViewModel>().ReverseMap()
                .ForMember(u => u.Users, opt => opt.Ignore())
                .ForMember(c => c.Claims, opt => opt.Ignore());
            CreateMap<User, UsersViewModel>().ReverseMap();
            CreateMap<Category, CategoryViewModel>().ReverseMap()
    .ForMember(p => p.Parent, opt => opt.Ignore())
    .ForMember(p => p.Categories, opt => opt.Ignore())
    .ForMember(p => p.ProductCategories, opt => opt.Ignore());
            CreateMap<Brand, BrandViewModel>().ReverseMap()
                .ForMember(u=>u.Products,opt=>opt.Ignore());
            CreateMap<Discount, DiscountViewModel>().ReverseMap()
                .ForMember(t=>t.Orders,opt=>opt.Ignore());

        }
    }
}
