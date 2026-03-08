using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.ViewModels;
using Microsoft.AspNetCore.Authentication;

namespace DentalClinicProject.Core.Interfaces.IServices
{
    public interface IExternalLoginService
    {
        Task<ApiResponse<AuthResult>> ExternalSignInAsync(string provider, string providerKey, string email, string firstName, string lastName);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(Provider provider, string redirectUrl);
    }
}
