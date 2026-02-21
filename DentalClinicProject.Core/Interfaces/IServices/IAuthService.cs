using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.ViewModels;

namespace DentalClinicProject.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResult>> Register(RegisterDTO registerDTO);
        Task<AuthResult> Login(LoginDTO loginDTO);
        Task<bool> LogoutAsync(string userId, string accessToken);
        Task<bool> LogoutAllAsync(string userId, string accessToken);
        Task<ApiResponse<AuthResult>> VerifyEmail(string email, string code);
    }
}
