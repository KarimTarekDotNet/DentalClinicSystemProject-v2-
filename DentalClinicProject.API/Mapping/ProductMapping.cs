using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;

namespace DentalClinicProject.Infrastructure.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDTO>();
        }
    }
}