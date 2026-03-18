using DentalClinicProject.Core.DTOs.Core.Get;
using DentalClinicProject.Core.DTOs.Core.Update;
using Microsoft.AspNetCore.Identity;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IProfileRepository
    {
        Task<AppUserDTO?> GetProfileAsync(string userId);
        Task<IdentityResult> UpdateProfileAsync(string userId, UpdateAccountDTO user);
        Task<bool> DeleteProfileAsync(string userId);
    }
}