using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Entities.Users;

namespace DentalClinicProject.API.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping() 
        {
            CreateMap<RegisterDTO, AppUser>().ReverseMap();
        }
    }
}
