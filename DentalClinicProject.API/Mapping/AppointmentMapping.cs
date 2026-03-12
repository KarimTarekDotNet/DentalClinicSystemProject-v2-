using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.Entities.Core;

namespace DentalClinicProject.API.Mapping
{
    public class AppointmentMapping : Profile
    {
        public AppointmentMapping() 
        {
            CreateMap<Appointment, AppointmentDTO>()
             .ForMember(dest => dest.PatientAppUserId,
                 opt => opt.MapFrom(src => src.Patient.AppUser.Id))
             .ForMember(dest => dest.PatientName,
                 opt => opt.MapFrom(src => src.Patient.AppUser.FirstName + " " + src.Patient.AppUser.LastName))
             .ForMember(dest => dest.DoctorName,
                 opt => 
                 opt.MapFrom(src => src.Doctor.AppUser.FirstName + " " + src.Doctor.AppUser.LastName))
             .ForMember(dest => dest.ServiceName,
                 opt => opt.MapFrom(src => src.Service.Name));
        }
    }
}
