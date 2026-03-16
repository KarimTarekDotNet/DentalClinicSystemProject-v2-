using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;

namespace DentalClinicProject.Infrastructure.Mapping
{
    public class RateMapping : Profile
    {
        public RateMapping()
        {
            // Base → RateDTO
            CreateMap<Rate, RateDTO>()
                .ForMember(d => d.Value,
                    opt => opt.MapFrom(s => s.Value.ToString()));

            // DoctorRate → RateDoctorDTO
            CreateMap<DoctorRate, RateDoctorDTO>()
                .IncludeBase<Rate, RateDTO>()
                .ForMember(d => d.DoctorName,
                    opt => opt.MapFrom(s =>
                        s.Doctor != null
                            ? s.Doctor.AppUser.FirstName + " " + s.Doctor.AppUser.LastName
                            : null));

            // ProductRate → RateProductDTO
            CreateMap<ProductRate, RateProductDTO>()
                .IncludeBase<Rate, RateDTO>()
                .ForMember(d => d.ProductName,
                    opt => opt.MapFrom(s =>
                        s.Product != null ? s.Product.Name : null));

            // AppointmentRate → RateAppointmentDTO
            CreateMap<AppointmentRate, RateAppointmentDTO>()
                .IncludeBase<Rate, RateDTO>();
        }
    }
}