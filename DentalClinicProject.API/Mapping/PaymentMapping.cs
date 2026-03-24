using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;

namespace DentalClinicProject.API.Mapping
{
    public class PaymentMapping : Profile
    {
        public PaymentMapping()
        {
            CreateMap<Payment, PaymentDTO>().ForMember(dest => dest.Status,
                opt => opt.MapFrom(x => x.Status.ToString()))
                .ForMember(dest => dest.PaymentMethod,
                opt => opt.MapFrom(x => x.PaymentMethod.ToString()));
        }
    }
}
