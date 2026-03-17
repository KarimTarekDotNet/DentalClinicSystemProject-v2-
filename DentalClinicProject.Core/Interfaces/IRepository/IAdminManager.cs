using DentalClinicProject.Core.DTOs.Auth;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Entities.Users;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IAdminManager
    {
        Task<AppUser> CreateAccountFromAdminAsync(RegisterDTO dto);
        Task<bool> DeleteAccountFromAdminAsync(DeleteAccountFromAdminDTO dto);
        Task<AppUser> UpdateAccountFromAdminAsync(UpdateAccountFromAdminDTO dto);
    }
}