using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.ViewModels;

namespace DentalClinicProject.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResult>> Register(RegisterDTO registerDTO);
        Task<ApiResponse<AuthResult>> Login(LoginDTO loginDTO);
        Task<ApiResponse<AuthResult>> VerifyLoginCode(string identifier, string code);
        Task<ApiResponse<bool>> LogoutAsync(string userId, string accessToken);
        Task<ApiResponse<bool>> LogoutAllAsync(string userId, string accessToken);
    }
    public interface IEmailVerificationService
    {
        Task<ApiResponse<AuthResult>> VerifyEmailAsync(string email, string code);
        Task<bool> ResendEmailVerificationCodeAsync(string sessionToken);
    }
    public interface IPhoneVerificationService
    {
        Task<ApiResponse<AuthResult>> VerifyPhoneAsync(string userId, string code);
        Task<bool> SendPhoneVerificationCodeAsync(string userId);
        Task<bool> ResendPhoneVerificationCodeAsync(string userId);
    }
}
