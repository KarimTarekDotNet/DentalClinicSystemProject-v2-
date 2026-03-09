using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.ViewModels;
using Microsoft.AspNetCore.Authentication;

namespace DentalClinicProject.Core.Interfaces.IServices
{
    public interface IExternalLoginService
    {
        Task<ApiResponse<AuthResult>> ExternalSignInAsync(ExternalLoginCallbackDTO externalLoginCallbackDTO);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(Provider provider, string redirectUrl);
    }
}
