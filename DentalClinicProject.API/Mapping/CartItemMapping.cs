using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;

namespace DentalClinicProject.API.Mapping
{
    public class CartItemMapping : Profile
    {
        public CartItemMapping() 
        {
            CreateMap<CartItem, CartItemDTO>()
                        .ForMember(dest => dest.Products,
                                   opt => opt.MapFrom(src => src.Products.Select(p => p.Name).ToList()))
                        .ForMember(dest => dest.ProductIds,
                                   opt => opt.MapFrom(src => src.Products.Select(p => p.Id).ToList()));
        }
    }
}
