using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;

namespace DentalClinicProject.Infrastructure.Mapping
{
    public class CartMapping : Profile
    {
        public CartMapping()
        {
            CreateMap<CartItem, CartProductDTO>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name));

            CreateMap<Cart, CartDTO>()
                .ForMember(d => d.TotalPrice, opt => opt.MapFrom(s => s.TotalPrice))
                .ForMember(d => d.TotalItems, opt => opt.MapFrom(s => s.Items.Count));
        }
    }
}